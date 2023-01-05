using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gather.Interact
{
    public class Prompt : InteractGroup<Prompt>
    {
        public Canvas canvas;

        public override void Awake()
        {
            commandList = new List<InteractCommand<Prompt>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/Prompt");
        }
        public override int Priority { get { return (int)InteractGroupPriority.Prompt; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            OpenPrompt openPrompt = new OpenPrompt { priority = 100 };
            ClosePrompt closePrompt = new ClosePrompt { priority = 10 };
            openPrompt.closeP = closePrompt;
            closePrompt.openP = openPrompt;

            AddCommand(openPrompt);
            AddCommand(closePrompt);
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<Prompt>)
            {
                InteractCommand<Prompt> c = command as InteractCommand<Prompt>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class OpenPrompt : InteractCommand<Prompt>
    {
        public OpenPrompt()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "팝업 열기";
        }
        
        public ClosePrompt closeP;
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Open Prompt");
            // add command
            target.canvas.enabled = true;
            // ******* 푸후 플레이어의 방향에 맞춰 캔버스 회전하는 기능까지 추가??
            // -> 클릭한 플레이어의 정보를 어떻게 가져오지..??

            this.priority = 10;
            closeP.priority = 100;
        }
    }

    [System.Serializable]
    public class ClosePrompt : InteractCommand<Prompt>
    {
        public ClosePrompt()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "팝업 닫기";
        }
        
        public OpenPrompt openP;
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Close Prompt");
            // add command
            target.canvas.enabled = false;

            this.priority = 10;
            openP.priority = 100;
        }
    }
    #endregion
}
