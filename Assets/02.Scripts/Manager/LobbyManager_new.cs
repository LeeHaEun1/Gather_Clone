using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;
using Unity.WebRTC;
using Gather.Data;
using Mediapipe.SelfieSegmentation;

namespace Gather.Manager
{
    public class LobbyManager_new : MonoBehaviourPunCallbacks
    {
        [Header("Canvas")]
        public CanvasGroup roomNameGroup;
        public CanvasGroup nickNameGroup;
        public CanvasGroup micVideoGroup;
        public CanvasGroup enterRoomGroup;
        public Canvas roomName;
        public Canvas nickName;
        public Canvas micVideo;
        public Canvas enterRoom;

        [Header("Room Name")]
        //public TMP_InputField inputRoomName;
        public InputField textRoomName;
        public GameObject panelRoomName;

        [Header("NickName")]
        //public TMP_InputField inputNickName;
        public InputField textNickName;
        public GameObject panelNickName;

        [Header("Mic & Speaker & Video")]
        public TMP_Dropdown micDropdown;
        public TMP_Dropdown cameraDropdown;
        //public TMP_Dropdown speakerDropdown;
        //public RawImage cameraPreview; // 웹캡 영상 송출될 자리
        public MeshRenderer cameraPreview; // 웹캡 영상 송출될 자리
        //public Camera cam; // self camera 넣을 자리
        //public MeshRenderer receiveImage;
        WebCamTexture webCamTexture;

        SelfieSegmentation segmentation;
        [SerializeField] SelfieSegmentationResource segmentResource;

        [Header("Character")]
        public int xxx;


        // Start is called before the first frame update
        void Start()
        {
            // Canvas
            roomNameGroup.enabled = true;
            nickNameGroup.enabled = false;
            micVideoGroup.enabled = false;
            enterRoomGroup.enabled = false;
            roomName.enabled = true;
            nickName.enabled = false;
            micVideo.enabled = false;
            enterRoom.enabled = false;


            //마이크 & 비디오 옵션
            micDropdown.options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < Microphone.devices.Length; i++)
            {
                micDropdown.options.Add(new TMP_Dropdown.OptionData(Microphone.devices[i]));
            }
            micDropdown.value = 0;
            micDropdown.RefreshShownValue();
            //micDropdown.onValueChanged.AddListener(OnMicDropdownValueChanged);

            //speakerDropdown.options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("1"), new TMP_Dropdown.OptionData("2둘two두개는하나더하기하나입니다"), new TMP_Dropdown.OptionData("3셋three삼") };

            cameraDropdown.options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                cameraDropdown.options.Add(new TMP_Dropdown.OptionData(WebCamTexture.devices[i].name));
            }
            cameraDropdown.value = 0;
            cameraDropdown.RefreshShownValue();
            //cameraDropdown.onValueChanged.AddListener(OnCameraDropdownValueChanged);
            StartCoroutine(CaptureVideoStart());

            //receiveImage.material.SetTexture("_BaseMap", cam.targetTexture);

            textRoomName.Select();

            segmentation = new SelfieSegmentation(segmentResource);
        }


        // Update is called once per frame
        void Update()
        {
            // 방 이름 or 닉네임 (한 글자 이상) 입력하고 엔터 누를 시 다음 캔버스로 이동
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (roomNameGroup.enabled == true && textRoomName.text != "")
                {
                    EnableNickNameCanvas();
                }
                else if (nickNameGroup.enabled == true && textNickName.text != "")
                {
                    EnableMicVideoCanvas();
                }
                else if(micVideoGroup.enabled == true)
                {
                    EnableEnterRoomCanvas();
                }
                else if(enterRoomGroup.enabled == true)
                {
                    CreateRoom();
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad0) && micVideoGroup.enabled)
            {
                Debug.LogAssertion("0");
                OnHologramTemplateChange(0);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1) && micVideoGroup.enabled)
            {
                Debug.LogAssertion("1");
                OnHologramTemplateChange(1);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2) && micVideoGroup.enabled)
            {
                Debug.LogAssertion("2");
                OnHologramTemplateChange(2);
            }

            Texture receiveTexture = cameraPreview.material.GetTexture("_BaseMap");
            if (receiveTexture != null)
            {
                segmentation.ProcessImage(receiveTexture);
                cameraPreview.material.SetTexture("_SegmentMask", segmentation.texture);
            }
        }


        public void EnableNickNameCanvas()
        {
            roomNameGroup.enabled = false;
            roomName.enabled = false;
            
            nickNameGroup.enabled = true;
            nickName.enabled = true;

            panelRoomName.SetActive(false);

            textNickName.Select();
        }

        public void EnableMicVideoCanvas()
        {
            nickNameGroup.enabled = false;
            nickName.enabled = false;

            micVideoGroup.enabled = true;
            micVideo.enabled = true;

            panelNickName.SetActive(false);
        }

        public void EnableEnterRoomCanvas()
        {
            micVideoGroup.enabled = false;
            micVideo.enabled = false;

            enterRoomGroup.enabled = true;
            enterRoom.enabled = true;
        }
     
        // 방 생성 요청
        public void CreateRoom()
        {
            // User NickName
            PhotonNetwork.LocalPlayer.NickName = textNickName.text;

            RoomOptions room = new RoomOptions(); // 디폴트는 최대인원 & IsVisible
            room.PublishUserId = true;
            PhotonNetwork.CreateRoom("Room", room);
        }

        // 방 생성 성공 시
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        // 방 생성 실패 시
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            print("OnCreateRoomFailed: " + returnCode + ", " + message);
            JoinRoom();
        }

        // [방 참가 요청]
        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom("Room");
        }

        // 방 참가 성공 시
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name);
            PhotonNetwork.LoadLevel(2);
        }

        // 방 참가 실패 시
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            print("OnJoinedRoomFailed: " + returnCode + ", " + message);
        }

        

        private IEnumerator CaptureVideoStart()
        {
            if (WebCamTexture.devices.Length == 0)
            {
                Debug.LogFormat("WebCam device not found");
                yield break;
            }

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogFormat("authorization for using the device is denied");
                yield break;
            }

            webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, VideoSetting.StreamSize.x, VideoSetting.StreamSize.y, 30);
            webCamTexture.Play();
            yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);

            //cameraPreview.texture = webCamTexture;
            cameraPreview.material.SetTexture("_BaseMap", webCamTexture);
        }

        private IEnumerator OnCaptureVideoChange()
        {
            webCamTexture.Stop();
            webCamTexture = new WebCamTexture(VideoSetting.SelectedDevice.name, VideoSetting.StreamSize.x, VideoSetting.StreamSize.y, 30);
            webCamTexture.Play();
            yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);

            //cameraPreview.texture = webCamTexture;
            cameraPreview.material.SetTexture("_BaseMap", webCamTexture);
        }

        public void OnMicDropdownValueChanged(int value)
        {
            AudioSetting.SelectedDevice = Microphone.devices[value];
        }

        public void OnCameraDropdownValueChanged(int value)
        {
            VideoSetting.SelectedDevice = WebCamTexture.devices[value];
            StartCoroutine(OnCaptureVideoChange());
        }

        public void OnHologramTemplateChange(int value)
        {
            VideoSetting.hologramTamplateIndex = value;
        }

        private void OnDestroy()
        {
            if (webCamTexture != null)
            {
                webCamTexture.Stop();
                webCamTexture = null;
            }
        }
    }
}
