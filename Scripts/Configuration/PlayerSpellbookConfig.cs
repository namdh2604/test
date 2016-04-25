using System.Collections.Generic;

namespace Voltage.Witches.Configuration
{
	public class PlayerSpellbookConfig
	{
		public PlayerSpellbookConfig(string id)
		{
			ID = id;
			Recipes = new List<PlayerRecipeConfig>();
		}

		public string ID { get; protected set; }
		public bool IsComplete { get; set; }
		public List<PlayerRecipeConfig> Recipes { get; protected set; }
	}
}

