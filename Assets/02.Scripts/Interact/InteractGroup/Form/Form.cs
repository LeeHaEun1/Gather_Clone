using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.Interact
{
    public class Form : InteractGroup<Form>
    {
        public override void Awake()
        {
            commandList = new List<InteractCommand<Form>>();
            base.Awake();
        }

        public override int Priority { get { return (int)InteractGroupPriority.Form; } }
      
        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            AddCommand(new OpenForm() { priority = 100 });
            AddCommand(new EditForm() { priority = 90 });
            AddCommand(new ShowResult() { priority = 10 });
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<Form>)
            {
                InteractCommand<Form> c = command as InteractCommand<Form>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class OpenForm : InteractCommand<Form>
    {
        public OpenForm()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "설문 열기";
        }

        public override void Execute()
        {
            base.Execute();
            Debug.Log("Open Form");
            // add command
            // 
        }
    }

    [System.Serializable]
    public class EditForm : InteractCommand<Form>
    {
        public EditForm()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "설문 편집";
        }

        public override void Execute()
        {
            base.Execute();
            Debug.Log("Edit Form");
            // add command
            // 
        }
    }

    [System.Serializable]
    public class ShowResult : InteractCommand<Form>
    {
        public ShowResult()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "설문 결과 확인";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Show Result");
            // add command
            // 
        }
    }
    #endregion
}
