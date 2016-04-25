
using System;
using System.Collections.Generic;
using System.Linq;
using Voltage.Common.Logging;
using Voltage.Witches.Exceptions;

namespace Voltage.Witches.Models
{
	using System.Runtime.Serialization;

    using Voltage.Common.Collections;

	using Voltage.Story.User;
    using Voltage.Witches.User;

	using Voltage.Story.General;
	using Voltage.Common.Serialization;
	using Voltage.Story.StoryDivisions;
    using Voltage.Story.Models.Nodes;
    using Voltage.Witches.Models.Avatar;

	using Voltage.Story.Mapper;

	using Voltage.Witches.Models;

	using Voltage.Witches.Configuration;

    using Voltage.Witches.Login;

    using System.Collections.ObjectModel;       // for Readonly list


	public class Player : IPlayer
	{
        
        private IPlayerWriter _writer;
        private PlayerDataStore _dataStore;
        private readonly PlayerStaminaManager _staminaManager;
        private readonly PlayerFocusManager _focusManager;


		protected PlayerDataStore CloneDataStore()	// or make _dataStore protected, IF a need arises...
		{
			return _dataStore.Clone ();
		}


		public string FirstName { get { return _dataStore.firstName; } }
		public string LastName { get { return _dataStore.lastName; } }
		public string FullName { get { return string.Format ("{0}{1}", !string.IsNullOrEmpty (FirstName) ? FirstName : string.Empty, !string.IsNullOrEmpty (LastName) ? " " + LastName : string.Empty); } }

		public virtual bool NotificationsEnabled { 
			get { return _dataStore.notificationsEnabled; }
			set { 
				_dataStore.notificationsEnabled = value; 
				if (value && (OnNotificationEnable != null))
				{
					OnNotificationEnable(this, new EventArgs());
				}
			}
		}


		public string UserID { get { return _dataStore.userID; } }	


      public Player(PlayerDataStore dataStore, List<Spellbook> books, IPlayerWriter writer, PlayerStaminaManager staminaManager, PlayerFocusManager focusManager)
		{
			_staminaManager = staminaManager;
			
			_focusManager = focusManager;
			
			_books = books;
			
            _writer = writer;
			
			_dataStore = dataStore; 
			
			CreateResourceEventHandlers();
		}



		//HACK To get around the tutorial thing for the mailbox
		public bool ShowMailBoxHowTo { get; set; }

        public virtual void SetPlayerName(string firstName, string lastName)
        {
            _dataStore.firstName = firstName;
            _dataStore.lastName = lastName;
        }



		public virtual void ExchangePotionForStamina()
		{
			if(!IsStaminaMaxed())
			{
				UpdateStaminaPotion(-1);
				_staminaManager.GiveStamina(1);
			}
		}

		public bool CanRefillStamina			// IsStaminaMaxed added, so can probably remove, SHOULD test for whether refill is enabled
		{
			get { return !IsStaminaMaxed(); }
		}



		public int Stamina 
		{ 
            get { return _staminaManager.Stamina; }
		}

        public bool IsStaminaMaxed()
        {
            return _staminaManager.IsMaxed();
        }

        public DateTime StaminaNextUpdate { 
            get { return _staminaManager.NextUpdate; }
        }

        public DateTime FocusNextUpdate {
            get { return _focusManager.NextUpdate; }
        }


		public virtual void IncreaseBitProgress()
		{
			_dataStore.currentBitProgress += 1;
		}

		public  virtual void DeductStamina()
		{
            // Turned into a no-op because bit nodes no longer shoudl be deducting stamina.
            // However, we can't remove this call yet because the network deduct stamina call ends up telling the server what scene we're on


            // ignore the unreachable code until tutorial flag is implemented
//            #pragma warning disable 0162
//            if (false)  // if not tutorial, deduct stamina
            {
//                int newValue = _dataStore.stamina - value;
//                _dataStore.stamina = (newValue < 0 ? 0 : newValue);
//                _staminaManager.Consume();
            }
//            #pragma warning restore 0162
		}

		public bool StaminaDeductionEnabled
		{
			get { return DeductStaminaEnabled; } 
		}


		protected bool DeductStaminaEnabled
		{
			get 
			{
                IList<string> sceneHistory = new List<string>(CompletedScenes);

                if (!string.IsNullOrEmpty(CurrentScene))
                {
                    sceneHistory.Add(CurrentScene);
                }
                else
                {
                    AmbientLogger.Current.Log("Player::DeductStaminaEnabled >>> CurrentScene is null!", LogLevel.ERROR);
                }
                return sceneHistory.Contains(_dataStore.enableStaminaDeductionScene);   
			}	
		}

        public virtual void UpdateStamina()
        {
            _staminaManager.Update();
        }

        public event EventHandler StaminaUpdate;
		public event EventHandler PotionUpdate;
		public event EventHandler CurrencyUpdate;

		public event EventHandler OnNotificationEnable;


        public virtual ReadOnlyDictionary<string, int> GetInventoryData()
        {
            return new ReadOnlyDictionary<string, int>(_dataStore.inventory);
        }

		public Dictionary<string,int> GetAllAffinities()	// maybe don't expose...should be readonly
		{
			return new Dictionary<string,int> (_dataStore.affinities);
		}

		public int CumulativeCharacterAffinity 
		{
			get
			{
				int sum = 0;
				foreach(KeyValuePair<string,int> kvp in _dataStore.affinities)
				{
					sum += kvp.Value;
				}

				return sum;
			}
		}

		private void ResetCharacterAffinities()
		{
			List<string> characters = _dataStore.affinities.Keys.ToList();
			for(int i=0; i < characters.Count; ++i)
			{
				_dataStore.affinities[characters[i]] = 0;
			}
		}

		public int TotalPriorAffinity 
		{ 
			get { return _dataStore.totalAffinity; } 
			private set { _dataStore.totalAffinity = value; } 
		}

		public int TotalAffinity
		{
			get
			{
				return TotalPriorAffinity + CumulativeCharacterAffinity;
			}
		}

		private void ResetAffinity()
		{
			TotalPriorAffinity = TotalAffinity;
			ResetCharacterAffinities ();
		}

		public virtual void CompleteRoute()
		{
			ResetRoute ();

			IncrementCompletedRouteCount ();
			ResetAffinity ();
		}

		private void ResetRoute()
		{
			const string REPLAY_START_SCENE = "Prologue/Prologue/Answers and More Questions";

			// update completed scenes
			int sceneIndexForReplay = _dataStore.completedScenes.IndexOf (REPLAY_START_SCENE);			// technically order is not guaranteed for lists
			if (sceneIndexForReplay >= 0) 
			{
				List<string> completedSenesForReplay = _dataStore.completedScenes.GetRange (0, sceneIndexForReplay);
				_dataStore.completedScenes = completedSenesForReplay;
			
				// update choices
				Dictionary<string,string> choicesForReplay = new Dictionary<string, string> ();
				foreach (String scene in _dataStore.completedScenes) {
					foreach (KeyValuePair<string, string> selection in GetAllPrologueSelections ()) {
						if (selection.Key.Contains (scene)) {
							choicesForReplay.Add (selection.Key, selection.Value);
						}
					}
				}
				_dataStore.sceneChoices = choicesForReplay;
			} else {
				throw new WitchesException("Missing Prologue/Prologue/Answers and More Questions");
			}
				
		}

		private Dictionary<string,string> GetAllPrologueSelections()
		{
			Dictionary<string,string> prologueSelections = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> selection in _dataStore.sceneChoices) 
			{
				if (selection.Key.StartsWith("Selections/Prologue/Prologue/"))
				{
					prologueSelections.Add (selection.Key, selection.Value);
				}
			}
			return prologueSelections;
		}


		private void IncrementCompletedRouteCount()
		{
			_dataStore.completedRouteCount++;	
		}

		public int GetAffinity(string id)
		{
			if(!string.IsNullOrEmpty(id))
			{
				return _dataStore.affinities[id];	// NOTE: can throw a KeyNotFound Exception
			}
			else
			{
				throw new ArgumentNullException("Player::GetAffinity >>> No character id given");
			}
		}

		public void AddAffinity (string id, int value)
		{
            Dictionary<string, int> currentAffinities = _dataStore.affinities;

            if (!currentAffinities.ContainsKey(id))
            {
                throw new Exception("Attempted to add affinity to a missing character: " + id);
            }

            int currentValue = currentAffinities[id];

            // Business Req: Affinity cannot be negative
            int requestedAffinity = Math.Max(currentValue + value, 0);
            UpdateAffinity(id, requestedAffinity);
		}

        // HACK: Composer does not currently support 'else' clauses, and production did not make their conditionals mutually exclusive;
        // instead, there are many areas that look like: 'IF A > B DO X', or, 'IF A < B DO Y', but nothing accounts for A = B. I.e., equality is a big problem.
        // This is a hack to prevent affinities from every being equal to each other; In that case, the affinity is increment to prevent the collision.
        // In the event of multiple collisions, they daisy-chain until everything is unique
		private void UpdateAffinity(string id, int requestedValue, bool first=true)
        {
            string existingCharacterWithAffinity = GetCharacterWithAffinity(requestedValue);
			if (string.IsNullOrEmpty(existingCharacterWithAffinity))
			{
				// the value is unique, set it
				_dataStore.affinities[id] = requestedValue;
			}
			else
			{
				if (first)
				{
					UpdateAffinity (id, requestedValue + 1, false);
				}
				else
				{
					_dataStore.affinities[id] = requestedValue;
					UpdateAffinity(existingCharacterWithAffinity, requestedValue + 1, false);
				}
			}
        }

        private string GetCharacterWithAffinity(int affinity)
        {
            foreach (var characterPair in _dataStore.affinities)
            {
                if (characterPair.Value == affinity)
                {
                    return characterPair.Key;
                }
            }

            return string.Empty;
        }

		protected Dictionary<string,int> CharacterAffinities { get { return _dataStore.affinities; } }	// as readonly


        public IList<string> AvailableScenes
        {
            get
            {
                return _dataStore.availableScenes.AsReadOnly();
            }
        }

        public IList<string> CompletedScenes
        {
            get { return _dataStore.completedScenes.AsReadOnly(); }
        }



		public void DebugClearAvailableScenes()	// until we can decorate with DebugPlayer 
		{
			ClearAllAvailableScenes ();
		}

		public void ClearAllAvailableScenes()	
		{
			_dataStore.availableScenes = new List<string> ();
		}

        public virtual void AddAvailableScene(string scenePath)
        {
            List<string> currentScenes = _dataStore.availableScenes;
            if (!currentScenes.Contains(scenePath))
            {
                currentScenes.Add(scenePath);
            }
        }

        public virtual void RemoveScene(string scenePath)
        {
            _dataStore.availableScenes.Remove(scenePath);
        }

        public virtual void CompleteScene(Action<bool> onComplete)
        {
            HandleSceneComplete(true, onComplete);
        }

        // HACK: Please do not use this -- scene completion now needs to be network based
        // and asynchronous (for guaranteed mail delivery). This exists to allow the tutorial
        // to close out scenes that don't use the normal system
        public void CompleteLocalScene()
        {
            HandleSceneComplete(true, null);
        }

        protected virtual void HandleSceneComplete(bool success, Action<bool> onComplete)
        {
            if (success)
            {
                RemoveScene(CurrentScene);
                _dataStore.completedScenes.Add(CurrentScene);

                _dataStore.currentScene = string.Empty;
                _dataStore.currentNodeID = string.Empty;
                _dataStore.currentBitProgress = 0;
                _dataStore.sceneHistory.Clear();
                DestroyTrackingForCurrentAffinities();
            }

            if (onComplete != null)
            {
                onComplete(success);
            }
        }

        public virtual void StartScene(string sceneID)
        {
            _dataStore.currentScene = sceneID;
            _dataStore.currentNodeID = "00000";
        }

        public virtual void UpdateSceneProgress(string sceneID, string id)
        {
            _dataStore.currentScene = sceneID;
            _dataStore.currentNodeID = id;
            _dataStore.sceneHistory.Add(id);
        }
        
		public virtual void UpdateHowTosSceneProgress(string sceneID, string id)
		{
			_dataStore.currentHowTosScene = sceneID;
		}

        public int Focus { get { return _focusManager.Focus; } }
        public event EventHandler FocusUpdate;

        public bool IsFocusMaxed()
        {
            return _focusManager.IsMaxed();
        }

		public void RestoreFocus(int amount)
		{
			_focusManager.AddFocus(amount);
            _writer.Save(_dataStore);
		}

        public virtual void Refresh()
        {
            UpdateStamina();
            UpdateFocus();
        }
		
        public virtual void UpdateFocus()
		{
            _focusManager.Update();
        }

        public virtual void DeductFocus()
        {
            _focusManager.Consume();
        }

        private void OnCurrencyUpdate()
        {
            if (CurrencyUpdate != null)
            {
                CurrencyUpdate(this, new EventArgs());
            }
        }

        public virtual void UpdatePremiumCurrency(int amount)
        {
            _dataStore.currencyPremium += amount;
            OnCurrencyUpdate();
        }

        public virtual void UpdateCurrency(int amount)
        {
            _dataStore.currencyGame += amount;
            OnCurrencyUpdate();
        }


		public int StaminaPotions
		{
			get { return _dataStore.staminaPotions; }
		}

		public virtual void UpdateStaminaPotion(int difference)
		{
            if (difference == 0)
            {
                return;
            }
           
			AmbientLogger.Current.Log ("Player::UpdateStaminaPotion >>> Adding " + difference, LogLevel.INFO);
			_dataStore.staminaPotions += difference;

			if(PotionUpdate != null)
			{
				PotionUpdate(this, new EventArgs());
			}
		}


		public event Action OnInventoryUpdate;

        public virtual void UpdateInventory(string id, int amount)
        {
            if (!_dataStore.inventory.ContainsKey(id))
            {
                _dataStore.inventory[id] = 0;
            }

            _dataStore.inventory[id] += amount;

			if (OnInventoryUpdate != null) 
			{
				OnInventoryUpdate();
			}
        }



        private void CreateResourceEventHandlers()
        {
            if (_staminaManager != null)
            {
                _staminaManager.ResourceUpdate += delegate {
                    if (StaminaUpdate != null)
                    {
                        StaminaUpdate(this, new EventArgs());
                    }
                };
            }

            if (_focusManager != null)
            {
                _focusManager.ResourceUpdate += delegate {
                    if (FocusUpdate != null)
                    {
                        FocusUpdate(this, new EventArgs());
                    }
                };
            }
        }




        public event Action OnPurchaseStarterPack;
        public bool StarterPackPurchased { get { return _dataStore.starterPackPurchased; } }
        public DateTime TimeToDisableStarterPack { get { return _dataStore.timeToDisableStarterPack; } }

        public virtual void MakeStarterPackAvailable(double durationInDays)
        {
            if (!StarterPackPurchased && !StarterPackTriggered)
            {
                _dataStore.timeToDisableStarterPack = DateTime.UtcNow.AddDays(durationInDays);
            }
            else
            {
                AmbientLogger.Current.Log("Starter Pack Aleady Triggered!", LogLevel.INFO);
            }
        }

        public virtual void PurchaseStarterPack()
        {
            _dataStore.starterPackPurchased = true;

            if(OnPurchaseStarterPack != null)
            {
                OnPurchaseStarterPack();
            }
        }

        public bool StarterPackTriggered
        {
            get { return TimeToDisableStarterPack != DateTime.MinValue; }
        }


        public int BonusesReceivedCount { get { return _dataStore.bonusesReceivedCount; } }
        public ReadOnlyCollection<BonusItem> BonusItems { get { return _dataStore.bonusItems.AsReadOnly(); } }
		public virtual void AwardedBonusItem()
        {
            _dataStore.bonusesReceivedCount += 1;
            _dataStore.bonusItems = new List<BonusItem>();  // clear bonus items
        }









		public int CompletedRouteCount{ get { return _dataStore.completedRouteCount; } }

        public int Currency { get { return _dataStore.currencyGame; } }
		
		public int CurrencyPremium { get { return _dataStore.currencyPremium; } }
		

		List<Spellbook> _books;
		
		public int MaxBookId { get; set; }
		
		public int ClosetSpace { get { return _dataStore.closetSpace; } }
		public virtual void AddClosetSpace()
		{
            _dataStore.closetSpace += 5;
		}

		//TODO Probably dhould add something to update this every purchase?
		public int AvailableClosetSpace { get; set; }
		
		public List<Spellbook> GetBooks()
		{
			return _books;
		}

		public virtual void AddBook(ISpellbook book)
		{
			_books.Add (book as Spellbook);
			_dataStore.books.Add (new PlayerSpellbookConfiguration (book));
		}

        public virtual void UpdateOutfit(Outfit newOutfit)
        {
            Dictionary<string, string> outfit = new Dictionary<string, string>();
            Dictionary<OutfitCategory, string> categoryMapping = newOutfit.GetValues();
            foreach (var pair in categoryMapping)
            {
                outfit[pair.Key.ToString()] = pair.Value;
            }

            _dataStore.currentOutfit = outfit;
        }

        public virtual void SaveOutfitPreset(string name, Outfit outfit)
        {
            List<string> items = new List<string>(outfit.GetValues().Values);

            _dataStore.savedOutfits[name] = items;
        }

        public virtual Dictionary<string, Outfit> GetSavedOutfitPresets()
        {
            Dictionary<string, Outfit> outfits = new Dictionary<string, Outfit>();

            foreach (var savedOutfit in _dataStore.savedOutfits)
            {
                Outfit outfit = new Outfit();
                foreach (var item in savedOutfit.Value)
                {
                    outfit.WearItem(item);
                }

                outfits[savedOutfit.Key] = outfit;
            }

            return outfits;
        }

        public Outfit GetOutfit()
        {
            Outfit outfit = new Outfit();
            foreach (var pair in _dataStore.currentOutfit)
            {
                outfit.WearItem(pair.Value);
            }

            return outfit;
        }

        public string CurrentScene { get { return _dataStore.currentScene; } }
		public string CurrentNodeID { get { return _dataStore.currentNodeID; } }
		public int CurrentBitProgress { get { return _dataStore.currentBitProgress; } }

		public void ResetSceneProgress()
		{
			_dataStore.currentNodeID = "00000";
			_dataStore.currentBitProgress = 0;
			_dataStore.sceneHistory.Clear();
		}

		public string CurrentHowTosScene { get { return _dataStore.currentHowTosScene; } }


		protected Dictionary<string,string> GetAllSelectedChoices()
		{
			return new Dictionary<string,string> (_dataStore.sceneChoices);
		}

		public string GetSelectionChoice (string scenePath, string selectionName)
		{
			string fullPath = string.Format ("Selections/{0}/{1}", scenePath, selectionName);
            if (!_dataStore.sceneChoices.ContainsKey(fullPath))
            {
                return string.Empty;
            }

			return _dataStore.sceneChoices [fullPath];			// can throw a key not found exception
		}

		public void RecordSelection(Voltage.Story.StoryDivisions.Scene scene, SelectionNode node, int choice)
		{
			RecordSelection (scene.Path, node.Name, choice);
		}
		public void RecordSelection(string scenePath, string selectionName, int choice)
		{
			if (string.IsNullOrEmpty(selectionName))
			{
				return; // only interested in recording named nodes
			}

			string fullPath = "Selections/" + scenePath + "/" + selectionName;
			_dataStore.sceneChoices[fullPath] = TranslateChoice(choice);

			AmbientLogger.Current.Log (string.Format ("PLAYER: Recording Selection B: {0} > {1}", fullPath, choice), LogLevel.INFO);
		}


		public virtual void UpdateMiniGameProgress(string recipeID, CompletionStage level)
		{
//			IRecipe recipe = 	from book in _books
//								from recipeCandidate in book.Recipes
//								where (recipeCandidate.Id == recipeID);

			IRecipe recipe = _books.SelectMany (b => b.Recipes).FirstOrDefault (r => r.Id == recipeID);

			if(recipe != null && recipe.CurrentStage < level)
			{
				AmbientLogger.Current.Log(string.Format("Updating Recipe ({0}) Progress from ({1}) to ({2})", recipe.Name, recipe.CurrentStage.ToString(), level.ToString()), LogLevel.INFO);
				recipe.SetStage ((int)level);

				UpdateDataStoreRecipe(recipeID, level);
			}
		}

		private void UpdateDataStoreRecipe(string recipeID, CompletionStage level)
		{
			PlayerRecipeConfig recipe = _dataStore.books.SelectMany (b => b.Recipes).FirstOrDefault (r => r.ID == recipeID);

			recipe.CompletionStage = (int)level;

//			PlayerSpellbookConfig book = _dataStore.books.Where (b => b.Recipes.Contains (recipe));
//			book.IsComplete = pointless should just iterate thru recipe
		}



        public void UpdateAffinities(Dictionary<string, int> changes)
        {
            foreach (var pair in changes)
            {
                AddAffinity(pair.Key, pair.Value);
            }
        }

        private string TranslateChoice(int index)
        {
            char startingChoice = 'A';
            return ((char)(startingChoice + index)).ToString();
        }

        public virtual void Serialize()
        {
			_dataStore.header.LastSaveDate = DateTime.UtcNow;
            _writer.Save(_dataStore);
        }

		public int TutorialProgress {get { return _dataStore.tutorialProgress; } }

		public virtual void SetTutorialProgress(int step, string name)
		{
			_dataStore.tutorialProgress = step;
		}

		#region Ambient tutorial hack
		public int AvatarTutorialProgress {get { return _dataStore.avatarTutorialProgress; } }

		public virtual void SetAvatarTutorialProgress(int step, string name)
		{
			_dataStore.avatarTutorialProgress = step;
		}
		public bool AvatarTutorialFlag{ get { return _dataStore.avatarTutorialFlag; } }

		public virtual void StartAvatarTutorial()
		{
			_dataStore.avatarTutorialFlag = true;
		}

		public virtual void FinishAvatarTutorial()
		{
			_dataStore.avatarTutorialFlag = false;
		}
		#endregion

		public bool TutorialFlag{ get { return _dataStore.tutorialFlag; } }


		public virtual void FinishTutorial()
		{
			_dataStore.tutorialFlag = false;
		}


		public bool NewTutorialResetDone{ get { return _dataStore.didResetForNewTutorial; } }


		public void SetNewTutorialResetDone()
		{
			_dataStore.didResetForNewTutorial = true;
		}

		public virtual void RefillStamina()
		{
			_staminaManager.RefillStamina ();
		}

        public int GetSecondsUntilStaminaMaxed()
        {
            return _staminaManager.GetSecondsUntilMaxed();
        }

        public int GetSecondsUntilFocusMaxed()
        {
            return _focusManager.GetSecondsUntilMaxed();
        }

		public Dictionary<string, int> GetCurrentAffectedCharacters()
		{
			return _dataStore.currentAffectedCharacters;
		}

		//tracking characters who are in the current scene to show their affinities at the end of the scene
		public virtual void TrackCurrentSceneAffectedCharacters(string initial, int affinity)
		{
			Dictionary<string, int> currentAffectedCharacters = _dataStore.currentAffectedCharacters;
			if (currentAffectedCharacters.Count () > 0) 
			{
				UpdateAffectedCharactersAffinities(currentAffectedCharacters, initial, affinity);
			} else 
			{
				currentAffectedCharacters = new Dictionary<string,int> ();
				currentAffectedCharacters.Add (initial, affinity);
				_dataStore.currentAffectedCharacters = currentAffectedCharacters;
			}
			
		}
		
		private void UpdateAffectedCharactersAffinities(Dictionary<string, int> currentAffectedCharacters, string initial, int affinity)
		{
			for (int i = 0; i < currentAffectedCharacters.Count(); i++) 
			{
				if(currentAffectedCharacters.ContainsKey(initial))
				{
					currentAffectedCharacters[initial]+=affinity;
				}
				else
				{
					currentAffectedCharacters[initial] = affinity;
				}
			}
			_dataStore.currentAffectedCharacters = currentAffectedCharacters;
		}
		
		public virtual void DestroyTrackingForCurrentAffinities()
		{
			_dataStore.currentAffectedCharacters = new Dictionary<string,int>();
		}

        public virtual bool ShouldPromptUserForNotifications { 
            get { return _dataStore.shouldPromptUserForNotifications; }
            set { _dataStore.shouldPromptUserForNotifications = value; }
        }

	}
}
