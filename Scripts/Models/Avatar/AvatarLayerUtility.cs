using System;
using System.Collections.Generic;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Exceptions;

    public class AvatarLayerUtility
    {
        private const string BASE_AVATAR_NAME = "standardAvatarModel";
        private List<BundledAsset> _orderedLayers;

        public AvatarLayerUtility(Outfit outfit, OutfitType outfitType, string expression)
        {
            OutfitResolver resolver = new OutfitResolver();
            Dictionary<string, BundledAvatarAsset> layerMap = resolver.GetLayers(outfit, outfitType);
            _orderedLayers = GenerateOrderedAssets(expression, layerMap);

            _orderedLayers.Reverse();
        }

        // top is everything above the expression. If no expression is found, there is no "top"
        public List<BundledAsset> GetTopLayers()
        {
            int expressionIndex = _orderedLayers.FindIndex((layer) => layer.LayerName == AvatarLayerNames.Expressions);

            List<BundledAsset> topLayers;
            if (expressionIndex == -1)
            {
                topLayers = new List<BundledAsset>();
            }
            else
            {
                int startingIndex = expressionIndex + 1;
                int layerCount = _orderedLayers.Count - expressionIndex - 1;
                topLayers = _orderedLayers.GetRange(startingIndex, layerCount);
            }

            return topLayers;
        }

        // bottom is everything below the expression. If no expression is found, everything is considered "bottom"
        public List<BundledAsset> GetBotLayers()
        {
            int expressionIndex = _orderedLayers.FindIndex((layer) => layer.LayerName == AvatarLayerNames.Expressions);

            List<BundledAsset> bottomLayers;
            if (expressionIndex == -1)
            {
                bottomLayers = _orderedLayers;
            }
            else
            {
                int layerCount = expressionIndex;
                bottomLayers = _orderedLayers.GetRange(0, layerCount);
            }

            return bottomLayers;
        }

        public List<BundledAsset> GetAllLayers()
        {
            return _orderedLayers;
        }

        private List<BundledAsset> GenerateOrderedAssets(string expression, Dictionary<string, BundledAvatarAsset> layerMap)
        {
            var allLayers = new Dictionary<string, BundledAvatarAsset>(layerMap);

            List<BundledAsset> orderedAssets = new List<BundledAsset>();
            for (int i = 0; i < LAYER_ORDER.Count; ++i)
            {
                string layerName = LAYER_ORDER[i];
                if (layerName == AvatarLayerNames.Body)
                {
                    // Add the needed body parts -- body, correct feet
                    var entry = new BundledAsset(BASE_AVATAR_NAME, "default.prefab", AvatarLayerNames.Body);
                    orderedAssets.Add(entry);
                    allLayers.Remove(AvatarLayerNames.Body);
                }
                else if (layerName == AvatarLayerNames.Feet)
                {
                    var feet = layerMap[AvatarLayerNames.Feet];
                    var entry = new BundledAsset(BASE_AVATAR_NAME, feet.LayerName + ".prefab", AvatarLayerNames.Feet);
                    orderedAssets.Add(entry);
                    allLayers.Remove(AvatarLayerNames.Feet);
                }
                else if (layerName == AvatarLayerNames.Expressions)
                {
                    var entry = new BundledAsset(BASE_AVATAR_NAME, "expressions/" + expression + ".prefab", AvatarLayerNames.Expressions);
                    orderedAssets.Add(entry);
                    allLayers.Remove(AvatarLayerNames.Expressions);
                }
                else if (layerMap.ContainsKey(layerName))
                {
                    var assetPath = layerName + "/" + layerMap[layerName].LayerName + ".prefab";
                    var entry = new BundledAsset(layerMap[layerName].BundleName, assetPath, layerName);
                    orderedAssets.Add(entry);
                    allLayers.Remove(layerName);
                }
            }

            if (allLayers.Count > 0)
            {
                string[] errors = new string[allLayers.Count];
                int i = 0;
                foreach (var layer in allLayers)
                {
                    errors[i] = string.Format("{0}: {1}", layer.Key, layer.Value.LayerName);
                    i++;
                }

                throw new WitchesException("Unhandled Layers: " + string.Join("\n", errors));
            }

            return orderedAssets;
        }

        public static List<string> LAYER_ORDER = new List<string> {
            AvatarLayerNames.Tiaras,
            AvatarLayerNames.Hats,
            AvatarLayerNames.Glasses,
            AvatarLayerNames.HairMid,
            AvatarLayerNames.Earrings,
            AvatarLayerNames.Expressions,
            AvatarLayerNames.Eye,
            AvatarLayerNames.Mouth,
            AvatarLayerNames.Eyebrow,
            AvatarLayerNames.Cheek,
            AvatarLayerNames.Scarves,
            AvatarLayerNames.Purses,
            AvatarLayerNames.HeavyCoat,
            AvatarLayerNames.Bracelets,
            AvatarLayerNames.Coats,
            AvatarLayerNames.GlovesTop,
            AvatarLayerNames.Belts,
            AvatarLayerNames.DressesTop,
            AvatarLayerNames.TopsCoverNeck,
            AvatarLayerNames.Necklaces,
            AvatarLayerNames.TopsRevealNeck,
            AvatarLayerNames.DressesBot,
            AvatarLayerNames.BottomsOverShoe,
            AvatarLayerNames.ShoesTop,
            AvatarLayerNames.BottomsUnderShoe,
            AvatarLayerNames.ShoesBot,
            AvatarLayerNames.GlovesBot,
            AvatarLayerNames.Socks,
            AvatarLayerNames.Intimates,
            AvatarLayerNames.Tattoo,
            AvatarLayerNames.Feet,
            AvatarLayerNames.Body,
            AvatarLayerNames.HairBot
        };
    }
}
