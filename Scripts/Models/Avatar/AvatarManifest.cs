using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using Voltage.Witches.Exceptions;

namespace Voltage.Witches.Models.Avatar
{
    public class AvatarManifest
    {
        public class AvatarItemEntry
        {
            public string Category;
            public string SubCategory;
            public string Bundle;
        }

        private const string MANIFEST_RESOURCE_PATH = "AvatarManifest";
        private const string MANIFEST_EXT = ".json";
        private const string MANIFEST_FULL_PATH = "Assets/Resources/" + MANIFEST_RESOURCE_PATH + MANIFEST_EXT;
        private Dictionary<string, AvatarItemEntry> _assetToBundleMapping;
        private JsonClothingConverter _clothingConverter;

        private JsonClothingConverter Serializer
        {
            get 
            {
                if (_clothingConverter == null)
                {
                    _clothingConverter = new JsonClothingConverter();
                }

                return _clothingConverter;
            }
        }

        public AvatarManifest()
        {
            LoadManifest();
        }

        public bool ContainsItem(string itemId)
        {
            return _assetToBundleMapping.ContainsKey(itemId);
        }

        public string GetBundleForItem(string itemId)
        {
            if (!ContainsItem(itemId))
            {
                throw new WitchesException("Unrecognized item: " + itemId);
            }

            return _assetToBundleMapping[itemId].Bundle;
        }

        public OutfitCategory GetCategoryForItem(string itemId)
        {
            if (!ContainsItem(itemId))
            {
                throw new WitchesException("Unrecognized item: " + itemId);
            }

            return (OutfitCategory)Enum.Parse(typeof(OutfitCategory), _assetToBundleMapping[itemId].Category);
        }

        public string GetSubCategoryForItem(string itemId)
        {
            if (!ContainsItem(itemId))
            {
                throw new WitchesException("Unrecognized item: " + itemId);
            }

            return _assetToBundleMapping[itemId].SubCategory;
        }

        public void UpdateWithBundleEntries(string bundleName, Dictionary<string, List<string>> entries)
        {
            RemoveExistingEntries(bundleName);
            AddNewEntries(bundleName, entries);
        }


        public void Save()
        {
            File.WriteAllText(MANIFEST_FULL_PATH, JsonConvert.SerializeObject(_assetToBundleMapping, Formatting.Indented, Serializer));
        }

        private void RemoveExistingEntries(string bundleName)
        {
            // locate all existing entries with the specified bundle id
            var existingEntries = _assetToBundleMapping.Where(x => x.Value.Bundle == bundleName).Select(x => x.Key).ToList();
            foreach (var entry in existingEntries)
            {
                _assetToBundleMapping.Remove(entry);
            }
        }

        private void AddNewEntries(string bundleName, Dictionary<string, List<string>> entries)
        {
            OutfitResolver resolver = new OutfitResolver();

            foreach (var categoryEntry in entries)
            {
                List<string> itemEntries = categoryEntry.Value;
                foreach (var itemEntry in itemEntries)
                {
                    string category = resolver.GetCategoryForLayerName(categoryEntry.Key).ToString();
                    _assetToBundleMapping[itemEntry] = new AvatarItemEntry { Bundle = bundleName, Category = category };

                    string subCategory = resolver.GetSubCategory(categoryEntry.Key);
                    if (!string.IsNullOrEmpty(subCategory))
                    {
                        _assetToBundleMapping[itemEntry].SubCategory = subCategory;
                    }
                }
            }
        }

        private void LoadManifest()
        {
            UnityEngine.TextAsset textAsset = UnityEngine.Resources.Load<UnityEngine.TextAsset>(MANIFEST_RESOURCE_PATH);

            if (textAsset != null)
            {
                _assetToBundleMapping = JsonConvert.DeserializeObject<Dictionary<string, AvatarItemEntry>>(textAsset.text);
            }
            else
            {
                _assetToBundleMapping = new Dictionary<string, AvatarItemEntry>();
            }
        }
    }
}
