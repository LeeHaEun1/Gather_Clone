using Gather.Interact;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.UI.PlayerUI
{
    public class CommandList : MonoBehaviour
    {
        CommandButton[] commandButtons;
        InterfaceAnimManager interfaceAnimManager;

        // Start is called before the first frame update
        void Awake()
        {
            commandButtons = GetComponentsInChildren<CommandButton>(false);
            interfaceAnimManager = GetComponent<InterfaceAnimManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Show()
        {
            interfaceAnimManager.startAppear();
        }

        public void Hide()
        {
            interfaceAnimManager.startDisappear();
        }

        public void SetCommandButtons<T>(List<InteractCommand<T>> commandList)
        {
            print($"SetCommandButtons: {commandList.Count} / {commandButtons.Length}");
            for (int i = 0; i < commandButtons.Length; i++)
            {
                if (i < commandList.Count)
                {
                    commandButtons[i].SetEnable(true);
                    commandButtons[i].SetText(commandList[i].title);
                    commandButtons[i].onClick.RemoveAllListeners();
                    commandButtons[i].onClick.AddListener(commandList[i].Execute);
                }
                else
                {
                    commandButtons[i].SetEnable(false);
                    commandButtons[i].SetText(null);
                }
            }
        }
    }
}