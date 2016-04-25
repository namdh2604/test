using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Voltage.Witches.AssetManagement
{
    using Voltage.Common.Utilities;

    public class CharacterBundleManager : MonoBehaviour, ICharacterBundleManager
    {
		const int CHAR_BUNDLE_VERSION = 1;
        Dictionary<string, AssetBundle> _loadedCharacters;
		JObject _config;

		private void Awake()
		{
			_loadedCharacters = new Dictionary<string, AssetBundle>();
		}

		public void SetConfiguration(JObject config)
		{
			Debug.Log("Setting Config from JObject");
			_config = config;
			Debug.Log("Config set");
		}

		private string GetBundleName(string charName)
		{
			return _config[charName].Value<string>("bundle");
		}

		private const string ASSET_PREFIX = "Assets/Characters";
        public T GetAsset<T>(string charName, string assetPath) where T : UnityEngine.Object
		{
			string bundleName = GetBundleName(charName);
			string fullAssetPath = ASSET_PREFIX + "/" + bundleName + "/" + assetPath;
			AssetBundle bundle = null;
			if (_loadedCharacters.TryGetValue(bundleName, out bundle))
			{
				if (!bundle.Contains(fullAssetPath))
				{
					Debug.Log ("bundle doesn't contain: " + fullAssetPath);
					return null;
				}
				T loadedAsset = bundle.LoadAsset(fullAssetPath) as T;
				return loadedAsset;
			}

			Debug.Log ("No bundle found when trying to retrieve char: " + charName + " and path: " + assetPath);

			return null;
		}

        public IEnumerator DownloadBundle(string charName)
        {
			string bundleName = GetBundleName(charName);
            AssetBundle bundle = null;
            _loadedCharacters.TryGetValue(bundleName, out bundle);
            if (bundle != null)
            {
                yield break;
            }

            // TODO: Move platform specific logic somewhere else, if possible
            #if UNITY_IOS && !UNITY_EDITOR
            // IOS devices don't accept spaces in asset names.
            string safeBundleName = bundleName.Replace(" ", "%20");
            #else
            // Newer android devices support the %20 syntax, but apparently old ones only understand spaces
            string safeBundleName = bundleName;
            #endif

			safeBundleName = safeBundleName.ToLower();

            WWW www = WWW.LoadFromCacheOrDownload(StreamingAssetsHelper.GetWWWPath() + "/Characters/" + safeBundleName, CHAR_BUNDLE_VERSION);
            yield return www;
            if (www.error != null)
            {
				UnityEngine.Debug.Log("Error with " + bundleName);
//                throw new Exception("WWW download: " + www.error);
				UnityEngine.Debug.LogWarning("WWW download: " + www.error);
			}

			if(www.assetBundle != null)
			{
				var characterBundle = www.assetBundle;
	//			characterBundle.Unload(false);
				_loadedCharacters[bundleName] = characterBundle;
				Debug.Log ("bundle set for: " + charName);
			}
			else
			{
				Debug.Log ("bundle error for: " + charName);
			}
        }

        public void Cleanup()
        {
            foreach (var bundlePair in _loadedCharacters)
            {
				if (bundlePair.Value != null)
				{
	                bundlePair.Value.Unload(true);
				}
            }
        }

		private void OnDestroy()
		{
			Cleanup ();
		}
    }
}

