
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Witches.Screens;
	using Voltage.Witches.Services;
	using Voltage.Witches.Controllers;

	using Voltage.Witches.Exceptions;


	// NOTE: be sure you don't inject ErrorController into any DisplayController...otherwise circular!

    public class StartupDisplayController
    {
		private readonly IMoviePlayer _introMovPlayer;
		private readonly IScreenFactory _screenFactory;
		private readonly IBuildNumberService _versionService;
		private readonly ScreenNavigationManager _screenNavManager;

		private readonly RestorePlayerController _restorePlayerController;	// Unfortunately coupled to display controller

		public StartupDisplayController(ScreenNavigationManager screenNavManager, IScreenFactory screenFactory, IBuildNumberService versionService, IMoviePlayer introMoviePlayer, RestorePlayerController restorePlayerController)
		{
			if(screenNavManager == null || screenFactory == null || versionService == null || introMoviePlayer == null || restorePlayerController == null)
			{
				throw new ArgumentNullException();
			}

			_introMovPlayer = introMoviePlayer;
			_screenNavManager = screenNavManager;
			_screenFactory = screenFactory;
			_versionService = versionService;

			_restorePlayerController = restorePlayerController;
		}

		public void ShowTopScreen(Action appStart, Action restore)
		{
			IScreenController mainLoading = new MainLoadingScreenController(_screenFactory, _versionService, (int)MainLayoutType.TOP_SCREEN, appStart, restore, _restorePlayerController);
			_screenNavManager.Add(mainLoading);
		}


		public void ShowLoadingScreen()
		{	
			IScreenController loadingScreen = new MainLoadingScreenController(_screenFactory, _versionService, (int)MainLayoutType.LOADING,null,null, _restorePlayerController);
			_screenNavManager.Add (loadingScreen);
		}

        public void HideLoadingScreen()
        {
			if(_screenNavManager.CurrentController is MainLoadingScreenController)
            {
				_screenNavManager.CloseCurrentScreen();
            }
        }
        
        public void StopLoadingScreen()
		{
			MainLoadingScreenController controller = _screenNavManager.CurrentController as MainLoadingScreenController;
//			if(controller != null)
			{
				controller.StopLoading();
			}
		}

		public void ShowMarquee()
		{
            MainLoadingScreenController loadingScreen = new MainLoadingScreenController(_screenFactory, _versionService, (int)MainLayoutType.LOADING_MARQUEE,null,null, _restorePlayerController);
			loadingScreen.EnableBG ();

			_screenNavManager.Add (loadingScreen);
		}

        // NOt calling this so I remove the code for now. - Hung Nguyen
		public void HideMarquee()
		{
            // TODO The Margarquee is fading out and will destory itself when done. I comment this code out.  Next Step is to remove the calling code.

			MainLoadingScreenController controller = _screenNavManager.CurrentController as MainLoadingScreenController;

			if(controller != null && controller.LayoutType == MainLayoutType.LOADING_MARQUEE)
			{
                _screenNavManager.HideCurrent();
                //_screenNavManager.CloseCurrent();
			}
		}

		public void ShowMovie(Action callback)
		{
//			Logger.Log ("Showing Movie Screen", LogLevel.INFO);
			
			_introMovPlayer.Play (callback);
		}

		public void CleanupView()
		{
			_screenNavManager.CloseAll();
//			_screenNavManager.CloseCurrentScreen ();
//			_screenNavManager.GoToExistingScreen ("/");	// clear top screen
			
			// TODO: RELEASE OTHER INITIATION RESOURCES!!!
		}
			

        public bool IsLoadingScreenLoaded()
        {
            MainLoadingScreenController loadingScreen = _screenNavManager.CurrentController as MainLoadingScreenController;
            if (loadingScreen != null)
            {
                return loadingScreen.IsScreenLoaded();
            }
            return false;
        }

		public IScreenController GetCurrentScreenController()
		{
			return _screenNavManager.CurrentController;
		}


		public void ShowLoadingScreenDialogue(DialogueType type)
		{
			MainLoadingScreenController controller = _screenNavManager.CurrentController as MainLoadingScreenController;
			if(controller != null)
			{
				switch(type)
				{
					case DialogueType.ERROR_FORCEUPDATE:
						controller.CallForceUpdate();
						break;

					case DialogueType.ERROR_NETWORK:
						controller.CallNetworkError();
						break;

					case DialogueType.ERROR_MAINTENANCE:
						controller.CallMaintenance();
						break;

					case DialogueType.ERROR_INIT:
						break;

					case DialogueType.RESTORE_SUCCESS:
						break;

					default:
						throw new WitchesException("StartupDisplayController >>> No Dialogue of Type: " + type.ToString());
				}
			}
			else
			{
				throw new WitchesException("StartupDisplayController >>> No Controller");
			}
		}


		public enum DialogueType
		{
			NONE = 0,
			RESTORE_SUCCESS,
			ERROR_FORCEUPDATE,
			ERROR_MAINTENANCE,
			ERROR_NETWORK,
			ERROR_INIT
		}
    }
    
}

//			IScreenController loadingScreen = new MainLoadingScreenController(ScreenFactory, _versionService, (int)MainLayoutType.LOADING,null,null);
//			var controller = (loadingScreen as MainLoadingScreenController);
//			controller.DoneLoading += OnLoadingComplete;
//			StopLoadingRoutine = controller.StopLoading;
//			CallForceUpgrade = controller.CallForceUpdate();
//			CallCantConnect = controller.CallCantConnect();
//			CallNetworkError = controller.CallNetworkError();
//			CallMaintenance = controller.CallMaintenance();
//			ScreenNavManager.Add(loadingScreen);

//			CallForceUpdate = DisplayForceUpdateDialog;
//			CallMaintenance = DisplayMaintenanceDialog;
//			CallNetworkError = DisplayNetworkErrorDialog;
//			CallCantConnect = DisplayCantConnectDialog;




