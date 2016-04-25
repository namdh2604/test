using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Models
{
	public class PlayerStateNew
	{
		public PlayerStateNew()
		{
			Inventory = new Dictionary<string, int>();
			Books = new List<PlayerSpellbookConfiguration>();
		}
		
		public PlayerStateNew(PlayerStateData data)
		{
			Inventory = new Dictionary<string,int>();
			Books = new List<PlayerSpellbookConfiguration>();
			
			Stamina = data.stamina;
			Focus = data.focus;
			StaminaPotions = data.stamina_potion;
			Currency = data.currency;
			PremiumCurrency = data.premium_currency;
			MailBadge = data.mail_badge;
			ClosetSpace = data.closet_space;
			AffinityRef = data.affinity;
			First_Name = data.first_name;
			Last_Name = data.last_name;
			Scene_ID = data.scene_id;
			Tutorial_Incomplete = data.tutorial_flag;
		}

		public string First_Name { get; set; }
		public string Last_Name { get; set; }
		public string Scene_ID { get; set; }
		public bool Tutorial_Incomplete { get; set; }
		public int Stamina { get; set; }
		public int StaminaPotions { get; set; }
		public int Focus { get; set; }
		public int Currency { get; set; }
		public int PremiumCurrency { get; set; }
		public int MailBadge { get; set; }
		public int ClosetSpace { get; set; } 
		public int AffinityRef { get; set; }
		public Dictionary<string, int> Inventory { get; protected set; }
		public List<PlayerSpellbookConfiguration> Books { get; protected set; }

        // HACK - Hardcoding values -- eventually this should be passed down from the server
        private List<string> _availableScenes = new List<string>(new string[] { "Prologue/Prologue/Slow Day Swiftly Interrupted" } );
        public List<string> AvailableScenes {
            get
            {
                return _availableScenes;
            }
        }
	}
}
