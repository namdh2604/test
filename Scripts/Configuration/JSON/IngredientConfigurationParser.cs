using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Models;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public interface IIngredientConfigurationParser
	{
		IngredientConfiguration Construct(IngredientData ingredientData);
	}

	public class IngredientConfigurationParser : IIngredientConfigurationParser
	{
		public IngredientConfigurationParser()
		{
		}

		public IngredientConfiguration Construct(IngredientData ingredientData)
		{
			IngredientConfiguration ingredientConfig = new IngredientConfiguration();
			ingredientConfig.Bottle_BG_ID = ingredientData.bottle_bg;
			ingredientConfig.Category_Id = ingredientData.category_id;
			ingredientConfig.Coins_Price = Convert.ToInt32(ingredientData.coins_price);
			ingredientConfig.Color_HEX = ingredientData.color;
			ingredientConfig.Currency_Flag = ingredientData.currency_flag;
			ingredientConfig.Description = ingredientData.description;
			ingredientConfig.Display_Order = ingredientData.display_order;
			ingredientConfig.Id = ingredientData.id;
			ingredientConfig.IsInfinite = ingredientData.isInfinite;
			ingredientConfig.Name = ingredientData.name;
			ingredientConfig.Premium_Price = Convert.ToInt32(ingredientData.premium_price);
			ingredientConfig.Quality = ingredientData.quality;
			ingredientConfig.Item_Category = (ItemCategory)ingredientData.item_cat;

			return ingredientConfig;
		}
	}
}