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
// <ĵ����1>
// 1. ��ư Ŭ���� ���� Space Type ���� �� (1) ���� ����� (2) �г��� �� �����ϴ� ĵ������ �Ѿ��

// <ĵ����2>
// 1. �� �̸� ����

// <ĵ����3>
// 1. �г��� ���� (InputField)
// 2. ĳ���� ����
// 3. ����ũ & ���� On/Off ����
// 4. Room ���� ��ư
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
        public TMP_InputField spaceNameInput; // ĵ����2���� ������ �� �̸�
        public TextMeshProUGUI spaceNameHeadline; // ĵ����3 ��ܿ� ��� �� �̸�

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

            // ����ũ ���� UI ����
            micVolumeBar.SetActive(true);
            micOn.enabled = true;
            micMute.enabled = false;
            micPopupDown.enabled = true;
            micPopupUp.enabled = false;
            micPopup.SetActive(false);

            // ī�޶� ���� UI ����
            videoOn.enabled = true;
            videoOff.enabled = false;
            videoPopupDown.enabled = true;
            videoPopupUp.enabled = false;
            videoPopup.SetActive(false);

            // �ɼ� ����
            micDropdown.options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < Microphone.devices.Length; i++)
            {
                micDropdown.options.Add(new TMP_Dropdown.OptionData(Microphone.devices[i]));
            }
            micDropdown.value = 0;
            micDropdown.RefreshShownValue();
            micDropdown.onValueChanged.AddListener(OnMicDropdownValueChanged);

            speakerDropdown.options = new List<TMP_Dropdown.OptionData>() { new TMP_Dropdown.OptionData("1"), new TMP_Dropdown.OptionData("2��two�ΰ����ϳ����ϱ��ϳ��Դϴ�"), new TMP_Dropdown.OptionData("3��three��") };

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
        // Space Type ���� �� �� �̸� �����ϴ� �˾� ����
        public void EnableSpaceNameCanvas()
        {
            spaceTypeGroup.enabled = false;
            spaceType.enabled = false;

            spaceNameGroup.enabled = true;
            spaceName.enabled = true;
        }

        // [Canvas 2]
        // Space Name ���� �� ĳ����, �г���, ����ũ&���� On/Off �����ϴ� ĵ���� ����
        public void EnableCharacterCanvas()
        {
            // �� �̸� ���Է� �� ���� ��ư ��Ȱ��ȭ
            if (spaceNameInput.text != "")
            {
                spaceNameGroup.enabled = false;
                spaceName.enabled = false;

                characterGroup.enabled = true;
                character.enabled = true;
            }

            // ��ܿ� �ߴ� ����
            spaceNameHeadline.text = "Welcome to " + spaceNameInput.text;
        }

        // [Canvas 3]
        // ����ũ ��ư
        public void MicOnOff()
        {
            if (isMicOn == true)
            {
                micOn.enabled = false;
                micMute.enabled = true;

                micVolumeBar.SetActive(false);

                // ***** ���� ����ũ ��ɵ� ���� *****
                // ~~~~

                isMicOn = false;
            }
            else
            {
                micOn.enabled = true;
                micMute.enabled = false;

                micVolumeBar.SetActive(true);

                // ***** ���� ����ũ ��ɵ� �ѱ� *****
                // ~~~~

                isMicOn = true;
            }
        }

        // ����ũ �˾� ��ư
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

        // ī�޶� ��ư
        public void CameraOnOff()
        {
            if (isVideoOn == true)
            {
                videoOn.enabled = false;
                videoOff.enabled = true;

                // ***** ���� ī�޶� ��ɵ� ���� *****
                // ~~~~

                isVideoOn = false;
            }
            else
            {
                videoOn.enabled = true;
                videoOff.enabled = false;

                // ***** ���� ī�޶� ��ɵ� �ѱ� *****
                // ~~~~

                isVideoOn = true;
            }
        }

        // ī�޶� �˾� ��ư
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


        // [�� ���� ��û] -> �̰� Join the Gathering��ư�� ����
        public void CreateRoom()
        {
            // User NickName
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;

            RoomOptions conferenceRoom = new RoomOptions(); // ����Ʈ�� �ִ��ο� & IsVisible
            conferenceRoom.PublishUserId = true;
            PhotonNetwork.CreateRoom("Conference Room", conferenceRoom);
        }

        // �� ���� ���� ��
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        // �� ���� ���� ��
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            print("OnCreateRoomFailed: " + returnCode + ", " + message);
            JoinRoom();
        }

        // [�� ���� ��û]
        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom("Conference Room");
        }

        // �� ���� ���� ��
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            print(System.Reflection.MethodBase.GetCurrentMethod().Name);
            PhotonNetwork.LoadLevel(2);
        }

        // �� ���� ���� ��
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
