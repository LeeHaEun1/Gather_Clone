using Gather.Interact;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gather.UI.PlayerUI
{    
    public class PlayerUI : MonoBehaviour
    {
        public static PlayerUI Instance { get; private set; }

        public CommandGroupList CommandGroupList;
        public CommandList CommandList;
        public ChattingPanel ChatPanel;
        public InputPanel inputPanel;

        private void Awake()
        {
            PhotonView photonview = GetComponentInParent<PhotonView>();
            print($"PlayerUI Awake {photonview}");
            if (Instance == null && photonview != null && photonview.IsMine)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void ShowCommandGroupList(List<InteractGroupBase> interactGroups)
        {
            CommandGroupList.SetCommandGroupIcons(interactGroups);
            CommandGroupList.Show();
        }
        
        public void HideCommandGroupList()
        {
            CommandGroupList.Hide();
        }

        public void ShowCommandList<T>(List<InteractCommand<T>> commandList)
        {
            CommandList.SetCommandButtons<T>(commandList);
            CommandList.Show();
        }

        public void HideCommandList()
        {
            CommandList.Hide();
        }

        public void ShowChatPanel()
        {
            ChatPanel.Show();
        }

        public void ShowHideChatPanel()
        {
            ChatPanel.ShowHide();
        }

        public void HideInputPanel()
        {
            inputPanel.Hide();
        }

        public void ShowInputPanel()
        {
            inputPanel.Show();
        }

        public void ShowHideInputPanel()
        {
            inputPanel.ShowHide();
        }
    }
}