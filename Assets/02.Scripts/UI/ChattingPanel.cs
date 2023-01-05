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

            // ���� ��� ���� �޽��� Ÿ���� ���ϵ��� �Ѵ�.
            Message message = new Message();
            message.type = MessageType.Normal;
            message.sender = PhotonNetwork.LocalPlayer.NickName;
            message.receiver = "Everyone";
            message.content = inputField.text;
            message.time = System.DateTime.Now;

            // ���� ����� ��� �÷��̾�鿡�� �޽��� ����
            bool success = true;
            foreach (WebRTCClient client in playerConnection.Connections)
            {
                Debug.LogAssertion("Send message to " + client.GetInstanceID());
                if (!client.SendMessage(message))
                    success = false;
            }
            
            if (success)
            {
                // �޽����� ���� �÷��̾�Ե� �޽����� ������
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