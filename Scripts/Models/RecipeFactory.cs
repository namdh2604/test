using Voltage.Witches.Configuration;

namespace Voltage.Witches.Models
{
	public interface IRecipeFactory
	{
		Recipe Create(PlayerRecipeConfig playerRecipeConfig, RecipeRef recipeConfig);
	}

	public class RecipeFactory : IRecipeFactory
	{
		GameConfiguration _gameConfig;

		public RecipeFactory(GameConfiguration gameConfig)
		{
			_gameConfig = gameConfig;
		}

		public Recipe Create(PlayerRecipeConfig playerRecipeConfig, RecipeRef recipeConfig)
		{
			Recipe recipe = new Recipe(recipeConfig.Name);
			if (playerRecipeConfig != null)
			{
				recipe.IsAccessible = true;
				recipe.HighScore = playerRecipeConfig.HighScore;
			}

			recipe.Hint = recipeConfig.Hint;
			recipe.StaminaReq = recipeConfig.StaminaReq;
			recipe.ClearScore = recipeConfig.ClearScore;
			recipe.BrewGameCategory = recipeConfig.BrewGameCategory;

			foreach (var requirement in recipeConfig.IngredientRequirements)
			{
				IngredientCategory category = _gameConfig.IngredientCategories[requirement.Key];
				recipe.IngredientRequirements.Add(new IngredientRequirement(category, requirement.Value));
			}

			foreach (string itemId in recipeConfig.Products)
			{
				recipe.Products.Add(_gameConfig.Items[itemId]);
			}
			return recipe;
		}
	}
}

