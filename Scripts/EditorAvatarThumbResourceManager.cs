#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

using Voltage.Witches.Models;

namespace Voltage.Witches.Bundles
{
	public class EditorAvatarThumbResourceManager
	{
        // this prefix is included in all icon paths sent from the server. It needs to be replaced with the actual prefix in the project
        private const string iconPrefix = "Avatar/Icons/";

        private const string pathFormat = "Assets/AvatarSprites/{0}/{1}.png";

        // temporary bundle name. Eventually the server needs to be augmented to include bundle information with each piece of clothing
        private const string BUNDLE_NAME = "basic";

        public static Sprite GetIcon(Clothing clothing)
        {
            // remove the prefix from the server
            string basePath = clothing.IconFilePath.Substring(iconPrefix.Length);

            string fullpath = string.Format(pathFormat, BUNDLE_NAME, basePath);
            return AssetDatabase.LoadAssetAtPath(fullpath, typeof(Sprite)) as Sprite;
        }

//		public static T GetAsset<T>(string bundleName, string path) where T : UnityEngine.Object
//		{
//			string fullpath = string.Format(pathFormat, bundleName, path);
//            UnityEngine.Debug.LogWarning("attempting to load: " + fullpath);
//			return AssetDatabase.LoadAssetAtPath(fullpath, typeof(T)) as T;
//		}
	}
}


#endif