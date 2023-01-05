using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using EPOOutline;
using Gather.UI.PlayerUI;

namespace Gather.Interact
{
    [RequireComponent(typeof(Outlinable))]
    public class InteractObject : MonoBehaviourPun, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public float activateDistance;
        List<InteractGroupBase> commandGroupList = new List<InteractGroupBase>();
        Outlinable outline;

        private void Awake()
        {
            foreach (InteractGroupBase group in GetComponents<InteractGroupBase>())
                commandGroupList.Add(group);

            AddCommandGroup(InteractGroup.Default);

            //AllocateViewID();
        }

        private void Start()
        {
            outline = GetComponent<Outlinable>();
            outline.enabled = false;
        }

        /// <summary>
        /// 우선 순위가 가장 높은 명령을 실행한다.
        /// </summary>
        public void Execute()
        {
            print("Execute0");
            int maxGroupPriority=int.MinValue;
            InteractCommandBase maxPriorityCommand = null;
            
            foreach (var group in commandGroupList)
            {
                print("Execute1" + group);
                if (group.priority> maxGroupPriority)
                {
                    maxGroupPriority = group.priority;
                    maxPriorityCommand = group.GetMaxPriorityCommand();
                }
            }
            
            print("maxPriorityCommand: " + maxPriorityCommand);
            if (maxPriorityCommand != null)
            {
                print("maxPriorityCommand: " + maxPriorityCommand.id);
                maxPriorityCommand.Execute();
            }
        }

        /// <summary>
        /// 명령어의 리스트를 출력하게 합니다.
        /// </summary>
        public void Expand()
        {
            Debug.LogAssertion(PlayerUI.Instance);
            print("Exoand commandGroupList.Count: " + commandGroupList.Count);
            PlayerUI.Instance.ShowCommandGroupList(commandGroupList);
        }

        public void AddCommandGroup(InteractGroup type)
        {
            switch (type)
            {
                case InteractGroup.Default:
                    DefaultGroup group = gameObject.AddComponent<DefaultGroup>();
                    commandGroupList.Add(group);
                    break;
            }
        }

        public void RemoveCommandGroup(InteractGroupBase group)
        {
            //컴포넌트도 삭제할것
            commandGroupList.Remove(group);
        }
        
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            // 아웃라인 켜주기
            outline.enabled = true;
            // 이름 켜주기?
            print("OnPointerEnter");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            // 아웃라인 꺼주기
            outline.enabled = false;
            // 이름 꺼주기?
            print("OnPointerExit");
        }
        
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            print("OnPointerClick");
            // 좌측 클릭이면 실행
            // 우측 클릭이면 명령어 리스트 출력
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Execute();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Expand();
            }
        }
    }
}