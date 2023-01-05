using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Gather.Character;
//using Photon.Pun;

namespace Gather.Controller
{
    public class CameraController : MonoBehaviour
    {
        public enum CameraState
        {
            First,
            Thrid,
        }
        public CameraState cameraState = CameraState.Thrid;

        public CinemachineVirtualCamera firstPersonCam; // FirstPersonCamera
        public CinemachineVirtualCamera thirdPersonCam; // ThirdPersonCamera

        //public Controller.CharacterController characterController;

        // ��Ī ��ȭ�� ���� ī�޶� ��ġ��ų Trnasform
        /*Transform thirdPersonViewTr;
        Transform thirdPersonViewLook;
        Transform firstPersonViewTr;
        Transform firstPersonViewLook;*/

        //GameObject player;

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            /*// ��Ī ��ȭ�� ���� CM vcam ��ġ��ų Trnasform ã��
            thirdPersonViewTr = characterController.transform.Find("Player(Clone)/Third Person CamPos");
            thirdPersonViewLook = characterController.transform.Find("Player(Clone)/Third Person CamLookAt");
            firstPersonViewTr = characterController.transform.Find("Player(Clone)/First Person CamPos");

            // ó������ 3��Ī���� ����
            thirdPersonCam.Follow = thirdPersonViewTr;
            thirdPersonCam.LookAt = thirdPersonViewLook;*/

            // 1��Ī ī�޶�� ��Ȱ��ȭ�ϰ� ����
            /*if (firstPersonCam != null)
            {
                firstPersonCam.enabled = false;
            }*/
        }

        // Update is called once per frame
        void Update()
        {
            // 1. Tab Key�� ������ 3��Ī <-> 1��Ī ���� ��ȯ
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchCamera();
            }

            // 1-1. 1��Ī ī�޶� ������ ���� 1��Ī ī�޶� �̵� & ȸ��
            // Update���� �̵�, ȸ�� ������, ���� �����ϴ�. FixedUpdate�� �ű�ų�, �������ϰ� �����ؾ� �Ұ̴ϴ�.
            /*if(firstPersonCam.enabled == true)
            {
                firstPersonCam.transform.position = firstPersonViewTr.position;
                firstPersonCam.transform.rotation = firstPersonViewTr.rotation;
            }*/


            // 2. CharacterController���� Player ���� �̺�Ʈ �߻� �� CamPos ���Ҵ�

        }

        /// <summary>
        /// 3��Ī <-> 1��Ī ���� ��ȯ
        /// �� ��Ī ī�޶��� �켱�� ��ȯ���� ����
        /// </summary>
        public void SwitchCamera()
        {
            if (cameraState == CameraState.Thrid)
            {
                cameraState = CameraState.First;
                firstPersonCam.Priority = 10;
                thirdPersonCam.Priority = 0;
            }
            else
            {
                cameraState = CameraState.Thrid;
                firstPersonCam.Priority = 0;
                thirdPersonCam.Priority = 10;
            }
        }

        public void OnChangePlayer(Player player)
        {
            firstPersonCam.Follow = player.firstCameraPosition;
            firstPersonCam.LookAt = player.firstCameraLookAt;

            thirdPersonCam.Follow = player.thirdCameraPosition;
            thirdPersonCam.LookAt = player.thirdCameraLookAt;
        }
    }
}
