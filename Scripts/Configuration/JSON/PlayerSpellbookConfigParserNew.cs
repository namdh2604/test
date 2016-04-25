using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration.JSON
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	
	public class PlayerSpellbookConfigParserNew : IPlayerSpellbookConfigParser
	{
		IPlayerRecipeConfigParser _recipeParser;
		
		public PlayerSpellbookConfigParserNew(IPlayerRecipeConfigParser recipeParser)
		{
			_recipeParser = recipeParser;
		}
		
		public PlayerSpellbookConfig Parse(string json)
		{
			JObject jsonobject = JObject.Parse(json);
			BooksData data = JsonConvert.DeserializeObject<BooksData>(jsonobject.ToString());

//			JSONNode node = JSON.Parse(json);
			string id = data.id;
			
			PlayerSpellbookConfig config = new PlayerSpellbookConfig(id);
			config.IsComplete = data.is_complete;
			
			foreach(var recipe in data.recipes)
			{
				//TODO Update this after getting all the recipe data
				config.Recipes.Add(_recipeParser.Parse(recipe.recipe_id));
			}
			
			return config;
		}
	}
}