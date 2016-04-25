namespace Voltage.Witches.Models
{
	using System.Collections.Generic;

	public enum BottleBGType
	{
		LIQUID = 0,
		DRIED_2 = 1,
		DRIED_1 = 2,
		SPHERE = 3
	}

	public enum QualityTier
	{
		INFINITE = 0,
		FIRST = 1,
		SECOND = 2,
		THIRD = 3
	}

	public enum QualityBadge
	{
		NONE = 0,
		BRONZE = 1,
		SILVER = 2,
		GOLD = 3
	}

	public class Ingredient : Item
	{
		public IngredientCategory IngredientCategory { get; protected set; }
		public int Value { get; protected set; }
		public bool IsInfinite { get; protected set; }
		public string Color { get; set; }
		public int DisplayOrder { get; set; }
		public int PremiumPrice { get; set; }
		public int RegularPrice { get; set; }
		public string IconFilePath { get; set; }
		public PURCHASE_TYPE CurrencyType { get; set; }
		public BottleBGType BottleType { get; set; }
		public QualityBadge QualityBadge { get; set; }
		public QualityTier QualityTier { get; set; }

//		private static int[][] _qualityTable = new int[][]
//		{
//			new int[] {10,20,30,40},
//			new int[] {40,50,60,70},
//			new int[] {70,80,90,100}
//		};

		public Ingredient(string id, string name, IngredientCategory ingredientCategory, int value, bool isInfinite)
		: base(id)
		{
			IngredientCategory = ingredientCategory;
			Name = name;
			Value = value;
			IsInfinite = isInfinite;
			Category = ItemCategory.INGREDIENT;
//			UnityEngine.Debug.LogWarning(Value.ToString () + " is the quality");
			AssignQualityBadge();
			AssignIconPath();
		}

		bool isInFirstTier()
		{
			return ((Value > 10) && (Value <= 40));
		}

		bool isInSecondTier()
		{
			return ((Value > 40) && (Value <= 70));
		}

		bool isInThirdTier()
		{
			return ((Value > 70) && (Value <= 100));
		}

		void AssignQualityBadge()
		{
			if(isInFirstTier())
			{
				QualityTier = QualityTier.FIRST;
				QualityBadge = QualityBadge.BRONZE;
//				UnityEngine.Debug.LogWarning("Should have bronze");
			}
			else if(isInSecondTier())
			{
				QualityTier = QualityTier.SECOND;
				QualityBadge = QualityBadge.SILVER;
//				UnityEngine.Debug.LogWarning("Should have silver");
			}
			else if(isInThirdTier())
			{
				QualityTier = QualityTier.THIRD;
				QualityBadge = QualityBadge.GOLD;
//				UnityEngine.Debug.LogWarning("Should have gold");
			}
			else
			{
				QualityBadge = QualityBadge.BRONZE;
				QualityTier = QualityTier.INFINITE;
//				UnityEngine.Debug.LogWarning("Should have no bronze now");
				return;
			}

//			var qualities = _qualityTable[((int)QualityTier) - 1];
//			for(int i = 0; i < qualities.Length; ++i)
//			{
//				if(qualities[i] == Value)
//				{
//					QualityBadge = (QualityBadge)i;
//					return;
//				}
//			}
		}

		void AssignIconPath()
		{
			string tierTag = GetTierTag();
			string categoryTag = GetCategoryTagFromCategoryName();
			IconFilePath = "IngredientIcons/" + categoryTag + "_" + tierTag;
		}

		string GetCategoryTagFromCategoryName()
		{
			string categoryTag = string.Empty;

			if(IngredientCategory.Name != "Rowan Wood")
			{
				categoryTag = IngredientCategory.Name.ToLower();
			}
			else
			{
				var shortName = IngredientCategory.Name.Trim((" Wood").ToCharArray());
				categoryTag = shortName.ToLower();
			}

			return categoryTag;
		}

		string GetTierTag()
		{
			string tierTag = string.Empty;

			if(QualityTier != QualityTier.INFINITE)
			{
				tierTag = ((int)QualityTier).ToString("D2");
			}
			else
			{
				tierTag = ((int)QualityTier.FIRST).ToString("D2");
			}

			return tierTag;
		}
	}
}

