using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gather.Interact
{
    [System.Serializable]
    public class WebLink : InteractGroup<WebLink>
    {
        public string url = "https://www.google.com/";

        public override void Awake()
        {
            commandList = new List<InteractCommand<WebLink>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/Link");
        }

        public override int Priority { get { return (int)InteractGroupPriority.WebLink; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            AddCommand(new OpenLink() { priority = 100 });
            AddCommand(new EditLink() { priority = 10 });
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<WebLink>)
            {
                InteractCommand<WebLink> c = command as InteractCommand<WebLink>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class OpenLink : InteractCommand<WebLink>
    {

        public OpenLink()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "링크 열기";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Open Link");
            // add command
            Application.OpenURL(target.url);
        }
    }


    [System.Serializable]
    public class EditLink : InteractCommand<WebLink>
    {

        public EditLink()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "링크 수정";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Edit Link");
            // add command
            target.url = "https://www.google.com/search?q=" + Random.Range(0, 100).ToString();
        }
    }
    #endregion
}
