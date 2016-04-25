#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Voltage.Witches.Bundles
{
    /* A class intended to be used only by the editor, which mimics the behavior of loading resources from an asset bundle */
	public class EditorAvatarResourceManager : IAvatarResourceManager
	{
		public IEnumerator LoadBundle(string bundleName)
		{
			return null;
		}

		public Coroutine PreloadBundle(string bundleName)
		{
			return null;
		}

		public void PreloadBundles()
		{
			;
		}

		public void UnloadBundle(string bundleName)
		{
			;
		}

        private const string prefabPathFormat = "Assets/AvatarAssets/{0}/Prefabs/{1}";
		public T GetAsset<T>(string bundleName, string path) where T : UnityEngine.Object
		{
            string fullpath = string.Format(prefabPathFormat, bundleName, path);
			return AssetDatabase.LoadAssetAtPath (fullpath, typeof(T)) as T;
		}
	}
}

#endif