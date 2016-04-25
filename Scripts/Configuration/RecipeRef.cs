using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class RecipeRef
	{
		public string Id { get; protected set; }
		public string Name { get; set; }
		public string Hint { get; set; }
		public int StaminaReq { get; set; }
		public int ClearScore { get; set; }
		public BrewGameType BrewGameCategory { get; set; }
		public Dictionary<string, int> IngredientRequirements { get; protected set; }
		public List<string> Products { get; protected set; }

		public RecipeRef(string id)
		{
			Id = id;
			IngredientRequirements = new Dictionary<string, int>();
			Products = new List<string>();
		}

		public void AddRequirement(string category, int maxContribution)
		{
			IngredientRequirements[category] = maxContribution;
		}


	}
}

