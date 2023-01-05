using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gather.Character; // Player Ŭ���� �̿��ϱ� ����
using Gather.Network;
using Gather.Data;

// [LHE 0912]
// 1. adws & ȭ��ǥ�� �յ��¿� �̵� ����(3D)
// 2. Ghost Mode (Layer ����: Ghost <-> Player�� �浹 ���ϵ��� layer collision matrix���� ����) (G key)
// 3. ������Ʈ(tag = Object)�� �����Ÿ� ���� ���� �� ��ȣ�ۿ� ��� On (=������Ʈ ��� Ȱ��ȭ) (X key) -> Player Ŭ������ OnTrigger�� Update�� �ȿ��� ����

// CharacterController������ ȣ�⸸ �ϰ� ���� ������ Player����

namespace Gather.Controller
{
    public class CharacterController : MonoBehaviour
    {
        public static CharacterController Instance;

        public Player player;

        public UnityEvent<Player> onPlayerChange;
        public GameObject note;

        // ��ȣ�ۿ��ϴ� Chair�� �ڽĿ�����Ʈ�� PlayerSeat�� Tr ���� ����Ʈ
        public List<Transform> chairTr;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            player = GetComponentInChildren<Player>();
        }


        // Update is called once per frame
        void Update()
        {
            // [1. �̵�]
            // ��Ʈ Ȱ��ȭ ���¿����� �̵� �Ұ�(��Ʈ�� adws ���� �Է½� �̵��ϴ� ���� ����)
            float v = Input.GetAxis("Vertical");
            Vector3 dirMove = new Vector3(0, 0, v);
            if (dirMove.magnitude > 0.1f && note.activeSelf == false)
            {
                player.Move(dirMove);
            }

            // [1-1. ȸ��]
            // ��Ʈ Ȱ��ȭ ���¿����� ȸ�� �Ұ�(��Ʈ�� adws ���� �Է½� ȸ���ϴ� ���� ����)
            float h = Input.GetAxis("Horizontal");
            Vector3 dirRotate = new Vector3(h, 0, 0);
            if (dirRotate.magnitude > 0.1f && note.activeSelf == false)
            {
                player.Rotate(dirRotate);
            }


            // [2. Ghost Mode]
            // Mechanism: GŰ�� ������ ���� ��� On, ���� ���� ��� Off
            if (Input.GetKey(KeyCode.G))
            {
                player.GhostModeOn();
            }
            else if (Input.GetKeyUp(KeyCode.G))
            {
                player.GhostModeOff();
            }

            if (Input.GetKeyDown(KeyCode.Keypad0) && Input.GetKey(KeyCode.LeftControl))
            {
                VideoSetting.hologramTamplateIndex = 0;
                player.GetComponent<WebRTCClient>().OnHologramTemplateChange();
            }
            if (Input.GetKeyDown(KeyCode.Keypad1) && Input.GetKey(KeyCode.LeftControl))
            {
                VideoSetting.hologramTamplateIndex = 1;
                player.GetComponent<WebRTCClient>().OnHologramTemplateChange();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2) && Input.GetKey(KeyCode.LeftControl))
            {
                VideoSetting.hologramTamplateIndex = 2;
                player.GetComponent<WebRTCClient>().OnHologramTemplateChange();
            }
        }

        public void ChangePlayer(Player player)
        {
            this.player = player;
            onPlayerChange.Invoke(player);
        }

        // [Object Interaction]
        // 1. Sit on Chair
        public void ChairInteract(Transform seatTr)
        {
            player.ChairInteract(seatTr);
        }
    }
}
