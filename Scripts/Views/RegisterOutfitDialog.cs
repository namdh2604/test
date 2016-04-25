using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Screens
{
	public class RegisterOutfitDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUITextfield outfit_input;

		[HideInInspector]
		public iGUIButton btn_popup_close,btn_save;

		[HideInInspector]
		public iGUIContainer save_grp;

		IGUIHandler _buttonHandler;
		public GUIEventHandler OutfitNameInput;

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected void Start()
		{
			outfit_input.focusCallback += ClearDefault;

			btn_save.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit;
		}

		void ClearDefault(iGUIElement caller)
		{
			if(caller == outfit_input)
			{
				outfit_input.focusCallback -= ClearDefault;
				outfit_input.setValue(string.Empty);
			}
		}

		void UpdateNameField(iGUIElement caller)
		{
			if(caller == outfit_input)
			{
				Debug.Log(outfit_input.value);
			}
			
			save_grp.setEnabled(doesNameHaveAValue());
		}
		
		bool doesNameHaveAValue()
		{
			return (!string.IsNullOrEmpty(outfit_input.value));
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				button.colorTo(Color.grey,0f);
			}
		}

		void HandleMovedAway(iGUIButton button)
		{
			button.colorTo(Color.white,0.3f);
		}

		void HandleMovedBack(iGUIButton button)
		{
			button.colorTo(Color.grey,0f);
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_save)
				{
					if(OutfitNameInput != null)
					{
						OutfitNameInput(this, new GUIEventArgs());
					}
					SubmitResponse((int)DialogResponse.OK);
				}
				else if(button == btn_popup_close)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
			}

			button.colorTo(Color.white,0.3f);
		}
	}
}