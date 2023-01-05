using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// [LHE 0914]
// XŰ�� ������ �� ��ȣ�ۿ��� ����� ��� ����ڿ��� ���̴� ������Ʈ
// ex. Chair, Door, VideoPlayer, Frame
// (�ٸ� ����� ���ڿ� �ɰų�, ���� ���ݰų�, ������ ����ϰų�, ����?�� ���� ���� �� �� �ִ�)
namespace Gather.Object
{
    public abstract class GlobalObject : MonoBehaviourPun, Interactable
    {
        public bool canInteract;
        public string popupPhrase;

        // ��ȣ�ۿ� ����
        public virtual void Interact()
        {
            // Note�� ��� ��ȣ�ۿ��� ���(UI����� ���ȴ�)�ϴ� �� Ÿ�ο��� �ð������� ������ ����
            if (gameObject.GetComponent<Note>())
            {
                return;
            }
            photonView.RPC("RpcGlobalObjectInteract", RpcTarget.All);
        }

        // ��Ʈ�� ��� �ǽð����� ��ȣ�ۿ� ���� �ݿ� �
        public virtual void Update()
        {
            if (gameObject.GetComponent<Note>())
            {
                photonView.RPC("RpcGlobalObjectInteract", RpcTarget.All);
            }
        }

        // �÷��̾ ù �� °�� ȣ���ϴ� �Լ�, ��ȣ�ۿ� �����ϸ� ��ȣ�ۿ����� ����
        public virtual bool TryInteract()
        {
            if(isInteractable() == true)
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
        public virtual bool isInteractable()
        {
            if(canInteract == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // ������Ʈ �̸� ��ȯ -> �˾� Text�� ��� ����
        public virtual string GetName()
        {
            return popupPhrase;
        }

        // ������Ʈ ������ ��ȯ
        public abstract Texture2D GetIcon();


        [PunRPC]
        public virtual void RpcGlobalObjectInteract()
        {

        }
    }
}
