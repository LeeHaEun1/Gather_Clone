using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Gather.Object;

// [LHE 0913]
// 1. adws & 화살표로 앞뒤좌우 이동 가능(3D)
// 2. Ghost Mode (Layer 변경: Ghost <-> Player는 충돌 안하도록 layer collision matrix에서 설정) (G key)
// 3. 오브젝트(tag = Object)에 일정거리 내로 접근 시 상호작용 기능 On (=오브젝트 기능 활성화) (X key)

// CharacterController에서는 호출만 하고 실제 구현은 Player에서

namespace Gather.Character
{
    public class Player : MonoBehaviourPun, IPunObservable
    {
        [Header("Move")]
        public float moveSpeed = 5;

        [Header("Ghost Mode")]
        public bool isGhostMode = false;
        CharacterController cc;

        [Header("Object Interact")]
        public GameObject interactPopup;
        public TextMeshProUGUI interactPopupText;
        public bool interactOn = false;
        public List<Interactable> nearObjectInterface = new List<Interactable>();
        // Test: Door 여닫았다 다른 오브젝트가도 Door랑 상호작용하는 현상 Test 용도
        public List<int> nearObjectID = new List<int>();
        public List<GameObject> nearObject = new List<GameObject>();

        [Header("Camera")]
        public Transform firstCameraPosition;
        public Transform firstCameraLookAt;
        public Transform thirdCameraPosition;
        public Transform thirdCameraLookAt;
        
        [Header("etc")]
        public MeshRenderer bodyRenderer;
        public Text nickName;

        public PlayerConnection playerConnection;

        public GameObject[] onlyForMine;
        public GameObject[] onlyForOthers;

        // Start is called before the first frame update
        void Start()
        {
            this.gameObject.layer = 6;
            cc = GetComponent<CharacterController>();

            playerConnection = GetComponent<PlayerConnection>();


            nickName.text = photonView.Owner.NickName;
            bodyRenderer.material.color = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
            // ???? ???? ??? ??? UI?? ?÷???? ???η? ????, ????
            if (photonView.IsMine)
            {
                //interactPopup = GameObject.Find("ObjectInteractPopup");
                //interactPopupText = interactPopup.GetComponentInChildren<TextMeshProUGUI>();
                //print("============== " + interactPopup);
                //interactPopup.SetActive(false);
                //nickName.enabled = false;
                PlayerNickname nickName = GetComponent<PlayerNickname>();
                if (nickName != null)
                {
                    nickName.enabled = false;
                }

                foreach (var item in onlyForOthers)
                {
                    item.SetActive(false);
                }
            }
            else
            {
                foreach (var item in onlyForMine)
                {
                    item.SetActive(false);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // [3-1. 상호작용 가능 오브젝트들의 우선순위 정렬]



            // [3. 오브젝트 상호작용]
            // X키 말고 마우스 좌클릭으로 상호작용 (단, 해당 오브젝트의 스크린상 좌표 범위 안에 마우스가 존재하는 상태에서만 상호작용 가능)
            //print("mousePosition " + Input.mousePosition);
            //Vector3 objectScreenPos = Camera.main.WorldToScreenPoint(nearObject[0].transform.position);
            //print("objectScreenPos " + objectScreenPos); // 이렇게하면 딱 중심점으로만 나오는듯..? boxextents?로 해줘야하나?


            // 기존 방식: X키로 상호작용
            //if (photonView.IsMine)
            //{
            //    // (2/3) Popup 끄며 상호작용 시작
            //    if (interactPopup.activeSelf == true && Input.GetKeyDown(KeyCode.X))
            //    {
            //        interactPopup.SetActive(false);
            //        interactOn = true;

            //        if (nearObjectInterface.Count > 0)
            //        {
            //            // ***** 추후 목록의 모든 대상들을 펼쳐 보여주고 사용자가 선택할 수 있게 한다
            //            nearObjectInterface[0].TryInteract();
            //        }
            //    }
            //    // (3/3) 상호작용중인 상태에서 X키 누르면 상태 변경 & 상호작용 중단
            //    // (상호작용의 중단은 각 오브젝트의 Interact 함수에서 구현)
            //    else if (interactOn = true && Input.GetKeyDown(KeyCode.X))
            //    {
            //        if (nearObjectInterface.Count > 0)
            //        {
            //            // ***** 추후 목록의 모든 대상들을 펼쳐 보여주고 사용자가 선택할 수 있게 한다
            //            nearObjectInterface[0].TryInteract();
            //        }

            //        interactOn = false;
            //    }
            //}
        }
                
        // [1. 이동]
        // WS 또는 화살표를 통해 앞뒤 이동
        internal void Move(Vector3 dir)
        {
            dir = transform.TransformDirection(dir);
            dir.Normalize();

            cc.SimpleMove(dir * moveSpeed);
        }

        // [1-1. 회전]
        // AD 또는 화살표를 통해 몸체 회전
        internal void Rotate(Vector3 dir)
        {
            dir = transform.TransformDirection(dir);
            dir.Normalize();

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);
        }


        // [2. 유령 모드] 
        internal void GhostModeOn()
        {
            // Layer를 Ghost로 바꿔주고
            this.gameObject.layer = 7;

            transform.Find("Body/Drone_LODs").gameObject.layer = 7;
            // 유령모드 켜준다
            isGhostMode = true;
        }
        internal void GhostModeOff()
        {
            // Layer를 다시 Player로 바꿔주고
            this.gameObject.layer = 6;

            transform.Find("Body/Drone_LODs").gameObject.layer = 6;
            // 유령모드 꺼준다
            isGhostMode = false;
        }


        // [3. 오브젝트 상호작용] (사용X)
        internal void ChairInteract(Transform seatTr)
        {
            transform.position = seatTr.position;
            transform.rotation = seatTr.rotation;


            //Vector3 beforeInteractPos = transform.position;
            //if(transform.position != Gather.Controller.CharacterController.Instance.chairTr[0].position)
            //{
            //    transform.position = Gather.Controller.CharacterController.Instance.chairTr[0].position;
            //    transform.rotation = Gather.Controller.CharacterController.Instance.chairTr[0].rotation;
            //}
            //// 의자의 경우 직진하는 키 누르면 자연히 의자에서 내려올 수 있으므로 굳이 필요없는듯
            //else
            //{
            //    transform.position = beforeInteractPos;
            //    Gather.Controller.CharacterController.Instance.chairTr.RemoveAt(0);
            //}
        }




        // [X. 오브젝트 상호작용] (사용X)
        // (1/3) Trigger
        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
                return;
            Interactable Interactable =  other.gameObject.GetComponent<Interactable>();
            if(Interactable != null && interactOn == false)
            {
                interactPopupText.text = Interactable.GetName(); // ***** 여기서 넣어주면 추후 한 번에 여러개 접촉했을 때 제대로 매칭 안되는 문제 발생 가능하긴 할 듯..?
                interactPopup.SetActive(true);
                nearObjectInterface.Add(Interactable);

                nearObjectID.Add(other.gameObject.GetInstanceID());
                nearObject.Add(other.gameObject);
            }

        }
        private void OnTriggerExit(Collider other)
        {
            if (!photonView.IsMine)
                return;
            Interactable Interactable =  other.gameObject.GetComponent<Interactable>();
            // Door 여닫았다 다른 오브젝트가도 Door랑 상호작용하는 현상 해결 위해 주석처리
            if (Interactable != null /*&& interactOn == false*/)
            {
                interactPopup.SetActive(false);
                nearObjectInterface.Remove(Interactable);

                nearObjectID.Remove(other.gameObject.GetInstanceID());
                nearObject.Remove(other.gameObject);
            }
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                transform.position = (Vector3)stream.ReceiveNext();
                transform.rotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
