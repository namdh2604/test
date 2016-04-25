using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration.JSON
{
	public interface IItemRawParser
	{
		Item CreateIngredient(IngredientData data);
		Item CreateAvatarItem(AvatarItemData data);
		Item CreatePotion(PotionData data);
	}

	public class ItemRawParser: IItemRawParser
	{
		MasterConfiguration _gameConfig;

		public ItemRawParser(MasterConfiguration gameConfig)
		{
			_gameConfig = gameConfig;
		}

		public Item CreateIngredient(IngredientData data)
		{
			var categoryConfig = _gameConfig.Categories[data.category_id];
			IngredientCategory category = new IngredientCategory(categoryConfig.id, categoryConfig.name);
			Ingredient ingredient = new Ingredient (data.id, data.name, category, data.quality, data.isInfinite);
			ingredient.Description = data.description;
			ingredient.BottleType = (BottleBGType)data.bottle_bg;
			ingredient.CurrencyType = (PURCHASE_TYPE)data.currency_flag;
			ingredient.DisplayOrder = data.display_order;
			ingredient.PremiumPrice = Convert.ToInt32(data.premium_price);
			ingredient.RegularPrice = Convert.ToInt32(data.coins_price);
			ingredient.Color = data.color;

			return ingredient;
		}

		public Item CreateAvatarItem(AvatarItemData data)
		{
			Clothing avatarItem = new Clothing(data.id, data.name, string.Empty, data.description, data.category_id);
			avatarItem.CoinPrice = data.coins_price;
			avatarItem.PremiumPrice = data.premium_price;
			avatarItem.CurrencyType = (PURCHASE_TYPE)data.currency_flag;
			avatarItem.DisplayOrder = data.display_order;
			avatarItem.Layer_Name = data.layer_name;
			avatarItem.ItemID = data.id;
			avatarItem.AssignFilePaths();
			return avatarItem;
		}

		public Item CreatePotion(PotionData data)
		{
			var effects = new Dictionary<string,int>();
			foreach(var element in data.effect_list)
			{
				var keys = element.Keys.ToArray();
				var values = element.Values.ToArray();
				for(int i = 0; i < keys.Length; ++i)
				{
					effects[keys[i]] = values[i];
				}
			}


			Potion potion = new Potion(data.id,data.name,data.description,data.color,effects);
			potion.CurrencyType = (PURCHASE_TYPE)data.type;
			potion.Display_Order = data.display_order;
			potion.Coin_Price = 10;
			potion.Premium_Price = 5;
			potion.Icon_Path = "Potions/" + (AdjustDataNameForFilePath(data.name));

			return potion;
		}

		//TODO Remove this eventually
		private string AdjustNameForFolderUse(string categoryName)
		{
			string adjustedName = char.ToUpper(categoryName[0]) + categoryName.Substring(1) + "/";
			return adjustedName;
		}

		//TODO Remove this eventually
		private string AdjustDataNameForFilePath(string dataName)
		{
			if(dataName != "JACKETS & COATS")
			{
				string adjustedName = dataName.ToLower();
				adjustedName = adjustedName.Replace(" ", "_");
				return adjustedName;
			}
			else
			{
				string specialCase = dataName.Replace("JACKETS & COATS","Jackets_Coats");
				return specialCase;
			}
		}
	}
}