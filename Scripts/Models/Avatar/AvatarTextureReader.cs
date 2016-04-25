using System.IO;
using UnityEngine;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Unity;

    /***
     * Responsible for reading avatar textures from disk
     */
    public class AvatarTextureReader
    {
        public WrappedTexture GetTexture(AvatarType avatarType)
        {
            string path = AvatarTexturePathInfo.GetAvatarPath(avatarType);
            WrappedTexture tex = LoadTexture(path);

            if (tex != null)
            {
                return tex;
            }
            else
            {
                path = AvatarTexturePathInfo.GetDefaultAvatarPath(avatarType);
                return WrappedTexture.CreateFromResources(path);
            }
        }

        public WrappedTexture GetStoryTexture(string name, OutfitType outfitType)
        {
            string path = AvatarTexturePathInfo.GetStoryPath(name, outfitType);
            WrappedTexture tex = LoadTexture(path);

            if (tex != null)
            {
                return tex;
            }
            else
            {
                path = AvatarTexturePathInfo.GetDefaultStoryPath(name, outfitType);
                return WrappedTexture.CreateFromResources(path);
            }
        }

        private WrappedTexture LoadTexture(string path)
        {
            WrappedTexture tex = null;

            if (File.Exists(path))
            {
                tex = WrappedTexture.Create(TextureFormat.ARGB32, false);
                tex.Texture.wrapMode = TextureWrapMode.Clamp;
                tex.Texture.LoadImage(File.ReadAllBytes(path));
            }

            return tex;
        }
    }
}
