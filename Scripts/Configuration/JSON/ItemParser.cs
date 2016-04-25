using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration.JSON
{
	using SimpleJSON;

	public interface IItemParser
	{
		Item Parse(string json);
	}

	public class ItemParser : IItemParser
	{
		public ItemParser()
		{
		}

		public Item Parse(string json)
		{
			JSONNode node = JSON.Parse(json);

			//Was .asInt
			string id = (string)node["id"];
			Item item = new Item(id);

			string name = node["name"].Value;
			item.Name = name;

			int rawCategory = node["type"].AsInt;
			item.Category = (ItemCategory)rawCategory;

			string desc = node["description"].Value;
			item.Description = desc;

			return item;
		}
	}
}

