using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Bundles
{
    using Voltage.Witches.Exceptions;

    public class AssetBundleManager : MonoBehaviour, IAssetBundleManager
    {
        private readonly Dictionary<string, AssetBundle> _loadedBundles = new Dictionary<string, AssetBundle>();
        private readonly Dictionary<string, IEnumerator> _bundlesLoading = new Dictionary<string, IEnumerator>();

        private void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }

		public void Cleanup()
		{
			foreach (var bundlePair in _loadedBundles)
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
			
        public bool IsLoaded(string path)
        {
			path = GetSanitizedPath(path);
            return _loadedBundles.ContainsKey(path);
        }

		public bool IsCached(string path)
        {
			path = GetSanitizedPath(path);
			return Caching.IsVersionCached(path, GetBundleVersion(path));
        }

		public bool IsLoading(string path)
        {
			path = GetSanitizedPath(path);
            return _bundlesLoading.ContainsKey(path);
        }

		public Coroutine PreLoad(string path)
        {
			path = GetSanitizedPath(path);
			IEnumerator loadRoutine = Load(path, false);
			if (loadRoutine != null)
			{
				return StartCoroutine(loadRoutine);
			}
			else
			{
				return null;
			}
        }

		public IEnumerator Load(string path)
        {
            return Load(path, true);
        }

        private IEnumerator Load(string path, bool loadIfCached)
        {
			path = GetSanitizedPath(path);

            if ((IsLoaded(path)) ||
                (!loadIfCached && IsCached(path)))
            {
                return null;
            }
            else if (IsLoading(path))
            {
                return _bundlesLoading[path];
            }

            IEnumerator routine = PerformLoad(path);
            _bundlesLoading[path] = routine;
            return routine;
        }

        private IEnumerator PerformLoad(string path)
        {
            WWW request = WWW.LoadFromCacheOrDownload(path, GetBundleVersion(path));
            yield return request;

            if (!string.IsNullOrEmpty(request.error))
            {
                throw new WitchesException("Failed loading bundle " + path + ": " + request.error);
            }

            _loadedBundles[path] = request.assetBundle;
            _bundlesLoading.Remove(path);
        }

        public AssetBundle Get(string path)
        {
			path = GetSanitizedPath(path);
			if (!IsLoaded(path))
            {
                throw new WitchesException("Attempted to retrieve an unloaded bundle: " + path);
            }

            return _loadedBundles[path];
        }

        public void Unload(string path)
        {
			path = GetSanitizedPath(path);
            if (IsLoaded(path))
            {
                var bundle = Get(path);
                bundle.Unload(true);

                _loadedBundles.Remove(path);
            }
        }

		private string GetSanitizedPath(string path)
		{
			var tokens = path.Split('/');
			if (tokens.Length == 1)
			{
				return ConvertBundleName(tokens[0]);
			}
			else
			{
				var lastIndex = tokens.Length - 1;
				tokens[lastIndex] = ConvertBundleName(tokens[lastIndex]);
				return string.Join("/", tokens);
			}
		}

		private string ConvertBundleName(string rawBundle)
		{
			return rawBundle.ToLower();
		}


        private int GetBundleVersion(string path)
        {
            // placeholder for when we actually need to version assets
            return 1;
        }
    }
}

