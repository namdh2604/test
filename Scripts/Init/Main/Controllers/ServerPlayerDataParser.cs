
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Voltage.Witches.DI
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	
	using Voltage.Story.General;
	using Voltage.Witches.User;

	using Voltage.Witches.Configuration;
	using Voltage.Common.Logging;
    using Voltage.Witches.Models.Avatar;


	public class ServerPlayerDataParser : IParser<PlayerDataStore>	// Adapter
	{
		public PlayerDataStore Parse(string text)
		{
//			return JsonConvert.DeserializeObject<PlayerDataStore> (text);	
			ProxyPlayerData proxy = JsonConvert.DeserializeObject<ProxyPlayerData> (text);	// can throw exception


//			AmbientLogger.Current.Log ("PLAYER DATA STORE: Available Scenes is Stubbed", LogLevel.WARNING);


            // The server only contains a list of worn objects, rather than the dictionary the client expects.
            // Because existing client data had a dictionary of items, we need to translate one to the other
            Outfit outfit = new Outfit();
            if (proxy.current_outfit != null)
            {
                foreach (var entry in proxy.current_outfit)
                {
                    outfit.WearItem(entry);
                }
            }

			return new PlayerDataStore
			{
				userID = proxy.phone_ID,

				firstName = proxy.first_name,
				lastName = proxy.last_name,

				stamina = proxy.stamina,
				focus = proxy.focus,
				currencyGame = proxy.currency,
				currencyPremium = proxy.premium_currency,
				staminaPotions = proxy.stamina_potion,

				affinities = proxy.character_affinities,
				totalAffinity = proxy.total_affinity,

				currentScene = proxy.scene_id,
				currentNodeID = proxy.node_id,

				currentHowTosScene = proxy.howtos_scene_id,

				books = proxy.books,

				closetSpace = proxy.closet_space,
                currentOutfit = outfit.GetSerializableValues(),

//				availableScenes = proxy.available_scenes,
				availableScenes = new List<string>() {"Prologue/Prologue/Starflood"},
				enableStaminaDeductionScene = "Prologue/Prologue/Mending Luna", 				// the scene before "Prologue/Prologue/The Private Collection" (currently)

				inventory = proxy.inventory,

				tutorialFlag = proxy.tutorial_flag,
				tutorialProgress = proxy.tutorial_progress,
				notificationsEnabled = proxy.notificationsEnabled
				// mail_badge
			};
		}

		private struct ProxyPlayerData	// TODO: almost the same, align with server and drop proxy
		{
			public string phone_ID;
			public string first_name;
			public string last_name;

			public int stamina;
			public int focus;
			public int currency;
			public int premium_currency;
			public int stamina_potion;

			public Dictionary<string,int> character_affinities;
			public int total_affinity;

			public string scene_id;
			public string node_id;
			public string howtos_scene_id;


			public List<PlayerSpellbookConfiguration> books;

			public int closet_space;
            public List<string> current_outfit;

			public int mail_badge;

			public List<string> available_scenes;

			public Dictionary<string,int> inventory;
	
			public bool tutorial_flag;
			public int tutorial_progress;

			[DefaultValue(true)]
			[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
			public bool notificationsEnabled;
			// avatar_items
		}

	}
	


	
}




