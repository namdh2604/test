using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Voltage.Witches.Configuration.JSON;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;

namespace Voltage.Witches.Controllers
{
	using Debug = UnityEngine.Debug;

	using Voltage.Story.StoryPlayer;
	using Voltage.Story.Models.Nodes;
	using Voltage.Story.StoryDivisions;
	using Voltage.Witches.Story;
	using Voltage.Witches.Controllers.Factories;
    using Voltage.Witches.Services;

	using Voltage.Common.Net;
	using Voltage.Witches.DI;
	
	public class MainLoadingScreenController : ScreenController
	{
		private IScreenFactory _factory;
        private IBuildNumberService _versionService;
		private iGUISmartPrefab_MainLoadingScreen _screen;
//		private Player _player;
		private ControllerRepo _repo;

		public Action AppStartEvent { get; private set; }
		public Action RestoreEvent { get; private set; }

		public delegate void LoadingUIEvent();
		public event LoadingUIEvent DoneLoading;

		public MainLayoutType ScreenType { get; protected set; }

		public Action StopLoading;

		private iGUISmartPrefab_RestoreDataDialog _restoreDialog;
		private Action<string,string> _restoreHandler;

		private RestorePlayerController _restoreController;

		public INetworkTransportLayer Request 
		{ 
			get
			{
				return _screen.Request;		// Can throw exception if _screen is null!
			}
			set
			{
				_screen.Request = value;	// Can throw exception if _screen is null!
			}
		}

		public MainLoadingScreenController(IScreenFactory factory, IBuildNumberService versionService, int mainLayout, Action callback1, Action callback2, 
                                           RestorePlayerController restoreController):base(null)	// , INetworkTransportLayer request
		{
            if (versionService == null) // || restoreController == null)
            {
                throw new ArgumentException("versionService cannot be null", "versionService");
            }

			_factory = factory;
            _versionService = versionService;

			ScreenType = (MainLayoutType)mainLayout;
			_restoreController = restoreController;

			AppStartEvent = callback1;
			RestoreEvent = callback2;

			InitializeView(mainLayout);
		}

        public bool IsScreenLoaded()
        {
            return GetScreen().IsScreenLoaded();
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
        }

		protected override IScreen GetScreen()
		{
			if(_screen != null)
			{
				return _screen;
			}
            return null;
            // Keeping this code here, in case there is some need for the mainloading screen always need to have a screen, if 
            // that turns out to be true, we should make this screen static. - Hung Nguyen
//			else 
//			{
//				_screen = _factory.GetScreen<iGUISmartPrefab_MainLoadingScreen>();
//                _screen.Init(this,Request,(int) MainLayoutType.LOADING_MARQUEE);
//				return _screen;
//			}
		}

		public override void MakePassive (bool value)
		{
			_screen.MakePassive(value);
		}


        public string GetBuildVersion()
        {
            return _versionService.GetBuildVersion();
        }

		public MainLayoutType LayoutType { get; private set; }

		void InitializeView(int layoutType)
		{
			LayoutType = (MainLayoutType)layoutType;

			_screen = _factory.GetScreen<iGUISmartPrefab_MainLoadingScreen>();
			_screen.Init(this,Request,layoutType);
			StopLoading = _screen.StopLoading;
		}

		public override void Close ()
		{
			StopLoading();	// NOTE: may happen anyway when screen is closed, making explicit...hopefully any routines are handled when that happens
			base.Close ();
		}

        public void CallForceUpdate()
		{
			 _screen.CallForceUpdate();
		}

		public Action CallNetworkError()
		{
			return _screen.CallNetworkError;
		}

		public Action CallMaintenance()
		{
			return _screen.CallMaintenance;
		}

		public IDialog GetForceUpdateDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_ForceUpdateDialog>();
		}

		public IDialog GetMaintenanceDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_MaintenanceDialog>();
		}

		public IDialog GetNetworkErrorDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_NetworkErrorDialog>();
		}

		public IDialog GetRestoreDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_RestoreDataDialog>();
		}

		public IDialog GetRestoreSuccessDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_RestoreSuccessDialog>();
		}

		public void ProcessAction(int response)
		{
			Dispose();
		}

		public void LoadingComplete()
		{
			if(DoneLoading != null)
			{
				DoneLoading();
			}
		}

		private Action<bool> serverResponseDelegate;	// circular!!!

		public void RestoreUser(Action onSuccess)
		{
			AppStartEvent = onSuccess;

			serverResponseDelegate = InitiateRestoreUser ((id, password) => _restoreController.RequestRestore (id, password, () => serverResponseDelegate (true), () => serverResponseDelegate (false)));
		}

		private Action<bool> InitiateRestoreUser(Action<string,string> responseHandler)
		{
			ToggleTopButtons();
			_restoreHandler = responseHandler;

			var dialog = GetRestoreDialog();

			dialog.Display(HandleRestoreResponse);
			_restoreDialog = (dialog as iGUISmartPrefab_RestoreDataDialog);
			_restoreDialog.UserIDPassInput += HandleUserAndPassword;

			Action<bool> serverResponseHandler = delegate(bool isSuccess) { _restoreDialog.HandleServerCallback(isSuccess); };

			return serverResponseHandler;
		}

		void ToggleTopButtons()
		{
			if(_screen.buttons_container.enabled)
			{
				_screen.buttons_container.setEnabled(false);
			}
			else
			{
				_screen.buttons_container.setEnabled(true);
			}
		}

		void HandleUserAndPassword(object sender, Voltage.Witches.Events.GUIEventArgs e)
		{
			var password = _restoreDialog.password_input.value;
			var userID = _restoreDialog.user_id_input.value;

			if(_restoreHandler != null)
			{
				_restoreHandler(userID,password);
			}
			UnityEngine.Debug.LogWarning("USER :: " + userID + " PASSWORD :: " + password);
			//TODO Send off for verification to Witches Startup Sequence
		}

		void HandleRestoreResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
				case DialogResponse.OK:
					_restoreDialog = null;
					CallSuccessDialog();
					break;
				case DialogResponse.Cancel:
					_restoreDialog = null;
					ToggleTopButtons();
					break;
			}
		}

		void CallSuccessDialog()
		{
			var dialog = GetRestoreSuccessDialog();
			dialog.Display(HandleRestoreSuccessResponse);
		}

		void HandleRestoreSuccessResponse(int answer)
		{
			//HACK Just to display functionality
			AppStartEvent();
			Dispose ();
		}

		public void SetEnabled(bool value)
		{
			_screen.SetEnabled(value);
		}
		
		public void Unload()
		{
			_screen.SetEnabled(false);
		}

		public void EnableBG()
		{
			if(_screen != null)
			{
				_screen.bg.gameObject.SetActive(true);
			}
			else
			{
				UnityEngine.Debug.LogWarning ("Could not enableBG on null screen");
			}
		}
	}

	public enum MainLayoutType
	{
		TOP_SCREEN = 0,
		LOADING = 1,
		LOADING_MARQUEE = 2
	}
}