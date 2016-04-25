using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Screens
{
	public class NoStaminaPotionDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUIButton btn_galaxy_long,btn_popup_close;

		IGUIHandler _buttonHandler;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		
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
			btn_galaxy_long.clickDownCallback += ClickInit;

			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement> ()
			{
				{btn_popup_close,btn_popup_close},
				{btn_galaxy_long,btn_galaxy_long.getTargetContainer()}
			};
		}
		
		void ClickInit(iGUIElement element)
		{
			if(_buttonHandler.IsActive)
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey, 0f);
			}
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white, 0.3f);
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey, 0f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_galaxy_long)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
				else if(button == btn_popup_close)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
			}
			
			_buttonArtMap[button].colorTo(Color.white, 0.3f);
		}
	}
}