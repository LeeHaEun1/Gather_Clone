using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

// [��ȣ�ۿ� ����]
// X ���� UI�� ���� ������ �ϴ� ���� �����Ը� ���̰�
// �������� X���� ��ȣ�ۿ�(�����۾�) ���϶��� Operational Transform Ȱ���� Google Docs�� ���� �����۾� ����
namespace Gather.Object
{
    public class Note : GlobalObject
    {
        public GameObject note; // Note UI(InputField)�� ���� ���ӿ�����Ʈ
        public TMP_InputField inputNote; // ���� �Է��� �޴� ���� inputNote.text

        // Start is called before the first frame update
        void Start()
        {
            note.SetActive(false);

            canInteract = true;

            if (PlayerPrefs.HasKey("Note"))
                inputNote.text = PlayerPrefs.GetString("Note");
        }

        // UI ��ü�� ��ȣ�ۿ��� ������ �������Ը� ������ ��
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

        // ��Ʈ �Է� ���� ���� �Լ� -> OnValueChanged�� ȣ��
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
            // ��Ȱ�� ���¶��
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
