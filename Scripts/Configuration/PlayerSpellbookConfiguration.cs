using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class PlayerSpellbookConfiguration
	{
		public string Id { get; set; }
		public bool IsComplete { get; set; }
		public List<PlayerRecipeConfig> Recipes { get; set; }

		public PlayerSpellbookConfiguration()
		{
			Recipes = new List<PlayerRecipeConfig>();
		}


		public PlayerSpellbookConfiguration(ISpellbook book)
		{
			Id = book.Id;
			IsComplete = book.IsClear ();
			Recipes = GetListOfRecipeConfigs (book.Recipes);
		}

		private List<PlayerRecipeConfig> GetListOfRecipeConfigs(IEnumerable<IRecipe> recipes)
		{
			List<PlayerRecipeConfig> recipeConfigs = new List<PlayerRecipeConfig> ();

			foreach (IRecipe recipe in recipes)
			{
				PlayerRecipeConfig config = new PlayerRecipeConfig(recipe.Id)
				{
					HighScore = recipe.HighScore,
					CompletionStage = (int)recipe.CurrentStage,
				};
				recipeConfigs.Add (config);
			}

			return recipeConfigs;
		}
	}
}