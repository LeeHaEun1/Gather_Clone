using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun; 
using Photon.Realtime;

// [LHE 0910]
// 1. LobbyScene 이동 전까지 로딩 UI 띄워주기(Logo 회전)
// 2. 사용자가 닉네임 직접 지정하고 입장할 수 있게 하기(추후 수정도 가능해야함) -> 공간을 먼저 선택하고 닉네임 설정이 나오므로 뒤로 미루기!
namespace Gather.Manager
{
    public class ConnectionManager : MonoBehaviourPunCallbacks
    {
        //public TMP_InputField NickNameInput;
        public Image logo;

        // Start is called before the first frame update
        void Start()
        {
            // NameServer 접속(AppId, GameVersion, 지역이 모두 같은 경우 같이 묶임)
            PhotonNetwork.ConnectUsingSettings();
        }

        // Lobby 생성 및 진입 불가 상태
        public override void OnConnected()
        {
            base.OnConnected();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name); // 진행되는 함수 이름 반환
        }

        // Master Server 접속 성공, Lobby 생성 및 진입 가능
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name);

            // ****** 닉네임 설정할 수 있게 만들기 *********
            //PhotonNetwork.NickName = "User " + Random.Range(1, 101); 
            //PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;

            // 기본 Lobby 진입 요청
            PhotonNetwork.JoinLobby();

            // 특정 Lobby 진입 요청
            // ****** 이걸 사용해서 사용자마다 개인의 mySpace로 이동하게 하는게 맞으려나..?
            //PhotonNetwork.JoinLobby(new TypedLobby("myLobby", LobbyType.Default));
        }

        // Lobby 진입 성공
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name);

            PhotonNetwork.LoadLevel(1);
        }

        float currentTime = 1;
        public float rotateTime = 1;
        public float rotSpeed = 100;
        // Update is called once per frame
        void Update()
        {
            // 일정시간이 될 때마다 로고 회전(시작 시에는 바로 회전)
            // ***** 추후 한바퀴만 회전하는 공식으로 수정
            currentTime += Time.deltaTime;
            if (currentTime > rotateTime)
            {
                logo.transform.Rotate(new Vector3(0, 0, rotSpeed * Time.deltaTime));
                currentTime = 0;
            }
        }
    }
}
