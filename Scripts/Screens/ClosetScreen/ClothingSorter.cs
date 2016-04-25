using System;
using System.Collections.Generic;
using Voltage.Witches.Models;

using Voltage.Witches.Controllers;

/*
 * Extracts all clothing items from a player's inventory and categorizes them
 */
namespace Voltage.Witches.Screens.Closet
{
	public interface IClosetSorter
	{
		Dictionary<ScreenClothingCategory, List<Clothing>> CreateClothingDictionaryFromData (List<Item> items);
		Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> CreateClothingDictionaryFromViewModel (List<AvatarShopItemViewModel> items);
	}


	public class ClothingDefaultSorter : IClosetSorter
	{
		public virtual Dictionary<ScreenClothingCategory, List<Clothing>> CreateClothingDictionaryFromData(List<Item> items)
		{
			var result = CreateEmptyClothingDictionary();

			for(int i = 0; i < items.Count; ++i)
			{
				Clothing currentItem = items[i] as Clothing;

                ScreenClothingCategory translatedCategory = _categoryMapping[currentItem.ClothingCategory];
                if (!result.ContainsKey(translatedCategory))
                {
                    UnityEngine.Debug.LogWarning("does not contain: " + translatedCategory);
                }
                else
                {
                    result[translatedCategory].Add(currentItem);

                    result[ScreenClothingCategory.All].Add(currentItem);
                }
			}

			return result;
		}


        public virtual Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> CreateClothingDictionaryFromViewModel(List<AvatarShopItemViewModel> items)
        {
            var result = CreateEmptyViewDictionary();

            for(int i = 0; i < items.Count; ++i)
            {
                AvatarShopItemViewModel currentItem = items[i];

                ScreenClothingCategory translatedCategory = _categoryMapping[currentItem.Clothing.ClothingCategory];
                if (!result.ContainsKey(translatedCategory))
                {
                    UnityEngine.Debug.LogWarning("does not contain: " + translatedCategory);
                }
                else
                {
                    result[translatedCategory].Add(currentItem);

                    result[ScreenClothingCategory.All].Add(currentItem);
                }

            }

            return result;
        }

        private Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> CreateEmptyViewDictionary()
        {
            Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> dict = new Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>>();
            foreach (var pair in _categoryMapping)
            {
                if (!dict.ContainsKey(pair.Value))
                {
                    dict[pair.Value] = new List<AvatarShopItemViewModel>();
                }
            }
            dict[ScreenClothingCategory.All] = new List<AvatarShopItemViewModel>();

            return dict;
        }
			

		private Dictionary<ScreenClothingCategory, List<Clothing>> CreateEmptyClothingDictionary()
		{
			Dictionary<ScreenClothingCategory, List<Clothing>> dict = new Dictionary<ScreenClothingCategory, List<Clothing>>();
			foreach (var pair in _categoryMapping)
			{
				if (!dict.ContainsKey(pair.Value))
				{
					dict[pair.Value] = new List<Clothing>();
				}
			}
			dict[ScreenClothingCategory.All] = new List<Clothing>();

			return dict;
		}

		private static Dictionary<ClothingCategory, ScreenClothingCategory> _categoryMapping = new Dictionary<ClothingCategory, ScreenClothingCategory>()
		{
			{ ClothingCategory.ACCESSORIES, ScreenClothingCategory.Accessories },
			{ ClothingCategory.BAGS, ScreenClothingCategory.Accessories },
			{ ClothingCategory.BELTS, ScreenClothingCategory.Accessories },
			{ ClothingCategory.BOTTOMS, ScreenClothingCategory.Bottoms },
			{ ClothingCategory.BRACELETS, ScreenClothingCategory.Accessories },
			{ ClothingCategory.DRESSES, ScreenClothingCategory.Dresses },
			{ ClothingCategory.EARRINGS, ScreenClothingCategory.Accessories },
			{ ClothingCategory.GLASSES, ScreenClothingCategory.Accessories },
			{ ClothingCategory.GLOVES, ScreenClothingCategory.Accessories },
			{ ClothingCategory.HAIRSTYLES, ScreenClothingCategory.Hair },
			{ ClothingCategory.HATS, ScreenClothingCategory.Hats },
			{ ClothingCategory.INTIMATES, ScreenClothingCategory.Intimates },
			{ ClothingCategory.JACKETS, ScreenClothingCategory.Jackets },
			{ ClothingCategory.NECKLACES, ScreenClothingCategory.Accessories },
			{ ClothingCategory.RINGS, ScreenClothingCategory.Accessories },
			{ ClothingCategory.SHOES, ScreenClothingCategory.Shoes },
			{ ClothingCategory.SKIN, ScreenClothingCategory.Skin },
			{ ClothingCategory.SOCKS, ScreenClothingCategory.Accessories },
			{ ClothingCategory.TATTOOES, ScreenClothingCategory.Accessories },
			{ ClothingCategory.TOPS, ScreenClothingCategory.Tops }
		};
	}


	public sealed class ClothingTutorialSorter : ClothingDefaultSorter
	{
		private readonly string _clothingID;
		private readonly int _closetIndex;

		public ClothingTutorialSorter(string id, int closetIndex)
		{
			if(string.IsNullOrEmpty (id)) 
			{
				_clothingID = id;
			}

			_clothingID = id;
			_closetIndex = closetIndex;
		}

		public override Dictionary<ScreenClothingCategory, List<Clothing>> CreateClothingDictionaryFromData (List<Item> items)
		{
			Dictionary<ScreenClothingCategory, List<Clothing>> clothingMap = base.CreateClothingDictionaryFromData (items);
		
			return GetMapWithPositionedItem<Clothing> (clothingMap, (c) => c.Id == _clothingID, _closetIndex);	// will find first, can only ever have one of any clothing item
		}

		public override Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> CreateClothingDictionaryFromViewModel (List<AvatarShopItemViewModel> items)
		{
			Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> itemMap = base.CreateClothingDictionaryFromViewModel (items);

			return GetMapWithPositionedItem<AvatarShopItemViewModel> (itemMap, (c) => c.Clothing.Id == _clothingID, _closetIndex); 		// will find first, can only ever have one of any clothing item
		}

		private Dictionary<ScreenClothingCategory, List<T>> GetMapWithPositionedItem<T>(Dictionary<ScreenClothingCategory, List<T>> itemMap, Predicate<T> predicate, int closetIndex)
		{
			List<T> all = itemMap [ScreenClothingCategory.All];
			T item = all.Find (predicate);

			all.Remove (item);					// will remove first, can only ever have one of any clothing item
			all.Insert (closetIndex, item);

			return itemMap;
		}
	}
}
