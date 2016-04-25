namespace Voltage.Witches.Configuration.JSON
{
	using SimpleJSON;

	public interface IPlayerSpellbookConfigParser
	{
		PlayerSpellbookConfig Parse(string json);
	}

	public class PlayerSpellbookConfigParser : IPlayerSpellbookConfigParser
	{
		IPlayerRecipeConfigParser _recipeParser;

		public PlayerSpellbookConfigParser(IPlayerRecipeConfigParser recipeParser)
		{
			_recipeParser = recipeParser;
		}

		public PlayerSpellbookConfig Parse(string json)
		{
			JSONNode node = JSON.Parse(json);
//			int id = node["id"].AsInt;
			string id = (string)node["id"];

			PlayerSpellbookConfig config = new PlayerSpellbookConfig(id);
			config.IsComplete = node["isComplete"].AsBool;

			foreach (JSONNode recipeNode in node["recipes"].AsArray)
			{
				config.Recipes.Add(_recipeParser.Parse(recipeNode.ToString()));
			}

			return config;
		}
	}
}

