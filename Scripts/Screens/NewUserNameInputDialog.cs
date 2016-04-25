using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Screens
{
	public class NewUserNameInputDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUITextfield first_input, last_input;

		[HideInInspector]
		public iGUIContainer btn_ok;

		[HideInInspector]
		public iGUIButton btn_galaxy_med;
		
		[HideInInspector]
		public iGUIImage book_bg, TransparentImage;

		public GUIEventHandler NameSelected;
		private Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		IGUIHandler _buttonHandler;

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
			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement>()
			{
				{btn_galaxy_med,btn_ok}
			};
			
			btn_galaxy_med.clickDownCallback += ClickInit;
			
			first_input.valueChangeCallback += UpdateTextField;
			last_input.valueChangeCallback += UpdateTextField;
		}

		void UpdateTextField(iGUIElement caller)
		{
			if(caller == last_input)
			{
				Debug.Log(last_input.value);
			}
			else if(caller == first_input)
			{
				Debug.Log(first_input.value);
			}
			
			btn_ok.setEnabled(AreFirstAndLastInputsNotEmpty());
		}

		bool AreFirstAndLastInputsNotEmpty()
		{
			return ((!string.IsNullOrEmpty(last_input.value)) && (!string.IsNullOrEmpty(first_input.value)));
		}
		
		//TODO Add in name validation stuff??
		
		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey,0f);
			}
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey,0f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_galaxy_med)
				{
					if(NameSelected != null)
					{
						NameSelected(this, new GUIEventArgs());
					}
					SubmitResponse((int)DialogResponse.OK);
				}
			}
			
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}	
	}
}