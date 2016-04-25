using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Voltage.Witches.Components;

namespace Voltage.Witches.Models
{
	public interface IClothing
	{
		string Id { get; }
		string Name { get; }
		ItemCategory Category { get; }
		string Description { get; }
		ClothingCategory ClothingCategory { get; }
		string Layer_Name { get; }
		int LayerID { get; }
		string BundleID { get; }
		string FilePath { get; }
		string IconFilePath { get; }
		string ItemID { get; }
		int CoinPrice { get; }
		int PremiumPrice { get; }
		int DisplayOrder { get; }
		PURCHASE_TYPE CurrencyType { get; }
	}

	public class Clothing : Item, IClothing 
	{
		public ClothingCategory ClothingCategory { get; protected set; }
		public int DisplayOrder { get; set; }
		public int LayerID { get; set; }
		public string Layer_Name { get; set; }
		public string CategoryID { get; protected set; }
		public string BundleID { get; set; }
		public string FilePath { get; set; }
		public string IconFilePath { get; set; }
		public string ItemID { get; set; }
		public int CoinPrice { get; set; }
		public int PremiumPrice { get; set; }
		public PURCHASE_TYPE CurrencyType { get; set; }

		private static string _folderPath = "Avatar/";
		private static string _iconPath = "Icons/";
		private static string _spritePath = "Parts/";
		private static List<string> _folderCategories = new List<string> ()
		{
			"Accessories/","Bottoms/","Dresses/","Hairstyles/","Hats/","Intimates/","Jackets_Coats/","Shoes/","Skin/","Tops/"
		};

		//TODO Update this once we have Matt's stuff ready for the Avatar
		private enum FolderCategories
		{
			ACCESSORIES = 0,BOTTOMS = 1,DRESSES = 2,HAIR = 3,HATS = 4,INTIMATES = 5,JACKETS = 6,SHOES = 7,SKIN = 8,TOPS = 9
		}

		private static ClothingMap _clothingMap = new ClothingMap(new string[]{ "54da8ad76f983f60ee01f84f", "54da8ad76f983f60ee01f84c", "54da8ad76f983f60ee01f84e", "54da8ad76f983f60ee01f848",
			"54da8ad76f983f60ee01f850", "54da8ad76f983f60ee01f849", "54da8ad76f983f60ee01f84d", "54da8ad76f983f60ee01f847", "54da8ad76f983f60ee01f84a", "54da8ad76f983f60ee01f84b"});

		public Clothing(string id, string fileName, string filePath, string description, string categoryID): base(id)
		{
			//HACK To get around something of a problem in the original layout of this
			if(fileName.Contains("_")) 
			{
				Name = fileName;
				CategoryID = categoryID;
				Category = ItemCategory.CLOTHING;
				ClothingCategory = _clothingMap.GetClothingCategory(CategoryID);
				Description = GetNameFromFile(description);
				FilePath = filePath;
				CurrencyType = PURCHASE_TYPE.NONE;
				CoinPrice = 0;
				PremiumPrice = 0;

				LayerID = (int)ClothingCategory;
				if(ClothingCategory == ClothingCategory.ACCESSORIES)
				{
					LayerID = (int)GetAccessoryCategoryFromName();
				}

				ItemID = Name + " " + CategoryID;
			}
			else
			{
				Name = fileName;
				CategoryID = categoryID;
				Category = ItemCategory.CLOTHING;
				ClothingCategory = _clothingMap.GetClothingCategory(CategoryID);
				Description = description;
				FilePath = filePath;

				LayerID = (int)ClothingCategory;
				if(ClothingCategory == ClothingCategory.ACCESSORIES)
				{
					LayerID = (int)GetAccessoryCategoryFromName();
				}

//				AssignFilePaths();
			}
		}

		public void AssignFilePaths()
		{
			var identifier = CreateFilePathNameIdentifier();
			switch(ClothingCategory)
			{
				case ClothingCategory.ACCESSORIES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.BAGS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.BELTS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.BOTTOMS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.BOTTOMS] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.BOTTOMS] + identifier;
					break;
				case ClothingCategory.BRACELETS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.DRESSES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.DRESSES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.DRESSES] + identifier;
					break;
				case ClothingCategory.EARRINGS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.GLASSES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.GLOVES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.HAIRSTYLES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.HAIR] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.HAIR] + identifier;
					break;
				case ClothingCategory.HATS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.HATS] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.HATS] + identifier;
					break;
				case ClothingCategory.INTIMATES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.INTIMATES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.INTIMATES] + identifier;
					break;
				case ClothingCategory.JACKETS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.JACKETS] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.JACKETS] + identifier;
					break;
				case ClothingCategory.NECKLACES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.RINGS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.SHOES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.SHOES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.SHOES] + identifier;
					break;
				case ClothingCategory.SKIN:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.SKIN] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.SKIN] + identifier;
					break;
				case ClothingCategory.SOCKS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.TATTOOES:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.ACCESSORIES] + identifier;
					break;
				case ClothingCategory.TOPS:
					FilePath = _folderPath + _spritePath + _folderCategories[(int)FolderCategories.TOPS] + identifier;
					IconFilePath = _folderPath + _iconPath + _folderCategories[(int)FolderCategories.TOPS] + identifier;
					break;
			}
		}

		string CreateFilePathNameIdentifier()
		{
			string identifier = Layer_Name;

			string[] parts = identifier.Split(('_'));
			StringBuilder builder = new StringBuilder();
			int length = parts.Length;
			for(int i = 0; i < length; ++i)
			{
				builder.Append(parts[i]);
				if((i + 1) < length)
				{
					builder.Append("_");
				}
			}

			identifier = builder.ToString();

			return identifier;
		}

		public void AssignPurchaseInformation(int currencyFlag)
		{
			CurrencyType = (PURCHASE_TYPE)currencyFlag;
		}

		public void SetItemId(string itemId)
		{
			ItemID = ItemID;
		}

		public void SetPrices(int coin,int premium)
		{
			CoinPrice = coin;
			PremiumPrice = premium;
		}

		bool areSocks()
		{
			return((Name.Contains("stockings")) || (Name.Contains("Stockings")) || (Name.Contains("socks")) || (Name.Contains("Socks")) || (Name.Contains("leggings")) || (Name.Contains("Leggings")) || (Name.Contains("leggins")) || (Name.Contains("Leggins")));
		}

		bool isNecklace()
		{
			return((Name.Contains("Necklace")) || (Name.Contains("necklace")) || (Name.Contains("scarf")) || (Name.Contains("Scarf")));
		}

		bool isBracelet()
		{
			return ((Name.Contains("bracelet")) || (Name.Contains("Bracelet")));
		}

		bool areGloves()
		{
			return ((Name.Contains("gloves")) || (Name.Contains("Gloves")));
		}

		bool areEarrings()
		{
			return ((Name.Contains("earrings")) || (Name.Contains("Earrings")));
		}

		bool areGlasses()
		{
			return ((Name.Contains("glasses")) || (Name.Contains("Glasses")) || (Name.Contains("sunglasses")) || (Name.Contains("Sunglasses")));
		}

		bool isBelt()
		{
			return ((Name.Contains("belt")) || (Name.Contains("Belt")));
		}

		bool isBag()
		{
			return ((Name.Contains("bag")) || (Name.Contains("Bag")) || (Name.Contains("purse")) || (Name.Contains("Purse")));
		}

		ClothingCategory GetAccessoryCategoryFromName ()
		{
			if(areSocks())
			{
				return ClothingCategory.SOCKS;
			}
			else if(isNecklace())
			{
				return ClothingCategory.NECKLACES;
			}
			else if(isBracelet())
			{
				return ClothingCategory.BRACELETS;
			}
			else if(areGloves())
			{
				return ClothingCategory.GLOVES;
			}
			else if(areEarrings())
			{
				return ClothingCategory.EARRINGS;
			}
			else if(areGlasses())
			{
				return ClothingCategory.GLASSES;
			}
			else if(isBelt())
			{
				return ClothingCategory.BELTS;
			}
			else if(isBag())
			{
				return ClothingCategory.BAGS;
			}

			return ClothingCategory.ACCESSORIES;
		}

		string GetNameFromFile(string fileName)
		{

			string clothingName = fileName.Replace("_"," ");
			return clothingName;
		}

	}
	
	public enum PURCHASE_TYPE
	{
		NONE = 0,
		COIN = 1,
		PREMIUM = 2,
		BOTH = 3
	}
}