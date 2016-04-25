using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Bundles
{
    using Voltage.Common.Utilities;

    public interface IAvatarResourceManager
    {
        IEnumerator LoadBundle(string bundleName);
        Coroutine PreloadBundle(string bundleName);
        void PreloadBundles();
        void UnloadBundle(string bundleName);

		T GetAsset<T>(string bundleName, string path) where T : UnityEngine.Object;
    }

    public class AvatarResourceManager : IAvatarResourceManager
    {
        private readonly IAssetBundleManager _assetManager;
        private string AVATAR_BUNDLE_PREFIX = "Avatar/";

        private readonly string[] REQUIRED_BUNDLES = new string[] { "standardAvatarModel", "standardAvatar" };

        public static AvatarResourceManager Create()
        {
            GameObject resourceManagerGO = new GameObject("ResourceManager");
            var resourceManager = resourceManagerGO.AddComponent<AssetBundleManager>();
            return new AvatarResourceManager(resourceManager);
        }

        public AvatarResourceManager(IAssetBundleManager assetManager)
        {
            _assetManager = assetManager;
        }

        public IEnumerator LoadBundle(string bundleName)
        {
            return _assetManager.Load(GetPrefix() + bundleName);
        }

        public Coroutine PreloadBundle(string bundleName)
        {
            return _assetManager.PreLoad(GetPrefix() + bundleName);
        }

        public void PreloadBundles()
        {
            foreach (var bundleName in REQUIRED_BUNDLES)
            {
                PreloadBundle(bundleName);
            }
        }

        public void UnloadBundle(string bundleName)
        {
            _assetManager.Unload(GetPrefix() + bundleName);
        }

		public T GetAsset<T>(string bundleName, string path) where T : UnityEngine.Object
        {
            AssetBundle bundle = _assetManager.Get(GetPrefix() + bundleName);
			string fullpath = GetAssetPath(path, bundleName);
			if (!bundle.Contains(fullpath))
			{
				Debug.LogWarning("could not find asset: " + fullpath);
			}
			return bundle.LoadAsset(GetAssetPath(path, bundleName)) as T;
        }

		private string GetAssetPath(string path, string bundleName)
		{
			string prefix = "Assets/AvatarAssets/" + bundleName;
			if (path.EndsWith(".prefab"))
			{
				prefix += "/prefabs";
			}

			return prefix + "/" + path;
		}

        private string GetPrefix()
        {
            return StreamingAssetsHelper.GetWWWPath() + "/" + AVATAR_BUNDLE_PREFIX;
        }
    }
}

