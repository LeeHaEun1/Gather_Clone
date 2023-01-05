using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// [LHE 0914]
// ������ �� ������(�÷��̾ cc.Move�� �����̹Ƿ� ���⼭ ������ �ʿ� X)
// ������ ������ �������� isInteractable = false
namespace Gather.Object
{
    public class Door : GlobalObject
    {
        public bool isOpen = false;
        Vector3 originPos;
        Vector3 openedPos;
        public float doorSpeed = 10;
        
        // Start is called before the first frame update
        void Start()
        {
            originPos = transform.position;
            openedPos = transform.position + transform.right * (-4);
        }


        public override Texture2D GetIcon()
        {
            throw new System.NotImplementedException();
        }


        // ��ȣ�ۿ� ����(Interact()�� ���� ȣ��Ǵ� �Լ�)
        [PunRPC]
        public override void RpcGlobalObjectInteract()
        {
            // ���� ���� ���¶��
            if(isOpen == false)
            {
                // �� ����
                transform.position = Vector3.Lerp(transform.position, openedPos, 1);
                // ���� ��ȯ
                isOpen = true;
            }
            // ���� ���� ���¶��
            else if (isOpen == true)
            {
                // �� �ݰ�
                transform.position = Vector3.Lerp(transform.position, originPos, 1);
                // ���� ��ȯ
                isOpen = false;
            }
        }

        public override string GetName()
        {
            // ���� ���� ���¶��
            if (isOpen == false)
            {
                popupPhrase = "Press 'x' to open door";
            }
            // ���� ���� ���¶��
            else if (isOpen == true)
            {
                popupPhrase = "Press 'x' to close door";
            }
            return base.GetName();
        }

        // Update is called once per frame
        public override void Update()
        {
            // [isInteractable ����]
            // ������ �����ų� ���� ���¶��
            if (transform.position == originPos || transform.position == openedPos)
            {
                canInteract = true;
            }
            // �׷��� �ʴٸ�(=�����ų� ������ ���̶��)
            else
            {
                canInteract = false;
            }
        }
    }
}
