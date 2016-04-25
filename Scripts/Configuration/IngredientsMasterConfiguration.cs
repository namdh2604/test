using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class IngredientsMasterConfiguration 
	{
		public Dictionary<string,IngredientConfiguration> Ingredients { get; protected set; }

		public IngredientsMasterConfiguration()
		{
			Ingredients = new Dictionary<string,IngredientConfiguration>();
		}
	}
}