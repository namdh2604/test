using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System;
using iGUI;
using System.Collections.Generic;

namespace Voltage.Witches.Screens
{
	public class TutorialCompleteSceneDialog : AbstractDialogUGUI
	{
		public Button _button;
		private Action<int> _callback;

		protected void Awake ()
		{
			_button.onClick.AddListener(HandleButtonEvent);
		}

		private void HandleButtonEvent ()
		{	
			SubmitResponse ((int)DialogResponse.OK);
		}

		private void OnDestroy(){
			_button.onClick.RemoveListener (HandleButtonEvent);
		}
	}
}
