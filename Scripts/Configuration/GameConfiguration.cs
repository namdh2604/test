using System.Collections.Generic;

using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class GameConfiguration
	{
		public GameConfiguration(int version)
		{
			Version = version;
			Ingredients = new Dictionary<string, IngredientConfig>();
			IngredientCategories = new Dictionary<string, IngredientCategory>();
			Items = new Dictionary<string, Item>();
			Books = new Dictionary<string, SpellbookRef>();
			Recipes = new Dictionary<string, RecipeRef>();
		}

		public int StaminaRefreshRate { get; set; }
		public int Version { get; set; }

		public Dictionary<string, Item> Items;
		public Dictionary<string, IngredientConfig> Ingredients;
		public Dictionary<string, IngredientCategory> IngredientCategories;
		public Dictionary<string, SpellbookRef> Books;
		public Dictionary<string, RecipeRef> Recipes;

	}
}
