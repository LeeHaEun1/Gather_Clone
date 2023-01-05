using Gather.UI.PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

namespace Gather.Interact
{
    public enum InteractGroup
    {
        Default,
        WebLink,
        Form,
        WhiteBoard,
        Note,
        Prompt,
        VideoPlayer,
        Frame,
        Animation,
        PlayerCall
    }

    /// <summary>
    /// 인터렉션 그룹의 우선순위 정의
    /// 높은 순위의 그룹이 우선적으로 실행된다.
    /// </summary>
    public enum InteractGroupPriority
    {
        Default = 0,
        WebLink=1,
        Form=2,
        WhiteBoard=3,
        Note=4,
        Prompt=10,
        VideoPlayer=5,
        Frame=6,
        Animation=7,
        PlayerCall=8
    }

    [System.Serializable]
    public abstract class InteractGroupBase : MonoBehaviourPunCallbacks // 1003
    {
        public InteractObject parent;
        public Sprite icon;
        public int id;
        public int priority;
        public UnityEvent onExecute;

        public virtual void Awake()
        {
            parent = GetComponent<InteractObject>();
            SetDefaultCommands();
        }
                
        public virtual void SetDefaultCommands() { }
        
        public abstract bool HasChild { get; }
        public abstract int Priority { get; }

        public abstract void Expand();

        public abstract void AddCommand(InteractCommandBase command);

        public abstract InteractCommandBase GetMaxPriorityCommand();
    }

    [System.Serializable]
    public abstract class InteractGroup<T> : InteractGroupBase
    {
        public List<InteractCommand<T>> commandList = new List<InteractCommand<T>>();
        
        public override bool HasChild
        {
            get { return commandList.Count > 0; }
        }
        public override abstract int Priority { get; }

        public override void AddCommand(InteractCommandBase command) { }

        /// <summary>
        /// 우선 순위가 높은 순서로 명령을 정렬하여 출력해줍니다.
        /// </summary>
        public override void Expand() 
        {
            print("Expand List");
            PlayerUI.Instance.ShowCommandList<T>(commandList);
        }

        public override InteractCommandBase GetMaxPriorityCommand()
        {
            InteractCommandBase max = null;
            // get max priority command
            foreach (var command in commandList)
            {
                if (max == null)
                    max = command;
                else
                {
                    if (command.CompareTo(max) > 0)
                        max = command;
                }
            }
            if (max != null)
                return max;
            else
                return null;
        }
    }
}
