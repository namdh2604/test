using UnityEngine;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Common.Utilities;

namespace Voltage.Witches.Bundles
{
    public interface IAvatarThumbResourceManager
    {
        IEnumerator LoadBundle(string bundleName);
        Coroutine PreloadBundle(string bundleName);

        void PreloadBundles();
        void UnloadBundle(string bundleName);

        Sprite GetIcon(IClothing clothing);
    }

    public class AvatarThumbResourceManager : IAvatarThumbResourceManager
    {
        private const string iconPrefix = "Avatar/Icons/";
        private const string pathFormat = "Assets/AvatarSprites/{0}/{1}.png";

        private readonly IAssetBundleManager _assetManager;
        private string BUNDLE_PREFIX = "avatarthumbs/";

        private const string DEFAULT_BUNDLE = "basic";
        private readonly string[] REQUIRED_BUNDLES = new string[] { DEFAULT_BUNDLE };

        public AvatarThumbResourceManager(IAssetBundleManager assetManager)
        {
            _assetManager = assetManager;
        }

        public IEnumerator LoadBundle(string bundleName)
        {
            return _assetManager.Load(GetPrefix() + bundleName);
        }

        public Coroutine PreloadBundle(string bundleName)
        {
            string bundlePath = GetPrefix() + bundleName;
            Coroutine result = _assetManager.PreLoad(bundlePath);
            UnityEngine.Debug.LogWarning("loaded: " + bundlePath + ": " + (result == null));
            return result;
//            return _assetManager.PreLoad(GetPrefix() + bundleName);
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

        public Sprite GetIcon(IClothing clothing)
        {
            string bundleId = clothing.BundleID;
            if (string.IsNullOrEmpty(bundleId))
            {
                bundleId = DEFAULT_BUNDLE;
            }

            string bundlePath = GetPrefix() + bundleId;

            string basePath = clothing.IconFilePath.Substring(iconPrefix.Length);

            string fullpath = string.Format(pathFormat, DEFAULT_BUNDLE, basePath);

            AssetBundle bundle = _assetManager.Get(bundlePath);
            if (!bundle.Contains(fullpath))
            {
                Debug.LogWarning("could not find asset: " + fullpath);
            }

            return bundle.LoadAsset<Sprite>(fullpath);
        }

        private string GetPrefix()
        {
            return StreamingAssetsHelper.GetWWWPath() + "/" + BUNDLE_PREFIX;
        }
    }
}

