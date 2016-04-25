using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration.JSON
{
	using SimpleJSON;

	public interface IIngredientCategoryParser
	{
		IngredientCategory Parse(string json);
	}

	public class IngredientCategoryParser : IIngredientCategoryParser
	{
		public IngredientCategoryParser()
		{
		}

		public IngredientCategory Parse(string json)
		{
			JSONNode node = JSON.Parse(json);
			//Was int
			string id = (string)node["id"];
			string name = node["name"].Value;

			return new IngredientCategory(id, name);
		}
	}
}

