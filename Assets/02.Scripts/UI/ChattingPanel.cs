using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gather.Network;
using Gather.Character;
using Photon.Pun;

namespace Gather.UI
{
    public class ChattingPanel : MonoBehaviour
    {
        public Transform content;
        public InputField inputField;
        public Button submitButton;
        public PlayerConnection playerConnection;
        public ChattingMessage chattingMessagePrefab;
        InterfaceAnimManager interfaceAnimManager;

        private void Awake()
        {
            playerConnection = GetComponentInParent<PlayerConnection>();
            interfaceAnimManager = GetComponent<InterfaceAnimManager>();
        }

        public void Show()
        {
            interfaceAnimManager.startAppear();
        }

        public void Hide()
        {
            interfaceAnimManager.startDisappear();
        }

        public void ShowHide()
        {
            if (interfaceAnimManager.currentState == CSFHIAnimableState.disappeared)
            {
                Show();
            }
            else if (interfaceAnimManager.currentState == CSFHIAnimableState.appeared)
            {
                Hide();
            }
        }

        public void OnSubmit()
        {
            if (playerConnection == null || string.IsNullOrEmpty(inputField.text))
                return;

            // 보낼 대상에 따라 메시지 타입을 정하도록 한다.
            Message message = new Message();
            message.type = MessageType.Normal;
            message.sender = PhotonNetwork.LocalPlayer.NickName;
            message.receiver = "Everyone";
            message.content = inputField.text;
            message.time = System.DateTime.Now;

            // 현재 연결된 모든 플레이어들에게 메시지 전송
            bool success = true;
            foreach (WebRTCClient client in playerConnection.Connections)
            {
                Debug.LogAssertion("Send message to " + client.GetInstanceID());
                if (!client.SendMessage(message))
                    success = false;
            }
            
            if (success)
            {
                // 메시지를 보낸 플레이어에게도 메시지를 보여줌
                CreateMessage(message);
            }
            else
            {
                Debug.LogAssertion("Failed to send message");
            }
            
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }

        public void CreateMessage(Message message)
        {
            ChattingMessage chatMessage = Instantiate<ChattingMessage>(chattingMessagePrefab, content);
            chatMessage.SetMessage(message);
        }

        public void OnChangePlayer(Player player)
        {
            playerConnection = player.GetComponent<PlayerConnection>();
            player.GetComponent<WebRTCClient>().onReceiveMessage.AddListener(OnReceiveMessage);
        }

        public void OnReceiveMessage(Message message)
        {
            print($" {message.sender} {message.receiver} : {message.content}");
            CreateMessage(message);
        }

        public void SetActive()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}