using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration.JSON
{
	using SimpleJSON;

	public interface IPlayerStateParser
	{
		PlayerState Parse(string json);
	}

	public class PlayerStateParser : IPlayerStateParser
	{
		IPlayerSpellbookConfigParser _bookParser;

		public PlayerStateParser(IPlayerSpellbookConfigParser bookParser)
		{
			_bookParser = bookParser;
		}

		public PlayerState Parse(string json)
		{
			JSONNode node = JSON.Parse(json);

			PlayerState player = new PlayerState();
			player.Stamina = node["stamina"].AsInt;
			player.Currency = node["currency"].AsInt;
			player.PremiumCurrency = node["premiumCurrency"].AsInt;

			foreach (JSONNode itemNode in node["inventory"].AsArray)
			{
				var itemId = (string)itemNode["id"];
				var quantity = itemNode["quantity"].AsInt;
				player.Inventory[itemId] = quantity;
			}

			foreach (JSONNode itemNode in node["books"].AsArray)
			{
				player.Books.Add(_bookParser.Parse(itemNode.ToString()));
			}
			return player;
		}
	}
}

