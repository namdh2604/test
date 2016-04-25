using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Exceptions;

    public class BundleManifest
    {
        Dictionary<string, List<string>> _assetCategoryMapping;

        const string MANIFEST_ROOT = "Assets/StreamingAssets";
        const string MANIFEST_EXT = ".json";

        public BundleManifest(string bundleId)
        {
            Load(bundleId);
        }

        public List<string> GetLayerContents(string layerName)
        {
            if (!_assetCategoryMapping.ContainsKey(layerName))
            {
                throw new WitchesException("No Layer: " + layerName);
            }

            return _assetCategoryMapping[layerName];
        }

        public bool LayerContainsItem(string layerName, string itemName)
        {
            if (!_assetCategoryMapping.ContainsKey(layerName))
            {
                return false;
            }

            List<string> allItems = _assetCategoryMapping[layerName];
            return allItems.Contains(itemName);
        }

        private void Load(string bundleId)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(bundleId);
            _assetCategoryMapping = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(textAsset.text);
        }
    }
}
