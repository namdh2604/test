using System;
using System.Collections.Generic;


namespace Voltage.Witches.User
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;
	using System.IO;

	using System.Text;

	public class JSONHashedPlayerDataSerializer : IPlayerDataSerializer
	{
		private readonly JsonSerializer _serializer;
		private readonly JsonSerializer _deserializer;

        private readonly int _key;
        private readonly bool _applyHash;

        public JSONHashedPlayerDataSerializer(int key, bool applyHash=true)
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Converters.Add(new IsoDateTimeConverter());
			_serializer = JsonSerializer.Create(settings);
			_deserializer = new JsonSerializer();

            _key = key;
            _applyHash = applyHash;
		}

		public string Serialize(PlayerDataStore playerData, bool prettyPrint=false)
		{
			string result;

			using (StringWriter sw = new StringWriter())
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(sw))
				{
					if (prettyPrint)
					{
						jsonWriter.Formatting = Formatting.Indented;
					}

                    if (_applyHash)
                    {
                        HashedProxyData hashedData = new HashedProxyData().SetData(playerData, _key); 
                        _serializer.Serialize(jsonWriter, hashedData);
                    }
                    else
                    {
                        _serializer.Serialize(jsonWriter, playerData);
                    }
				}

				result = sw.ToString();
			}

			return result;
		}



		public PlayerDataStore Deserialize(string rawData)
		{
			PlayerDataStore result = null;

			using (StringReader sr = new StringReader(rawData))
			{
				using (JsonTextReader jsonReader = new JsonTextReader(sr))
				{
                    // HashedProxyData can handle both the unencrypted and encrypted jsons
                    HashedProxyData hashedData = _deserializer.Deserialize<HashedProxyData>(jsonReader);
                    result = hashedData.GetPlayerData(_key);
				}
			}

			return result;
		}


        // FIXME: FRAGILE! MUST stay in sync with PlayerDataStore!!!!
        // FIXME: possibly use a custom JSON converter to handle differing fields
        private sealed class HashedProxyData // : PlayerDataStore
		{

            // used for serialize/deserialize
            public HashedProxyData(){}

            public HashedProxyData SetData(PlayerDataStore playerData, int key) 
            {
                SetExposedData(playerData);
                SetHashedData(playerData, key); 

                return this;
            }

            // FIXME: MUST stay in sync with PlayerDataStore!!!!
            private void SetExposedData(PlayerDataStore playerData)
			{
                // FIXME: determine better way to set this data...custom json converter!
                userID = playerData.userID;
                firstName = playerData.firstName;
                lastName = playerData.lastName;
                currencyGame = playerData.currencyGame;
                currencyPremium = playerData.currencyPremium;
                affinities = playerData.affinities;
                totalAffinity = playerData.totalAffinity;
                availableScenes = playerData.availableScenes;
                completedScenes = playerData.completedScenes;
                currentScene = playerData.currentScene;
                currentNodeID = playerData.currentNodeID;
                currentBitProgress = playerData.currentBitProgress;
                currentHowTosScene = playerData.currentHowTosScene;
                sceneHistory = playerData.sceneHistory;
                sceneChoices = playerData.sceneChoices;
                currentOutfit = playerData.currentOutfit;
                savedOutfits = playerData.savedOutfits;
                inventory = playerData.inventory;
                staminaPotions = playerData.staminaPotions;
                books = playerData.books;
                closetSpace = playerData.closetSpace;
                stamina = playerData.stamina;
                staminaLastUpdate = playerData.staminaLastUpdate;
                enableStaminaDeductionScene = playerData.enableStaminaDeductionScene;
                focus = playerData.focus;
                focusLastUpdate = playerData.focusLastUpdate;
                didResetForNewTutorial = playerData.didResetForNewTutorial;
                tutorialFlag = playerData.tutorialFlag;
                avatarTutorialFlag = playerData.avatarTutorialFlag;
                tutorialProgress = playerData.tutorialProgress;
                avatarTutorialProgress = playerData.avatarTutorialProgress;
                currentAffectedCharacters = playerData.currentAffectedCharacters;
                notificationsEnabled = playerData.notificationsEnabled;
                bonusItems = playerData.bonusItems;
                bonusesReceivedCount = playerData.bonusesReceivedCount;
				completedRouteCount = playerData.completedRouteCount;
			}

            private void SetHashedData(PlayerDataStore playerData, int key) 
			{
                timeToDisableStarterPack = EncryptDecryptXOR(playerData.timeToDisableStarterPack.ToString(), key); 
                starterPackPurchased = EncryptDecryptXOR(playerData.starterPackPurchased.ToString(), key); 
			}

			


            public PlayerDataStore GetPlayerData(int key) 
            {
                // HACK: validation to handle unencrypted/encrypted configuration changes
                DateTime starterPackDateValue;
				if (string.IsNullOrEmpty (this.timeToDisableStarterPack)) 
				{
					this.timeToDisableStarterPack = DateTime.MinValue.ToString();
				}
				if(!DateTime.TryParse(this.timeToDisableStarterPack, out starterPackDateValue))
                {
                    // NOTE: if key differs from one used to hash field, parse will throw an exception
                    starterPackDateValue = DateTime.Parse(EncryptDecryptXOR(this.timeToDisableStarterPack, key));   // ParseExact
                }

                // HACK: validation to handle hash-to-not-hashed configuration changes
                if (string.IsNullOrEmpty (this.starterPackPurchased)) 
                {
                    this.starterPackPurchased = "False";
                }
                bool packPurchased = false; 
                if (!Boolean.TryParse(this.starterPackPurchased, out packPurchased))    // handle unencrypted case with 'this.starterPackPurchased[0].ToUpper() + this.starterPackPurchased.Substring(1)'? string is immutable so won't affect this.starterPackPurchased
                {
                    packPurchased = Convert.ToBoolean(EncryptDecryptXOR(this.starterPackPurchased, key)); 
                }


                return new PlayerDataStore() 
                {
                    // FIXME: determine better way to set this data...custom json converter!
                    userID = this.userID,
                    firstName = this.firstName,
                    lastName = this.lastName,
                    currencyGame = this.currencyGame,
                    currencyPremium = this.currencyPremium,
                    affinities = this.affinities,
                    totalAffinity = this.totalAffinity,
                    availableScenes = this.availableScenes,
                    completedScenes = this.completedScenes,
                    currentScene = this.currentScene,
                    currentNodeID = this.currentNodeID,
                    currentBitProgress = this.currentBitProgress,
                    currentHowTosScene = this.currentHowTosScene,
                    sceneHistory = this.sceneHistory,
                    sceneChoices = this.sceneChoices,
                    currentOutfit = this.currentOutfit,
                    savedOutfits = this.savedOutfits,
                    inventory = this.inventory,
                    staminaPotions = this.staminaPotions,
                    books = this.books,
                    closetSpace = this.closetSpace,
                    stamina = this.stamina,
                    staminaLastUpdate = this.staminaLastUpdate,
                    enableStaminaDeductionScene = this.enableStaminaDeductionScene,
                    focus = this.focus,
                    focusLastUpdate = this.focusLastUpdate,
                    didResetForNewTutorial = this.didResetForNewTutorial,
                    tutorialFlag = this.tutorialFlag,
                    avatarTutorialFlag = this.avatarTutorialFlag,
                    tutorialProgress = this.tutorialProgress,
                    avatarTutorialProgress = this.avatarTutorialProgress,
                    currentAffectedCharacters = this.currentAffectedCharacters,
                    notificationsEnabled = this.notificationsEnabled,

                    bonusesReceivedCount = this.bonusesReceivedCount,
                    bonusItems = this.bonusItems,

					completedRouteCount = this.completedRouteCount,
                    // encrypted values
                    timeToDisableStarterPack = starterPackDateValue,
                    starterPackPurchased = packPurchased,


                };
            }



            private string EncryptDecryptXOR(String input, int key)
            {
                char[] output = input.ToCharArray();
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = (char)(output[i] ^ key);
                }

                return new string(output);
            }


            // FIXME: determine better way to set this data...custom json converter!
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
            public List<Voltage.Witches.Configuration.PlayerSpellbookConfiguration> books = new List<Voltage.Witches.Configuration.PlayerSpellbookConfiguration>(); 
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
            public bool notificationsEnabled = true;

            public int bonusesReceivedCount = 0;
            public List<Voltage.Witches.Login.BonusItem> bonusItems = new List<Voltage.Witches.Login.BonusItem>();

			public int completedRouteCount = 0;

            // HN specified that the field names should be "obfuscated"
//            [JsonProperty(PropertyName="foo")]
            public string timeToDisableStarterPack = DateTime.MinValue.ToString();  
//            [JsonProperty(PropertyName="bar")]
            public string starterPackPurchased = "False";       // needs to be in Boolean.TrueString/Boolean.FalseString format for Boolean::TryParse
		}



	}
}





// using System.Security.Cryptography;

//  private readonly MD5 _md5;
// _md5 = MD5.Create();
// doh! can't reverse a hash
//private string GetHash(string value)
//{
//    if (!string.IsNullOrEmpty(value) && _applyHash) 
//    {   
//        byte[] bytes = Encoding.UTF8.GetBytes (value);
//        byte[] hash = _md5.ComputeHash (bytes); 
//
//        StringBuilder sb = new StringBuilder();
//        foreach(byte b in hash)
//        {
//            sb.Append(b.ToString("X2"));
//        }
//
//        return sb.ToString();
//    }
//
//    return value;
//}
