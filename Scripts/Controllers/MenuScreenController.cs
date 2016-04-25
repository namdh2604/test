using System;
using System.Collections.Generic;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;

namespace Voltage.Witches.Controllers
{
	using Voltage.Witches.Configuration;
    using URLs = Voltage.Witches.Net.URLs;
//    using WitchesNetworkController = Voltage.Witches.Net.WitchesNetworkController;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
    using Voltage.Witches.Controllers.Factories;
	using Newtonsoft.Json.Linq;

	using Voltage.Story.Reset;
	using Voltage.Witches.Story.Reset;


	public interface IMenuScreenController
	{
		IDialog GetStoryResetDialog();
		IDialog GetStoyResetConfirmDialog();
		IDialog GetStoryResetCompleteDialog();
		IDialog GetDataTransferDialog();
		IDialog GetGeneratePasswordSuccessDialog(string id, string password);
		IDialog GetGenerateEmailSentDialog();
		void HandleBeginDataTransferResponse(Action<string,string> responseHandler);
		void HandleEmailRequest(string emailAddress, Action response);
		void ResetStoryProgress(Action responseHandler);
	}

	public class MenuScreenController : ScreenController, IMenuScreenController
	{
		private IScreenFactory _factory;
		private Player _player;
        private IControllerRepo _repo;

		private iGUISmartPrefab_MenuScreen _screen;

		public INetworkTimeoutController<WitchesRequestResponse> NetworkController { get; protected set; }
        private Action<string,string> _generatePasswordResponse;
        private Action _emailSentResponse;

		private readonly IStoryResetter _storyResetter;

        public MenuScreenController(ScreenNavigationManager controller, IScreenFactory factory, Player player, IControllerRepo repo, MasterConfiguration masterConfig) : base(controller)
		{
			_factory = factory;
			_player = player;
            _repo = repo;

			_storyResetter = _repo.Get<WitchesOptionsResetter> ();

			NetworkController = _repo.Get<INetworkTimeoutController<WitchesRequestResponse>>();
			InitializeView();

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

		protected override IScreen GetScreen ()
		{
			if(_screen != null)
			{
				return _screen;
			}
			else
			{
				_screen = _factory.GetScreen<iGUISmartPrefab_MenuScreen>();
				_screen.Init(_player, this);
				return _screen;
			}
		}

		public override void MakePassive (bool value)
		{
			_screen.MakePassive(value);
		}
		
		void InitializeView()
		{
			_screen = _factory.GetScreen<iGUISmartPrefab_MenuScreen>();
			_screen.Init(_player, this);
		}

		public IDialog GetOptionsDialog()
		{
    		OptionsDialogController dialogController = _repo.Get<OptionsDialogController>();
    		var screen = _factory.GetDialog<iGUISmartPrefab_OptionsDialog>();
    		screen.Init(dialogController, _player);
			screen.MenuController = this;
   		
    		return screen;
		}

		public IDialog GetStoryResetDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_StoryResetDialog>();
		}

		public IDialog GetStoyResetConfirmDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_ResetStoryConfirmDialog>();
		}

		public IDialog GetStoryResetCompleteDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_StoryResetCompleteDialog>();
		}

		public IDialog GetDataTransferDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_GeneratePasswordDialog>();
		}

		public IDialog GetGeneratePasswordSuccessDialog(string id,string password)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_GeneratePasswordSuccessDialog>();
			dialog.SetIDAndPassword(id, password);
			return dialog;
		}

		public IDialog GetGenerateEmailSentDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_GenerateEmailSentDialog>();
		}

		public IDialog GetSystemDialog(string message)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialog.SetText(message);
			return dialog;
		}

		public void HandleBeginDataTransferResponse(Action<string,string> responseHandler)
		{
			_generatePasswordResponse = responseHandler;
			if(_generatePasswordResponse == null)
			{
				throw new NullReferenceException("Response handler was null");
			}
			string userID = _player.UserID;

			if(string.IsNullOrEmpty(userID))
			{
				userID = UnityEngine.PlayerPrefs.GetString("id");
				if(string.IsNullOrEmpty(userID))
				{
					userID = UnityEngine.PlayerPrefs.GetString("phone_id");
				}
				if(string.IsNullOrEmpty(userID))
				{
					throw new NullReferenceException("User ID is null");
				}
			}

			Dictionary<string,string> parameters = new Dictionary<string,string> ()
			{
				{"phone_id",userID}
			};


			if(NetworkController == null)
			{
				throw new NullReferenceException("Network Controller was Null");
			}

			NetworkController.Send(URLs.REQUEST_PASSWORD, parameters, HandlePasswordRequestSuccess, HandlePasswordRequestFail);
		}

		void HandlePasswordRequestSuccess(WitchesRequestResponse obj)
		{
			var response = obj.WWW.text;
			JObject parsedInfo = JObject.Parse(response);
			var userID = parsedInfo["user_id"].ToString();
			var password = parsedInfo["password"].ToString();
			if(_generatePasswordResponse != null)
			{
				_generatePasswordResponse(userID,password);
			}

			_generatePasswordResponse = null;
		}

		void HandlePasswordRequestFail(WitchesRequestResponse obj)
		{
			UnityEngine.Debug.LogWarning("User Password request Failed");
			var dialog = GetSystemDialog("Password Request Failed");
			dialog.Display(HandleSystemDialogResponse);
		}

		void HandleSystemDialogResponse(int answer)
		{
			UnityEngine.Debug.LogWarning("Close System Dialog");
		}

		public void HandleEmailRequest(string emailAddress,Action response)
		{
			_emailSentResponse = response;
			//TODO Fill in after Yoshi adds an actual server call
			CheckEmail(emailAddress);
		}

		//HACK Just to test the overall flow
		void CheckEmail(string address)
		{
			HandleEmailSentSuccess(null);
		}

		void HandleEmailSentSuccess(WitchesRequestResponse obj)
		{
			UnityEngine.Debug.LogWarning("Email sent successfully");
			_emailSentResponse();
			_emailSentResponse = null;
		}

		void HandleEmailSentFail(WitchesRequestResponse obj)
		{
			UnityEngine.Debug.LogWarning("Email sent failure");
			var dialog = GetSystemDialog("There was a problem sending to your email address");
			dialog.Display(HandleSystemDialogResponse);
			_emailSentResponse = null;
		}

		public void ResetStoryProgress(Action responseHandler)
		{
			_storyResetter.Reset();
			responseHandler();
		}

		public void GoToInventory()
		{
			var defaultView = 1;
            IInventoryScreenControllerFactory factory = _repo.Get<IInventoryScreenControllerFactory>();
            IScreenController nextScreen = factory.Create(defaultView);
			Manager.Add(nextScreen);
		}

		public void GoToMailBox()
		{
            IScreenController nextScreen = _repo.Get<MailboxScreenController>();
			Manager.Add(nextScreen);
		}

		public void GoToGlossary()
		{
			IScreenController nextScreen = new GlossaryScreenController(Manager,_factory,_player);
			Manager.Add(nextScreen);
		}

		public Glossary GetBasicGlossary()
		{
			Glossary basicGlossary = new Glossary();

			return basicGlossary;
		}

		public void GoHome()
		{
			if(Manager.GetCurrentPath().Contains("/Home"))
			{
				Manager.GoToExistingScreen("/Home");
			}
			else
			{
				HomeScreenController nextScreen = _repo.Get<HomeScreenController>();
				Manager.Add(nextScreen);
			}
		}
		
		public void SetEnabled(bool value)
		{
			_screen.SetEnabled(value);
		}
		
		public void Unload()
		{
			_screen.SetEnabled(false);
		}
	}
}
