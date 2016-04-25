using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration.JSON
{
	public interface IPlayerSpellbookConfigurationParser
	{
		PlayerSpellbookConfiguration Construct(BooksData bookData);
	}

	public class PlayerSpellbookConfigurationParser : IPlayerSpellbookConfigurationParser
	{
		IPlayerRecipeConfigParser _recipeConfigParser;
		public PlayerSpellbookConfigurationParser(IPlayerRecipeConfigParser recipeConfigParser)
		{
			_recipeConfigParser = recipeConfigParser;
		}

		public PlayerSpellbookConfiguration Construct(BooksData bookData)
		{
			PlayerSpellbookConfiguration spellBookConfig = new PlayerSpellbookConfiguration();
			spellBookConfig.Id = bookData.id;
			spellBookConfig.IsComplete = bookData.is_complete;
			foreach(var item in bookData.recipes)
			{
				//TODO Update when completion stage is included
				spellBookConfig.Recipes.Add(_recipeConfigParser.Construct(item.recipe_id));
			}

			return spellBookConfig;
		}
	}
}