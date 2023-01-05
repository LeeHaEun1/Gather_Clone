using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// [LHE 0914]
// X키를 눌러서 한 상호작용의 결과가 모든 사용자에게 보이는 오브젝트
// ex. Chair, Door, VideoPlayer, Frame
// (다른 사람이 의자에 앉거나, 문을 여닫거나, 비디오를 재생하거나, 액자?를 보는 것을 볼 수 있다)
namespace Gather.Object
{
    public abstract class GlobalObject : MonoBehaviourPun, Interactable
    {
        public bool canInteract;
        public string popupPhrase;

        // 상호작용 동작
        public virtual void Interact()
        {
            // Note의 경우 상호작용의 결과(UI띄웠다 내렸다)하는 게 타인에게 시각적으로 보이진 않음
            if (gameObject.GetComponent<Note>())
            {
                return;
            }
            photonView.RPC("RpcGlobalObjectInteract", RpcTarget.All);
        }

        // 노트의 경우 실시간으로 상호작용 내용 반영 要
        public virtual void Update()
        {
            if (gameObject.GetComponent<Note>())
            {
                photonView.RPC("RpcGlobalObjectInteract", RpcTarget.All);
            }
        }

        // 플레이어가 첫 번 째로 호출하는 함수, 상호작용 가능하면 상호작용으로 연결
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

        // ex. Door 여닫힘, 현재 상호작용 가능한 상태인지
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

        // 오브젝트 이름 반환 -> 팝업 Text에 사용 가능
        public virtual string GetName()
        {
            return popupPhrase;
        }

        // 오브젝트 아이콘 반환
        public abstract Texture2D GetIcon();


        [PunRPC]
        public virtual void RpcGlobalObjectInteract()
        {

        }
    }
}
