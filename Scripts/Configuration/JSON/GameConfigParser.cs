using System;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Models;
	using SimpleJSON;

	public interface IGameConfigParser
	{
		GameConfiguration Parse(string json);
	}

	public class GameConfigParser : IGameConfigParser
	{
		IIngredientConfigParser _ingredientParser;
		IIngredientCategoryParser _ingredientCategoryParser;
		IItemParser _itemParser;
		IBookParser _bookParser;
		IRecipeRefParser _recipeParser;

		public GameConfigParser(IIngredientConfigParser ingredientParser, IIngredientCategoryParser ingredientCategoryParser, 
		                        IItemParser itemParser, IBookParser bookParser, IRecipeRefParser recipeParser)
		{
			_ingredientParser = ingredientParser;
			_ingredientCategoryParser = ingredientCategoryParser;
			_itemParser = itemParser;
			_bookParser = bookParser;
			_recipeParser = recipeParser;
		}

		public GameConfiguration Parse(string json)
		{
			JSONNode node = JSON.Parse(json);
			int version = node["version"].AsInt;

			GameConfiguration config = new GameConfiguration(version);

			int refreshRate = node["staminaRefreshRate"].AsInt;
			config.StaminaRefreshRate = refreshRate;

			var ingredients = node["ingredients"].AsArray;
			foreach (JSONNode ingredientNode in ingredients)
			{
				IngredientConfig ingredient = _ingredientParser.Parse(ingredientNode.ToString());
				config.Ingredients[ingredient.ItemId] = ingredient;
			}

			var ingredientCategories = node["ingredientCategories"].AsArray;
			foreach (JSONNode ingredientCategoryNode in ingredientCategories)
			{
				IngredientCategory category = _ingredientCategoryParser.Parse(ingredientCategoryNode.ToString());
				config.IngredientCategories[category.Id] = category;
			}

			var items = node["items"].AsArray;
			foreach (JSONNode itemNode in items)
			{
				Item item = _itemParser.Parse(itemNode.ToString());
				config.Items[item.Id] = item;
			}

			var books = node["books"].AsArray;
			foreach (JSONNode bookNode in books)
			{
				SpellbookRef book = _bookParser.Parse(bookNode.ToString());
				config.Books[book.Id] = book;
			}

			var recipes = node["recipes"].AsArray;
			foreach (JSONNode recipeNode in recipes)
			{
				RecipeRef recipe = _recipeParser.Parse(recipeNode.ToString());
				config.Recipes[recipe.Id] = recipe;
			}

			return config;
		}

	}
}

