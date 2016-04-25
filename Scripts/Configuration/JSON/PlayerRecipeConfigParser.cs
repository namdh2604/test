namespace Voltage.Witches.Configuration.JSON
{
	using SimpleJSON;

	public interface IPlayerRecipeConfigParser
	{
		PlayerRecipeConfig Parse(string json);
		PlayerRecipeConfig Construct(string id);
	}

	public class PlayerRecipeConfigParser : IPlayerRecipeConfigParser
	{
		public PlayerRecipeConfigParser()
		{
		}

		public PlayerRecipeConfig Parse(string json)
		{
			JSONNode node = JSON.Parse(json);

			string id = (string)node["id"];
			PlayerRecipeConfig config = new PlayerRecipeConfig(id);
			config.HighScore = node["highscore"].AsInt;

			return config;
		}

		public PlayerRecipeConfig Construct(string id)
		{
			PlayerRecipeConfig config = new PlayerRecipeConfig(id);
			//HACK Just to stub this out until the data is there
			config.CompletionStage = 0;
			return config;
		}
	}
}

