using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gather.Data;

namespace Gather.Manager
{
    public class SettingManager : MonoBehaviour
    {
        public static SettingManager Instance;
        
        //public VideoSettingData videoSetting;
        //public AudioSettingData audioSetting;

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
            DontDestroyOnLoad(gameObject);
        }
        /*
        public WebCamDevice SelectedVideoDevice
        {
            get { return videoSetting.SelectedDevice; }
            set { videoSetting.SelectedDevice = value; }
        }

        public string SelectedAudioDevice
        {
            get { return audioSetting.SelectedDevice; }
            set { audioSetting.SelectedDevice = value; }
        }

        public Vector2Int StreamSize
        {
            get { return videoSetting.StreamSize; }
            set { videoSetting.StreamSize = value; }
        }*/
    }
}
