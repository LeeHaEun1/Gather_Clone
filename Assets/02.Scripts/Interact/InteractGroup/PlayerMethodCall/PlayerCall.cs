using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// PlayerCall -> CharacterController -> Player
// 의자 등 플레이어의 행동을 유발하는 오브젝트들
namespace Gather.Interact
{
    public class PlayerCall : InteractGroup<PlayerCall>
    {
        public Transform playerSeat;

        public override void Awake()
        {
            commandList = new List<InteractCommand<PlayerCall>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/PlayerCall");
        }

        public override int Priority { get { return (int)InteractGroupPriority.PlayerCall; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            AddCommand(new CallMethod() { priority = 100 });
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<PlayerCall>)
            {
                InteractCommand<PlayerCall> c = command as InteractCommand<PlayerCall>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class CallMethod : InteractCommand<PlayerCall>
    {
        public CallMethod()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "플레이어 동작: ";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Call Method");
            // add command
            Gather.Controller.CharacterController.Instance.ChairInteract(target.playerSeat);

            // 타겟(Chair)를 제외한 순수 자식(PlayerSeat)의 TR만 담는 list
            //List<Transform> list = new List<Transform>();
            //foreach(Transform tr in target.GetComponentsInChildren<Transform>())
            //{
            //    list.Add(tr);
            //}

            //// Invalid Operation Exception
            ////foreach (Transform tr in list)
            ////{
            ////    if (tr == target.transform)
            ////    {
            ////        list.Remove(tr);
            ////    }
            ////}

            ////Gather.Controller.CharacterController.Instance.chairTr.Add(target.GetComponentInChildren<Transform>()); // Chair의 tr만 담는 문제
            //Gather.Controller.CharacterController.Instance.chairTr.Add(list[1]);
            //Gather.Controller.CharacterController.Instance.ChairInteract();
        }
    }
    #endregion
}
