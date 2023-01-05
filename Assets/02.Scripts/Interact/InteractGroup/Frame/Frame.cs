using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;
//using UnityEngine.Networking;
//using System.IO;


namespace Gather.Interact
{
    public class Frame : InteractGroup<Frame> //, IPunObservable
    {
        public RawImage rawImage;
        public Texture2D texture2D;
        //public UnityEvent onUploadEvent = new UnityEvent();

        public override void Awake()
        {
            commandList = new List<InteractCommand<Frame>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/Image");
            //onUploadEvent.AddListener(XXX);
        }

        public override int Priority { get { return (int)InteractGroupPriority.Frame; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            UploadImage uploadImage = new UploadImage { priority = 100 };

            AddCommand(uploadImage);
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<Frame>)
            {
                InteractCommand<Frame> c = command as InteractCommand<Frame>;
                c.target = this;
                commandList.Add(c);
            }
        }

        /*public void XXX()
        {
            texture2D = rawImage.texture as Texture2D;

            byte[] bytes = texture2D.EncodeToPNG();

            print("bytesbytesbytesbytesbytesbytesbytes");

            photonView.RPC("RpcSyncImage", RpcTarget.OthersBuffered, bytes, texture2D.width, texture2D.height); //0926 1120 이밑에 안들어옴....
            //photonView.RPC("RpcSyncImage", RpcTarget.OthersBuffered, bytes);

            print("RpcSyncImageRpcSyncImageRpcSyncImageRpcSyncImageRpcSyncImage");
        }*/

        public IEnumerator XXX()
        {
            yield return new WaitUntil(() => rawImage.texture != null);
            texture2D = rawImage.texture as Texture2D;

            byte[] bytes = texture2D.EncodeToPNG();

            print("bytesbytesbytesbytesbytesbytesbytes");

            photonView.RPC("RpcSyncImage", RpcTarget.OthersBuffered, bytes, texture2D.width, texture2D.height); //0926 1120 이밑에 안들어옴....
            //photonView.RPC("RpcSyncImage", RpcTarget.OthersBuffered, bytes);

            print("RpcSyncImageRpcSyncImageRpcSyncImageRpcSyncImageRpcSyncImage");
            yield break;
        }

        [PunRPC]
        public void RpcSyncImage(byte[] bytes, int width, int height)
        {
            print("eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");

            texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);

            //((Texture2D)rawImage.texture).LoadImage(bytes);
            //texture2D = rawImage.texture as Texture2D;

            bool canLoad = texture2D.LoadImage(bytes);
            
            if (canLoad)
            {
                rawImage.color = Color.white;
            }

            rawImage.texture = (Texture)texture2D;

            //(Texture2D)rawImage.texture = texture2D;

            //rawImage.texture = GetComponent<FileUploader>().rawImage.texture;
            //rawImage.texture = rawImage.texture;

            print("bytes" + bytes.Length);
            print("rawImage.texture" + rawImage.texture);
            print("texture2D" + texture2D);


            //    print("ffffffffffffffffffffffffffffffffffffffffff");
            //}

            //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            //{
            //    if (stream.IsWriting)
            //    {
            //        stream.SendNext(GetComponent<FileUploader>().rawImage);
            //    }
            //    else
            //    {
            //        GetComponent<FileUploader>().rawImage = (RawImage)stream.ReceiveNext();
            //    }
            //}
        }

    }

    #region Commands
    [System.Serializable]
    public class UploadImage : InteractCommand<Frame>
    {
        public UploadImage()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "이미지 업로드";
        }
        
        public override void Execute()
        {
            base.Execute();
            UnityEngine.Debug.Log("Upload Image");

            // add command
            // 1. 유저 폴더 접근
            //Process.Start(target.filePath);
            //OpenFilePanel ofp = new OpenFilePanel();
            //ofp.Apply();
            //FileManager fm = new FileManager();
            //fm.OpenFileBrowser();

            //new FileManager().OpenFileBrowser();
            //new FileUploader().OpenExplorer();

            FileUploader fu;
            fu = target.GetComponent<FileUploader>();
            fu.OpenExplorer();

            //target.XXX();
            //target.onUploadEvent.Invoke();
            target.StartCoroutine(target.XXX());

            // 2. 파일 선택하면 업로드.,,,, 어케하지??
            // (1). 이미지를 sprite로 변환
            // (2). Image 컴포넌트에 해당 이미지 할당
            // (3). set native size
        }
    }
    #endregion
}
