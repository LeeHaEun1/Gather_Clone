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

// [LHE 0911]
// <캔버스1>
// 1. 버튼 클릭을 통해 Space Type 선택 시 (1) 방을 만들고 (2) 닉네임 등 설정하는 캔버스로 넘어가기

// <캔버스2>
// 1. 방 이름 설정

// <캔버스3>
// 1. 닉네임 설정 (InputField)
// 2. 캐릭터 설정
// 3. 마이크 & 비디오 On/Off 설정
// 4. Room 입장 버튼
namespace Gather.Manager
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [Header("Canvas")]
        public CanvasGroup spaceTypeGroup;
        public CanvasGroup spaceNameGroup;
        public CanvasGroup characterGroup;
        public Canvas spaceType;
        public Canvas spaceName;
        public Canvas character;

        [Header("Space")]
        public TMP_InputField spaceNameInput; // 캔버스2에서 설정한 방 이름
        public TextMeshProUGUI spaceNameHeadline; // 캔버스3 상단에 띄울 방 이름

        [Header("Character")]
        public TMP_InputField NickNameInput;
        public GameObject characterSelect;

        [Header("Microphone")]
        public GameObject micVolumeBar;
        public Image micOn;
        public Image micMute;
        public Image micPopupDown;
        public Image micPopupUp;
        public GameObject micPopup;
        public TMP_Dropdown micDropdown;
        public TMP_Dropdown speakerDropdown;
        bool isMicOn = true;
        bool isMicPopupDown = true;

        [Header("Camera")]
        public RawImage cameraPreview;
        WebCamTexture webCamTexture;
        public Image videoOn;
        public Image videoOff;
        public Image videoPopupDown;
        public Image videoPopupUp;
        public GameObject videoPopup;
        public TMP_Dropdown cameraDropdown;
        bool isVideoOn = true;
        bool isVideoPopupDown = true;


        void Start()
        {
            spaceNameGroup.enabled = false;
            characterGroup.enabled = false;
            spaceName.enabled = false;
            character.enabled = false;

            characterSelect.SetActive(false);

            // 마이크 관련 UI 세팅
            micVolumeBar.SetActive(true);
            micOn.enabled = true;
            micMute.enabled = false;
            micPopupDown.enabled = true;
            micPopupUp.enabled = false;
            micPopup.SetActive(false);

            // 카메라 관련 UI 세팅
            videoOn.enabled = true;
            videoOff.enabled = false;
            videoPopupDown.enabled = true;
            videoPopupUp.enabled = false;
            videoPopup.SetActive(false);

            // 옵션 삽입
            micDropdown.options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < Microphone.devices.Length; i++)
            {
                micDropdown.options.Add(new TMP_Dropdown.OptionData(Microphone.devices[i]));
            }
            micDropdown.value = 0;
            micDropdown.RefreshShownValue();
            micDropdown.onValueChanged.AddListener(OnMicDropdownValueChanged);

            speakerDropdown.options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("1"), new TMP_Dropdown.OptionData("2둘two두개는하나더하기하나입니다"), new TMP_Dropdown.OptionData("3셋three삼") };

            cameraDropdown.options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                cameraDropdown.options.Add(new TMP_Dropdown.OptionData(WebCamTexture.devices[i].name));
            }
            cameraDropdown.value = 0;
            cameraDropdown.RefreshShownValue();
            cameraDropdown.onValueChanged.AddListener(OnCameraDropdownValueChanged);
            StartCoroutine(CaptureVideoStart());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (spaceNameGroup.enabled == true)
                {
                    spaceNameGroup.enabled = false;
                    spaceTypeGroup.enabled = true;
                    spaceName.enabled = false;
                    spaceType.enabled = true;
                }
                else if (characterGroup.enabled == true)
                {
                    spaceNameGroup.enabled = true;
                    characterGroup.enabled = false;
                    spaceName.enabled = true;
                    character.enabled = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (spaceNameGroup.enabled == true && spaceNameInput.text != "")
                {
                    EnableCharacterCanvas();
                }
                else if (characterGroup.enabled == true && NickNameInput.text != "")
                {
                    CreateRoom();
                }
            }
        }

        // [Canvas 1]
        // Space Type 선택 시 방 이름 설정하는 팝업 띄우기
        public void EnableSpaceNameCanvas()
        {
            spaceTypeGroup.enabled = false;
            spaceType.enabled = false;

            spaceNameGroup.enabled = true;
            spaceName.enabled = true;
        }

        // [Canvas 2]
        // Space Name 설정 시 캐릭터, 닉네임, 마이크&비디오 On/Off 설정하는 캔버스 띄우기
        public void EnableCharacterCanvas()
        {
            // 방 이름 미입력 시 생성 버튼 비활성화
            if (spaceNameInput.text != "")
            {
                spaceNameGroup.enabled = false;
                spaceName.enabled = false;

                characterGroup.enabled = true;
                character.enabled = true;
            }

            // 상단에 뜨는 문구
            spaceNameHeadline.text = "Welcome to " + spaceNameInput.text;
        }

        // [Canvas 3]
        // 마이크 버튼
        public void MicOnOff()
        {
            if (isMicOn == true)
            {
                micOn.enabled = false;
                micMute.enabled = true;

                micVolumeBar.SetActive(false);

                // ***** 실제 마이크 기능도 끄기 *****
                // ~~~~

                isMicOn = false;
            }
            else
            {
                micOn.enabled = true;
                micMute.enabled = false;

                micVolumeBar.SetActive(true);

                // ***** 실제 마이크 기능도 켜기 *****
                // ~~~~

                isMicOn = true;
            }
        }

        // 마이크 팝업 버튼
        public void MicPopupOnOff()
        {
            if (isMicPopupDown == true)
            {
                micPopupDown.enabled = false;
                micPopupUp.enabled = true;

                micPopup.SetActive(true);

                isMicPopupDown = false;
            }
            else
            {
                micPopupDown.enabled = true;
                micPopupUp.enabled = false;

                micPopup.SetActive(false);

                isMicPopupDown = true;
            }
        }

        // 카메라 버튼
        public void CameraOnOff()
        {
            if (isVideoOn == true)
            {
                videoOn.enabled = false;
                videoOff.enabled = true;

                // ***** 실제 카메라 기능도 끄기 *****
                // ~~~~

                isVideoOn = false;
            }
            else
            {
                videoOn.enabled = true;
                videoOff.enabled = false;

                // ***** 실제 카메라 기능도 켜기 *****
                // ~~~~

                isVideoOn = true;
            }
        }

        // 카메라 팝업 버튼
        public void CameraPopupOnOff()
        {
            if (isVideoPopupDown == true)
            {
                videoPopupDown.enabled = false;
                videoPopupUp.enabled = true;

                videoPopup.SetActive(true);

                isVideoPopupDown = false;
            }
            else
            {
                videoPopupDown.enabled = true;
                videoPopupUp.enabled = false;

                videoPopup.SetActive(false);

                isVideoPopupDown = true;
            }
        }


        // [방 생성 요청] -> 이걸 Join the Gathering버튼과 연결
        public void CreateRoom()
        {
            // User NickName
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;

            RoomOptions conferenceRoom = new RoomOptions(); // 디폴트는 최대인원 & IsVisible
            conferenceRoom.PublishUserId = true;
            PhotonNetwork.CreateRoom("Conference Room", conferenceRoom);
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
            PhotonNetwork.JoinRoom("Conference Room");
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

        void WebCamConnect()
        {
            
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

            cameraPreview.texture = webCamTexture;
        }

        private IEnumerator OnCaptureVideoChange()
        {
            webCamTexture.Stop();
            webCamTexture = new WebCamTexture(VideoSetting.SelectedDevice.name, VideoSetting.StreamSize.x, VideoSetting.StreamSize.y, 30);
            webCamTexture.Play();
            yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);
            
            cameraPreview.texture = webCamTexture;
        }

        void OnMicDropdownValueChanged(int value)
        {
            AudioSetting.SelectedDevice = Microphone.devices[value];
        }

        void OnCameraDropdownValueChanged(int value)
        {
            VideoSetting.SelectedDevice = WebCamTexture.devices[value];
            StartCoroutine(OnCaptureVideoChange());
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
