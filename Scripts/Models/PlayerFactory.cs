using System;
using Voltage.Witches.Configuration;
using Voltage.Witches.Models;
using System.Collections.Generic;
using Voltage.Witches.Components;


namespace Voltage.Witches.Models
{
    using Voltage.Story.StoryDivisions;
    using Voltage.Witches.User;

	using Voltage.Common.Logging;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Witches.Converters;
    using Voltage.Witches.Scheduling;

    using Voltage.Witches.Services;

	public interface IPlayerFactory
	{
		Player Create (PlayerDataStore dataStore, MasterConfiguration masterConfig);
	}

	public interface IPlayerFactory<T> where T: Player
	{
		T Create (PlayerDataStore dataStore, MasterConfiguration masterConfig);
	}




	public class WitchesPlayerFactory : IPlayerFactory
	{
		private readonly bool _localPlayer;

        private readonly IPlayerWriter _writer;

		private readonly MonitoringNetworkController _networkController;
		private readonly ILogger _logger;
		private readonly DictionaryToJsonConverter<string,int> _affinityJsonConverter;
		private readonly DictionaryToJsonConverter<string,string> _choiceJsonConverter;

        private readonly IBuildNumberService _buildNumberService;

        public WitchesPlayerFactory(MonitoringNetworkController networkController, IPlayerWriter writer, DictionaryToJsonConverter<string,int> affinityJsonConverter, 
            DictionaryToJsonConverter<string,string> choiceJsonConverter,IBuildNumberService buildNumberService, ILogger logger, bool localPlayer=false)
		{
			_networkController = networkController;
            _writer = writer;
			_logger = logger;

			_affinityJsonConverter = affinityJsonConverter;
			_choiceJsonConverter = choiceJsonConverter;

            _buildNumberService = buildNumberService;

			_localPlayer = localPlayer;
		}

		public Player Create(PlayerDataStore playerDataStore, MasterConfiguration masterConfig)
		{
			List<Spellbook> books = GetBooks (playerDataStore, masterConfig);
			PlayerStaminaManager staminaManager = new PlayerStaminaManager(masterConfig.Ticket_Refresh_Rate, masterConfig.Max_Tickets, playerDataStore);
			PlayerFocusManager focusManager = new PlayerFocusManager(masterConfig.Focus_Refresh_Rate, masterConfig.Max_Focus, playerDataStore);

            playerDataStore.header = new DataStoreHeader(_buildNumberService.GetBuildVersion());

			if(_localPlayer)
			{
				UnityEngine.Debug.LogWarning ("PlayerFactory::Create >>> Using Local Player!");
                return new PersistentPlayer (books, playerDataStore, _writer, staminaManager, focusManager);
			}
			else
			{
                return new WitchesNetworkedPlayer (_networkController, _logger, books, playerDataStore, _writer, 
	                _affinityJsonConverter, _choiceJsonConverter, staminaManager, focusManager);
			}
		}



		private List<Spellbook> GetBooks(PlayerDataStore playerDataStore, MasterConfiguration masterConfig)
		{
			ISpellbookFactoryNew bookFactory = new SpellbookFactoryNew (masterConfig, new RecipeFactoryNew (masterConfig)); // HACK!!! FIXME didn't want to change spellbookfactory interface

			List<Spellbook> books = new List<Spellbook>();
			if(playerDataStore.books.Count > 0)
			{
				foreach (var bookEntry in playerDataStore.books)
				{
					try
					{
						SpellbookRefConfig bookConfig = masterConfig.Books_Configuration.Books_Reference[bookEntry.Id];
						PlayerSpellbookConfiguration playerBookConfig = GetCorrespondingBookConfig(bookConfig, playerDataStore.books);
						books.Add(bookFactory.Create(playerBookConfig, bookConfig));
					}
					catch(System.Exception e)
					{
						System.Console.WriteLine(e.Message);
						throw;
					}
				}
			}

			return books;
		}

		PlayerSpellbookConfiguration GetCorrespondingBookConfig(SpellbookRefConfig bookData, List<PlayerSpellbookConfiguration> playerConfigs)
		{
			return playerConfigs.Find(x => (x.Id == bookData.Id));
		}

	}


}
