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
        /// �켱 ������ ���� ���� ����� �����Ѵ�.
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
        /// ��ɾ��� ����Ʈ�� ����ϰ� �մϴ�.
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
            //������Ʈ�� �����Ұ�
            commandGroupList.Remove(group);
        }
        
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            // �ƿ����� ���ֱ�
            outline.enabled = true;
            // �̸� ���ֱ�?
            print("OnPointerEnter");
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            // �ƿ����� ���ֱ�
            outline.enabled = false;
            // �̸� ���ֱ�?
            print("OnPointerExit");
        }
        
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            print("OnPointerClick");
            // ���� Ŭ���̸� ����
            // ���� Ŭ���̸� ��ɾ� ����Ʈ ���
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