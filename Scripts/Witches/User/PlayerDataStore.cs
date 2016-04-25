using System;
using System.Collections.Generic;

namespace Voltage.Witches.User
{
	using Voltage.Witches.Configuration;


    /* A humble data object -- meant to be serialized and deserialized and serve as the backing data store for a higher level player object */
    [Serializable]
    public class PlayerDataStore
    {
		public DataStoreHeader header = new DataStoreHeader();

		public string userID = string.Empty;

		public string firstName = string.Empty;
		public string lastName = string.Empty;

        public int currencyGame = 0;
        public int currencyPremium = 0;

        public Dictionary<string, int> affinities = new Dictionary<string, int>();
        public int totalAffinity = 0;

        public List<string> availableScenes = new List<string>();
        public List<string> completedScenes = new List<string>();
        public string currentScene = string.Empty;
        public string currentNodeID = string.Empty;
		public int currentBitProgress = 0;
        
		public string currentHowTosScene = string.Empty;
        

        public List<string> sceneHistory = new List<string>();
        public Dictionary<string, string> sceneChoices = new Dictionary<string, string>();

        public Dictionary<string, string> currentOutfit = new Dictionary<string, string>();

        public Dictionary<string, List<string>> savedOutfits = new Dictionary<string, List<string>>();

        public Dictionary<string, int> inventory = new Dictionary<string, int>();

		public int staminaPotions;

		public List<PlayerSpellbookConfiguration> books = new List<PlayerSpellbookConfiguration>();	// requires Voltage.Witches.Configuration

		public int closetSpace;		
        public int stamina;
        public DateTime staminaLastUpdate;
		public string enableStaminaDeductionScene = string.Empty;

        public int focus;
        public DateTime focusLastUpdate;

		public bool didResetForNewTutorial = false;

		public bool tutorialFlag = false;
		public bool avatarTutorialFlag = false;

		public int tutorialProgress = 0;
		public int avatarTutorialProgress = 0;

		public Dictionary<string, int> currentAffectedCharacters = new Dictionary<string, int>();
        public bool shouldPromptUserForNotifications = false;
		public bool notificationsEnabled = true; // Defaulting to true so that existing users will have notifications enabled. New users rely on the server configuration to pass down this value


		// FIXME: need to sync JSONHashedPlayerDataSerializer (if being used)
		public DateTime timeToDisableStarterPack = DateTime.MinValue;
		public bool starterPackPurchased = false;


        public int bonusesReceivedCount = 0;
		public List<Voltage.Witches.Login.BonusItem> bonusItems = new List<Voltage.Witches.Login.BonusItem>();
		public int completedRouteCount = 0;




		public PlayerDataStore Clone()
		{
			PlayerDataStore proxy = this.MemberwiseClone() as PlayerDataStore;	// shallow copy!!!
			proxy.sceneChoices = new Dictionary<string,string>(this.sceneChoices);
			proxy.affinities = new Dictionary<string,int> (this.affinities);

			return proxy;
		}
    }

	public class DataStoreHeader 
	{
		[Newtonsoft.Json.JsonProperty(PropertyName="version")]
		public string Version { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName="last_save_date")]
        public DateTime LastSaveDate { get; set; }

        public DataStoreHeader(string version="")
        {
            Version = version;
            LastSaveDate = DateTime.UtcNow;
        }
	}
}

