using Voltage.Witches.Configuration;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public interface ISpellbookFactory
	{
		Spellbook Create(PlayerSpellbookConfig playerConfig, SpellbookRef config);
	}

	public class SpellbookFactory : ISpellbookFactory
	{
		GameConfiguration _gameConfig;
		IRecipeFactory _recipeFactory;

		public SpellbookFactory(GameConfiguration gameConfig, IRecipeFactory recipeFactory)
		{
			_gameConfig = gameConfig;
			_recipeFactory = recipeFactory;
		}

		public Spellbook Create(PlayerSpellbookConfig playerConfig, SpellbookRef config) 
		{
			Spellbook book = new Spellbook(config.Name);
			if (playerConfig != null)
			{
				book.IsAccessible = true;
			}

			foreach (string recipeId in config.Recipes)
			{
				RecipeRef recipeConfig = _gameConfig.Recipes[recipeId];
				if(playerConfig != null)
				{
					PlayerRecipeConfig playerRecipeConfig = GetCorrespondingRecipeConfig(playerConfig, recipeConfig);
					book.AddRecipe(_recipeFactory.Create(playerRecipeConfig, recipeConfig));
				}
			}

			foreach (KeyValuePair<string, int> itemEntry in config.ClearRewards)
			{
				Item item = _gameConfig.Items[itemEntry.Key];
				var entry = new KeyValuePair<Item, int>(item, itemEntry.Value);
				book.ClearItems.Add(entry);
			}
			return book;
		}

		PlayerRecipeConfig GetCorrespondingRecipeConfig(PlayerSpellbookConfig playerConfig, RecipeRef recipeConfig)
		{
			return playerConfig.Recipes.Find(x => (x.ID.ToString() == recipeConfig.Id.ToString()));
		}
	}
}

