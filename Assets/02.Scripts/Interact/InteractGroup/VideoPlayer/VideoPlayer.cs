using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// ************ Video ������ ��ġ!!!
namespace Gather.Interact
{
    public class VideoPlayer : InteractGroup<VideoPlayer>
    {
        // Video to Play
        // �տ� UnityEngine.Video. �� ���̸� Ŭ���� VideoPlayer�� �ν�
        public UnityEngine.Video.VideoPlayer video;
        public VideoClip video2;

        public override void Awake()
        {
            commandList = new List<InteractCommand<VideoPlayer>>();
            base.Awake();
            icon = Resources.Load<Sprite>("Icons/Interact/Video");
        }

        public override int Priority { get { return (int)InteractGroupPriority.VideoPlayer; } }

        public override void SetDefaultCommands()
        {
            base.SetDefaultCommands();
            // *********** �켱���� ���� �ٽ� ���� �ʿ� *********** 
            PlayVideo playVideo = new PlayVideo() { priority = 100 };
            StopVideo stopVideo = new StopVideo() { priority = 90 };
            SyncVideo syncVideo = new SyncVideo() { priority = 80 };
            UploadVideo uploadVideo = new UploadVideo() { priority = 70 };
            playVideo.stopV = stopVideo;
            playVideo.uploadV = uploadVideo;
            stopVideo.playV = playVideo;
            stopVideo.uploadV = uploadVideo;
            uploadVideo.playV = playVideo;
            uploadVideo.stopV = stopVideo;

            AddCommand(playVideo);
            AddCommand(stopVideo);
            AddCommand(syncVideo);
            AddCommand(uploadVideo);
        }

        public override void AddCommand(InteractCommandBase command)
        {
            base.AddCommand(command);
            if (command is InteractCommand<VideoPlayer>)
            {
                InteractCommand<VideoPlayer> c = command as InteractCommand<VideoPlayer>;
                c.target = this;
                commandList.Add(c);
            }
        }
    }

    #region Commands
    [System.Serializable]
    public class PlayVideo : InteractCommand<VideoPlayer>
    {
        public StopVideo stopV;
        public SyncVideo syncV;
        public UploadVideo uploadV;

        public PlayVideo()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "������ ���";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Play Video");
            // add command
            target.video.Play();

            this.priority = 90;
            stopV.priority = 100;
        }
    }


    [System.Serializable]
    public class StopVideo : InteractCommand<VideoPlayer>
    {
        public PlayVideo playV;
        public SyncVideo syncV;
        public UploadVideo uploadV;

        public StopVideo()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "������ ����";
        }
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Stop Video");
            // add command
            target.video.Pause(); // Stop ����ؾߵǳ�...?! �ƴϸ� Pause��� Ŭ���� �߰�?!

            this.priority = 90;
            playV.priority = 100;
        }
    }

    [System.Serializable]
    public class SyncVideo : InteractCommand<VideoPlayer>
    {
        public PlayVideo playV;
        public StopVideo stopV;
        public UploadVideo uploadV;

        public SyncVideo()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "������ ��ũ ���߱�";
        }
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Sync Video");
            // add command
            //target.video.~~~~~~~~~~  Sync�� ��Ȯ�� �ǹ�?? : �ϴ� Pass
        }
    }

    [System.Serializable]
    public class UploadVideo : InteractCommand<VideoPlayer>
    {
        public PlayVideo playV;
        public StopVideo stopV;
        public SyncVideo syncV;

        public UploadVideo()
        {
            authority = 0;
            priority = 0;
            isActivate = true;
            password = "";
            isPassword = false;
            title = "������ ���ε�";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Upload Video");
            // add command
            // RawImage ������Ʈ�� Texture�� �ٲ��ִ� ������??
            // (����) �ƴϸ� TestVideo GO�� VideoPlayer ������Ʈ�� VideoClip�� �ٲ��ִ� ������?? 
            target.video.clip = target.video2;

            // (����) UploadImage�� ���� �������� ���� .mp4 clip upload�ϴ� �������
        }
    }
    #endregion
}
