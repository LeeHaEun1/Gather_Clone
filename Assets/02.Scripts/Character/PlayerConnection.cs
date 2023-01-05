using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gather.Network;
using Gather.Map;
using Photon.Pun;
using Gather.Manager;
using Unity.WebRTC;

namespace Gather.Character
{
    public class PlayerConnectionDistanceComparer : IComparer<PlayerConnection>
    {
        public int Compare(PlayerConnection x, PlayerConnection y)
        {
            return x.DistanceFromPlayer.CompareTo(y.DistanceFromPlayer);
        }
    }
    
    public class PlayerConnection : MonoBehaviourPun
    {
        public int areaID;
        public bool inPrivate = false;
        public bool inSpotlight = false;
        public SortedSet<PlayerConnection> connections = new SortedSet<PlayerConnection>(new PlayerConnectionDistanceComparer());

        [Header("Connection")]
        private WebRTCClient webRTCClient;

        [Header("trigger")]
        public float connectRadius = 5f;
        public float disconnectRadius = 8f;
        public LayerMask connectLayer;

        [Header("Events")]
        public UnityEvent onConnectionChange = new UnityEvent();
        public UnityEvent<PlayerConnection> onAreaChange = new UnityEvent<PlayerConnection>();
        public UnityEvent<PlayerConnection> onDestroy = new UnityEvent<PlayerConnection>();

        public float DistanceFromPlayer
        {
            get
            {
                return Vector3.Distance(transform.position, GameManager.Instance.characterController.player.transform.position);
            }
        }

        public WebRTCClient[] Connections
        {
            get
            {
                //IEnumerator a = connections.GetEnumerator();
                List<WebRTCClient> peerConnections = new List<WebRTCClient>();
                foreach (PlayerConnection connection in connections)
                {
                    if (connection.webRTCClient != null)
                    {
                        peerConnections.Add(connection.webRTCClient);
                    }
                }
                return peerConnections.ToArray();
            }
        }

        private void Awake()
        {
            webRTCClient = GetComponent<WebRTCClient>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (photonView.IsMine)
                onConnectionChange.AddListener(OnConnectionChange);
        }

        // Update is called once per frame
        void Update()
        {
            // public이면 근처 연결 확인
            if (!inPrivate)
            {
                ConnectPublicCheck();
            }
        }

        void ConnectPublicCheck()
        {
            if (!photonView.IsMine)
                return;
            
            bool isChanged = false;
            RaycastHit[] connectHits = Physics.SphereCastAll(transform.position, connectRadius, Vector3.up, 0, connectLayer);
            RaycastHit[] disconnectHits = Physics.SphereCastAll(transform.position, disconnectRadius, Vector3.up, 0, connectLayer);
            
            // 종료 범위 안이고 연결 범위 안이면, 종료 범위에서 지운다
            RaycastHit[] onlyDisconnectHits = new RaycastHit[disconnectHits.Length - connectHits.Length];
            int index = 0;
            foreach (RaycastHit hit in disconnectHits)
            {
                bool inConnect = false;
                foreach (RaycastHit hit2 in connectHits)
                {
                    if (hit.collider == hit2.collider)
                    {
                        inConnect = true;
                        break;
                    }
                }
                if (!inConnect)
                {
                    onlyDisconnectHits[index++] = hit;
                }
            }

            // 종료 범위 밖인 경우
            // 1. 현재 연결된 상대가 범위 밖에 있으면 연결 해제
            List<PlayerConnection> toRemove = new List<PlayerConnection>();
            foreach (PlayerConnection connection in connections)
            {
                bool isDisconnect = true;
                foreach (RaycastHit hit in disconnectHits)
                {
                    if (hit.collider.gameObject == connection.gameObject)
                    {
                        isDisconnect = false;
                        break;
                    }
                }
                // 만약 스포트라이트 상태면 연결 해제하지 않는다
                if (isDisconnect && connection.inSpotlight)
                {
                    isDisconnect = false;
                }
                if (isDisconnect)
                {
                    connection.webRTCClient.Disconnect();
                    connection.onAreaChange.RemoveListener(OnAreaChange);
                    connection.onDestroy.RemoveListener(OnClientDestroy);
                    toRemove.Add(connection);
                    isChanged = true;
                }
            }
            foreach (PlayerConnection connection in toRemove)
            {
                connections.Remove(connection);
            }

            // 종료 범위 내에 있다면
            // 1. 연결되지 않은 상대에게 연결 요청
            List<PlayerConnection> toAdd = new List<PlayerConnection>();
            foreach (RaycastHit hit in disconnectHits)
            {
                PlayerConnection connection = hit.collider.GetComponent<PlayerConnection>();
                if (connection != null)
                {
                    if (connection == this)
                        continue;
                    if (!connections.Contains(connection) && !connection.inPrivate)
                    {
                        connection.webRTCClient.Connect();
                        connection.onAreaChange.AddListener(OnAreaChange);
                        connection.onDestroy.AddListener(OnClientDestroy);
                        toAdd.Add(connection);
                        isChanged = true;
                    }
                }
            }
            foreach (PlayerConnection connection in toAdd)
            {
                connections.Add(connection);
            }

            // 종료 범위 내에 있지만 연결 범위 밖인 경우
            // 1. 현재 연결된 상대이면 비디오와 오디오를 흐릿한 효과를 추가  // 추후 구현
            foreach (RaycastHit hit in onlyDisconnectHits)
            {
                WebRTCClient client = hit.collider.GetComponent<WebRTCClient>();
                PlayerConnection connection = hit.collider.GetComponent<PlayerConnection>();
                if (client != null && connection != null && !connection.inSpotlight)
                {
                    float phase = 1 - ((client.DistanceFromPlayer - connectRadius) / (disconnectRadius - connectRadius));
                    if (phase > 0.7)
                        client.SetBlur(1);
                    else
                        client.SetBlur(phase);
                }
                else if (client != null && connection != null && connection.inSpotlight)
                {
                    client.SetBlur(1);
                }
            }

            if (isChanged)
            {
                onConnectionChange.Invoke();
            }
        }

        void ConnectPrivateArea()
        {
            var id = GetInstanceID();
            if (!photonView.IsMine)
                return;
            Debug.LogAssertion($"{GetInstanceID()} ConnectPrivateArea");
            PlayerConnection[] privateAreaPlayers = PrivateArea.GetPlayers(areaID);
            if (privateAreaPlayers == null)
            {
                privateAreaPlayers = new PlayerConnection[0];
            }

            // private area에 들어가면, private area에 있는 모든 상대와 연결
            // 기존 연결 중 같은 지역에 없는 연결을 끊는다.
            List<PlayerConnection> toRemove = new List<PlayerConnection>();
            foreach (PlayerConnection connection in connections)
            {
                bool isDisconnect = true;
                foreach (PlayerConnection player in privateAreaPlayers)
                {
                    if (connection.transform == player.transform)
                    {
                        isDisconnect = false;
                        break;
                    }
                }
                if (isDisconnect)
                {
                    connection.webRTCClient.Disconnect();
                    connection.onAreaChange.RemoveListener(OnAreaChange);
                    connection.onDestroy.RemoveListener(OnClientDestroy);
                    toRemove.Add(connection);
                }
            }
            foreach (PlayerConnection connection in toRemove)
            {
                connections.Remove(connection);
            }

            // 같은 지역에 있는 연결은 그대로 유지한다.

            // 같은 지역에 있는 새로운 연결은 추가한다.
            foreach (PlayerConnection connection in privateAreaPlayers)
            {
                if (connection == this)
                    continue;
                if (!connections.Contains(connection))
                {
                    connection.webRTCClient.Connect();
                    connection.onAreaChange.AddListener(OnAreaChange);
                    connection.onDestroy.AddListener(OnClientDestroy);
                    connections.Add(connection);
                }
            }
            onConnectionChange.Invoke();
        }

        void DisconnectPrivateArea()
        {
            if (!photonView.IsMine)
                return;

            List<PlayerConnection> toRemove = new List<PlayerConnection>();
            foreach (PlayerConnection connection in connections)
            {
                connection.webRTCClient.Disconnect();
                connection.onAreaChange.RemoveListener(OnAreaChange);
                connection.onDestroy.RemoveListener(OnClientDestroy);
                toRemove.Add(connection);
            }
            foreach (PlayerConnection connection in toRemove)
            {
                connections.Remove(connection);
            }
        }
        
        public void OnAreaChange(PlayerConnection connection)
        {
            // 상대의 지역이 변경되면, 연결을 끊는다.
            connection.webRTCClient.Disconnect();
            connection.onAreaChange.RemoveListener(OnAreaChange);
            connection.onDestroy.RemoveListener(OnClientDestroy);
            connections.Remove(connection);
        }
        
        public void OnConnectionChange()
        {

        }
        
        public void PrintConnections()
        {
            string log = $"PlayerConnection ({GetInstanceID()}): ";
            foreach (PlayerConnection connection in connections)
            {
                log += connection.name + "(" + connection.GetInstanceID() + ")" + ", ";
            }
            log += $"\n{areaID} {inPrivate}";
            //Debug.Log(log);
            Debug.LogAssertion(log);
        }

        public void OnClientDestroy(PlayerConnection webRTCClient)
        {
            connections.Remove(webRTCClient);
        }

        public void EnterPrivateArea(int areaID)
        {
            Debug.LogAssertion($"{GetInstanceID()} EnterPrivateArea {areaID}");
            PrivateArea.PrintPlayers(areaID);
            this.areaID = areaID;
            inPrivate = true;
            PrivateArea.onAreaPlayerChanges[areaID].AddListener(ConnectPrivateArea);
            onAreaChange.Invoke(this);
        }

        public void ExitPrivateArea()
        {
            Debug.LogAssertion($"{GetInstanceID()} ExitPrivateArea");
            PrivateArea.PrintPlayers(areaID);
            inPrivate = false;
            PrivateArea.onAreaPlayerChanges[areaID].RemoveListener(ConnectPrivateArea);
            DisconnectPrivateArea();
            onAreaChange.Invoke(this);
        }

        public void EnterSpotlight()
        {
            Debug.LogAssertion($"{GetInstanceID()} EnterSpotlight");
            inSpotlight = true;

            webRTCClient.receiveAudio.spatialBlend = 0;
            webRTCClient.Connect();
            connections.Add(this);
        }

        public void ExitSpotlight()
        {
            Debug.LogAssertion($"{GetInstanceID()} ExitSpotlight");
            inSpotlight = false;

            webRTCClient.receiveAudio.spatialBlend = 1;
            webRTCClient.Disconnect();
            connections.Remove(this);
        }

        private void OnDestroy()
        {
            if (inPrivate)
            {
                ExitPrivateArea();
            }
            onDestroy.Invoke(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, connectRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, disconnectRadius);
        }
    }
}