using Voltage.Witches.Models;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using SimpleJSON;

	public interface IIngredientConfigParser
	{
		IngredientConfig Parse(string json);
	}

	public class IngredientConfigParser : IIngredientConfigParser
	{
		public IngredientConfigParser()
		{
		}

		public IngredientConfig Parse(string json)
		{
			JSONNode node = JSON.Parse(json);
			string itemId = (string)node["itemId"];
			string category = (string)node["category"];
			int value = node["value"].AsInt;
			bool isInfinite = node["isInfinite"].AsBool;

			IngredientConfig ingredient = new IngredientConfig(itemId);
			ingredient.Value = value;
			ingredient.Category = category;
			ingredient.IsInfinite = isInfinite;

			return ingredient;
		}
	}
}

