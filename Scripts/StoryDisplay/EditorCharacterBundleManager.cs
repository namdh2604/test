#if UNITY_EDITOR

using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using Voltage.Witches.AssetManagement;

namespace Voltage.Witches.AssetManagement
{
    using Voltage.Witches.Exceptions;

    public class EditorCharacterBundleManager : ICharacterBundleManager
	{
		private JObject _config;

		public void SetConfiguration(JObject config)
		{
			_config = config;
		}

		public IEnumerator DownloadBundle(string charName)
		{
			yield return null;
		}

		public T GetAsset<T>(string charName, string assetPath) where T : UnityEngine.Object
		{
			string bundleName = _config[charName].Value<string>("bundle");
			string basePath = "Assets/Characters/" + bundleName;
            string fullPath = basePath + "/" + assetPath;
            T asset = AssetDatabase.LoadAssetAtPath(basePath + "/" + assetPath, typeof(T)) as T;

            if (asset == null)
            {
                throw new MissingAssetException(fullPath);
            }

            return asset;
		}

        public void Cleanup()
        {
            Resources.UnloadUnusedAssets();
        }
	}
}

#endif