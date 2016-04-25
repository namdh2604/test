using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Exceptions;

    public enum AvatarType
    {
        Headshot,
        Fullbody,
        Story
    }

    /***
     * Manages avatar texture offset information. Can be used to retrieve and store this information
     */
    public class AvatarOffsets
    {
        // TODO: Move this information to a common class -- multiple different places reference this path prefix
        private static readonly string PATH_PREFIX = Application.persistentDataPath + "/AvatarImages";
        private static readonly string OFFSET_PATH = PATH_PREFIX + "/offsets.json";
        private static readonly string RESOURCES_PATH = "DefaultAvatar/offsets";

        private Dictionary<string, Rect> _positioningData;
        private readonly JsonConverter _rectConverter;


        public AvatarOffsets()
        {
            _positioningData = new Dictionary<string, Rect>();
            _rectConverter = new JsonRectConverter();
        }

        public void Load()
        {
            TextAsset defaultAsset;

            if (File.Exists(OFFSET_PATH))
            {
                _positioningData = Deserialize(File.ReadAllText(OFFSET_PATH));
            }
            else
            {
                defaultAsset = Resources.Load<TextAsset>(RESOURCES_PATH);
                if (defaultAsset == null)
                {
                    _positioningData = new Dictionary<string, Rect>();
                }
                else
                {
                    _positioningData = Deserialize(defaultAsset.text);
                    Resources.UnloadAsset(defaultAsset);
                }
            }
        }

        public void Save()
        {
            File.WriteAllText(OFFSET_PATH, JsonConvert.SerializeObject(_positioningData, Formatting.Indented, _rectConverter));
        }

        private Dictionary<string, Rect> Deserialize(string text)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, Rect>>(text, _rectConverter);
        }

        public void SetInfo(string imageName, Rect position)
        {
            _positioningData[imageName] = position;
        }

        public Rect GetInfo(string imageName)
        {
            if (!_positioningData.ContainsKey(imageName))
            {
                throw new WitchesException("No offsets found for: " + imageName);
            }

            return _positioningData[imageName];
        }
    }
}
