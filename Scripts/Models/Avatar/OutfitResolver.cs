using System;
using System.Collections.Generic;

namespace Voltage.Witches.Models.Avatar
{
    using Voltage.Witches.Exceptions;

    public class OutfitResolver
    {
        private Dictionary<OutfitCategory, List<string>> _categoryLayerMapping;
        private const string STANDARD_AVATAR = "standardAvatar";
        private const string AVATAR_MODEL_NAME = "standardAvatarModel";
        private const string DEFAULT_HAIR = "wavy_long_hair_darkbrown";
        private const string HAT_HAIR = "hat_hair_darkbrown";
        private const string DEFAULT_UNDERWEAR = "default_underwear";

        public OutfitResolver()
        {
            _categoryLayerMapping = new Dictionary<OutfitCategory, List<string>>(); 
            _categoryLayerMapping[OutfitCategory.Hat] = new List<string>() { AvatarLayerNames.Hats, AvatarLayerNames.Tiaras };
            _categoryLayerMapping[OutfitCategory.Glasses] = new List<string>() { AvatarLayerNames.Glasses };
            _categoryLayerMapping[OutfitCategory.Hair] = new List<string>() { AvatarLayerNames.HairMid, AvatarLayerNames.HairBot };
            _categoryLayerMapping[OutfitCategory.Earrings] = new List<string>() { AvatarLayerNames.Earrings };
            _categoryLayerMapping[OutfitCategory.Eye] = new List<string>() { AvatarLayerNames.Eye };
            _categoryLayerMapping[OutfitCategory.Mouth] = new List<string>() { AvatarLayerNames.Mouth };
            _categoryLayerMapping[OutfitCategory.Eyebrow] = new List<string>() { AvatarLayerNames.Eyebrow };
            _categoryLayerMapping[OutfitCategory.Cheek] = new List<string>() { AvatarLayerNames.Cheek };
            _categoryLayerMapping[OutfitCategory.Scarf] = new List<string>() { AvatarLayerNames.Scarves };
            _categoryLayerMapping[OutfitCategory.Bag] = new List<string>() { AvatarLayerNames.Purses };
            _categoryLayerMapping[OutfitCategory.Coat] = new List<string>() { AvatarLayerNames.HeavyCoat, AvatarLayerNames.Coats };
            _categoryLayerMapping[OutfitCategory.Bracelet] = new List<string>() { AvatarLayerNames.Bracelets };
            _categoryLayerMapping[OutfitCategory.Gloves] = new List<string>() { AvatarLayerNames.GlovesTop, AvatarLayerNames.GlovesBot };
            _categoryLayerMapping[OutfitCategory.Belt] = new List<string>() { AvatarLayerNames.Belts };
            _categoryLayerMapping[OutfitCategory.Dress] = new List<string>() { AvatarLayerNames.DressesTop, AvatarLayerNames.DressesBot };
            _categoryLayerMapping[OutfitCategory.Top] = new List<string>() { AvatarLayerNames.TopsCoverNeck, AvatarLayerNames.TopsRevealNeck };
            _categoryLayerMapping[OutfitCategory.Necklace] = new List<string>() { AvatarLayerNames.Necklaces };
            _categoryLayerMapping[OutfitCategory.Bottom] = new List<string>() { AvatarLayerNames.BottomsOverShoe, AvatarLayerNames.BottomsUnderShoe };
            _categoryLayerMapping[OutfitCategory.Shoes] = new List<string>() { AvatarLayerNames.ShoesTop, AvatarLayerNames.ShoesBot };
            _categoryLayerMapping[OutfitCategory.Socks] = new List<string>() { AvatarLayerNames.Socks };
            _categoryLayerMapping[OutfitCategory.Intimates] = new List<string>() { AvatarLayerNames.Intimates };
            _categoryLayerMapping[OutfitCategory.Tattoo] = new List<string>() { AvatarLayerNames.Tattoo };
        }

        public OutfitCategory GetCategoryForLayerName(string layerName)
        {
            foreach (var categoryPair in _categoryLayerMapping)
            {
                if (categoryPair.Value.Contains(layerName))
                {
                    return categoryPair.Key;
                }
            }

            throw new WitchesException("unrecognized layer name: " + layerName);
        }

        public string GetSubCategory(string layerName)
        {
            if (layerName == AvatarLayerNames.DressesBot)
            {
                return OutfitSubCat.FULLBODY_DRESS;
            }

            return string.Empty;
        }

        private static readonly OutfitCategory[] NAKED_CATEGORIES = new OutfitCategory[] {
                OutfitCategory.Hair, 
                OutfitCategory.Intimates, 
                OutfitCategory.Eye,
                OutfitCategory.Mouth,
                OutfitCategory.Eyebrow,
                OutfitCategory.Cheek,
                OutfitCategory.Tattoo
        };

        private bool IsPartOfNaked(OutfitCategory category)
        {
            return (Array.IndexOf(NAKED_CATEGORIES, category) != -1);
        }

        public Dictionary<string, BundledAvatarAsset> GetLayers(Outfit outfit, OutfitType outfitType)
        {
            Dictionary<string, BundledAvatarAsset> layers = new Dictionary<string, BundledAvatarAsset>();

            AvatarManifest manifest = new AvatarManifest();

            bool hasHat = false;
            bool hasHeels = HasHeels(outfit);

            var clothing = outfit.GetValues();

            foreach (var article in clothing)
            {
                OutfitCategory category = article.Key;
                if ((outfitType == OutfitType.Naked) && (!IsPartOfNaked(category)))
                {
                    continue;
                }

                if (category == OutfitCategory.Hair)
                {
                    // hair gets handled later
                    continue;
                }

                string itemId = article.Value;

                // Retrieve the associated bundle, or fail if not found
                string bundleName = manifest.GetBundleForItem(itemId);
                if (string.IsNullOrEmpty(bundleName))
                {
                    string errorMessageFmt = "Referenced item: {0} not found in global manifest";
                    throw new WitchesException(string.Format(errorMessageFmt, itemId));
                }

                List<string> displayableLayers = GetAffectedLayers(itemId, category, bundleName);
                if (displayableLayers.Count == 0)
                {
                    throw new WitchesException("invalid asset specified: " + itemId);
                }
                foreach (var layer in displayableLayers)
                {
                    if (layer == AvatarLayerNames.Socks)
                    {
                        itemId += GetSocksSuffix(hasHeels);
                    }
                    else if (layer == AvatarLayerNames.Hats)
                    {
                        hasHat = true;
                    }

                    layers[layer] = new BundledAvatarAsset(itemId, bundleName);
                }

            }

            // handle feet
            string footId = (hasHeels) ? "feet_heeled" : "feet_flat";
            layers[AvatarLayerNames.Feet] = new BundledAvatarAsset(footId, AVATAR_MODEL_NAME);

            // handle underwear
            if (!clothing.ContainsKey(OutfitCategory.Intimates))
            {
                string bundleName = manifest.GetBundleForItem(DEFAULT_UNDERWEAR);
                if (string.IsNullOrEmpty(bundleName))
                {
                    string errorMessageFmt = "Referenced item: {0} not found in global manifest";
                    throw new WitchesException(string.Format(errorMessageFmt, DEFAULT_UNDERWEAR));
                }

                List<string> displayableLayers = GetAffectedLayers(DEFAULT_UNDERWEAR, OutfitCategory.Intimates, bundleName);
                foreach (var layer in displayableLayers)
                {
                    layers[layer] = new BundledAvatarAsset(DEFAULT_UNDERWEAR, bundleName);
                }
            }

            // handle hair
            if (hasHat)
            {
                List<string> displayableLayers = GetAffectedLayers(HAT_HAIR, OutfitCategory.Hair, STANDARD_AVATAR);
                foreach (var layer in displayableLayers)
                {
                    layers[layer] = new BundledAvatarAsset(HAT_HAIR, STANDARD_AVATAR);
                }
            }
            else
            {
                string existingHair = DEFAULT_HAIR;
                clothing.TryGetValue(OutfitCategory.Hair, out existingHair);
                if (string.IsNullOrEmpty(existingHair))
                {
                    existingHair = DEFAULT_HAIR;
                }

                string bundleName = manifest.GetBundleForItem(existingHair);
                if (string.IsNullOrEmpty(bundleName))
                {
                    string errorMessageFmt = "Referenced item: {0} not found in global manifest";
                    throw new WitchesException(string.Format(errorMessageFmt, existingHair));
                }
                List<string> displayableLayers = GetAffectedLayers(existingHair, OutfitCategory.Hair, bundleName);
                foreach (var layer in displayableLayers)
                {
                    layers[layer] = new BundledAvatarAsset(existingHair, bundleName);
                }
            }

            return layers;
        }

        private List<string> GetAffectedLayers(string itemId, OutfitCategory category, string bundleName)
        {
            List<string> result = new List<string>();

            var mappedLayers = _categoryLayerMapping[category];

            foreach (var layerName in mappedLayers)
            {
                BundleManifest manifest = new BundleManifest(bundleName);
                if (manifest.LayerContainsItem(layerName, itemId))
                {
                    result.Add(layerName);
                }
            }

            return result;
        }

        private string GetSocksSuffix(bool hasHeels)
        {
            return (hasHeels) ? "__heels" : "";
        }

        private bool HasHeels(Outfit outfit)
        {
            var currentItems = outfit.GetValues();
            if (!currentItems.ContainsKey(OutfitCategory.Shoes))
            {
                return false;
            }

            string shoeName = currentItems[OutfitCategory.Shoes];
            if (shoeName.EndsWith("__heels"))
            {
                return true;
            }

            return false;
        }
    }
}
