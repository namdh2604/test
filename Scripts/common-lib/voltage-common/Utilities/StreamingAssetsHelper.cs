using System;
using UnityEngine;

namespace Voltage.Common.Utilities
{
    public static class StreamingAssetsHelper
    {
        public static string GetPath()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            string result = Application.dataPath + "!/assets";
#elif UNITY_IOS && !UNITY_EDITOR
            string result = Application.dataPath + "/Raw";
#else
            string result = Application.dataPath + "/StreamingAssets";
#endif
            return result;
        }

        public static string GetWWWPrefix()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            string prefix = "jar:file://";
#elif UNITY_IOS && !UNITY_EDITOR
            string prefix = "file://";
#else
            string prefix = "file://";
#endif
            return prefix;
        }

        public static string GetWWWPath()
        {
            return GetWWWPrefix() + GetPath();
        }
    }
}

