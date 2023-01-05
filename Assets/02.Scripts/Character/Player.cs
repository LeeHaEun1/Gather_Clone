using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Gather.Object;

// [LHE 0913]
// 1. adws & ȭ��ǥ�� �յ��¿� �̵� ����(3D)
// 2. Ghost Mode (Layer ����: Ghost <-> Player�� �浹 ���ϵ��� layer collision matrix���� ����) (G key)
// 3. ������Ʈ(tag = Object)�� �����Ÿ� ���� ���� �� ��ȣ�ۿ� ��� On (=������Ʈ ��� Ȱ��ȭ) (X key)

// CharacterController������ ȣ�⸸ �ϰ� ���� ������ Player����

namespace Gather.Character
{
    public class Player : MonoBehaviourPun, IPunObservable
    {
        [Header("Move")]
        public float moveSpeed = 5;

        [Header("Ghost Mode")]
        public bool isGhostMode = false;
        CharacterController cc;

        [Header("Object Interact")]
        public GameObject interactPopup;
        public TextMeshProUGUI interactPopupText;
        public bool interactOn = false;
        public List<Interactable> nearObjectInterface = new List<Interactable>();
        // Test: Door ���ݾҴ� �ٸ� ������Ʈ���� Door�� ��ȣ�ۿ��ϴ� ���� Test �뵵
        public List<int> nearObjectID = new List<int>();
        public List<GameObject> nearObject = new List<GameObject>();

        [Header("Camera")]
        public Transform firstCameraPosition;
        public Transform firstCameraLookAt;
        public Transform thirdCameraPosition;
        public Transform thirdCameraLookAt;
        
        [Header("etc")]
        public MeshRenderer bodyRenderer;
        public Text nickName;

        public PlayerConnection playerConnection;

        public GameObject[] onlyForMine;
        public GameObject[] onlyForOthers;

        // Start is called before the first frame update
        void Start()
        {
            this.gameObject.layer = 6;
            cc = GetComponent<CharacterController>();

            playerConnection = GetComponent<PlayerConnection>();


            nickName.text = photonView.Owner.NickName;
            bodyRenderer.material.color = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
            // ???? ???? ??? ??? UI?? ?��???? ???��? ????, ????
            if (photonView.IsMine)
            {
                //interactPopup = GameObject.Find("ObjectInteractPopup");
                //interactPopupText = interactPopup.GetComponentInChildren<TextMeshProUGUI>();
                //print("============== " + interactPopup);
                //interactPopup.SetActive(false);
                //nickName.enabled = false;
                PlayerNickname nickName = GetComponent<PlayerNickname>();
                if (nickName != null)
                {
                    nickName.enabled = false;
                }

                foreach (var item in onlyForOthers)
                {
                    item.SetActive(false);
                }
            }
            else
            {
                foreach (var item in onlyForMine)
                {
                    item.SetActive(false);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // [3-1. ��ȣ�ۿ� ���� ������Ʈ���� �켱���� ����]



            // [3. ������Ʈ ��ȣ�ۿ�]
            // XŰ ���� ���콺 ��Ŭ������ ��ȣ�ۿ� (��, �ش� ������Ʈ�� ��ũ���� ��ǥ ���� �ȿ� ���콺�� �����ϴ� ���¿����� ��ȣ�ۿ� ����)
            //print("mousePosition " + Input.mousePosition);
            //Vector3 objectScreenPos = Camera.main.WorldToScreenPoint(nearObject[0].transform.position);
            //print("objectScreenPos " + objectScreenPos); // �̷����ϸ� �� �߽������θ� �����µ�..? boxextents?�� ������ϳ�?


            // ���� ���: XŰ�� ��ȣ�ۿ�
            //if (photonView.IsMine)
            //{
            //    // (2/3) Popup ���� ��ȣ�ۿ� ����
            //    if (interactPopup.activeSelf == true && Input.GetKeyDown(KeyCode.X))
            //    {
            //        interactPopup.SetActive(false);
            //        interactOn = true;

            //        if (nearObjectInterface.Count > 0)
            //        {
            //            // ***** ���� ����� ��� ������ ���� �����ְ� ����ڰ� ������ �� �ְ� �Ѵ�
            //            nearObjectInterface[0].TryInteract();
            //        }
            //    }
            //    // (3/3) ��ȣ�ۿ����� ���¿��� XŰ ������ ���� ���� & ��ȣ�ۿ� �ߴ�
            //    // (��ȣ�ۿ��� �ߴ��� �� ������Ʈ�� Interact �Լ����� ����)
            //    else if (interactOn = true && Input.GetKeyDown(KeyCode.X))
            //    {
            //        if (nearObjectInterface.Count > 0)
            //        {
            //            // ***** ���� ����� ��� ������ ���� �����ְ� ����ڰ� ������ �� �ְ� �Ѵ�
            //            nearObjectInterface[0].TryInteract();
            //        }

            //        interactOn = false;
            //    }
            //}
        }
                
        // [1. �̵�]
        // WS �Ǵ� ȭ��ǥ�� ���� �յ� �̵�
        internal void Move(Vector3 dir)
        {
            dir = transform.TransformDirection(dir);
            dir.Normalize();

            cc.SimpleMove(dir * moveSpeed);
        }

        // [1-1. ȸ��]
        // AD �Ǵ� ȭ��ǥ�� ���� ��ü ȸ��
        internal void Rotate(Vector3 dir)
        {
            dir = transform.TransformDirection(dir);
            dir.Normalize();

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);
        }


        // [2. ���� ���] 
        internal void GhostModeOn()
        {
            // Layer�� Ghost�� �ٲ��ְ�
            this.gameObject.layer = 7;

            transform.Find("Body/Drone_LODs").gameObject.layer = 7;
            // ���ɸ�� ���ش�
            isGhostMode = true;
        }
        internal void GhostModeOff()
        {
            // Layer�� �ٽ� Player�� �ٲ��ְ�
            this.gameObject.layer = 6;

            transform.Find("Body/Drone_LODs").gameObject.layer = 6;
            // ���ɸ�� ���ش�
            isGhostMode = false;
        }


        // [3. ������Ʈ ��ȣ�ۿ�] (���X)
        internal void ChairInteract(Transform seatTr)
        {
            transform.position = seatTr.position;
            transform.rotation = seatTr.rotation;


            //Vector3 beforeInteractPos = transform.position;
            //if(transform.position != Gather.Controller.CharacterController.Instance.chairTr[0].position)
            //{
            //    transform.position = Gather.Controller.CharacterController.Instance.chairTr[0].position;
            //    transform.rotation = Gather.Controller.CharacterController.Instance.chairTr[0].rotation;
            //}
            //// ������ ��� �����ϴ� Ű ������ �ڿ��� ���ڿ��� ������ �� �����Ƿ� ���� �ʿ���µ�
            //else
            //{
            //    transform.position = beforeInteractPos;
            //    Gather.Controller.CharacterController.Instance.chairTr.RemoveAt(0);
            //}
        }




        // [X. ������Ʈ ��ȣ�ۿ�] (���X)
        // (1/3) Trigger
        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
                return;
            Interactable Interactable =  other.gameObject.GetComponent<Interactable>();
            if(Interactable != null && interactOn == false)
            {
                interactPopupText.text = Interactable.GetName(); // ***** ���⼭ �־��ָ� ���� �� ���� ������ �������� �� ����� ��Ī �ȵǴ� ���� �߻� �����ϱ� �� ��..?
                interactPopup.SetActive(true);
                nearObjectInterface.Add(Interactable);

                nearObjectID.Add(other.gameObject.GetInstanceID());
                nearObject.Add(other.gameObject);
            }

        }
        private void OnTriggerExit(Collider other)
        {
            if (!photonView.IsMine)
                return;
            Interactable Interactable =  other.gameObject.GetComponent<Interactable>();
            // Door ���ݾҴ� �ٸ� ������Ʈ���� Door�� ��ȣ�ۿ��ϴ� ���� �ذ� ���� �ּ�ó��
            if (Interactable != null /*&& interactOn == false*/)
            {
                interactPopup.SetActive(false);
                nearObjectInterface.Remove(Interactable);

                nearObjectID.Remove(other.gameObject.GetInstanceID());
                nearObject.Remove(other.gameObject);
            }
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                transform.position = (Vector3)stream.ReceiveNext();
                transform.rotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
