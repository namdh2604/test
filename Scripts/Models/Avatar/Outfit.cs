using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage.Witches.Models.Avatar
{
    public class Outfit : IEnumerable<KeyValuePair<OutfitCategory, string>>
    {
        private Dictionary<OutfitCategory, string> _currentSlots;
        private AvatarManifest _manifest;

        public Outfit()
        {
            _currentSlots = new Dictionary<OutfitCategory, string>();
            _manifest = new AvatarManifest();
        }

        public Outfit(Dictionary<OutfitCategory, string> existingSlots)
        {
            _currentSlots = existingSlots;
        }

        public void Add(string item)
        {
            WearItem(item);
        }

        public void WearItem(string item)
        {
            OutfitCategory category = _manifest.GetCategoryForItem(item);
            _currentSlots[category] = item;
        }

        public void RemoveItem(string item)
        {
            OutfitCategory category = _manifest.GetCategoryForItem(item);
            if (_currentSlots.ContainsKey(category) && (_currentSlots[category] == item))
            {
                _currentSlots.Remove(category);
            }
        }

        /***
         * Retrieves the item worn in the specified category, or an empty string if no item is being worn
         */
        public string GetWornItem(OutfitCategory itemCategory)
        {
            if (!_currentSlots.ContainsKey(itemCategory))
            {
                return string.Empty;
            }

            return _currentSlots[itemCategory];
        }

        public bool IsWearingItem(string item)
        {
            OutfitCategory category = _manifest.GetCategoryForItem(item);
            if (!_currentSlots.ContainsKey(category))
            {
                return false;
            }

            return (_currentSlots[category] == item);
        }

        public void RemoveAllClothing()
        {
            _currentSlots = new Dictionary<OutfitCategory, string>();
        }

        public bool HasNoClothes()
        {
            return (_currentSlots.Count == 0);
        }

        public Dictionary<OutfitCategory, string> GetValues()
        {
            return _currentSlots;
        }

        public List<string> GetClothingValues()
        {
            List<string> result = new List<string>();
            foreach (var entry in _currentSlots)
            {
                result.Add(entry.Value);
            }

            return result;
        }

        public Dictionary<string, string> GetSerializableValues()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var pair in GetValues())
            {
                result[pair.Key.ToString()] = pair.Value;
            }

            return result;
        }

        public IEnumerator<KeyValuePair<OutfitCategory, string>> GetEnumerator()
        {
            return _currentSlots.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
