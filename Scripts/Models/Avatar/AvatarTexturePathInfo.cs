using System;
using System.IO;
using UnityEngine;

namespace Voltage.Witches.Models.Avatar
{
    /***
     * Contains path information for avatar image access
     */
    public static class AvatarTexturePathInfo
    {
        private static readonly string PATH_PREFIX = Application.persistentDataPath + "/AvatarImages";
        private static readonly string DEFAULT_PREFIX = "DefaultAvatar";
        private const string STORY_FILENAME_FORMAT = "{0}_{1}";
        private const string EXT = ".png";

        public static string GetAvatarRoot()
        {
            return PATH_PREFIX;
        }

        public static string GetAvatarPath(AvatarType avatarType)
        {
            return Path.Combine(PATH_PREFIX, avatarType.ToString()) + EXT;
        }

        public static string GetDefaultAvatarPath(AvatarType avatarType)
        {
            return DEFAULT_PREFIX + "/" + avatarType.ToString();
        }

        public static string GetStoryPath(string name, OutfitType outfitType)
        {
            string filename = GetStorySaveName(name, outfitType) + EXT;
            return Path.Combine(PATH_PREFIX, filename);
        }

        public static string GetDefaultStoryPath(string expression, OutfitType outfitType)
        {
            string filename = GetStorySaveName(expression, outfitType);
            return DEFAULT_PREFIX + "/" + filename;
        }

        public static string GetStorySaveName(string name, OutfitType outfitType)
        {
            return string.Format(STORY_FILENAME_FORMAT, GetOutfitName(outfitType), name);
        }

        public static string GetAvatarSaveName(AvatarType avatarType)
        {
            return avatarType.ToString();
        }

        public static string GetPath(string name)
        {
            return string.Format(STORY_FILENAME_FORMAT, name) + EXT;
        }


        private static string GetOutfitName(OutfitType outfitType)
        {
            return outfitType.ToString().ToLower();
        }
    }
}
