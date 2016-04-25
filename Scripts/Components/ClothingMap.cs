using Voltage.Witches.Lib;
using Voltage.Witches.Models;

namespace Voltage.Witches.Components
{
	public class ClothingMap 
	{
		string[] _categoryIDs;

		public ClothingMap(string[] categoryIDs)
		{
			_categoryIDs = categoryIDs;
		}

		private static readonly ClothingCategory[] _clothingCategories = new ClothingCategory[] {
			ClothingCategory.SKIN,
			ClothingCategory.INTIMATES,
			ClothingCategory.SHOES,
			ClothingCategory.BOTTOMS,
			ClothingCategory.TOPS,
			ClothingCategory.DRESSES,
			ClothingCategory.JACKETS,
			ClothingCategory.ACCESSORIES,
			ClothingCategory.HAIRSTYLES,
			ClothingCategory.HATS
		};

		public ClothingCategory GetClothingCategory(string categoryID)
		{
			int index = 0;
			for(int i = 0; i < _categoryIDs.Length; ++i)
			{
				if(_categoryIDs[i] == categoryID)
				{
					index = i;
				}
			}

			return _clothingCategories[index];
		}

	}
}