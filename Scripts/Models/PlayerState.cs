using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Models
{
	public class PlayerState
	{
		public PlayerState()
		{
			Inventory = new Dictionary<string, int>();
			Books = new List<PlayerSpellbookConfig>();
		}

		public PlayerState(PlayerStateData data)
		{
			Inventory = new Dictionary<string, int>();
			Books = new List<PlayerSpellbookConfig>();

			Stamina = data.stamina;
			Focus = data.focus;
			Currency = data.currency;
			PremiumCurrency = data.premium_currency;
			MailBadge = data.mail_badge;
			ClosetSpace = data.closet_space;
		}

		public int Stamina { get; set; }
		public int Focus { get; set; }
		public int Currency { get; set; }
		public int PremiumCurrency { get; set; }
		public int MailBadge { get; set; }
		public int ClosetSpace { get; set; } 
		public Dictionary<string, int> Inventory { get; protected set; }
		public List<PlayerSpellbookConfig> Books { get; protected set; }
	}
}

