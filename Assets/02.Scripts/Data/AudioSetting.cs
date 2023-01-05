using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gather.Data
{
    //[CreateAssetMenu(fileName = "AudioSettingData", menuName = "Scriptable Object/Audio Setting Data", order = 0)]
    //[FilePath("Setting/AudioSetting.foo", FilePathAttribute.Location.PreferencesFolder)]
    /*public class AudioSetting : ScriptableSingleton<AudioSetting>
    {
        [Header("Audio Select")]
        public string SelectedDevice;
        
    }*/

    public static class AudioSetting
    {
        [Header("Audio Select")]
        public static string SelectedDevice;

    }
}