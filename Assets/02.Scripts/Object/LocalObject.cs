using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [LHE 0914]
// XŰ�� ������ �� ��ȣ�ۿ��� ����� XŰ�� ���� ������Ը� ���̴� ������Ʈ
// ex. WebLink, Quiz, Survey, MiniGame, Whiteboard, Note, Prompt
// (������ ����ũ, ȭ��Ʈ����, ����, ����, �̴ϰ����� X�� ���� ��ȣ�ۿ��� ������Ը� ���δ�)
// (�ٸ� ����鿡�Դ� �� ����� �� ��ü ������ ���ִ� �͸� ����)
namespace Gather.Object
{
    public abstract class LocalObject : MonoBehaviour, Interactable
    {
        public bool canInteract;
        public string popupPhrase;

        // ��ȣ�ۿ� ����
        public virtual void Interact()
        {
            
        }

        // �÷��̾ ù �� °�� ȣ���ϴ� �Լ�, ��ȣ�ۿ� �����ϸ� ��ȣ�ۿ����� ����
        public bool TryInteract()
        {
            if (isInteractable() == true)
            {
                Interact();
                return true;
            }
            else
            {
                return false;
            }
        }

        // ex. Door ������, ���� ��ȣ�ۿ� ������ ��������
        public bool isInteractable()
        {
            if (canInteract == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // ������Ʈ �̸� ��ȯ -> �˾� Text�� ��� ����
        public string GetName()
        {
            return popupPhrase;
        }

        // ������Ʈ ������ ��ȯ
        public abstract Texture2D GetIcon();


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
