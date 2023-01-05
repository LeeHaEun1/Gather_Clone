using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.WebRTC;

namespace Gather.Data
{
    //[CreateAssetMenu(fileName = "VideoSettingData", menuName = "Scriptable Object/Video Setting Data", order = 0)]
    /*[FilePath("Setting/NetworkSetting.foo", FilePathAttribute.Location.PreferencesFolder)]
    public class NetworkSetting : ScriptableSingleton<NetworkSetting>
    {
        public RTCConfiguration rtcConfiguration = new RTCConfiguration
        {
            iceServers = new RTCIceServer[]
            {
                new RTCIceServer
                {
                    urls = new string[] { "stun:stun.l.google.com:19302" }
                }
            }
        };
    }*/

    public static class NetworkSetting
    {
        public static RTCConfiguration rtcConfiguration = new RTCConfiguration
        {
            iceServers = new RTCIceServer[]
            {
                new RTCIceServer
                {
                    urls = new string[] { "stun:stun.l.google.com:19302" }
                }
            }
        };
    }
}