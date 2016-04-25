using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class IngredientPurchaseRequestEventArgs : GUIEventArgs
	{
		public Ingredient RequestedIngredient { get; protected set; }

		public IngredientPurchaseRequestEventArgs(Ingredient ingredient)
		{
			RequestedIngredient = ingredient;
		}
	}
}

