using System;
using Voltage.Witches.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Voltage.Witches.Configuration.JSON
{
    using Voltage.Story.StoryDivisions;

	public interface IPlayerStateParserNew
	{
		PlayerStateNew Parse(string json);
	}

	public class PlayerStateParserNew: IPlayerStateParserNew
	{
		IPlayerSpellbookConfigurationParser _bookParser;

        public PlayerStateParserNew(IPlayerSpellbookConfigurationParser bookParser)
		{
			_bookParser = bookParser;
		}
		
        public PlayerStateNew Parse(string json)
		{
			JObject jsonobject = null;
			try
			{
				jsonobject = JObject.Parse(json);
			}
			catch(Exception)
			{
				System.Console.WriteLine("Player State Json was invalid");
				throw;
			}

			PlayerStateData data = JsonConvert.DeserializeObject<PlayerStateData>(jsonobject.ToString());
			//TODO Needs something related to updating  recipe stage of completion
			PlayerStateNew player = new PlayerStateNew();
			player.Currency = data.currency;
			player.PremiumCurrency = data.premium_currency;
			player.Stamina = data.stamina;
			player.MailBadge = data.mail_badge;
			player.Focus = data.focus;
			player.ClosetSpace = data.closet_space;
			player.AffinityRef = data.affinity;

			foreach(var item in data.inventory)
			{
				if(!player.Inventory.ContainsKey(item.id))
				{
					player.Inventory.Add(item.id,item.quantity);
				}
			}

			foreach(var item in data.avatar_items)
			{
				if(!player.Inventory.ContainsKey(item.id))
				{
					player.Inventory.Add(item.id,item.quantity);
				}
			}

			foreach(var item in data.books)
			{
				player.Books.Add(_bookParser.Construct(item));
			}

			return player;
		}
	}
}