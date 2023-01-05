using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.Interact
{
    [System.Serializable]
    public class DefaultGroup : InteractGroup<DefaultGroup>
    {
        public override void Awake()
        {
            commandList = new List<InteractCommand<DefaultGroup>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/Default");
        }
        private void Start()
        {
            //commandList.Add(new DeleteObjectCommand());
        }

        public override int Priority { get { return (int)InteractGroupPriority.Default; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            AddCommand(new AddCommandCommand() { priority = 100 });
            AddCommand(new DeleteObjectCommand() { priority = 10 });
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<DefaultGroup>)
                commandList.Add(command as InteractCommand<DefaultGroup>);
        }

    }

    #region Commands
    [System.Serializable]
    public class AddCommandCommand : InteractCommand<DefaultGroup>
    {

        public AddCommandCommand()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "기능 추가";
        }

        public override void Execute()
        {
            base.Execute();
            // add command
        }
    }


    [System.Serializable]
    public class DeleteObjectCommand : InteractCommand<DefaultGroup>
    {

        public DeleteObjectCommand()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "오브젝트 제거";
        }

        public override void Execute()
        {
            base.Execute();
            // add command
        }
    }
    #endregion
}
