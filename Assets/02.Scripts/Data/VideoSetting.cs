using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.WebRTC;

namespace Gather.Data
{
    public struct HologramTamplate
    {
        public float x;
        public float y;
        public float offsetY;
    }
    
    public static class VideoSetting
    {
        [Header("Video Select")]
        public static WebCamDevice SelectedDevice;
        public static int DefaultStreamWidth = 1280;
        public static int DefaultStreamHeight = 720;

        private static bool s_limitTextureSize = true;
        private static Vector2Int s_StreamSize = new Vector2Int(DefaultStreamWidth, DefaultStreamHeight);
        private static RTCRtpCodecCapability s_useVideoCodec = null;

        public static HologramTamplate[] hologramTamplates = new HologramTamplate[3] { 
            new HologramTamplate { x = 2f, y = 2f, offsetY = 0.0f },
            new HologramTamplate { x = 3f, y = 3f, offsetY = 0.0f },
            new HologramTamplate { x = 5f, y = 5f, offsetY = 0.0f }
        };
        public static int hologramTamplateIndex = 2;

        public static bool LimitTextureSize
        {
            get { return s_limitTextureSize; }
            set { s_limitTextureSize = value; }
        }

        public static Vector2Int StreamSize
        {
            get { return s_StreamSize; }
            set { s_StreamSize = value; }
        }

        public static RTCRtpCodecCapability UseVideoCodec
        {
            get { return s_useVideoCodec; }
            set { s_useVideoCodec = value; }
        }
    }
}