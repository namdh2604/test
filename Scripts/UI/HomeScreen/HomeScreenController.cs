using System;
using System.Collections.Generic;
using System.Collections;


namespace Voltage.Witches.Controllers
{
//	using Voltage.Witches.Controllers;

	using Voltage.Witches.Screens;
	using Voltage.Witches.Models;

	using URLs = Voltage.Witches.Net.URLs;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Story.StoryPlayer;
	using Voltage.Story.Models.Nodes;
	using Voltage.Story.StoryDivisions;
	using Voltage.Witches.Story;
	using Voltage.Witches.Controllers.Factories;
	using Voltage.Witches.Shop;
	using Voltage.Witches.UI;

	using Voltage.Witches.Configuration;
	using Voltage.Witches.Events;

	using Newtonsoft.Json.Linq;

	using Voltage.Witches.DI; 	// for AmbientTutorialResumer
	using UnityEngine;			// evidently PlayerPrefs is being used directly here

    using Voltage.Witches.Login;


	public class HomeScreenController : ScreenController
	{
		private IScreenFactory _screenFactory;
		protected Player _player;
		private IControllerRepo _repo;

		protected HomeScreenView _screen;

		private MasterConfiguration _masterConfig;
		private readonly ISceneHeaderFactory _headerFactory;

		private readonly UIRibbonController _ribbonController;

		private readonly ShopDialogueController _shopDialogueController;	// should be in some UI/interfaceview model
		public INetworkTimeoutController<WitchesRequestResponse> NetworkController { get; protected set; }

		public bool HasNewMail { get { return _hasNewMail; } }
		private bool _hasNewMail = false;

		private readonly HomeScreenFeatureLockHandler _featureLockHandler;

		private bool _isRibbonOpen;


		private readonly bool _enableLoginBonus;
        private readonly BonusManager _loginBonusManager;


		public HomeScreenController(ScreenNavigationManager controller, IScreenFactory screenFactory, Player player, 
            IControllerRepo repo, MasterConfiguration masterConfig, ShopDialogueController shopDialogueController, 
			HomeScreenFeatureLockHandler featureLockHandler, BonusManager bonusManager, bool enableLoginBonus)
			: base(controller)
		{
			_screenFactory = screenFactory;
			_player = player;

			_repo = repo;
			_masterConfig = masterConfig;

			_shopDialogueController = shopDialogueController;

			_featureLockHandler = featureLockHandler;                  
            _loginBonusManager = bonusManager;
			_enableLoginBonus = enableLoginBonus;


			NetworkController = _repo.Get<INetworkTimeoutController<WitchesRequestResponse>>();
			_headerFactory = _repo.Get<ISceneHeaderFactory>();
			_ribbonController = new UIRibbonController (_player, _shopDialogueController, _screenFactory, _masterConfig);       // would prefer to pass this dependency in or not at all.
			SubscribeRibbonEvents();

			SubscribeEvents ();
		}

		private void SubscribeEvents()
		{
			_featureLockHandler.HandleAvatarClosetLock += UnlockAvatarCloset;
			_featureLockHandler.HandleClothingStoreLock += UnlockClothingStore;
			_featureLockHandler.HandleMinigameLock += UnlockMiniGame;
            _featureLockHandler.HandleStarterPackLock += UnlockStarterPack;

			_player.OnPurchaseStarterPack += OnStarterPackPurchased;
		}

		private void OnStarterPackPurchased()
		{
			// hides starter pack button on purchase
			UnlockStarterPack (false);
		}


		private void UnsubscribeEvents()
		{
			_featureLockHandler.HandleAvatarClosetLock -= UnlockAvatarCloset;
			_featureLockHandler.HandleClothingStoreLock -= UnlockClothingStore;
			_featureLockHandler.HandleMinigameLock -= UnlockMiniGame;
            _featureLockHandler.HandleStarterPackLock -= UnlockStarterPack;

			_player.OnPurchaseStarterPack -= OnStarterPackPurchased;
		}

		private void SubscribeRibbonEvents()
		{
			_ribbonController.OnShopOpen += OnShopOpen;
			_ribbonController.OnShopClosed += OnShopClose;

			_ribbonController.OnOpenEvent += OnRibbonOpen;
			_ribbonController.OnCloseEvent += OnRibbonClose;
		}

		private void UnsubscribeRibbonEvents()
		{
			_ribbonController.OnShopOpen -= OnShopOpen;
			_ribbonController.OnShopClosed -= OnShopClose;

			_ribbonController.OnOpenEvent -= OnRibbonOpen;
			_ribbonController.OnCloseEvent -= OnRibbonClose;
		}
			




		public override void MakePassive (bool value)
		{
            if (_screen != null) 
			{
				_screen.MakePassive (value);

                _screen.MakeLeftSidePassive(value || _isRibbonOpen);
			}

			if (_ribbonController != null) 
			{
				_ribbonController.MakePassive (value);
			}
		}

		private void OnShopOpen()
		{
			MakePassive(true);
		}

		private void OnShopClose()
        {
			MakePassive(false);
		}

		private void OnRibbonOpen()
		{
            _screen.MakeLeftSidePassive(true);
			_isRibbonOpen = true;
		}

		private void OnRibbonClose()
        {
            _screen.MakeLeftSidePassive(false);
			_isRibbonOpen = false;
		}



		private void UpdateFeatureLocks()
		{
			_featureLockHandler.HandleLocks ();
		}

		public override void Dispose()
		{
			if (_ribbonController != null) 
			{
				UnsubscribeRibbonEvents();			
				_ribbonController.Dispose();
			}

			// TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
			// We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
			if (_screen != null)
			{
				UnsubscribeEvents();
				UnsubscribeViewEvents(_screen);

				_screen.Dispose();
				_screen = null;
			}
		}

		public override void Hide()
		{
			_ribbonController.Hide();
			base.Hide();
		}



		private void UnlockAvatarCloset(bool value)
		{
			_screen.UnlockAvatarCloset(value);
		}

		private void UnlockClothingStore(bool value)
		{
			_screen.UnlockClothingStore(value);
		}

		private void UnlockMiniGame(bool value)
		{
			_screen.UnlockMiniGame(value);
		}

        private void UnlockStarterPack(bool value)
        {
            _screen.UnlockStarterPack(value);
        }


		public Player GetPlayer { get { return _player; } }


		public override void Show()
		{
            base.Show();               		// indirectly calls InitScreen()
			_ribbonController.Show();  		// RibbonController::Show() needs to be called after Screen::Show(), so that IF a ribbon view is created on RibbonController::Show() it will appears over the screen (in heirarchy)                 	

			// updates screen to initial state, accounts for any changes due to transition (leaving and returning)
			// not great as we may not want to "enable" screen before initialization has completed
			// also MakePassive HERE handles both screen AND ribbon...if call sequence is changed ribbon has to be handled separately to account for its own RibbonController::Show()
			MakePassive(false);

			UpdateFeatureLocks ();

			if (_isRibbonOpen) 
			{
				// syncs screen w/ ribbon state, this is to ensure that the minigame button remains passive when returning to the screen with the ribbon already open
				// needs to be called after UpdateFeatureLocks() modifies button states
                _screen.MakeLeftSidePassive(true);
			}

            PostShowAction ();
		}

        // FIXME: organize what needs to initialize a screen, the action to Show a screen, what needs to be performed/shown after a screen is visibl PostShow()e
		// also rename
        // HACK: needs to be virtual because TutorialHomeScreenController does not need to call HandleLoginBonus (or any of the other checks)
        protected virtual void PostShowAction()
        {
			if (_enableLoginBonus) 
			{
				DisplayLoginBonus();
			}

            // needs to be called on Show() as both the act and response requires the view to be active [implementation detail]
            _screen.ShowAvatarTapText (5f);
            CheckMailBadge ();  
        }



        
        private void DisplayLoginBonus()
        {
            if (_loginBonusManager.HasBonusItem())
            {
                bool receivedLastItem = _player.BonusesReceivedCount > 0;     // bonus items are received sequentially the only situation where a past item in the list would not be received is the first case

                LoginBonusDialog dialog = _screenFactory.GetDialog<LoginBonusDialog>();
                dialog.Init(_loginBonusManager.GetViewModelList(), receivedLastItem);      // possibly move GetViewModelList() out of BonusManager

                MakePassive(true);
                dialog.Display(HandleLoginBonusProcessed);

				// FIXME: ORDER needs to be preserved. GiveBonusItem() must be called AFTER! dialog, as it will clear player's bonus list before GetViewModelList() has 
				// a chance to read from it...could potentially get a copy of player's data first (but that order of operation would need to be preserved as well)
                _loginBonusManager.GiveBonusItem();
            }
            else
            {
                Voltage.Common.Logging.AmbientLogger.Current.Log("No Login Bonus", Voltage.Common.Logging.LogLevel.INFO);
                PromptForNotifications();
            }
        }

        private void HandleLoginBonusProcessed(int choice)
        {
            MakePassive(false);
            PromptForNotifications();
        }

        private void PromptForNotifications()
        {
            if (_player.ShouldPromptUserForNotifications)
            {
                // if player has not been prompted for notifications already, turn notifications on
                _player.NotificationsEnabled = true;
                _player.ShouldPromptUserForNotifications = false;
            }
        }

		protected override IScreen GetScreen()
		{
			InitScreen();
		
			return _screen;
		}

		private void InitScreen()
		{
			if(_screen == null) 
			{
				_screen = _screenFactory.GetScreen<HomeScreenView> ();
				SubscribeViewEvents(_screen);
			}

			bool inScene = IsCurrentlyInAScene ();
			string polaroidPath = GetPolaroidPath(inScene);

            _screen.Refresh(_player, inScene, polaroidPath);	// move into Show()?
		}
		
		private const string DEFAULT_POLAROID = "Polaroids/polaroid_bg_julies_loft";
		public string GetPolaroidPath(bool inScene)
		{	
			if(inScene) 
			{				
				var header = _headerFactory.Create(_player.CurrentScene);
				return header.PolaroidPath;
			}
			else 
			{
				return DEFAULT_POLAROID;
			}
		}

		public bool IsCurrentlyInAScene()
		{
			return (!string.IsNullOrEmpty(_player.CurrentScene));
		}

		private void SubscribeViewEvents(HomeScreenView view)
		{
			view.OnAvatarButtonSelected += HandleClosetButtonPress;
			view.OnMailButtonSelected += GoToMailBox;
			view.OnOptionButtonSelected += HandleMenuButtonPress;
			view.OnMiniGameButtonSelected += HandleMiniGameButtonPress;
			view.OnShopButtonSelected += HandleStoreButtonPress;
			view.OnStoryButtonSelected += HandlePlayStoryButton;
            view.OnStarterPackSelected += HandleStarterPackButtonPress;
		}

		private void UnsubscribeViewEvents(HomeScreenView view)
		{
			view.OnAvatarButtonSelected -= HandleClosetButtonPress;
			view.OnMailButtonSelected -= GoToMailBox;
			view.OnOptionButtonSelected -= HandleMenuButtonPress;
			view.OnMiniGameButtonSelected -= HandleMiniGameButtonPress;
			view.OnShopButtonSelected -= HandleStoreButtonPress;
			view.OnStoryButtonSelected -= HandlePlayStoryButton;
            view.OnStarterPackSelected -= HandleStarterPackButtonPress;
		}








		// DEPRECATED!
		public void SetEnabled(bool value)
		{
			_screen.MakePassive(!value);
		}



		public IDialog GetChooseOutfitDialog()
		{
			var dialog = _screenFactory.GetDialog<iGUISmartPrefab_ChooseOutfitDialog>();
			return dialog;
		}

		public IDialog GetSystemDialog(string message)
		{
			var dialog = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialog.SetText (message);
			return dialog;
		}





		public void CheckMailBadge()
		{
			string userID = _player.UserID;
			if(string.IsNullOrEmpty(userID))
			{
				userID = PlayerPrefs.GetString("phone_id");
			}

			Dictionary<string,string> parameters = new Dictionary<string,string> ()
			{
				{"phone_id",userID}
			};

			NetworkController.Send (URLs.CHECK_MAIL, parameters, HandleMailCheckSuccess, HandleMailCheckFail);
		}

		void HandleMailCheckSuccess (Voltage.Common.Net.WWWNetworkPayload payload)
		{	
			var response = payload.WWW.text;
			if(string.IsNullOrEmpty(response))
			{
				throw new NullReferenceException("Response from server was invalid");
			}
			JObject badge = JObject.Parse(response);

			var number = (int)Convert.ToSingle(badge["mail_badge"].ToString());

			if(number > 0)
			{
				_hasNewMail = true;
				_screen.DisplayMailButton();
			}
			else
			{
				_hasNewMail = false;
			}
		}

		void HandleMailCheckFail (Voltage.Common.Net.WWWNetworkPayload payload)
		{
			UnityEngine.Debug.LogWarning("Mail Badge NOT checked");
			_hasNewMail = false;
		}
			

		private void HandleClosetButtonPress()
		{
			NewClosetScreenControllerFactory closetScreenFactory = _repo.Get<NewClosetScreenControllerFactory>();
			NewClosetScreenController nextScreen = closetScreenFactory.Create(_player);
			Manager.Add(nextScreen);
		}

		private void HandleMenuButtonPress()
		{	
			UnityEngine.Debug.Log ("handling menu button press");
			IScreenController nextScreen = new MenuScreenController(Manager,_screenFactory,_player, _repo, _masterConfig);
			Manager.Add(nextScreen);
		}

		private void GoToMailBox()
		{
			_screen.HideMailButton();

			IScreenController nextScreen = _repo.Get<MailboxScreenController>();
			Manager.Add(nextScreen);
		}

		private void HandleStoreButtonPress()
		{
			AvatarShopScreenControllerFactory shopScreenFactory = _repo.Get<AvatarShopScreenControllerFactory>();
			IScreenController nextScreen = shopScreenFactory.Create();
			Manager.Add(nextScreen);
		}


		private void HandleStarterPackButtonPress()
		{
			MakePassive(true);

            Action<bool> onComplete = (success) =>
            {
                // refresh features...hides starter pack button if successfully purchased...or call UnlockStarterPack(false) directly
				// UnlockStarterPack(false) is already subscribed to Player::OnPurchaseStarterPack
//                UpdateFeatureLocks();   

                MakePassive(false);
            };

            _shopDialogueController.ShowStarterPackDialogue(onComplete);
		}     



		private void HandleMiniGameButtonPress()
		{
			IScreenController nextScreen = new BookshelfScreenController(Manager, _screenFactory, _player, _repo, _masterConfig, _shopDialogueController);
			Manager.Add(nextScreen);
		}


		private void HandlePlayStoryButton()
		{
			if(IsCurrentlyInAScene())
			{
				GoToStory();
			}
			else
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
					GoToStoryMap ();
				}
			}
		}

		void GoToStory()
		{
			IStoryLoaderFactory storyLoaderFactory = _repo.Get<IStoryLoaderFactory>();
			IStoryLoaderFacade storyLoader = storyLoaderFactory.CreateFacade(_player);

            LoadStatus status = storyLoader.GetLoadStatus(_player.CurrentScene);
            if (status.IsReady())
            {
                // safe to resume
                _screen.MakePassive(true); // Need to make the screen passive so the user can't spam other buttons.  This is only done when going into the story player.
                var loading = DisplayLoadingMarquee();
                Action<int> loadingCallback = (loading as MainLoadingScreenController).ProcessAction;

                // ignore errors for now
                Action<Exception> missingErrorHandler = null;
                storyLoader.Load(status.settings, loadingCallback, missingErrorHandler);
            }
            else
            {
                GoToStoryMap(status);
            }
		}

		StoryMapUGUIScreenController GoToStoryMap(LoadStatus status=null)
		{
			IStoryMapScreenControllerFactory storyMapScreenFactory = _repo.Get<IStoryMapScreenControllerFactory>();

			var loading = DisplayLoadingMarquee();
			Action<int> callback = (loading as MainLoadingScreenController).ProcessAction;

			StoryMapUGUIScreenController nextScreen = storyMapScreenFactory.CreateUGUI(_player, callback);
			Manager.Add(nextScreen);

            if (status != null)
            {
                nextScreen.ShowInformationForScene(status);
            }

			return nextScreen;
		}

		public ScreenController DisplayLoadingMarquee()
		{
			MainLoadingScreenControllerFactory factory = _repo.Get<MainLoadingScreenControllerFactory>();
			MainLoadingScreenController loading = factory.Create((int)MainLayoutType.LOADING_MARQUEE, null, null);
			loading.Show();
			return loading;
		}






	}
}




