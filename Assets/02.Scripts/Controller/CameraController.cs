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

        // 인칭 변화에 따라 카메라 위치시킬 Trnasform
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
            /*// 인칭 변화에 따라 CM vcam 위치시킬 Trnasform 찾기
            thirdPersonViewTr = characterController.transform.Find("Player(Clone)/Third Person CamPos");
            thirdPersonViewLook = characterController.transform.Find("Player(Clone)/Third Person CamLookAt");
            firstPersonViewTr = characterController.transform.Find("Player(Clone)/First Person CamPos");

            // 처음에는 3인칭으로 시작
            thirdPersonCam.Follow = thirdPersonViewTr;
            thirdPersonCam.LookAt = thirdPersonViewLook;*/

            // 1인칭 카메라는 비활성화하고 시작
            /*if (firstPersonCam != null)
            {
                firstPersonCam.enabled = false;
            }*/
        }

        // Update is called once per frame
        void Update()
        {
            // 1. Tab Key를 누르면 3인칭 <-> 1인칭 시점 변환
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchCamera();
            }

            // 1-1. 1인칭 카메라 시점일 때의 1인칭 카메라 이동 & 회전
            // Update에서 이동, 회전 만지면, 덜덜 떨립니다. FixedUpdate로 옮기거나, 스무스하게 보간해야 할겁니다.
            /*if(firstPersonCam.enabled == true)
            {
                firstPersonCam.transform.position = firstPersonViewTr.position;
                firstPersonCam.transform.rotation = firstPersonViewTr.rotation;
            }*/


            // 2. CharacterController에서 Player 변경 이벤트 발생 시 CamPos 재할당

        }

        /// <summary>
        /// 3인칭 <-> 1인칭 시점 변환
        /// 각 인칭 카메라의 우선도 변환으로 구현
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
