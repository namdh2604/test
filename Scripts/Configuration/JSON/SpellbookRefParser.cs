using Voltage.Witches.Models;
using System.Collections.Generic;

namespace Voltage.Witches.Configuration.JSON
{
	using SimpleJSON;

	public interface IBookParser
	{
		SpellbookRef Parse(string json);
	}

	public class SpellbookRefParser : IBookParser
	{
		IRecipeRefParser _recipeParser;

		public SpellbookRefParser()
		{
		}

		public SpellbookRef Parse(string json)
		{
			JSONNode node = JSON.Parse(json);
			string id = (string)node["id"];
			SpellbookRef book = new SpellbookRef(id);

			string name = node["name"].Value;
			book.Name = name;

			foreach (JSONNode child in node["clearRewards"].AsArray)
			{
				string itemId = (string)child["id"];
				int quantity = child["quantity"].AsInt;
				book.ClearRewards[itemId] = quantity;
			}

			foreach (JSONNode recipeChild in node["recipes"].AsArray)
			{
				book.Recipes.Add((string)recipeChild);
				//book.Recipes.Add(_recipeParser.Parse(recipeChild.ToString()));
			}

			return book;
		}
	}
}

