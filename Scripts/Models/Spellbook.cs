using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public interface ISpellbook
	{
	 	string Id { get; }		
		string Name { get; }
		bool IsClear();
		bool IsAccessible { get; }
		List<IRecipe> Recipes { get; }
	}

	public class Spellbook : ISpellbook
	{
		public string Id { get; private set; }

		public string Name { get; protected set; }
		public bool IsAccessible { get; set; }
		//TODO Redo the prize stuff ,because a list is no longer relevant, it's one prize per book
		public List<KeyValuePair<Item, int>> ClearItems { get; set; }
		public int ClearCurrency { get; set; }
		public int ClearPremiumCurrency { get; set; }
		public List<IRecipe> Recipes { get; protected set; }

		public Spellbook(string name, string id="")
		{
			Id = id;

			Name = name;
			ClearItems = new List<KeyValuePair<Item, int>>();
			Recipes = new List<IRecipe>();

			IsAccessible = false;
		}

		public void AddRecipe(IRecipe recipe)
		{
			Recipes.Add(recipe);
		}

		public bool IsClear()
		{
			foreach (IRecipe recipe in Recipes)
			{
				if((int)recipe.CurrentStage < 3)
				{
					return false;
				}
			}

			return true;
		}
	}
}