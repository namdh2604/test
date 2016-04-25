using UnityEngine;
using System.IO;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Unity;

    /***
     * Responsible for writing avatar textures to disk
     */
    public class AvatarTextureWriter
    {
        public void SaveStoryTexture(string expression, OutfitType outfitType, Texture2D texture)
        {
            string path = AvatarTexturePathInfo.GetStoryPath(expression, outfitType);
            WriteTexture(path, texture);
        }

        public void SaveTexture(AvatarType avatarType, Texture2D texture)
        {
            string path = AvatarTexturePathInfo.GetAvatarPath(avatarType);
            WriteTexture(path, texture);
        }

        private void WriteTexture(string path, Texture2D texture)
        {
            File.WriteAllBytes(path, texture.EncodeToPNG());
        }
    }
}
