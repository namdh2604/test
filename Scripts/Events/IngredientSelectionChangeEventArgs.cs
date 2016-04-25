using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class IngredientSelectionChangeEventArgs : GUIEventArgs
	{
		public Ingredient Ingredient { get; set; }

		public IngredientSelectionChangeEventArgs(Ingredient ingredient)
		{
			Ingredient = ingredient;
		}
	}
}

