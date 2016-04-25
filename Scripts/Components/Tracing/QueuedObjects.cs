using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Components
{
	public class QueuedObjects
	{
		public IRecipe Current_Recipe { get; protected set; }
		public List<Ingredient> Wagered_Ingredients { get; protected set; }

		public QueuedObjects(IRecipe recipe, List<Ingredient> wageredIngredients)
		{
			Current_Recipe = recipe;
			Wagered_Ingredients = wageredIngredients;
		}
	}
}