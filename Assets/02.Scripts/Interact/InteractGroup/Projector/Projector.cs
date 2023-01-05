using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWindowCapture;
using Gather.Network;
using Unity.WebRTC;

namespace Gather.Interact
{
    [System.Serializable]
    public class Projector : InteractGroup<Projector>
    {
        public WebRTCBroadClient webRTCBroadClient;
        public UwcWindowTexture windowTexture;
        public MeshRenderer meshRenderer;
        public RenderTexture outputTexture;

        public override void Awake()
        {
            commandList = new List<InteractCommand<Projector>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/Link");
            windowTexture = GetComponentInChildren<UwcWindowTexture>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            webRTCBroadClient = GetComponentInChildren<WebRTCBroadClient>();
            outputTexture = new RenderTexture(1920, 1080, 24);
        }

        public override int Priority { get { return (int)InteractGroupPriority.WebLink; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            AddCommand(new LinkProjector() { priority = 100 });
            AddCommand(new UnlinkProjector() { priority = 10 });
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<Projector>)
            {
                InteractCommand<Projector> c = command as InteractCommand<Projector>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class LinkProjector : InteractCommand<Projector>
    {
        public LinkProjector()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "공유 시작";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Open Link");
            // add command
            target.windowTexture.enabled = true;

            priority = 10;
            target.commandList.Find(x => x is UnlinkProjector).priority = 100;

            target.StartCoroutine(ExecuteCoroutine());
        }

        IEnumerator ExecuteCoroutine()
        {
            yield return new WaitUntil(() => target.meshRenderer.material.mainTexture != null);

            if (target.meshRenderer.material.mainTexture == null)
                target.webRTCBroadClient.VideoStreamTrack = new VideoStreamTrack(new Texture2D(1920, 1080));
            else
                target.webRTCBroadClient.VideoStreamTrack = new VideoStreamTrack(target.meshRenderer.material.mainTexture);
            
            target.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

            yield break;
        }
    }

    [System.Serializable]
    public class UnlinkProjector : InteractCommand<Projector>
    {
        public UnlinkProjector()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "공유 중지";
        }

        public override void Execute()
        {
            base.Execute();
            Debug.Log("Open Link");
            // add command
            target.windowTexture.enabled = false;
            target.meshRenderer.material.mainTexture = null;

            priority = 10;
            target.commandList.Find(x => x is LinkProjector).priority = 100;

            target.webRTCBroadClient.VideoStreamTrack = null;
        }
    }
    #endregion
}
