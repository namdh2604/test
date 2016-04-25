using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class RecipesConfiguration
	{
		public Dictionary<string,RecipeData> Recipes_Dictionary { get; set;}
		public Dictionary<string,RecipeReference> Recipes { get; set; }

		public RecipesConfiguration()
		{
			Recipes_Dictionary = new Dictionary<string,RecipeData>();
			Recipes = new Dictionary<string,RecipeReference>();
		}
	}
}