using Voltage.Witches.Configuration;
using Voltage.Witches.Configuration.JSON;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public interface ISpellbookFactoryNew
	{
		Spellbook Create (PlayerSpellbookConfiguration playerConfig, SpellbookRefConfig config);
		Spellbook Create (SpellbookRefConfig config);
		Spellbook CreateSpecial(string id);
	}
	
	public class SpellbookFactoryNew : ISpellbookFactoryNew
	{
		RecipesConfiguration _recipesConfig;
		MasterConfiguration _master;
		IRecipeFactoryNew _recipeFactory;
		ItemRawParser _itemParser;

		public SpellbookFactoryNew(MasterConfiguration gameConfig, IRecipeFactoryNew recipeFactory)
		{
			_master = gameConfig;
			_recipesConfig = gameConfig.Recipes_Configuration;
			_recipeFactory = recipeFactory;
			_itemParser = new ItemRawParser(_master);
		}

		public Spellbook CreateSpecial(string id)
		{
			var bookRef = _master.Books_Configuration.Books_Reference[id];
			return Create(bookRef);
		}

		public Spellbook Create(PlayerSpellbookConfiguration playerConfig, SpellbookRefConfig config) 
		{
			Spellbook book = new Spellbook(config.Name, config.Id);
			if (playerConfig != null)
			{
				book.IsAccessible = true;
			}

			foreach (string recipeId in config.Recipes)
			{
//				UnityEngine.Debug.Log(recipeId + " is the recipe Id");
				try
				{
					RecipeReference recipeConfig = _recipesConfig.Recipes[recipeId];
					if(playerConfig != null)
					{
						PlayerRecipeConfig playerRecipeConfig = GetCorrespondingRecipeConfig(playerConfig, recipeConfig);
						book.AddRecipe(_recipeFactory.Create(playerRecipeConfig, recipeConfig));
					}
				}
				catch(System.Exception e)
				{
					System.Console.WriteLine(e.Message);
					System.Console.WriteLine(recipeId + " is not in the recipes");
					System.Console.WriteLine("Recipes Config Recipes Dictionary<string,RecipeConfig> is null : " + (_recipesConfig == null).ToString());
					throw;
//					UnityEngine.Debug.Log(_recipesConfig.Recipes.Count.ToString() + " is the number of recipes in the config");
				}
			}

			//TODO Split this out into separate functions and get rid of this ungainly block
			if((_master.Book_Prizes != null) && (!string.IsNullOrEmpty(config.Book_Prize_ID)) && (_master.Book_Prizes.ContainsKey(config.Book_Prize_ID)))
			{
				var bookPrizeData = _master.Book_Prizes[config.Book_Prize_ID];
				int quantity = bookPrizeData.quantity;
				Item item = null;
				if(isAParseableItem(bookPrizeData.type))
				{
					var itemConfig = _master.Items_Master[bookPrizeData.reward_id];

					if(bookPrizeData.type == "avatar")
					{
						item = _itemParser.CreateAvatarItem(itemConfig.Item as AvatarItemData);
					}
					else
					{
						item = _itemParser.CreateIngredient(itemConfig.Item as IngredientData);
					}
				}
				else
				{
					if(bookPrizeData.type == "stamina")
					{
						item = new Potion("Stamina Potion", "Stamina Potion", "It's a stamina potion", string.Empty, new Dictionary<string,int>());
					}
					else if(bookPrizeData.type == "premium_currency")
					{
						item = new StarStoneItem("starstones");
					}
					else
					{
						//
					}
				}
				if(item != null)
				{
					book.ClearItems.Add(new KeyValuePair<Item, int>(item,quantity));
				}
			}
			return book;
		}



		bool isAParseableItem(string type)
		{
			return ((type == "ingredient") || (type == "avatar"));
		}

		public Spellbook Create(SpellbookRefConfig config)
		{
			Spellbook book = new Spellbook(config.Name, config.Id);
			book.IsAccessible = false;

			foreach(string recipeId in config.Recipes)
			{
				RecipeReference recipeConfig = _recipesConfig.Recipes[recipeId];
				book.AddRecipe(_recipeFactory.Create(null,recipeConfig));
			}

			//TODO Split this out into separate functions and get rid of this ungainly block
			if((_master.Book_Prizes != null) && (!string.IsNullOrEmpty(config.Book_Prize_ID)) && (_master.Book_Prizes.ContainsKey(config.Book_Prize_ID)))
			{
				var bookPrizeData = _master.Book_Prizes[config.Book_Prize_ID];
				System.Console.WriteLine(bookPrizeData.type);
				int quantity = bookPrizeData.quantity;
				Item item = null;
				if(isAParseableItem(bookPrizeData.type))
				{
					var itemConfig = _master.Items_Master[bookPrizeData.reward_id];
					
					if(bookPrizeData.type == "avatar")
					{
						item = _itemParser.CreateAvatarItem(itemConfig.Item as AvatarItemData);
					}
					else
					{
						item = _itemParser.CreateIngredient(itemConfig.Item as IngredientData);
					}
				}
				else
				{
					if(bookPrizeData.type == "stamina")
					{
						item = new Potion("Stamina Potion", "Stamina Potion", "It's a stamina potion", string.Empty, new Dictionary<string,int>());
					}
					else if(bookPrizeData.type == "premium_currency")
					{
						item = new StarStoneItem("starstones");
					}
					else
					{
						//
					}
				}
				if(item != null)
				{
					book.ClearItems.Add(new KeyValuePair<Item, int>(item,quantity));
				}
			}
			return book;
		}

		PlayerRecipeConfig GetCorrespondingRecipeConfig(PlayerSpellbookConfiguration playerConfig, RecipeReference recipeConfig)
		{
			return playerConfig.Recipes.Find(x => (x.ID == recipeConfig.Id));
		}
	}
}