using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Gather.Controller;
using Gather.UI;
using TMPro;
using Gather.Network;
using Gather.Character;

namespace Gather.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public Controller.CharacterController characterController;
        //public CinemachineVirtualCamera charcterCamera;

        public Canvas canvas;
        Canvas nonMasterCanvas;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            //OnPhotonSerializeView 호출 빈도
            PhotonNetwork.SerializationRate = 60;
            //Rpc 호출 빈도
            PhotonNetwork.SendRate = 60;

            // 추후 제거할것
            // 디버깅 도중 타임아웃 없애기
            //PhotonNetwork.networkingPeer.DisconnectTimeout = 30; // seconds. any high value for debug
            PhotonNetwork.MaxResendsBeforeDisconnect = 8; // count of resends. high value for debug

            //플레이어를 생성한다.
            //GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0,1,0), Quaternion.identity);
            GameObject player = PhotonNetwork.Instantiate("Player(Drone1)", new Vector3(0, 1, 0), Quaternion.identity);
            player.transform.parent = characterController.transform;
            //charcterCamera.Follow = player.transform.Find("Camera Position");
            //charcterCamera.LookAt = player.transform;

            // Curved Nickname 추가


            characterController.ChangePlayer(player.GetComponent<Player>());


            // Note를 생성한다(공동편집용)
            // Door
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.InstantiateRoomObject("Door_A_R", new Vector3(16.16334f, 2.216501f, -1.053994f), Quaternion.identity);
                PhotonNetwork.InstantiateRoomObject("Door_B_R", new Vector3(16.16334f, 2.216501f, 2.765095f), Quaternion.Euler(0, 180, 0));

                PhotonNetwork.InstantiateRoomObject("Door_A_L", new Vector3(-14.83666f, 2.216501f, 2.766329f), Quaternion.Euler(0, -180, 0));
                PhotonNetwork.InstantiateRoomObject("Door_B_L", new Vector3(-14.83666f, 2.216501f, -1.052761f), Quaternion.Euler(-180, -180, -180));

                PhotonNetwork.InstantiateRoomObject("Frame", new Vector3(-11.03866f, 1.630486f, 15.85617f), Quaternion.Euler(-90, -0, 90));

                PhotonNetwork.InstantiateRoomObject("Projector", new Vector3(-76.23f, 5.39f, 0.59f), Quaternion.Euler(0, 270, 0));

                /*GameObject myNote = PhotonNetwork.InstantiateRoomObject("NoteTest_InputField (TMP)", Vector3.zero, Quaternion.identity);
                myNote.transform.parent = GameObject.Find("Canvas").transform;
                myNote.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);*/

            }
            else
            {
                //GameObject mastersNote = GameObject.Find("NoteTest_InputField (TMP)(Clone)");

                //nonMasterCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

                //mastersNote.transform.SetParent(GameObject.Find("Canvas").transform);

                //mastersNote.transform.parent = nonMasterCanvas.transform;
                //mastersNote.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }

            // Door_A_R
            if (PhotonNetwork.IsMasterClient)
            {
                //GameObject doorAR = PhotonNetwork.Instantiate("Door_A_R", new Vector3(16.16334f, 2.216501f, -1.053994f), Quaternion.identity);
                //GameObject doorAR = PhotonNetwork.Instantiate("Door_A_R", Vector3.zero, Quaternion.identity);
            }
            //doorAR.transform.parent = GameObject.Find("Wall_Door_04_Inst").transform;
            
        }
    }
}
