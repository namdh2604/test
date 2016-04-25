using iGUI;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Android.DeviceInput
{
	using Voltage.Witches;
	using Voltage.Witches.Screens;
	using Voltage.Common.Logging;

	using Voltage.Common.ID;

	using Voltage.Witches.Models;
	using Voltage.Witches.Controllers;

	// FIXME: maybe need to rename class
    public class AndroidBackButtonHandler : MonoBehaviour
    {
		private IScreenFactory _screenFactory;
        private ScreenEnabler _screenEnabler;
		private bool _isShowingDialogue;	// not great design
		private bool _enabled=false;

		public void Init(ScreenNavigationManager navManager, IScreenFactory screenFactory)
		{
			_screenFactory = screenFactory;
            _screenEnabler = new ScreenEnabler(navManager);

			_isShowingDialogue = false;
			_enabled = true;
		}


		public void Activate()
		{
			_enabled = true;
		}

		public void Deactivate()
		{
			_enabled = false;
		}

		private void Update()
		{
			if(Input.GetKeyUp(KeyCode.Escape))
			{
				if(!_isShowingDialogue && _enabled)
				{
					ShowQuitDialogue();
				}
			}
		}




		private void ShowQuitDialogue()
		{
            if (_screenFactory != null)
			{
				MakeScreenPassive(true);

				iGUISmartPrefab_QuitAppDialog dialogue;

				// HACK: mail tutorial's overlay is layered above dialog, so putting Quit dialogue in overlay and relying on its KeepOnTop()
				dialogue = _screenFactory.GetOverlay<iGUISmartPrefab_QuitAppDialog> ();	

				dialogue.ButtonHandler.OverrideDisableAll = true;

				dialogue.Display ((choice) => 
				{
					HandleInput ((DialogResponse)choice);
					_isShowingDialogue = false;

					MakeScreenPassive(false);
				});

				_isShowingDialogue = true;
			} 
			else 
			{
				AmbientLogger.Current.Log ("AndroidBackButtonHandler::ShowQuitDialogue >>> No ScreenFactory!", LogLevel.WARNING);
			}


		}

	

		private void MakeScreenPassive(bool value)
		{
            _screenEnabler.EnableInput(!value);
		}

		private void OnDestroy()
		{
            _screenEnabler.EnableInput(true);
		}

		private void HandleInput(DialogResponse response)
		{
			switch(response)
			{
				case DialogResponse.OK:
					Application.Quit(); break;
				case DialogResponse.Cancel:
				default:
					AmbientLogger.Current.Log ("AndroidBackButtonHandler >>> Cancelled Quit", LogLevel.INFO);
					break;
			}
		}






    }

}




