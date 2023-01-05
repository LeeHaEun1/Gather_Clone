using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.Interact
{
    public class WhiteBoard : InteractGroup<WhiteBoard>
    {
        public override void Awake()
        {
            commandList = new List<InteractCommand<WhiteBoard>>();
            base.Awake();
        }
        public override int Priority { get { return (int)InteractGroupPriority.Frame; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            AddCommand(new OpenBoard() { priority = 100 });
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<WhiteBoard>)
            {
                InteractCommand<WhiteBoard> c = command as InteractCommand<WhiteBoard>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class OpenBoard : InteractCommand<WhiteBoard>
    {

        public OpenBoard()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "화이트 보드 열기";
        }

        public override void Execute()
        {
            base.Execute();
            Debug.Log("Open Board");
            // add command
            // 
        }
    }
    #endregion
}
