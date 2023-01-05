using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// ************ Video 폴더의 위치!!!
namespace Gather.Interact
{
    public class VideoPlayer : InteractGroup<VideoPlayer>
    {
        // Video to Play
        // 앞에 UnityEngine.Video. 안 붙이면 클래스 VideoPlayer로 인식
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
            // *********** 우선순위 점수 다시 생각 필요 *********** 
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
            title = "동영상 재생";
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
            title = "동영상 정지";
        }
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Stop Video");
            // add command
            target.video.Pause(); // Stop 사용해야되나...?! 아니면 Pause라는 클래스 추가?!

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
            title = "동영상 싱크 맞추기";
        }
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Sync Video");
            // add command
            //target.video.~~~~~~~~~~  Sync의 정확한 의미?? : 일단 Pass
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
            title = "동영상 업로드";
        }
        
        public override void Execute()
        {
            base.Execute();
            Debug.Log("Upload Video");
            // add command
            // RawImage 컴포넌트의 Texture를 바꿔주는 식으로??
            // (현재) 아니면 TestVideo GO의 VideoPlayer 컴포넌트의 VideoClip을 바꿔주는 식으로?? 
            target.video.clip = target.video2;

            // (수정) UploadImage와 같이 브라우저를 통해 .mp4 clip upload하는 방식으로
        }
    }
    #endregion
}
