using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Components;
using Voltage.Witches.Configuration;
using Voltage.Witches.Models;
using Voltage.Witches.Screens;

namespace Voltage.Witches.Controllers
{
	using URLs = Voltage.Witches.Net.URLs;
//	using WitchesNetworkController = Voltage.Witches.Net.WitchesNetworkController;
	using Voltage.Witches.Net;
	using Voltage.Common.Net;
	using Newtonsoft.Json.Linq;
	using Newtonsoft.Json;
	using Voltage.Witches.Controllers.Factories;
	using Voltage.Common.Logging;
	using Voltage.Common.DebugTool.Timer;

	public class TracingScreenController : ScreenController 
	{
		private const string UNITY_SCENE_NAME = "MinigameScene";

		Player _player;
		IScreenFactory _factory;
		IControllerRepo _repo;
		MasterConfiguration _config;

		TracingManager _screen;
		iGUISmartPrefab_MiniGameUI _uiScreen;

		int _difficulty;
		QueuedObjects _wager;
		RuneCauldronGameData _gameData;
		public IRuneCauldronGameData GameData { get { return _gameData; } }
		INetworkTimeoutController<WitchesRequestResponse> _networkController;

		AudioController _audio;

		public TracingScreenController(ScreenNavigationManager controller, Player player, IControllerRepo repo, MasterConfiguration masterConfig):base(controller)
		{
			AmbientDebugTimer.Current.Start ("Initializing Tracing Screen...");

			_player = player;
			_repo = repo;
			_config = masterConfig;
			_factory = _repo.Get<IScreenFactory>();
			_networkController = _repo.Get<INetworkTimeoutController<WitchesRequestResponse>>();
			PassInGameData(_config.Game_Properties_Config);

			AmbientDebugTimer.Current.Stop();

			Voltage.Common.Unity.UnitySingleton.Instance.StartCoroutine(InitializeTracing());

			_audio = AudioController.GetAudioController();
//			PlayMiniGameMusic();
		}

        public override void Dispose()
        {
            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
            if (_uiScreen != null)
            {
                _uiScreen.Dispose();
                _uiScreen = null;
            }

			SceneManager.UnloadScene(UNITY_SCENE_NAME);
        }
        

		public List<float> GetRecipeTargetScoreScalars()
		{
			var recipe = _wager.Current_Recipe;
			return recipe.ScoreScalars;
		}

		public void SetQueuedObject(QueuedObjects queue)
		{
			if(queue == null)
			{
				_wager = CreateTestQueue();
			}
			else
			{
				_wager = queue;
			}
			
//			Debug.LogWarning("Queued Objects is null? :: " + (_wager == null).ToString());
		}

		public void SetDifficulty(int difficultyType)
		{
			_difficulty = difficultyType;
		}

		private IEnumerator InitializeTracing()
		{
			AmbientDebugTimer.Current.Start("Loading Level Additively...TestTracingScene");
			AsyncOperation async = SceneManager.LoadSceneAsync(UNITY_SCENE_NAME, LoadSceneMode.Additive);

			yield return async;
			AmbientDebugTimer.Current.Stop();
			Manager.Add(this);
			_screen = GameObject.FindObjectOfType<TracingManager>();

			_uiScreen = _factory.GetScreen<iGUISmartPrefab_MiniGameUI>() as iGUISmartPrefab_MiniGameUI;
			_uiScreen.Init(this);
			try
			{
				_screen.SetController(this,DisplayPopup);
			}
			catch(Exception e)
			{
				throw e;
			}
			_screen.SetUI(_uiScreen);

//			DisplayPopup();
		}

		QueuedObjects CreateTestQueue()
		{
			var potions = CreateTestPotionList();
			var ingredients = CreateTestIngredientList();

			Recipe recipe = new Recipe("Test");
			recipe.SetProducts(potions);
			recipe.SetStage(0);

			QueuedObjects wager = new QueuedObjects(recipe, ingredients);
			return wager;
		}

		List<Item> CreateTestPotionList()
		{
			List<Item> testPotions = new List<Item>();
			for(int i = 0; i < 3; ++i)
			{
				var id = i.ToString();
				string name = string.Empty;
				if(i == 0)
				{
					name = "Test";
				}
				else if(i == 1)
				{
					name = "Superior Test";
				}
				else
				{
					name = "Master Test";
				}
				
				Dictionary<string,int> testDictionary = new Dictionary<string, int>()
				{
					{"A",(1000 * (i + 1))}
				};
				Potion newPotion = new Potion(id,name,(name + " " + id),"FFFFFF",testDictionary);
				newPotion.Category = ItemCategory.POTION;
				testPotions.Add(newPotion);
			}

			return testPotions;
		}
			
		List<Ingredient> CreateTestIngredientList()
		{
			List<Ingredient> testIngredients = new List<Ingredient>();

			for(int i = 0; i < 3; ++i)
			{
				var id = i.ToString();
				var type = "Type " + id;
				var name = "Ingredient " + id;

				var value = (i + 1) * 20;

				IngredientCategory category = new IngredientCategory(id,type);
				Ingredient ingredient = new Ingredient(id,name,category,value,false);
				testIngredients.Add(ingredient);
			}

			return testIngredients;
		}

		void DisplayPopup()
		{
			PlayMiniGameMusic();
			var potion = _wager.Current_Recipe.Products[0] as Potion;
			var dialog = GetIntroPopup(GetRuneCountFromDifficulty().ToString(),potion);
			HandlePlayAudioClip("Popup Redux 1");
			dialog.Display(HandleClosePopup);
			(dialog as iGUISmartPrefab_PlayIntroPopup).PlayIntroAnimation();
		}

		int GetRuneCountFromDifficulty()
		{
			//PRODUCTION ASKED TO TEST THIS ON DEVICE WITH 3
//			switch((Difficulty)_difficulty)
//			{
//				case Difficulty.EASY:
//					return 3;
//				case Difficulty.NORMAL:
//					return 4;
//				case Difficulty.TRICKY:
//					return 5;
//				case Difficulty.HARD:
//					return 6;
//				case Difficulty.TROUBLE:
//					return 7;
//			}

			return 3;
		}



		void HandleClosePopup(int answer)
		{
			BeginMiniGame();
		}

		public void BeginMiniGame()
		{
			_screen.BeginMiniGame(_difficulty);
		}

		void PassInGameData(GamePropertiesConfiguration gameConfig)
		{
			string name = "Rune Cauldron";
			var dataDictionary = gameConfig.Mini_Game_Difficulty;
			var speedAndZones = gameConfig.Mini_Game_Speed_And_Zones;
			var scoring = gameConfig.Mini_Game_Scoring;
			try
			{
				_gameData = new RuneCauldronGameData(name,dataDictionary,speedAndZones,scoring);
			}
			catch(Exception e)
			{
				UnityEngine.Debug.Log("Failed to make rune cauldron data: \n\n" + e.ToString());
				throw e;
			}
		}

//		public void SendMiniGameMail(int stage)
//		{
//			if(stage > 0)
//			{
//				Debug.Log("Can send mail...");
//				var player = _player;
//				string userID = player.UserID;
//
//				var recipe = _wager.Current_Recipe;
//				var recipeID = recipe.Id;
//				if(((int)recipe.CurrentStage) < stage)
//				{
//					recipe.SetStage(stage);
//				}
//				var potionID = (recipe.GetProductForCompleteStage(stage) as Potion).Id;
//				
//				Dictionary<string,string> parameters = new Dictionary<string, string> ()
//				{
//					{"phone_id",userID},
//					{"recipe_id",recipeID},
//					{"potion_id",potionID},
//					{"stars",stage.ToString()}
//				};
//				
//				_networkController.Send(URLs.SAVE_RECIPE,parameters,HandleSuccess,HandleFail);
//			}
//			else
//			{
//				Debug.Log("Cannot send mail...");
//			}
//		}

		public void CompleteMiniGame(CompletionStage result)
		{
			AmbientLogger.Current.Log ("Completing MiniGame...result: " + result, LogLevel.INFO);

			IRecipe recipe = _wager.Current_Recipe;

			_player.UpdateMiniGameProgress(recipe.Id, result);
			SendMiniGameMail(recipe, result);	// TODO: maybe move into WitchesNetworkedPlayer.UpdateMiniGameProgress

			DeductIngredients();		// TODO: move into Player (add interface), server call to NetworkedPlayer
		}

		public void SendMiniGameMail(IRecipe recipe, CompletionStage result)
		{
			if(recipe.CurrentStage > 0)
			{
				AmbientLogger.Current.Log (string.Format("Sending Progress: {0} [{1}]", recipe.Name, result), LogLevel.INFO);

//				int stage = (int)recipe.CurrentStage;
//				Item potion = recipe.GetProductForCompleteStage(stage); // as Potion;
				int score = (int)result;
//				Item potion = recipe.GetProductForCompleteStage(score);
				
				Dictionary<string,string> parameters = new Dictionary<string, string> ()
				{
					{"phone_id", _player.UserID},
					{"recipe_id", recipe.Id},
//					{"potion_id", potion.Id},
					{"stars", score.ToString()}
				};
				
				_networkController.Send(URLs.SAVE_RECIPE, parameters, HandleSuccess, HandleFail);
			}
			else
			{
				 AmbientLogger.Current.Log ("No progress made for recipe: " + recipe.Name, LogLevel.INFO);
			}
		}

		
		void HandleSuccess (WitchesRequestResponse obj)
		{
			UnityEngine.Debug.Log("Save recipe mail sent successfully");
		}
		
		void HandleFail (WitchesRequestResponse obj)
		{
			UnityEngine.Debug.LogWarning("Save recipe call was UNSUCCESSFUL");
		}

		public void DeductIngredients()
		{
			var player = _player;
			string userID = player.UserID;

			var ingredients = _wager.Wagered_Ingredients;
			List<string> ingredientIDs = new List<string>();

			for(int i = 0; i < ingredients.Count; ++i)
			{
				var ingredient = ingredients[i];
				if(!ingredient.IsInfinite)
				{
					var id = ingredient.Id;
					_player.UpdateInventory(id,-1);
					ingredientIDs.Add(id);
				}
			}

			if(ingredientIDs.Count > 0)
			{
				var serialized = JsonConvert.SerializeObject(ingredientIDs);

				string ingredientsString = serialized.ToString();

				Dictionary<string,string> parameters = new Dictionary<string,string>()
				{
					{"phone_id",userID},
					{"ingredients",ingredientsString}
				};

				_networkController.Send(URLs.USE_INGREDIENT, parameters, HandleIngredientSuccess, HandleIngredientFail);
			}
		}

		void HandleIngredientSuccess (WitchesRequestResponse obj)
		{
			UnityEngine.Debug.Log("Use Ingredient sent successfully");
		}

		void HandleIngredientFail (Voltage.Common.Net.WWWNetworkPayload obj)
		{
			UnityEngine.Debug.LogWarning("Use Ingredient call was UNSUCCESSFUL");
		}

		public void PauseGame()
		{
			_screen.Pause();
		}

		public void ResumeGame()
		{
			_screen.Resume();
		}

		public IDialog GetIntroPopup(string runeCount,Potion potion)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_PlayIntroPopup>();
			dialog.SetRunes(runeCount);
			dialog.SetPotion(potion);
			return dialog;
		}
		
		public IDialog GetResultsDialog(int score,int stages)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_PotionCompleteDialog>();
			Potion potion = _wager.Current_Recipe.GetProductForCompleteStage(stages) as Potion;
			dialog.AssignParameters(score,stages,potion);
			dialog.IsEditorTesting = _screen.IsUsingEditorValues;
			PlayMenuMusic();
			return dialog;
		}
		
		public IDialog GetPauseDialog()
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_PauseAndSettingsDialog>();
			dialog.Init(this);
			return dialog;
		}
		
		public IDialog GetQuitConfirmDialog()
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_GiveUpConfirmationDialog>();
			dialog.SetRecipe(_wager.Wagered_Ingredients);
			return dialog;
		}

		public void MoveToIngredientsSelectionScreen()
		{
			IIngredientsSelectScreenControllerFactory factory = _repo.Get<IIngredientsSelectScreenControllerFactory>();
			IScreenController nextScreen = factory.Create(_wager.Current_Recipe);
			Manager.OpenScreenAtPath(nextScreen, "/Home/Bookshelf");
		}
		
		public void GoToMailBox()
		{
			IScreenController nextScreen = _repo.Get<MailboxScreenController>();
            Manager.OpenScreenAtPath(nextScreen, "/Home");
		}
		
		public void MoveToPotionSelectScreen()
		{
			IScreenController nextScreen = _repo.Get<BookshelfScreenController>();
			Manager.OpenScreenAtPath(nextScreen, "/Home");
		}

		public void PlayMiniGameMusic()
		{
			_audio.PlayMinigameMusic();
			_audio.PlayStaticLoopedClip("Bubbling Cauldron");
		}

		public void PlayMenuMusic()
		{
            _audio.PlayBGMTrack(AudioController.DEFAULT_MUSIC);
			_audio.KillFXTracks();
		}

		public void HandleLoopedAudioClipPlay(string clipName)
		{
			_audio.PlayClipFromString(clipName,1.0f,true);
		}
		
		public void HandlePlayAudioClip(string clipName)
		{
			_audio.PlayClipFromString(clipName,1.0f,false);
		}

		protected override IScreen GetScreen()
		{
			return null;
		}


		// HACK: to help Quit button know when it can call MakePassive against the screen controller and when it should defer to the dialogue
		public bool ShowingPauseDialogue { get { return _uiScreen.ShowingPauseDialogue; } }

		public override void MakePassive(bool value)
		{
			_uiScreen.MakePassive (value);

			if(value) 
			{
				_screen.Pause();
			} 
			else 
			{
				_screen.Resume();
			}
		}
	}
}