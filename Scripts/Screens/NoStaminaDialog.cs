using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Screens
{
	public class NoStaminaDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIButton btn_regain_stamina,btn_popup_close;

		[HideInInspector]
		public iGUIContainer btn_container;

		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		IGUIHandler _buttonHandler;

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected void Start()
		{
			btn_regain_stamina.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit;

			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement> ()
			{
				{btn_regain_stamina,btn_container},
				{btn_popup_close,btn_popup_close}
			};
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey,0f);
			}
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey,0f);
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_regain_stamina)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
				else if(button == btn_popup_close)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
			}
			
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
	}
}