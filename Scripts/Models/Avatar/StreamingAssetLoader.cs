using System;
using System.Collections;
using UnityEngine;
using Voltage.Common.Utilities;

namespace Voltage.Witches.Models.Avatar
{
    public class StreamingAssetLoader : MonoBehaviour
    {
        private static StreamingAssetLoader _instance;

        public static StreamingAssetLoader Instance {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("StreamingAssetLoader");
                    _instance = go.AddComponent<StreamingAssetLoader>();
                }

                return _instance;
            }
        }

        public void LoadText(string path, Action<string> handler)
        {
            StartCoroutine(LoadSynchronousRoutine(path, handler));
        }

        private IEnumerator LoadSynchronousRoutine(string path, Action<string> handler)
        {
            string fullpath = StreamingAssetsHelper.GetWWWPath() + "/" + path;
            WWW www = new WWW(fullpath);

            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                throw new Exception(www.error);
            }

            handler(www.text);
        }
    }
}

