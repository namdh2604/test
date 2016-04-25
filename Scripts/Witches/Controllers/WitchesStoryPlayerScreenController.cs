
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Controllers
{
	using Ninject;
	using Ninject.Modules;
	
	
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UnityEngine.UI;
	
	using Voltage.Witches.Controllers;
	using Voltage.Witches.Controllers.Factories;
	using Voltage.Witches.Screens;

	using StoryPlayer;
//	using StoryPlayer.User;
	
	using Voltage.Witches.Dependencies;
	using Voltage.Common.Unity;
	
	using Voltage.Witches.Models;
	using Voltage.Story.StoryPlayer;
	
	using Voltage.Story.StoryDivisions;
	
	using Voltage.Story.Views;
	using Voltage.Witches.Layout;

	using Voltage.Witches.Configuration;
	using Voltage.Story.Configurations;
	using Voltage.Witches.Story.StoryPlayer;

	using Newtonsoft.Json.Linq;	// FIXME: LayoutDisplay.SetConfiguration needs JObject

	using Voltage.Witches.Net;

	using Voltage.Common.Metrics;
	using Voltage.Witches.Metrics;
    using Voltage.Story.Effects;

	using Voltage.Common.Logging;

	using Voltage.Story.Reset;

	using Voltage.Witches.Exceptions;

	using TermLevel = Voltage.Story.StoryDivisions.Scene.TermLevel;

	using Voltage.Story.General;

	using Voltage.Witches.Shop;
	using Voltage.Witches.User;

    using Voltage.Witches.Story;

	using Voltage.Witches.UI;
	using StoryPlayerUIScreen = Voltage.Witches.UI.StoryPlayerUIScreen;
	using Voltage.Witches.Screens.Dialogues;


	using Voltage.Common.Net;
	using Voltage.Witches.DI;


	public class StoryPlayerScreenControllerData
	{
		public Player Player { get; set; }
		public StoryPlayerSettings Settings { get; set; }
		public MasterStoryData StoryData { get; set; }
		public string GameResourceConfiguration { get; set; }
		public Action OnFailure { get; set; }
//		public ScreenNavigationManager ScreenNavManager { get; set; }
		public IControllerRepo ControllerRepo { get; set; }
		public IEffectResolver EffectResolver { get; set; }
		public IStoryResetter StoryResetter { get; set; }
		public StoryParser StoryParser { get; set; }
		public IScreenFactory ScreenFactory { get; set; }

		public INetworkTimeoutController<WitchesRequestResponse> NetworkController { get; set; }	// any side-effect of making the storyplayerscreencontroller networked?

		public bool IsValid
		{
			get
			{
				return Player != null && Settings != null && Settings.Scene != null && !string.IsNullOrEmpty(GameResourceConfiguration) && OnFailure != null && ControllerRepo != null && EffectResolver != null && StoryResetter != null && StoryData != null && ScreenFactory != null;	// ScreenNavManager != null 
			}
		}
	}


	public class WitchesStoryPlayerScreenController : IScreenController
	{
		public string Name { get; private set; }

		protected readonly Player _player;
		protected readonly StoryPlayerSettings _storySettings;
		private readonly MasterStoryData _storyData;
		private readonly Action _onFailure;
		private readonly ScreenNavigationManager _navManager;
		private readonly IControllerRepo _repo;
		private readonly IEffectResolver _effectResolver;
		private readonly IStoryResetter _storyResetter;
		private readonly ISceneHeaderFactory _headerFactory;
        private readonly StoryParser _storyDependencyParser;

		private readonly BooksConfiguration _bookConfig;		// maybe encapsulate in EndScene handler class
		private readonly IFactory<string,ISpellbook> _bookFactory;


		public const string NORMAL_DIALOG = "normal";
		public const string TUTORIAL_DIALOG = "tutorial";

		private const string UNITY_SCENE_NAME = "StoryPlayerScreen";

		private string _currentHeaderPath;

		protected WitchesStoryPlayer _activeStoryPlayer;	// FIXME: should be IStoryPlayer

        private bool _showInterface;
        private readonly IStoryPlayerDialogController _dialogController;
        public const float DELAY_TIME = 15f;
        protected float tap_delay_time;

        public float GetTapDelayTime()
        {
            return tap_delay_time;
        }

        public void SetTapDelayTime(float delay)
        {
            tap_delay_time = delay;
        }
        



		// FIXME: maybe shouldn't bother using an event to handle this...OR have other scenarios use it too
		protected event Action OnStoryPlayerExit;	
		private void SendExitEvent()				// should this be called on Dispose as well?
		{
			if(OnStoryPlayerExit != null)
			{
				OnStoryPlayerExit();
			}
		}


		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;


		protected readonly StoryPlayerUIScreen _storyPlayerUIScreen;
		private readonly UIRibbonController _ribbonController;
		private readonly IScreenFactory _screenFactory;
		private readonly IShopDialogueController _shopDialogueController;
        private readonly StoryMusicPlayer _musicPlayer;

        public WitchesStoryPlayerScreenController(StoryPlayerScreenControllerData data, IStoryPlayerDialogController dialogController, 
            StoryMusicPlayer musicPlayer, bool showInterface)
		{
			if(data == null && !data.IsValid)
			{
				throw new WitchesException("WitchesStoryPlayerScreenController::Ctor >>> data is invalid");
			}

			Name = "WitchesStoryPlayerScreenController";
			AmbientLogger.Current.Log ("Creating StoryPlayerScreenController...", LogLevel.INFO);

			_player = data.Player;
			_storySettings = data.Settings;
			_storyData = data.StoryData;
			_onFailure = data.OnFailure;

			_repo = data.ControllerRepo;
			_navManager = _repo.Get<ScreenNavigationManager> ();
			_effectResolver = data.EffectResolver;
			_storyResetter = data.StoryResetter;
			_headerFactory = _repo.Get<ISceneHeaderFactory>();

			_bookConfig = _repo.Get<MasterConfiguration> ().Books_Configuration;		
			_bookFactory = _repo.Get<IFactory<string,ISpellbook>> ();
			_storyDependencyParser = data.StoryParser;	

            _showInterface = showInterface;
            _dialogController = dialogController;
            _musicPlayer = musicPlayer;

            tap_delay_time = DELAY_TIME;


			_networkController = data.NetworkController;


			_shopDialogueController = _repo.Get<IShopDialogueController> ();
			_screenFactory = _repo.Get<IScreenFactory> (); 						//data.ScreenFactory;
			
            _storyPlayerUIScreen = _screenFactory.GetScreen<StoryPlayerUIScreen> ();
            UIRibbonView ribbonView = _screenFactory.GetScreen<UIRibbonView>();
            _storyPlayerUIScreen.Init(ribbonView);

			MasterConfiguration masterConfig = _repo.Get<MasterConfiguration> ();

			_ribbonController = new UIRibbonController (_player, _shopDialogueController, _screenFactory, masterConfig, ribbonView);	// TODO: RibbonFactory.Create(IUIRibbonView) to return controller
			SubscribeToUIEvents ();

			ShowUI (false);

			string gameResourceConfigs = data.GameResourceConfiguration;
			UnitySingleton.Instance.StartCoroutine (InitScene (gameResourceConfigs, (layoutDisplay) => SetupStoryPlayer(_player, _storySettings, _storyPlayerUIScreen, layoutDisplay)));
		}




		// TODO: change to a system that returns dialogue for use in subclasses
		protected virtual void OnInfoButton()
		{	
			PlayerPreferences prefs = PlayerPreferences.GetInstance ();

			StoryPlayerInfoDialogue infoDialogue = _screenFactory.GetDialog<StoryPlayerInfoDialogue> ();
			infoDialogue.RefreshInfo (_player, _storySettings, prefs.SoundEnabled, prefs.SFXEnabled);
			infoDialogue.OnCloseButton += () => 
			{
				infoDialogue.Dispose();
				_storyPlayerUIScreen.EnableMask(false);

				// the passive and ui selected conditions must be set here since MakePassive will enable the screens input
				OnUISelected();
				MakePassive(false);
			};
			infoDialogue.OnDialogueButton += () => 
			{
				infoDialogue.Dispose();
				_storyPlayerUIScreen.EnableMask(false);
				GoToStoryMap();
			};

			infoDialogue.OnRefillStamina += () =>
			{	
				HandleRefillStamina();
				infoDialogue.RefreshInfo (_player, _storySettings, prefs.SoundEnabled, prefs.SFXEnabled);
			};

			_storyPlayerUIScreen.EnableMask (true);
			MakePassive (true);

//			return infoDialogue;
		}


		protected void HandleRefillStamina()
		{
			UnityEngine.Debug.Log ("Temporary Refill Stamina Behaviour");

			if(_player.StaminaPotions > 0)
			{
				if(_player.CanRefillStamina)
				{
					_player.ExchangePotionForStamina();
					
					SendStoryStaminaMetrics (MetricEvent.STORY_USE_STAMINA_POTION);
				}
			}
			else 
			{
				SendStoryStaminaMetrics (MetricEvent.STORY_INSUFFICIENT_STAMINA_POTION);
			}
		}







		private void SubscribeToUIEvents()
		{	
			_storyPlayerUIScreen.OnInfoButton += OnInfoButton;

			_ribbonController.OnShopOpen += OnShopShown;
			_ribbonController.OnShopClosed += OnShopHide;

			// Key on UI clicks
			_storyPlayerUIScreen.OnUISelected += OnUISelected;
			_ribbonController.OnOpenEvent += OnUISelected;
			_ribbonController.OnCloseEvent += OnUISelected;	
		}

		private void UnSubscribeFromUIEvents()
		{
			_storyPlayerUIScreen.OnInfoButton -= OnInfoButton;
			
			_ribbonController.OnShopOpen -= OnShopShown;
			_ribbonController.OnShopClosed -= OnShopHide;
			
			// Key on UI clicks
			_storyPlayerUIScreen.OnUISelected -= OnUISelected;
			_ribbonController.OnOpenEvent -= OnUISelected;
			_ribbonController.OnCloseEvent -= OnUISelected;	
		}


		private void OnShopShown()
		{
			MakePassive (true);
		}
		private void OnShopHide()
		{
			MakePassive (false);
		}

		private void OnStaminaPromptShown()
		{
			MakePassive (true);
		}

		private void OnStaminaPromptHide()
		{
			MakePassive (false);
		}

        

		private bool _inSelection = false;

		private bool _playDisabled = false;
		protected int _passiveCounter = 0;
		public void MakePassive(bool value)
		{
			// counter to keep track of passive state
            _passiveCounter += (value ? 1 : -1);
			_passiveCounter = (_passiveCounter < 0 ? 0 : _passiveCounter);
            bool vettedValue = (_passiveCounter > 0);

//			if (vettedValue != value) 
//			{
//                AmbientLogger.Current.Log(string.Format("WitchesStoryPlayerScreenController::MakePassive >>> passed in value: {0}, actual: {1}, passive counter: {2}", value, vettedValue, _passiveCounter), LogLevel.WARNING);
//			}

			MakeStoryPlayPassive (vettedValue);

			// FIXME: unfortunately defers control of UI to StoryPlayerInputLayoutDisplay::OnSelectionDisplay/OnSelectionExit events when at a SelectionNode
			if (!_inSelection) 		
			{
				MakeUIPassive (vettedValue);
			}
		}

		private void MakeStoryPlayPassive(bool value)
		{
			_playDisabled = value;
            _tapToContinueDialogue.MakePassive(value);

			MakeLayoutDisplayPassive(value);
		}

		// ensure that any element on LayoutDisplay is passive
		private void MakeLayoutDisplayPassive(bool value)
		{
			if (_layoutDisplay != null) 
			{
				_layoutDisplay.MakePassive (value);
			} 
			else 
			{
				UnityEngine.Debug.LogWarning ("MakeLayoutDisplayPassive >>> No LayoutDisplay!");
			}
		}


		public void MakeUIPassive(bool value)
		{
//			UnityEngine.Debug.LogWarning (string.Format ("WitchesStoryPlayerScreenController::MakeUIPassive({0})", value));
			_storyPlayerUIScreen.MakePassive (value);
		}

		// for determining whether a press is meant for the storyplayer or a UI element
		private bool _UISelected = false;
		protected void OnUISelected()
		{
            _tapToContinueDialogue.StartTimer(DELAY_TIME);
			_UISelected = true;
		}







		protected LayoutDisplay _layoutDisplay;
		private ILayoutDisplay _storyPlayerLayoutDisplay;	// FIXME: would prefer to replace _layoutDisplay, but its providing additional functionality that I'm going to leave for now
		private TapToContinueDialogue _tapToContinueDialogue;
		private TapToContinueLayoutDisplay _tapPromptLayoutDisplay;		// FIXME: replace implementation, no longer necessary to be ILayoutDisplay


		private void SetLayoutDisplay(string resourceConfig)
		{
			_layoutDisplay = MonoBehaviour.FindObjectOfType<LayoutDisplay>();
			JObject config = JObject.Parse(resourceConfig);		// FIXME: relies on Json.Net
			_layoutDisplay.SetConfiguration(config);

			// FIXME: specifications for TapToContinue have been revised, being an ILayoutDisplay is inappropriate
			_tapToContinueDialogue = _screenFactory.GetDialog<TapToContinueDialogue> ();
			_tapPromptLayoutDisplay = new TapToContinueLayoutDisplay (_layoutDisplay, _tapToContinueDialogue, DELAY_TIME);

			Func<bool> inputClause = (() => !_UISelected && !_playDisabled);	
			StoryPlayerInputLayoutDisplay inputLayoutDisplay = new StoryPlayerInputLayoutDisplay (_tapPromptLayoutDisplay, inputClause);
			
			// this is called at the end input 
			Action<int> onInputProcessed = (response => { _UISelected = false; });		// now called after EVERY input	
			inputLayoutDisplay.OnInputProcessed += onInputProcessed;
			
			// HACK: PARTIAL passive screen...allow selection input, but disable ribbon/info buttons (which exist ABOVE storyplayer's selection mask
			Action onSelectionDisplay = (() =>{ _inSelection = true; MakeUIPassive(true); });		
			inputLayoutDisplay.OnSelectionDisplay += onSelectionDisplay;
			
			// HACK: PARTIAL passive screen...reenable ribbon/info buttons after selection node
			Action onSelectionExit = (() => { _inSelection = false; MakeUIPassive(false); });	
			inputLayoutDisplay.OnSelectionExit += onSelectionExit;

			// caching initial affinities
			CacheCurrentAffinities ();
			inputLayoutDisplay.OnSelectionExit += () => 
			{
				ShowAffinityChangeEffect();
				CacheCurrentAffinities();			// update affinity cache
			};

			_storyPlayerLayoutDisplay = inputLayoutDisplay;
		}


		void InitializeView()
		{
			Canvas storyCanvas = GameObject.Find ("StoryCanvas").GetComponent<Canvas> ();		// HACK: need to configure story player scene's canvas, maybe should update scene or getcomponentinparent
			storyCanvas.renderMode = RenderMode.ScreenSpaceCamera;
			storyCanvas.worldCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
//			storyCanvas.renderer.sortingOrder = -1;	// - away from camera, + toward camera

			_currentHeaderPath = _storySettings.Scene.Path;
		}
		
		
		private ILayoutDisplay GetDisplayController()
		{
			return _storyPlayerLayoutDisplay;
		}	


		protected void EnableInactivityPrompt(bool value)
		{
			_tapPromptLayoutDisplay.EnableTimedPrompt(value);
		}

		protected TapToContinueDialogue TapToContinueDialogue
		{
			get { return _tapToContinueDialogue; }
		}




		private IDictionary<string,int> _cachedAffinityScores;

		private void ShowAffinityChangeEffect()
		{
			KeyValuePair<string,int> affinityChange = GetAffinityChangeToDisplay ();

			if(affinityChange.Value > 0)
			{
				AffinityChangeDisplayDialogue affinityChangeEffect = _screenFactory.GetDialog<AffinityChangeDisplayDialogue> ();
				affinityChangeEffect.ShowEffect(affinityChange);
			}
		}

		private void CacheCurrentAffinities()
		{
			_cachedAffinityScores = _player.GetAllAffinities ();		// player.GetCurrentAffectedCharacters ();
		}

		private KeyValuePair<string,int> GetAffinityChangeToDisplay()
		{
			foreach(KeyValuePair<string,int> priorScore in _cachedAffinityScores)
			{
				string npc = priorScore.Key;
				int priorAffinity = priorScore.Value;

				int currentAffinity = _player.GetAffinity(npc);		// Player::GetAffinity() can throw exception if keynotfound

				int diff = currentAffinity - priorAffinity;

				if(diff > 0)
				{
					return new KeyValuePair<string,int>(npc, diff);			// by design, there should only be a SINGLE change
				}
			}
			
			return new KeyValuePair<string, int> (string.Empty, 0);
		}







		protected virtual void SetupStoryPlayer(Player player, StoryPlayerSettings settings, StoryPlayerUIScreen screen, ILayoutDisplay layoutDisplay)
		{
			// FIXME: putting the query for scene settings from the server here, would prefer it to be where the StoryPlayerSettings is created (StoryLoader), but its not designed for an async flow
			// FIXME: network requests require a delegate to be called on success, playScene will be called after QueryServerForSceneSettings completes successfully
			Action<StoryPlayerSettings> playScene = (updatedSettings) => CreateAndLaunchScene(player, updatedSettings, screen, layoutDisplay);
			QueryServerForSceneSettings (player, settings, playScene);		
		}


		private void QueryServerForSceneSettings(Player player, StoryPlayerSettings settings, Action<StoryPlayerSettings> onComplete)
		{
			IDictionary<string,string> data = new Dictionary<string,string> ()
			{
				{"phone_id", player.UserID},
				{"scene_path", settings.ScenePath}
			};

			_networkController.Send (URLs.MAIL_ON_SCENE_COMPLETE, data, (response) => OnSceneSettingsResponseSuccess (response, settings, onComplete), OnSceneSettingsResponseFailure);
		}

		private void OnSceneSettingsResponseSuccess(WitchesRequestResponse response, StoryPlayerSettings settings, Action<StoryPlayerSettings> onComplete)
		{
			string json = response.WWW.text;
			JObject jsonResponse = JObject.Parse(json);
			
			bool sceneHasMail = jsonResponse.Value<bool>("scene_mail");
			settings.MailOnComplete = sceneHasMail;

			onComplete (settings);			// FIXME: calls CreateAndLaunchScene() as delegate
		}

		private void OnSceneSettingsResponseFailure(WitchesRequestResponse response)
		{
			Action goToStoryMap = () => 
			{
				StoryDisplayed(0);	// this is set (via StoryLoader) to Dispose() the loading marquee initialized by StoryMapUGUIScreenController::HandleSceneSelected 
				GoToStoryMap();
			};

			ShowSystemDialog ("Could not connect to server", goToStoryMap);
		}

		private void ShowSystemDialog(string message, Action onComplete=null)
		{
			var dialogue = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialogue.SetText (message);
			
			dialogue.Display ((response) => 
			{
				AmbientLogger.Current.Log (string.Format("System Dialogue \"{0}\" Closed", message), LogLevel.INFO);
				if(onComplete != null) onComplete();
				MonoBehaviour.Destroy(dialogue.gameObject);
			});
		}



		protected void CreateAndLaunchScene (Player player, StoryPlayerSettings settings, StoryPlayerUIScreen screen, ILayoutDisplay layoutDisplay)
		{
			_activeStoryPlayer = CreateStoryPlayer(player, settings, screen, layoutDisplay);
			PlayScene (_activeStoryPlayer, settings, _showInterface);
		}


		private INoStaminaController _noStaminaController;

		private WitchesStoryPlayer CreateStoryPlayer(Player player, StoryPlayerSettings settings, StoryPlayerUIScreen screen, ILayoutDisplay layoutDisplay)
		{
			_noStaminaController = _repo.Get<INoStaminaController> ();
			_noStaminaController.OnNoStaminaOpen += OnStaminaPromptShown;
			_noStaminaController.OnNoStaminaClose += OnStaminaPromptHide;
			
			INinjectModule playerModule = new WitchesPlayerDependencies (player, _storyData);	
			INinjectModule storyPlayerModule = new WitchesStoryPlayerDependencies (layoutDisplay, _musicPlayer, screen, _effectResolver, () => OnSceneComplete(settings), () => OnFailure("failed"), _noStaminaController, _shopDialogueController, _screenFactory, _storySettings, this);
			
			IKernel kernel = GetIOC (playerModule, storyPlayerModule);

			return kernel.Get<IStoryPlayer> () as WitchesStoryPlayer;	// FIXME: maybe need to add a StartScene to IStoryPlayer interface
		}

		private IKernel GetIOC(params INinjectModule[] modules)
		{
			NinjectSettings settings = new NinjectSettings();
			settings.LoadExtensions = false;
			settings.UseReflectionBasedInjection = true;
			
			return new StandardKernel(settings, modules);
		}

        private IEnumerator LoadResources(Voltage.Story.StoryDivisions.Scene scene)
        {
            var dependencies = _storyDependencyParser.GetAllCharacters(scene);
            return _layoutDisplay.PreloadDependencies(dependencies);
        }


		private void ShowUI(bool visible)
		{
			if(visible)
			{
				_storyPlayerUIScreen.Show();
			}
			else
			{
				_storyPlayerUIScreen.Hide();
			}
		}
		
		private void PlayScene(WitchesStoryPlayer storyPlayer, StoryPlayerSettings settings, bool showInterface=true)
		{
			if (storyPlayer != null && settings.Scene != null)
			{
				ShowUI(showInterface);
				
				AmbientLogger.Current.Log (string.Format("Playing Scene: {0}/{1}", settings.Scene.Path, settings.Node.ToString()), LogLevel.INFO);

				if (FirstTimeEnteringScene()) 
				{
					SendStoryMetric (MetricEvent.STORY_SCENE_READ, settings);
				}	

				storyPlayer.StartScene(settings.Scene, settings.Node);
				if (StoryDisplayed != null)
				{
					StoryDisplayed(0);
				}
			}
			else
			{
				AmbientLogger.Current.Log ("WitchesStoryPlayerController::PlayScene >>> ERROR!", LogLevel.WARNING);
				OnFailure(string.Empty);
			}
		}

		private bool FirstTimeEnteringScene()
		{
			return string.IsNullOrEmpty(_player.CurrentNodeID); 	// actual scene is irrelevant 
		}


		public Action<int> StoryDisplayed { get; protected set; }


		public void SetStoryDisplayedHandler(Action<int> responsehandler)
		{
			StoryDisplayed = responsehandler;
		}

		private IEnumerator InitScene (string resourceConfig, Action<ILayoutDisplay> onComplete)
		{
			// HACK - Hung Nguyen
			// This is to let the loading bar know we are starting another stage of loading.
			Voltage.Witches.DI.WitchesStartupSequence.loadingStage++;

			AsyncOperation asyncLoad = LoadLevel ();
			yield return asyncLoad;

			SetLayoutDisplay(resourceConfig);
            yield return UnitySingleton.Instance.StartCoroutine(LoadResources(_storySettings.Scene));
			InitializeView();
			
			ILayoutDisplay displayController = GetDisplayController();

			if (displayController != null && onComplete != null)
			{
				onComplete(displayController);
			}
			else
			{
				OnFailure(string.Empty);
			}
		}
		
		private AsyncOperation LoadLevel ()
		{
			AmbientLogger.Current.Log ("Loading StoryPlayerScreen...", LogLevel.INFO);
			return SceneManager.LoadSceneAsync(UNITY_SCENE_NAME, LoadSceneMode.Additive);
		}

		protected void RestartBGM()
		{
			var audio = AudioController.GetAudioController();
			if(audio != null)
			{
                audio.PlayBGMTrack(AudioController.DEFAULT_MUSIC);
			}
		}

		private void OnSceneComplete(StoryPlayerSettings settings)
		{
			AmbientLogger.Current.Log (string.Format("Completed Scene: {0}", settings.Scene.Path), LogLevel.INFO);
			SendStoryMetric (MetricEvent.STORY_SCENE_COMPLETED, settings);
			HideSpeechBox ();
			DisplaySceneCompleteDialog();
		}

		// HACK - yoshi 
		// it shouldn't use layoutDisplay(view) to controll the view. Ideally use Dialog Contoller to hide the speech box but couldn't find quick solution to talk to the dialogue contoller from this controller.
		private void HideSpeechBox()
		{
			_layoutDisplay.HideSpeechBox ();
		}


		protected event Action OnProcessCompleteScene;
        protected virtual void DisplaySceneCompleteDialog()
        {
			_storyPlayerUIScreen.EnableMask(true);
			MakePassive (true);

            // kick off network request, store the success/failure result
            _networkCallReturned = false;
            _player.CompleteScene(ProcessNetworkSceneSync);

            _waitingForDialog = true;

            _dialogController.Show(_player, _storySettings, GetCurrentSceneHeader(), delegate(int response) {
                if (_waitingForDialog)
                {
                    _dialogChoice = response;
                    UnitySingleton.Instance.StartCoroutine(HandleCompleteSceneDialog());
                }
            });
        }

        private IEnumerator HandleCompleteSceneDialog()
        {
            while (!_networkCallReturned)
            {
                yield return null;
            }

            if (!_networkCallSuccess)
            {
                var dialog = _screenFactory.GetDialog<iGUISmartPrefab_CantConnectErrorDialog>();
                dialog.Display(RetryNetworkAndContinue);
            }
            else
            {
                ProcessSceneCompleteChoice(_dialogChoice);

                if (OnProcessCompleteScene != null)
                {
                    OnProcessCompleteScene();
                }
            }

            _waitingForDialog = false;
        }

        private void RetryNetworkAndContinue(int response)
        {
            _networkCallReturned = false;
            _player.CompleteScene(ProcessNetworkSceneSync);
            UnitySingleton.Instance.StartCoroutine(HandleCompleteSceneDialog());
        }

        bool _networkCallReturned = true;
        bool _networkCallSuccess = true;
        int _dialogChoice = 0;
        bool _waitingForDialog = false;

        private void ProcessNetworkSceneSync(bool success)
        {
            _networkCallReturned = true;
            _networkCallSuccess = success;
        }


        protected virtual void ProcessSceneCompleteChoice(int choice)
        {
            switch ((DialogResponse)choice)
            {
                case DialogResponse.OK:
					GoToStoryMap();
                    break;
                case DialogResponse.Cancel:
                    GoBackHome();
                    break;
				case DialogResponse.Mail:
					GoToMailBox();
					break;
            }
        }
		
		private void OnRouteCompleted()		
		{
			AmbientLogger.Current.Log (string.Format("Completed Route: {0}", _storySettings.Scene.Route), LogLevel.INFO);
			SendStoryMetric (MetricEvent.STORY_ROUTE_COMPLETED, _storySettings);
			
			_storyResetter.Reset();
		}

		private Func<string, bool> CreateBookUnlockedHandler()
		{
			return delegate(string sceneId) 
			{
				return TryToUnlockSpellbook(sceneId);
			};
		}

		private bool TryToUnlockSpellbook(string sceneID)
		{
			Dictionary<string,string> bookUnlockMap = _bookConfig.BookUnlockMap;
			
			if(bookUnlockMap.ContainsKey(sceneID))
			{
				string bookID = bookUnlockMap[sceneID];

				if(!PlayerHasBook(bookID))
				{
					ISpellbook book = _bookFactory.Create(bookID);
					_player.AddBook(book);

					AmbientLogger.Current.Log (string.Format("Added Book: {0} [{1}]", book.Name, book.Id), LogLevel.INFO);
					return true;
				}
				else
				{
					AmbientLogger.Current.Log (string.Format("Player already owns book: {0}", bookID), LogLevel.INFO);
				}
			}

			return false;
		}


		private bool PlayerHasBook(string bookID)
		{
			return _player.GetBooks ().Find (spellbook => spellbook.Id == bookID) != null;
		}

	

		
		private void OnFailure (string msg)
		{
			if(_onFailure != null)
			{
				_onFailure();
			}
		}		
		
		public virtual void Show ()
		{
			if (_layoutDisplay != null)
			{
				_layoutDisplay.Root.gameObject.SetActive(true);
			}
		}
		
		public virtual void Hide () // We might need to support this function ?
		{
			Debug.Log ("Hide for Story Player is not implemented");
		}

        public virtual void Close()
        {
            _navManager.CloseCurrentScreen();
        }

		public virtual void Dispose ()
		{
			SendExitEvent ();			// should this be here? 

			UnSubscribeFromUIEvents ();

			if (_noStaminaController != null) 
			{
				_noStaminaController.OnNoStaminaOpen -= OnStaminaPromptShown;
				_noStaminaController.OnNoStaminaClose -= OnStaminaPromptHide;
				_noStaminaController.Dispose();
			}


//			SavePlayerState ();
			KillView();
			if(_storyPlayerUIScreen != null)
			{
				_storyPlayerUIScreen.Dispose();
			}

			if(_layoutDisplay != null)
			{
           		KillAssetManager();
			}

			if(_ribbonController != null)
			{
				_ribbonController.Dispose();
			}

			if(_tapToContinueDialogue != null)
			{
				_tapToContinueDialogue.Dispose();
			}

			SceneManager.UnloadScene(UNITY_SCENE_NAME);
		}		

		private void KillView()
		{
			if(_layoutDisplay != null)
			{
				MonoBehaviour.Destroy(_layoutDisplay.Root.gameObject);
			}

			if(_storyPlayerUIScreen != null)
			{
				MonoBehaviour.Destroy(_storyPlayerUIScreen.gameObject);
			}
		}

        private void KillAssetManager()
        {
            _layoutDisplay.AssetManager.Cleanup();
			_layoutDisplay = null;
        }

		public Dictionary<string,string> GetStoryInformation()
		{
			Dictionary<string,string> storyInfo = new Dictionary<string,string> ()
			{
				{"Arc",_storySettings.Scene.Arc},
				{"Scene",_storySettings.Scene.Name},
				{"Route",_storySettings.Scene.Route}
			};

			return storyInfo;
		}

		public void GoToGlossary()
		{
			RestartBGM();
			IScreenController nextScreen = new GlossaryScreenController(_navManager, _screenFactory, _player);
			_navManager.Add(nextScreen);
		}

		private bool SceneIsCompleted
		{
			get
			{
				// NOTE: RELIES ON Player.CompleteScene() to be called first!!!
				return _player.CurrentScene != _storySettings.Scene.Path;	// or string.IsNullOrEmpty(_player.CurrentScene) 
			}
		}



		public SceneHeader GetCurrentSceneHeader()
		{
			SceneHeader header = null;
			try
			{
				header = _headerFactory.Create(_currentHeaderPath);
			}
			catch(Exception)
			{
				throw new Exception("There was a problem finding the header at path :: " + _currentHeaderPath);
			}
			return header;
		}
	

        // TODO: The hard-coded paths are a little brittle here. The home screen SHOULD always exist, but there's a possibly that it does not.
        // It may be worthwhile to allow this to be a relative path, or to search the entire path for a story map
		public void GoToStoryMap()	// FIXME: should be renamed to something more appropriate to do what it's doing (e.g., public void HandleScreenResponse() or public void HandleSceneExit())
		{
			if(SceneIsCompleted)	// NOTE: relies on SceneComplete() to be called on Player first!!!
			{
				#region Ambient tutorial hack
				AmbientTutorialResumer ambientResumer = _repo.Get<AmbientTutorialResumer>();
				if (ambientResumer.ShouldStartAvatarTutorial()) 
				{	
					ambientResumer.HandleAmbientTutorial();
				} 
				else 
				#endregion
				{
					HandleCompletedScene();
				}
			}
			else
			{
				HandleDefaultScreenRouting();
			}
			RestartBGM();
		}

		private void HandleCompletedScene()
		{
			Voltage.Story.StoryDivisions.Scene scene = _storySettings.Scene;
			switch(scene.TerminationLevel)
			{
				case TermLevel.Route:
					OnRouteCompleted();
					
					StoryCompleteScreenController routeCompleteController = _repo.Get<StoryCompleteScreenController>();
					_navManager.Add(routeCompleteController);
					break;
					
				case TermLevel.Arc:	
					HandleArcUnlockedRouting();	
					break;

				case TermLevel.None:	
				default:
					HandleDefaultScreenRouting();
					break;
			}
		}

		private void HandleArcUnlockedRouting()
		{
			var loading = DisplayLoadingMarquee();
			Action<int> storyMapLoaded = (loading as MainLoadingScreenController).ProcessAction;
			IStoryMapScreenControllerFactory storyMapScreenFactory = _repo.Get<IStoryMapScreenControllerFactory>();
            StoryMapUGUIScreenController nextScreen = storyMapScreenFactory.CreateUGUI(_player, storyMapLoaded);
			var nextArc = GetNextArcFromPlayer();
            nextScreen.TriggerArcAlert(nextArc);

			string sceneID = _storySettings.Scene.Path;

            var foundNewBook = TryToUnlockSpellbook(sceneID);
            if (foundNewBook)
            {
                nextScreen.TriggerBookAlert();
            }
			_navManager.OpenScreenAtPath(nextScreen, "/Home");

			SendStoryUnlockArcMetric(nextArc);
		}

		string GetNextArcFromPlayer()
		{
			string returnValue = string.Empty;
			var available = _player.AvailableScenes;
			for(int i = 0; i < available.Count; ++i)
			{
				var scene = available[i];

				var parts = scene.Split(('/'));
				var index = parts[1].IndexOf(' ');
				if(string.IsNullOrEmpty(returnValue))
				{
					returnValue = parts[1].Substring(index).Trim();
					break;
				}
			}

			return returnValue;
		}

		public ScreenController DisplayLoadingMarquee()
		{
			MainLoadingScreenControllerFactory factory = _repo.Get<MainLoadingScreenControllerFactory>();
			MainLoadingScreenController loading = factory.Create((int)MainLayoutType.LOADING_MARQUEE, null, null);
			loading.Show();
			return loading;
		}

		private void HandleDefaultScreenRouting()
		{
			var loading = DisplayLoadingMarquee();
			Action<int> storyMapLoaded = (loading as MainLoadingScreenController).ProcessAction;
			IStoryMapScreenControllerFactory storyMapScreenFactory = _repo.Get<IStoryMapScreenControllerFactory>();
            StoryMapUGUIScreenController nextScreen = storyMapScreenFactory.CreateUGUI(_player, storyMapLoaded);
			_navManager.OpenScreenAtPath(nextScreen, "/Home");
		}
            


		public bool CameFromStoryMap()
		{
			string[] path = _navManager.GetCurrentPath().Split(('/'));
			bool contains = false;
			for(int i = 0; i < path.Length; ++i)
			{
				if((path[i]).Contains("StoryMapNew"))
				{
					contains = true;
				}
			}

			return contains;
		}

		public void GoBackHome()
		{
			RestartBGM();
			_navManager.GoToExistingScreen("/Home");
		}


		public void GoToMailBox()
		{
			RestartBGM();
			IScreenController nextScreen = _repo.Get<MailboxScreenController>();
			_navManager.OpenScreenAtPath(nextScreen,"/Home");
		}














#region Analytics

		private void SendStoryUnlockArcMetric (string arcName)
		{
			IDictionary<string,object> data = new Dictionary<string,object>
			{
				{"arc_id", arcName}
			};

			AmbientMetricManager.Current.LogEvent (MetricEvent.STORY_UNLOCKED_ARC, data);
		}


		private void SendStoryStaminaMetrics(string metricEvent)	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{ 
				{"route_id", _storySettings.Scene.Route},
				{"scene_id", _storySettings.Scene.Name}
			};
			
			AmbientMetricManager.Current.LogEvent (metricEvent, data);
		}

		private void SendStoryMetric (string metricEvent, StoryPlayerSettings settings)	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> { 
				{ "player_scenes", settings.Scene.Path },
				{ "route_id", settings.Scene.Route },
				{ "scene_id", settings.Scene.Name },
				{ "node_id", settings.Node.ToString () }
			};

			AmbientMetricManager.Current.LogEvent (metricEvent, data);
		}

#endregion

	}
}





