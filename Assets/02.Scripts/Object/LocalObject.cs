using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [LHE 0914]
// X키를 눌러서 한 상호작용의 결과가 X키를 누른 사람에게만 보이는 오브젝트
// ex. WebLink, Quiz, Survey, MiniGame, Whiteboard, Note, Prompt
// (오픈한 웹링크, 화이트보드, 퀴즈, 설문, 미니게임은 X를 눌러 상호작용한 사람에게만 보인다)
// (다른 사람들에게는 그 사람이 그 물체 주위에 서있는 것만 보임)
namespace Gather.Object
{
    public abstract class LocalObject : MonoBehaviour, Interactable
    {
        public bool canInteract;
        public string popupPhrase;

        // 상호작용 동작
        public virtual void Interact()
        {
            
        }

        // 플레이어가 첫 번 째로 호출하는 함수, 상호작용 가능하면 상호작용으로 연결
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

        // ex. Door 여닫힘, 현재 상호작용 가능한 상태인지
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

        // 오브젝트 이름 반환 -> 팝업 Text에 사용 가능
        public string GetName()
        {
            return popupPhrase;
        }

        // 오브젝트 아이콘 반환
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
