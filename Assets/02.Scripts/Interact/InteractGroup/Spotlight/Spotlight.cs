using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;
using Gather.Network;
using Gather.Controller;
using Unity.WebRTC;

namespace Gather.Interact
{
    [System.Serializable]
    public class Spotlight : InteractGroup<Spotlight>
    {
        public override void Awake()
        {
            commandList = new List<InteractCommand<Spotlight>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/Link");
        }

        public override int Priority { get { return (int)InteractGroupPriority.WebLink; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            AddCommand(new StartSpotlight() { priority = 100 });
            AddCommand(new StopSpotlight() { priority = 10 });
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<Spotlight>)
            {
                InteractCommand<Spotlight> c = command as InteractCommand<Spotlight>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class StartSpotlight : InteractCommand<Spotlight>
    {
        public StartSpotlight()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "발표 시작";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Start Spotlight");
            // add command
            priority = 10;
            target.commandList.Find(x => x is StopSpotlight).priority = 100;

            Controller.CharacterController.Instance.player.playerConnection.EnterSpotlight();
        }
    }

    [System.Serializable]
    public class StopSpotlight : InteractCommand<Spotlight>
    {
        public StopSpotlight()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "발표 중지";
        }

        public override void Execute()
        {
            base.Execute();
            Debug.Log("Stop Spotlight");
            // add command
            priority = 10;
            target.commandList.Find(x => x is StartSpotlight).priority = 100;

            Controller.CharacterController.Instance.player.playerConnection.ExitSpotlight();
        }
    }
    #endregion
}
