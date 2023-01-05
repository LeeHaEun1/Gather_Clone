using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

// [상호작용 내용]
// X 눌러 UI를 띄우고 내리고 하는 것은 나에게만 보이게
// 여러명이 X눌러 상호작용(공동작업) 중일때는 Operational Transform 활용해 Google Docs와 같은 공동작업 구현
namespace Gather.Object
{
    public class Note : GlobalObject
    {
        public GameObject note; // Note UI(InputField)를 담은 게임오브젝트
        public TMP_InputField inputNote; // 실제 입력을 받는 것은 inputNote.text

        // Start is called before the first frame update
        void Start()
        {
            note.SetActive(false);

            canInteract = true;

            if (PlayerPrefs.HasKey("Note"))
                inputNote.text = PlayerPrefs.GetString("Note");
        }

        // UI 자체는 상호작용을 선택한 유저에게만 보여야 함
        public override void Interact()
        {
            if (note.activeSelf == false)
            {
                note.SetActive(true);
            }
            else if (note.activeSelf == true)
            {
                note.SetActive(false);
            }
        }

        // 노트 입력 내용 저장 함수 -> OnValueChanged시 호출
        public void Save()
        {
            PlayerPrefs.SetString("Note", inputNote.text);
        }

        [PunRPC]
        public override void RpcGlobalObjectInteract()
        {
            inputNote.text = inputNote.text;


        }


        public override string GetName()
        {
            // 비활성 상태라면
            if (note.activeSelf == false)
            {
                popupPhrase = "Press 'x' for note";
            }
            return base.GetName();
        }

        public override Texture2D GetIcon()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
        
        }
    }
}
