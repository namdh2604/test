
using UnityEngine;	// Temporary???
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ninject.Modules;

namespace Voltage.Witches.DI
{
	using Voltage.Common.Logging;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Witches.Screens;

	using Voltage.Common.Startup;

	using Voltage.Witches.Controllers;
    	using Voltage.Witches.Story;
    	using Voltage.Witches.Services;

	using Voltage.Witches.Models;

	using Voltage.Story.Configurations;

	using Voltage.Common.Metrics;
	using Voltage.Witches.Metrics;

	using Voltage.Story.StoryPlayer;
	using Voltage.Witches.Configuration;
    using Voltage.Witches.Scheduling;
    using Voltage.Witches.Scheduling.Tasks;

	using Voltage.Common.IAP;
	using Voltage.Witches.Login;
	using Voltage.Common.DebugTool.Timer;
    using Voltage.Witches.Init;

    using Voltage.Witches.Bundles;
    using Voltage.Witches.Notifications;
    using Voltage.Witches.Exceptions;

	public class WitchesStartupSequence
	{
		public static int loadingStage = 0;
		private Action<IWitchesData> _onInitializationComplete;

		private readonly EnvironmentController _environmentController;
		private readonly StartupDataController _dataController;
		private readonly StartupShopController _shopController;
		private readonly StartupSceneSetupController _sceneSetupController;

		private readonly StartupDisplayController _displayController;	// private readonly StartupDisplayController _displayController;
        private static bool _isNewUser = false;

		private readonly AudioController _audioController;
		private readonly IPlayerPreferences _playerPrefs;
//		private readonly IMetricManagerFactory _metricManagerFactory;

		private readonly IPlayerFactory _playerFactory;

        private readonly ITaskScheduler _scheduler;
        private readonly NetworkConnectivityMonitorFactory _connectionMonitorFactory;

		private readonly ILoginController _loginController;

        private readonly IAvatarResourceManager _avatarResourceManager;

		private MasterConfiguration _masterConfig;
        private readonly NotificationManager _notificationManager;
		private readonly SplashMovieController _movieController;
        private readonly GameErrorHandler _gameErrorHandler;

		public WitchesStartupSequence ( ILogger logger, EnvironmentController environmentController, StartupDataController dataController, 
		                               	StartupShopController shopController, StartupSceneSetupController sceneSetupController,
		                               	StartupDisplayController displayController, IPlayerFactory playerFactory, 
                                        AudioController audioController, IPlayerPreferences playerPrefs, 
                                        ITaskScheduler scheduler, NetworkConnectivityMonitorFactory connectionMonitorFactory,
		                               	ILoginController loginController, 
                                        IAssetBundleManager assetBundleManager, IAvatarResourceManager avatarResourceManager, 
                                        NotificationManager notificationManager, SplashMovieController movieController, 
                                        GameErrorHandler gameErrorHandler)
		{
            GameErrorHandler.DisplayUnhandledErrors = true;
            GameErrorHandler.RegisterHandler(gameErrorHandler);

			if(environmentController == null || dataController == null || shopController == null || displayController == null || logger ==  null || playerFactory == null || audioController == null || playerPrefs == null || sceneSetupController == null || loginController == null || scheduler == null || connectionMonitorFactory == null)
			{
				throw new ArgumentNullException("WitchesStartupSequence::Ctor");
			}

			AmbientLogger.Current = logger;
//			AmbientDebugTimer.Current = new DebugTimerLogger (new DebugTimer (), logger); // DebugTimerStack

			_environmentController = environmentController;
			_dataController = dataController;
			_shopController = shopController;
			_sceneSetupController = sceneSetupController;

			_displayController = displayController;
			_audioController = audioController;
			_playerPrefs = playerPrefs;

			_playerFactory = playerFactory;
            _scheduler = scheduler;
            _connectionMonitorFactory = connectionMonitorFactory;

			_loginController = loginController;

            _avatarResourceManager = avatarResourceManager;
            _notificationManager = notificationManager;
			_movieController = movieController;

            _gameErrorHandler = gameErrorHandler;
		}

		public void Start(Action<IWitchesData> callback)
		{
			_onInitializationComplete = callback;
			// Show the movie, then continue on
			_movieController.Show(MainLoadingSequence);
		}

		private void MainLoadingSequence()
		{
			AmbientDebugTimer.Current.Start("Start");
			GetEnvironment(Init);
		}

		private void GetEnvironment(Action<Exception> callback)
		{
			AmbientDebugTimer.Current.Start("Get Env");
			
    		_displayController.ShowMarquee(); // This need to happen to support force update dialog.  - Design prefer not to have this marquee.
			
			_environmentController.Execute(callback);
		}


		private void Init(Exception e)
		{
            if (e != null)
            {
                HandleError(e);
                return;
            }

			SetupScene ();
			
			SetPreferences ();		
			PlayBGMusic ();
			
//			GetEnvironment ();

            // This is where the Marquee was being hide/deleted again.  I comment this out until I am sure this is not needed. - Hung Nguyen
            // Uncommented to fix force update bug.
			_displayController.HideMarquee ();
			ShowTopMenu ();
		}




		private void SetupScene()
		{
			_sceneSetupController.Execute ();
		}

		private void SetPreferences()
		{
			if(!_playerPrefs.HasValues())
			{
				_playerPrefs.SetDefaults();
			}
		}
		
		private void PlayBGMusic()
		{
			if (_playerPrefs.SoundEnabled)
			{
                _audioController.PlayBGMTrack(AudioController.DEFAULT_MUSIC);
			}
		}

		private void ShowTopMenu()
		{
			AmbientDebugTimer.Current.Start ("Show Top Menu [IGNORE]");

			_displayController.ShowTopScreen (OnAppStart, OnRestore);
		}


		private void OnAppStart()
		{
			InitializeData ();
		}

		private void OnRestore()	// FIXME: cleanup after MainLoadingScreenController is splitup
		{
			AmbientDebugTimer.Current.Start ("On Restore");

			_displayController.ShowTopScreen (OnAppStart, OnRestore);
			MainLoadingScreenController controller = _displayController.GetCurrentScreenController () as MainLoadingScreenController;
			if(controller != null)
			{
				controller.RestoreUser(InitializeData);
			}
			else
			{
				ShowTopMenu();
			}
		}


		private void InitializeData()
		{
			AmbientDebugTimer.Current.Start ("Initializing Data");

            if (!_dataController.HasExistingPlayerData())
            {
                _isNewUser = true;
            }

			_displayController.ShowLoadingScreen ();
			// We need to reset this hard code loading state so it can follow the loading progression.
			loadingStage = 0;

			Voltage.Common.Unity.UnitySingleton.Instance.StartCoroutine (InitializeDataRoutine ());		// HACK: to give LoadingScreen time to display
		}

		private IEnumerator InitializeDataRoutine()
		{
//			yield return new WaitForEndOfFrame ();
            while (!_displayController.IsLoadingScreenLoaded())
            {
                yield return null;			// gives LoadingScreen time to display, is this duration enough across all devices?
            }

			_dataController.Execute(InitializeShop);	// _dataController.Execute (CompleteGameSetup);
		}


		private void InitializeShop(Exception e)
		{
            if (e != null)
            {
                HandleError(e);
                return;
            }

			AmbientDebugTimer.Current.Start ("Initializing Shop");
			// HACK - Hung Nguyen
			// This is a hack to add some knowledge to the load bar.   I identify this as a first stage in the progress via debug trace.
			// There is already some loading being done before this, and this is a good point set as a marker.
			// Debug.Log ("LOADING BAR STAGE 1");
			loadingStage++;

			_shopController.Execute(CompleteGameSetup);
		}


		private void CompleteGameSetup(Exception e)
		{
            if (e != null)
            {
                HandleError(e);
                return;
            }

			AmbientDebugTimer.Current.Start ("Make Witches Data");
//			UnityEngine.Debug.Log (string.Format ("\tWitchesStartupSequence::CompleteGameSetup >>> this ID: {0}", Voltage.Common.ID.UniqueObjectID.Default.GetID (this).ToString ()));
//			UnityEngine.Debug.Log ("\tSTACKTRACE\n" + new System.Diagnostics.StackTrace ().GetFrame (0).ToString ());

			_masterConfig = _dataController.Data.MasterConfig;
			_loginController.Execute(_dataController.Data, CompleteGameSetUpAfterLogIn);
		}

		private void CompleteGameSetUpAfterLogIn(Exception e, bool tutorialFlag)
		{
            if (e != null)
            {
                HandleError(e);
                return;
            }

			if (! _dataController.Data.PlayerData.didResetForNewTutorial){ // HACK reset some old scene data to avoid conflict with new prologue scenes(ver1.1) to do this only one time
				NewTutorialOnetimeReset reset = new NewTutorialOnetimeReset();
				reset.Execute(_dataController.Data.PlayerData, tutorialFlag);
			}

			Player player = _playerFactory.Create (_dataController.Data.PlayerData, _masterConfig);
            _notificationManager.Init(player);

			MasterStoryData storyData = _dataController.Data.StoryMain;
			ITransactionProcessor transactionProcessor = _shopController.Data;

			WitchesData data = new WitchesData (player, _masterConfig, storyData, transactionProcessor);

			_connectionMonitorFactory.Create(player as WitchesNetworkedPlayer);
			
			AmbientDebugTimer.Current.Start ("Setup Stamina Tasks");
			
			StaminaUpdateTask staminaTask = new StaminaUpdateTask(player, 30);
			FocusUpdateTask focusTask = new FocusUpdateTask(player, 30);
			
			_scheduler.ScheduleRecurring(staminaTask);
			_scheduler.ScheduleRecurring(focusTask);
            player.Refresh(); // force an initial sync
			
			AmbientDebugTimer.Current.Start ("Setup Metrics");
			
			SetupMetrics (player, _environmentController.Data);

            if (_isNewUser)
            {
                AmbientMetricManager.Current.LogEvent("InitialLoad_Start");
            }
			
			AmbientDebugTimer.Current.Stop ();

			_avatarResourceManager.PreloadBundles();
			
			AmbientDebugTimer.Current.Start ("Initialize Complete");

            GameErrorHandler.DisplayUnhandledErrors = false;
			_onInitializationComplete (data);
//			Cleanup ();

			// HACK - Hung Nguyen
			// This is a hack to add some knowledge to the load bar.   I identify this as a second stage in the progress via debug trace.
			// This is the final call in the startup sequence, the other stages are being track in the assetbundle loading.
			// Debug.Log ("LOADING BAR STAGE 2");
			loadingStage++;

			AmbientDebugTimer.Current.Stop ();
		}
			

		// HACK: until screenavmanager.replace is in place...need to be able to dispose of loading screen.  
		// the static nature of this call is to minimize code impact and avoid dependency injection that would need to be rolled back later
		// still need to determine how to unload the startupsequence that's likely residing in memory when the game runs
		public void Dispose()	
		{
            if (_isNewUser)
            {
                // Dispose currently get called multiple times in the tutorial sequence, so we need to turn off new user to avoid generating multiple end events
                _isNewUser = false;
                AmbientMetricManager.Current.LogEvent("InitialLoad_End");
            }

			if(_displayController != null)
			{
				Cleanup();
			}
			else
			{
				UnityEngine.Debug.LogWarning ("Can't Dispose");
			}
		}
			

        private void Cleanup()
		{
			// dispose of initialization resources

			_displayController.CleanupView ();		
		}

		private void SetupMetrics(Player player, EnvironmentData data)
		{
			AmbientMetricManager.Current = new WitchesMetricManagerFactory (player, AmbientLogger.Current).Create (data.Metrics);	// TODO: inject
		}

        private void HandleError(Exception e)
        {
            _gameErrorHandler.HandleError(e);
        }

	}



}


