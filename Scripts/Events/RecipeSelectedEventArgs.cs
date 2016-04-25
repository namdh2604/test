using System;
using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class RecipeSelectedEventArgs : EventArgs
	{
		public RecipeSelectedEventArgs(IRecipe recipe)
		{
			Recipe = recipe;
		}

		public IRecipe Recipe { get; protected set; }
	}
}

