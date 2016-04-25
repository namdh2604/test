﻿using UnityEngine;
using iGUI;
using System.Collections;

namespace Voltage.Witches.Screens
{
	public class NetworkErrorDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUIButton btn_popup_close,btn_resfresh;
		
		IGUIHandler _buttonHandler;
		
		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}
		
		protected void Start()
		{
			btn_popup_close.clickDownCallback += ClickInit;
			btn_resfresh.clickDownCallback += ClickInit;
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
		
		void HandleMovedBack(iGUIButton pressedButton)
		{
			pressedButton.colorTo(Color.grey,0f);
		}
		
		void HandleMovedAway(iGUIButton pressedButton)
		{
			pressedButton.colorTo(Color.white,0.3f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton pressedButton, bool isOverButton)
		{
			if(isOverButton)
			{
				if(pressedButton == btn_popup_close)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
				else if(pressedButton == btn_resfresh)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
			}
			
			pressedButton.colorTo(Color.white,0.3f);
		}
	}
}