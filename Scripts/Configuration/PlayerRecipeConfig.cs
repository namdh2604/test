namespace Voltage.Witches.Configuration
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class PlayerRecipeConfig
	{
		//TODO Need to add in stages here
		public PlayerRecipeConfig(string id)
		{
			ID = id;
		}

		[JsonProperty(PropertyName = "recipe_id")]	
		public string ID { get; protected set; }	// update to match server name

		public int HighScore { get; set; }	// deprecated

		[JsonProperty(PropertyName = "stars")]
		public int CompletionStage { get; set; }	// update to match server name
	}
}

