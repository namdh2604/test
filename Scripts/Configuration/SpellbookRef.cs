using System.Collections.Generic;

namespace Voltage.Witches.Configuration
{
	public class SpellbookRef
	{
		public SpellbookRef(string id)
		{
			Id = id;
			ClearRewards = new Dictionary<string, int>();
			Recipes = new List<string>();
		}

		public string Id { get; protected set; }
		public string Name { get; set; }
		public Dictionary<string, int> ClearRewards { get; protected set; }
		public List<string> Recipes { get; protected set; }
	}
}

