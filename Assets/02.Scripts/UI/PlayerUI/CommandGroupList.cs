using Gather.Interact;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.UI.PlayerUI
{
    public class CommandGroupList : MonoBehaviour
    {
        CommandGroupIcon[] commandGroupIcons;
        InterfaceAnimManager interfaceAnimManager;

        // Start is called before the first frame update
        void Start()
        {
            commandGroupIcons = GetComponentsInChildren<CommandGroupIcon>(true);
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

        public void SetCommandGroupIcons(List<InteractGroupBase> interactGroups)
        {
            print($"SetCommandGroupIcons: {interactGroups.Count} / {commandGroupIcons.Length}");
            for (int i = 0; i < commandGroupIcons.Length; i++)
            {
                if (i < interactGroups.Count)
                {
                    commandGroupIcons[i].SetEnable(true);
                    commandGroupIcons[i].SetIcon(interactGroups[i].icon);
                    commandGroupIcons[i].onClick.RemoveAllListeners();
                    commandGroupIcons[i].onClick.AddListener(interactGroups[i].Expand);
                }
                else
                {
                    commandGroupIcons[i].SetEnable(false);
                    commandGroupIcons[i].SetIcon(null);
                }
            }
        }
    }
}