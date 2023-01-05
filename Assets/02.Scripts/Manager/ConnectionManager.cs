using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun; 
using Photon.Realtime;

// [LHE 0910]
// 1. LobbyScene �̵� ������ �ε� UI ����ֱ�(Logo ȸ��)
// 2. ����ڰ� �г��� ���� �����ϰ� ������ �� �ְ� �ϱ�(���� ������ �����ؾ���) -> ������ ���� �����ϰ� �г��� ������ �����Ƿ� �ڷ� �̷��!
namespace Gather.Manager
{
    public class ConnectionManager : MonoBehaviourPunCallbacks
    {
        //public TMP_InputField NickNameInput;
        public Image logo;

        // Start is called before the first frame update
        void Start()
        {
            // NameServer ����(AppId, GameVersion, ������ ��� ���� ��� ���� ����)
            PhotonNetwork.ConnectUsingSettings();
        }

        // Lobby ���� �� ���� �Ұ� ����
        public override void OnConnected()
        {
            base.OnConnected();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name); // ����Ǵ� �Լ� �̸� ��ȯ
        }

        // Master Server ���� ����, Lobby ���� �� ���� ����
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name);

            // ****** �г��� ������ �� �ְ� ����� *********
            //PhotonNetwork.NickName = "User " + Random.Range(1, 101); 
            //PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;

            // �⺻ Lobby ���� ��û
            PhotonNetwork.JoinLobby();

            // Ư�� Lobby ���� ��û
            // ****** �̰� ����ؼ� ����ڸ��� ������ mySpace�� �̵��ϰ� �ϴ°� ��������..?
            //PhotonNetwork.JoinLobby(new TypedLobby("myLobby", LobbyType.Default));
        }

        // Lobby ���� ����
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
            // �����ð��� �� ������ �ΰ� ȸ��(���� �ÿ��� �ٷ� ȸ��)
            // ***** ���� �ѹ����� ȸ���ϴ� �������� ����
            currentTime += Time.deltaTime;
            if (currentTime > rotateTime)
            {
                logo.transform.Rotate(new Vector3(0, 0, rotSpeed * Time.deltaTime));
                currentTime = 0;
            }
        }
    }
}
