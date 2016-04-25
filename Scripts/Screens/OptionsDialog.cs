using UnityEngine;
using System.Collections.Generic;
using System;
using iGUI;
using Voltage.Witches.Events;
using Voltage.Witches.Models;
using Voltage.Witches.Services;
using Voltage.Witches.Controllers;

namespace Voltage.Witches.Screens
{
	using Voltage.Witches.Controllers;

	public class OptionsDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIImage switchframeoff_img_0,switchframeoff_img_1,btn_popup_close_img;
		
		[HideInInspector]
		public iGUIButton help_button,moregames_button,tou_button,datatransfer_button,storyreset_button,
						  btn_popup_close,bgm_toggle,sfx_toggle;

		[HideInInspector]
		public iGUIContainer story_reset_grp,help_support_grp,privacy_tou_grp,more_games_grp,data_transfer_grp;

		[HideInInspector]
		public iGUILabel user_id_label;

		iGUILabel _versionInfoLabel;
		string _buildNumber;

		IGUIHandler _buttonHandler;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;

		private OptionsDialogController _controller;
		private Player _player;

		public IMenuScreenController MenuController { get; set; }
		public GUIEventHandler StoryResetInitiated;
		public GUIEventHandler DataTransferInitiated;

		enum ToggleType
		{
			BGM = 0,
			SFX = 1,
		}

		enum ToggleState
		{
			DISABLED = 0,
			ENABLED = 1
		}

		ToggleState _bgm_state;
		ToggleState _sfx_state;

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
			
			SetStatesFromPrefs();

//#if DEBUG
			HackAddDebugSceneSelector ();	// HACK! FIXME! add debug pathing to screen architecture
//#endif
		}

		private void HackAddDebugSceneSelector()
		{
			if(Debug.isDebugBuild)
			{
				gameObject.AddComponent<Voltage.Witches.DebugTool.SceneSelector>();
			}
		}

		
		void SetStatesFromPrefs()
		{
			var valueForSound = Convert.ToInt32(PlayerPreferences.GetInstance().SoundEnabled);
			_bgm_state = (ToggleState)valueForSound;
			
			var valueForSFX = Convert.ToInt32(PlayerPreferences.GetInstance().SFXEnabled);
			_sfx_state = (ToggleState)valueForSFX;
		}

		public void Init(OptionsDialogController controller, Player player)
		{
			_player = player;
			_controller = controller;
			_buildNumber = _controller.GetBuildVersion();
		}

		protected void Start()
		{
			List<iGUIButton> pressableButtons = new List<iGUIButton> ()
			{
				help_button,moregames_button,tou_button,datatransfer_button,storyreset_button,
				btn_popup_close,bgm_toggle,sfx_toggle
			};

			_buttonArtMap = new Dictionary<iGUIButton,iGUIElement> ()
			{
				{help_button,help_support_grp},
				{moregames_button,more_games_grp},
				{tou_button,privacy_tou_grp},
				{datatransfer_button,data_transfer_grp},
				{storyreset_button,story_reset_grp},
				{btn_popup_close,btn_popup_close_img}
			};

			for(int i = 0; i < pressableButtons.Count; ++i)
			{
				var button = pressableButtons[i];
				button.clickDownCallback += ClickInit;
			}
			
			//TODO Need a conditional to set this in code dependent on version
			{
				more_games_grp.setEnabled(false);
				data_transfer_grp.setEnabled(false);
				user_id_label.setEnabled(true);
				var text = user_id_label.label.text;
				text = text.Replace("XXXXXXXX",_player.UserID);
				user_id_label.label.text = text;
			}

			UpdateToggles();

			//Production wants to display this in a produciton build, not just debug
//			if(Debug.isDebugBuild)
			{
				var container = gameObject.GetComponent<iGUIContainer>();
				_versionInfoLabel = container.addElement<iGUILabel>();
				_versionInfoLabel.name = "VERSION_INFO";
				_versionInfoLabel.setVariableName(_versionInfoLabel.name);
				_versionInfoLabel.setDynamicFontSize(0.2f);
				_versionInfoLabel.setLayer(container.itemCount + 1);
				_versionInfoLabel.setPositionAndSize(new Rect(1f,1f,0.25f,0.1f));
				_versionInfoLabel.label.text = " BUILD: " + _buildNumber;
			}

			DisplayResetButton (AtLeastOneRouteCompleted);
		}



		private void DisplayResetButton(bool enabled)
		{
			story_reset_grp.setEnabled (enabled);
		}

		private bool AtLeastOneRouteCompleted
		{
			get { return (_player.TotalPriorAffinity > 0); }
		}


		public void CloseDialog()
		{
			SubmitResponse((int)DialogResponse.Cancel);
		}

		void UpdateToggles()
		{
			var bgmOff = (_bgm_state == ToggleState.DISABLED);
			var sfxOff = (_sfx_state == ToggleState.DISABLED);

			switchframeoff_img_0.setEnabled(bgmOff);
			switchframeoff_img_1.setEnabled(sfxOff);
		}

		void ToggleToggle(int toggle)
		{
			switch((ToggleType)toggle)
			{
			case ToggleType.BGM:
				var bgmIsEnabled = (_bgm_state == ToggleState.ENABLED);
				switchframeoff_img_0.setEnabled(bgmIsEnabled);
				_bgm_state = (bgmIsEnabled) ? ToggleState.DISABLED : ToggleState.ENABLED;
				PlayerPreferences.GetInstance().SoundEnabled = Convert.ToBoolean((int)_bgm_state);
				break;
			case ToggleType.SFX:
				var sfxAreEnabled = (_sfx_state == ToggleState.ENABLED);
				switchframeoff_img_1.setEnabled(sfxAreEnabled);
				_sfx_state = (sfxAreEnabled) ? ToggleState.DISABLED : ToggleState.ENABLED;
				PlayerPreferences.GetInstance().SFXEnabled = Convert.ToBoolean((int)_sfx_state);
				break;
			}
		}
			

		bool SFXOn()
		{
			return (_sfx_state == ToggleState.ENABLED);
		}

		bool BGMOn()
		{
			return (_bgm_state == ToggleState.ENABLED);
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				iGUIButton pressedButton = (iGUIButton)element;
				_buttonHandler.SelectButton(pressedButton);
				
				if(_buttonArtMap.ContainsKey(pressedButton))
				{
					_buttonArtMap[pressedButton].colorTo(Color.grey,0f);
				}
				else if(pressedButton == sfx_toggle)
				{
					switchframeoff_img_1.setEnabled(SFXOn());
				}
				else if(pressedButton == bgm_toggle)
				{
					switchframeoff_img_0.setEnabled(BGMOn());
				}
			}
		}
		
		void HandleMovedBack(iGUIButton pressedButton)
		{
			if(_buttonArtMap.ContainsKey(pressedButton))
			{
				_buttonArtMap[pressedButton].colorTo(Color.grey,0f);
			}
			else if(pressedButton == sfx_toggle)
			{
				switchframeoff_img_1.setEnabled(SFXOn());
			}
			else if(pressedButton == bgm_toggle)
			{
				switchframeoff_img_0.setEnabled(BGMOn());
			}
		}
		
		void HandleMovedAway(iGUIButton pressedButton)
		{
			if(_buttonArtMap.ContainsKey(pressedButton))
			{
				_buttonArtMap[pressedButton].colorTo(Color.white,0.3f);
			}
			else if(pressedButton == sfx_toggle)
			{
				var isDisabled = (_sfx_state == ToggleState.DISABLED);
				switchframeoff_img_1.setEnabled(isDisabled);
			}
			else if(pressedButton == bgm_toggle)
			{
				var isDisabled = (_bgm_state == ToggleState.DISABLED);
				switchframeoff_img_0.setEnabled(isDisabled);
			}
		}

		void HandleReleasedButtonEvent(iGUIButton pressedButton, bool isOverButton)
		{
			if(isOverButton)
			{
				if(pressedButton == help_button)
				{
                    Application.OpenURL("https://voltageent.zendesk.com/home");
				}
				else if(pressedButton == moregames_button)
				{
					Application.OpenURL("http://www.voltage-ent.com/");
				}
				else if(pressedButton == tou_button)
				{
					Application.OpenURL("http://voltage-ent.com/curses/privacyandtou.html");
				}
				else if(pressedButton == datatransfer_button)
				{
					BeginDataTransferProcess();
				}
				else if(pressedButton == storyreset_button)
				{
					BeginStoryResetProcess();
				}
				else if(pressedButton == btn_popup_close)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
				else if(pressedButton == bgm_toggle)
				{
					ToggleToggle((int)ToggleType.BGM);
				}
				else if(pressedButton == sfx_toggle)
				{
					ToggleToggle((int)ToggleType.SFX);
				}
				
			}
			
			if(_buttonArtMap.ContainsKey(pressedButton))
			{
				_buttonArtMap[pressedButton].colorTo(Color.white,0.3f);
			}
		}

		void BeginDataTransferProcess()
		{
			_buttonHandler.Deactivate();
			var dialog = MenuController.GetDataTransferDialog();
			dialog.Display(GeneratePasswordResponse);
		}

		void GeneratePasswordResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
				case DialogResponse.OK:
					Debug.Log("Initiate the process to get the callback from the network stuff about generating password and getting user id to display");
					MenuController.HandleBeginDataTransferResponse(HandlePasswordResponse);
					break;
				case DialogResponse.Cancel:
					Debug.Log("Just Close");
					_buttonHandler.Activate();
					break;
			}
		}

		public void HandlePasswordResponse(string userID,string password)
		{
			var dialog = MenuController.GetGeneratePasswordSuccessDialog(userID,password);
			(dialog as iGUISmartPrefab_GeneratePasswordSuccessDialog).EmailInput += HandleEmailAddress;
			dialog.Display(HandlePasswordSuccessResponse);
		}

		void HandleEmailAddress(object sender, GUIEventArgs e)
		{
			var email = (sender as iGUISmartPrefab_GeneratePasswordSuccessDialog).email_input.value;
			MenuController.HandleEmailRequest(email, HandleEmailSentDialog);
			(sender as iGUISmartPrefab_GeneratePasswordSuccessDialog).CloseDialog();
		}

		void HandlePasswordSuccessResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
				case DialogResponse.OK:
				//
					break;
				case DialogResponse.Cancel:
				//
					break;
			}
			_buttonHandler.Activate();
		}

		void HandleEmailSentDialog()
		{
			_buttonHandler.Deactivate();
			var dialog = MenuController.GetGenerateEmailSentDialog();
			dialog.Display(HandleEmailSentResponse);
		}

		void HandleEmailSentResponse(int answer)
		{
			Debug.Log("Password request stuff is all good!");
			_buttonHandler.Activate();
		}

		void BeginStoryResetProcess()
		{
			_buttonHandler.Deactivate();
			var dialog = MenuController.GetStoryResetDialog();
			dialog.Display(HandleResetResponse);
		}

		void HandleResetResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
			case DialogResponse.OK:
				ConfirmRequestReset();
				break;
			case DialogResponse.Cancel:
				_buttonHandler.Activate();
				break;
			}
		}

		void ConfirmRequestReset()
		{
			var dialog = MenuController.GetStoyResetConfirmDialog();
			dialog.Display(HandleConfirmResponse);
		}

		void HandleConfirmResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
				case DialogResponse.OK:
					MenuController.ResetStoryProgress(StoryHasBeenReset);
					break;
				case DialogResponse.Cancel:
					_buttonHandler.Activate();
					break;
			}
		}

		public void StoryHasBeenReset()
		{
			var dialog = MenuController.GetStoryResetCompleteDialog();
			dialog.Display(HandleResetCompleteResponse);
		}

		void HandleResetCompleteResponse (int obj)
		{
			_buttonHandler.Activate();
			GoHome ();
		}

		private void GoHome()
		{
			SubmitResponse((int)DialogResponse.Cancel);
			(MenuController as MenuScreenController).GoHome ();	// maybe change to something more explicit and drop cast
		}
	}
}
