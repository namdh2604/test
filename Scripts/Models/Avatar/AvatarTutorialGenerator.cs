using System;
using UnityEngine;
using System.IO;

namespace Voltage.Witches.Models.Avatar
{
    public class AvatarTutorialGenerator
    {
        public enum TutorialOutfitPreset
        {
            Funky,
            Rebel,
            Preppy
        }

        private readonly string[] assetPaths = new string[] {
            "default_Bot",
            "default_Top",
            "Fullbody",
            "Headshot",
            "naked_Bot",
            "naked_Top",
            "offsets"
        };

        public void AssignOutfitResources(TutorialOutfitPreset preset)
        {
            Directory.CreateDirectory(AvatarTexturePathInfo.GetAvatarRoot());

            foreach (var resourcePath in assetPaths)
            {
                TextAsset asset = Resources.Load<TextAsset>("Avatar/" + preset.ToString() + "/" + resourcePath);
                string extension = (resourcePath == "offsets") ? ".json" : ".png";
                string dest = AvatarTexturePathInfo.GetAvatarRoot() + "/" + resourcePath + extension;
                File.WriteAllBytes(dest, asset.bytes);
                Resources.UnloadAsset(asset);
            }
        }
    }
}

