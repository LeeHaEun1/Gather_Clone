using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

// Car, Door
namespace Gather.Interact
{
    public class Animation : InteractGroup<Animation>
    {
        public Animator anim;

        public override void Awake()
        {
            commandList = new List<InteractCommand<Animation>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/Animate");
        }

        public override int Priority { get { return (int)InteractGroupPriority.Animation; } }
        
        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            PlayAnimation playAnimation = new PlayAnimation() { priority = 100 };
            StopAnimation stopAnimation = new StopAnimation() { priority = 10 };
            playAnimation.stopAnim = stopAnimation;
            stopAnimation.playAnim = playAnimation;

            AddCommand(playAnimation);
            AddCommand(stopAnimation);
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<Animation>)
            {
                InteractCommand<Animation> c = command as InteractCommand<Animation>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    // Door: Open, Car: Rotate(?)
    #region Commands
    [System.Serializable]
    public class PlayAnimation : InteractCommand<Animation>
    {

        public StopAnimation stopAnim;

        public PlayAnimation()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "애니메이션 시작";
            
        }

        public override void Execute()
        {
            base.Execute();
            Debug.Log("Play Animation");
            // add command
            target.anim.SetTrigger("Play");
            //target.anim.SetInteger("Anim", 0);

            //photonView.RPC(nameof(RpcSetTrigger), RpcTarget.Others);

            this.priority = 10;
            stopAnim.priority = 100;
        }

        //[PunRPC]
        //public void RpcSetTrigger()
        //{
        //    //target.anim.SetTrigger("Play");
        //}
    }


    // Door: Close, Car: Pause
    [System.Serializable]
    public class StopAnimation : InteractCommand<Animation>
    {
        public PlayAnimation playAnim;
        
        public StopAnimation()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "애니메이션 정지";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Stop Animation");
            // add command
            target.anim.SetTrigger("Stop");
            //target.anim.SetInteger("Anim", 1);

            this.priority = 10;
            playAnim.priority = 100;
        }
    }
    #endregion

    
}
