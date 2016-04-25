using System;
using UnityEngine;

namespace Voltage.Witches.Unity
{
    /***
     * A wrapped around a texture to facilitate memory management
     */
    public class WrappedTexture : IDisposable
    {
        public Texture2D Texture { get; protected set; }

        /***
         * Creates a new texture, loaded from Resources
         */
        public static WrappedTexture CreateFromResources(string path)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(path);
//            Texture2D texture = Resources.Load<Texture2D>(path);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.LoadImage(textAsset.bytes);

            Resources.UnloadAsset(textAsset);
            return new WrappedTexture(texture, true);
        }

        /***
         * Creates a texture which will need to be loaded manually with image damage
         */
        public static WrappedTexture Create(TextureFormat format, bool useMipmaps = false)
        {
            Texture2D texture = new Texture2D(2, 2, format, useMipmaps);
            return new WrappedTexture(texture, false);
        }

        protected WrappedTexture(Texture2D texture, bool isAsset)
        {
            Texture = texture;
        }

        public void Dispose()
        {
            DisposeTexture();
        }

        private void DisposeTexture()
        {
            if (Texture == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(Texture);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(Texture);
            }

            Texture = null;
        }
    }
}
