
using System;
using System.Collections.Generic;

namespace Integration.Witches.Models 
{
    using NUnit.Framework;
	using Moq;

	using Voltage.Witches.Models;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Common.Logging;
	using Voltage.Witches.User;
	using Voltage.Witches.Converters;


    [TestFixture]
    public class TestWitchesNetworkedPlayer
    {
		private SpyMonitoringNetworkController _spyNetworkController;


		private Player CreatePlayer(PlayerDataStore dataStore)
		{
			_spyNetworkController = new SpyMonitoringNetworkController ();
			ILogger logger = new ConsoleLogger ();
            Mock<IPlayerWriter> writer = new Mock<IPlayerWriter>();

			DictionaryToJsonConverter<string,int> affinityConverter = new DictionaryToJsonConverter<string, int> ();
			DictionaryToJsonConverter<string,string> choiceConverter = new DictionaryToJsonConverter<string, string> ();

//			Mock<PlayerStaminaManager> staminaMgr = new Mock<PlayerStaminaManager> ();
//			Mock<PlayerFocusManager> focusMgr = new Mock<PlayerFocusManager> ();
			PlayerStaminaManager staminaMgr = new PlayerStaminaManager (100f, 100, dataStore);
			PlayerFocusManager focusMgr = new PlayerFocusManager (100f, 100, dataStore);
         
            return new WitchesNetworkedPlayer (_spyNetworkController, logger, new List<Spellbook> (), dataStore, writer.Object, affinityConverter, choiceConverter, staminaMgr, focusMgr);
		}

        [Test]
        public void Construct()
        {
			PlayerDataStore data = new PlayerDataStore ();
			var player = CreatePlayer(data);

			Assert.That (player, Is.TypeOf<WitchesNetworkedPlayer> ());
        }



		[Test]
		public void CompleteScene_DefaultPlayer_ValidParms()
		{
			PlayerDataStore data = new PlayerDataStore ();
			var player = CreatePlayer(data);
			
			player.CompleteScene(null);

			Dictionary<string,string> expectedParm = new Dictionary<string, string>
			{
				{"phone_id", string.Empty},
				{"affinities", @"{}"},		//  @"{""A"":0,""R"":0,""M"":0,""T"":0,""N"":0}"
				{"stamina_potions", "0"},
				{"choices", @"{}"},
				{"pendingStaminaPotions","0"}
			};

			Assert.That (_spyNetworkController.Parameters, Is.EquivalentTo (expectedParm));
		}

		[Test]
		public void CompleteScene_NewChoice_ValidParms()
		{
			PlayerDataStore data = new PlayerDataStore ();
			data.sceneChoices.Add ("choice1", "A");

			var player = CreatePlayer(data);
			
			player.CompleteScene(null);
			
			Dictionary<string,string> expectedParm = new Dictionary<string, string>
			{
				{"phone_id", string.Empty},
				{"affinities", @"{}"},		//  @"{""A"":0,""R"":0,""M"":0,""T"":0,""N"":0}"
				{"stamina_potions", "0"},
				{"choices", @"{""choice1"":""A""}"},
				{"pendingStaminaPotions","0"}
			};
			
			Assert.That (_spyNetworkController.Parameters, Is.EquivalentTo (expectedParm));
		}

		[Test]
		public void MultipleDeductStamina_NewChoice_OnlyLastChoiceInParms()
		{
			PlayerDataStore data = new PlayerDataStore () { stamina = 5 };
			var player = CreatePlayer(data);

			player.RecordSelection ("choice", "1", 0);
			player.DeductStamina();

			player.RecordSelection ("choice", "2", 1);
			player.DeductStamina ();

			Dictionary<string,string> expectedParm = new Dictionary<string, string>
			{
				{"phone_id", string.Empty},
				{"scene_id", string.Empty},
				{"node_id", string.Empty},
				{"affinities", @"{}"},		//  @"{""A"":0,""R"":0,""M"":0,""T"":0,""N"":0}"
				{"stamina_potions", "0"},
				{"choices", @"{""Selections/choice/2"":""B""}"},
				{"pendingStaminaPotions","0"}
			};
			
			Assert.That (_spyNetworkController.Parameters, Is.EquivalentTo (expectedParm));
		}


		[Test]
		public void MultipleDeductStamina_MultipleChoices_CorrectChoiceCount()
		{
			PlayerDataStore data = new PlayerDataStore () { stamina = 5 };
			var player = CreatePlayer(data);
			
			player.RecordSelection ("choice", "1", 0);
			player.DeductStamina();
			
			player.RecordSelection ("choice", "2", 1);
			player.RecordSelection ("choice", "3", 2);
			player.DeductStamina ();


			Dictionary<string,string> expectedParm = new Dictionary<string, string>
			{
				{"phone_id", string.Empty},
				{"scene_id", string.Empty},
				{"node_id", string.Empty},
				{"affinities", @"{}"},		//  @"{""A"":0,""R"":0,""M"":0,""T"":0,""N"":0}"
				{"stamina_potions", "0"},
				{"choices", @"{""Selections/choice/2"":""B"",""Selections/choice/3"":""C""}"},
				{"pendingStaminaPotions","0"}
			};
			
			Assert.That (_spyNetworkController.Parameters, Is.EquivalentTo (expectedParm));
		}


		[Test]
		public void SelectionDeductThenComplete_NoChoices()
		{
			PlayerDataStore data = new PlayerDataStore () { stamina = 5 };
			var player = CreatePlayer(data);
			
			player.RecordSelection ("choice", "1", 0);
			player.DeductStamina();

			player.CompleteScene(null);
			
			Dictionary<string,string> expectedParm = new Dictionary<string, string>
			{
				{"phone_id", string.Empty},
				{"affinities", @"{}"},		
				{"stamina_potions", "0"},
				{"choices", @"{}"},
				{"pendingStaminaPotions","0"}
			};
			
			Assert.That (_spyNetworkController.Parameters, Is.EquivalentTo (expectedParm));
		}



		private class SpyMonitoringNetworkController: IMonitoringNetworkController
		{
			public IDictionary<string,string> Parameters { get; set; }

			public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
			{
				Parameters = parms;

				return default(INetworkTransportLayer);
			}

			public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
			{
				Parameters = parms;

				return default(INetworkTransportLayer);
			}

			public bool IsConnected { get { return true; } }
			#pragma warning disable 67
			public event EventHandler ConnectionStatusEvent;
			#pragma warning restore 67
		}


		private class SpyParametersNetworkController : INetworkTimeoutController<WitchesRequestResponse>
		{
			public IDictionary<string,string> Parameters { get; set; }

			public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
			{
				Parameters = parms;

				return default(INetworkTransportLayer);
			}

			public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
			{
				Parameters = parms;

				return default(INetworkTransportLayer);
			}
		}


//		private PlayerDataStore _basicDataStore = new PlayerDataStore
//		{
//			userID = "12345",
//
//			firstName = "jon",
//			lastName = "doe",
//			nickName = "johnny",
//
//			currencyGame = 0,
//			currencyPremium = 0,
//			
//			alignment = 0,
//			public Dictionary<string, int> affinities = new Dictionary<string, int>()
//			{
//				{"A", 0},
//				{"B", 0},
//			},
//			totalAffinity = 0,
//			
//			availableScenes = new List<string>(),
//			completedScenes = new List<string>(),
//			currentScene = string.Empty;
//			currentNodeID = string.Empty;
//			
//			public string currentHowTosScene = string.Empty;
//			
//			
//			public List<string> sceneHistory = new List<string>();
//			public Dictionary<string, string> sceneChoices = new Dictionary<string, string>();
//			
//			public Dictionary<string, string> currentOutfit = new Dictionary<string, string>();
//			
//			public Dictionary<string, int> inventory = new Dictionary<string, int>();
//			
//			public int staminaPotions;
//			
//			public List<PlayerSpellbookConfiguration> books = new List<PlayerSpellbookConfiguration>();	// requires Voltage.Witches.Configuration
//			
//			public int closetSpace;		
//			public int stamina;
//			public DateTime staminaLastUpdate;
//			
//			public int focus;
//			public DateTime focusLastUpdate;
//
//		}
    }
}