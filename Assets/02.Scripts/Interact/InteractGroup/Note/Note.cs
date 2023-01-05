using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.Interact
{
    public class Note : InteractGroup<Note>
    {
        public GameObject note;

        public override void Awake()
        {
            commandList = new List<InteractCommand<Note>>();
            base.Awake();
        }
        public override int Priority { get { return (int)InteractGroupPriority.Note; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            AddCommand(new OpenNote() { priority = 100 });
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<Note>)
            {
                InteractCommand<Note> c = command as InteractCommand<Note>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class OpenNote : InteractCommand<Note>
    {
        public OpenNote()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "노트 열기";
        }

        public override void Execute()
        {
            base.Execute();
            Debug.Log("Open Note");
            // add command
            target.note.SetActive(true);
        }
    }
    #endregion
}
