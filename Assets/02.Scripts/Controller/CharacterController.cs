using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gather.Character; // Player 클래스 이용하기 위해
using Gather.Network;
using Gather.Data;

// [LHE 0912]
// 1. adws & 화살표로 앞뒤좌우 이동 가능(3D)
// 2. Ghost Mode (Layer 변경: Ghost <-> Player는 충돌 안하도록 layer collision matrix에서 설정) (G key)
// 3. 오브젝트(tag = Object)에 일정거리 내로 접근 시 상호작용 기능 On (=오브젝트 기능 활성화) (X key) -> Player 클래스의 OnTrigger와 Update문 안에서 구현

// CharacterController에서는 호출만 하고 실제 구현은 Player에서

namespace Gather.Controller
{
    public class CharacterController : MonoBehaviour
    {
        public static CharacterController Instance;

        public Player player;

        public UnityEvent<Player> onPlayerChange;
        public GameObject note;

        // 상호작용하는 Chair의 자식오브젝트인 PlayerSeat의 Tr 담을 리스트
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
            // [1. 이동]
            // 노트 활성화 상태에서는 이동 불가(노트에 adws 글자 입력시 이동하는 현상 제거)
            float v = Input.GetAxis("Vertical");
            Vector3 dirMove = new Vector3(0, 0, v);
            if (dirMove.magnitude > 0.1f && note.activeSelf == false)
            {
                player.Move(dirMove);
            }

            // [1-1. 회전]
            // 노트 활성화 상태에서는 회전 불가(노트에 adws 글자 입력시 회전하는 현상 제거)
            float h = Input.GetAxis("Horizontal");
            Vector3 dirRotate = new Vector3(h, 0, 0);
            if (dirRotate.magnitude > 0.1f && note.activeSelf == false)
            {
                player.Rotate(dirRotate);
            }


            // [2. Ghost Mode]
            // Mechanism: G키를 누르는 동안 모드 On, 떼는 순간 모드 Off
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
