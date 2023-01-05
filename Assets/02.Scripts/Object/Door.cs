using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// [LHE 0914]
// 닫히면 못 지나감(플레이어가 cc.Move로 움직이므로 여기서 구현할 필요 X)
// 열리고 닫히는 순간에는 isInteractable = false
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


        // 상호작용 내용(Interact()에 의해 호출되는 함수)
        [PunRPC]
        public override void RpcGlobalObjectInteract()
        {
            // 문이 닫힌 상태라면
            if(isOpen == false)
            {
                // 문 열고
                transform.position = Vector3.Lerp(transform.position, openedPos, 1);
                // 상태 변환
                isOpen = true;
            }
            // 문이 열린 상태라면
            else if (isOpen == true)
            {
                // 문 닫고
                transform.position = Vector3.Lerp(transform.position, originPos, 1);
                // 상태 변환
                isOpen = false;
            }
        }

        public override string GetName()
        {
            // 문이 닫힌 상태라면
            if (isOpen == false)
            {
                popupPhrase = "Press 'x' to open door";
            }
            // 문이 열린 상태라면
            else if (isOpen == true)
            {
                popupPhrase = "Press 'x' to close door";
            }
            return base.GetName();
        }

        // Update is called once per frame
        public override void Update()
        {
            // [isInteractable 관련]
            // 완전히 닫히거나 열린 상태라면
            if (transform.position == originPos || transform.position == openedPos)
            {
                canInteract = true;
            }
            // 그렇지 않다면(=열리거나 닫히는 중이라면)
            else
            {
                canInteract = false;
            }
        }
    }
}
