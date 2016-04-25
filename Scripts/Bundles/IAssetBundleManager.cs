using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Bundles
{
    public interface IAssetBundleManager
    {
        bool IsLoaded(string path);
        Coroutine PreLoad(string path);
        IEnumerator Load(string path);
        AssetBundle Get(string path);
        void Unload(string path);
    }
}

