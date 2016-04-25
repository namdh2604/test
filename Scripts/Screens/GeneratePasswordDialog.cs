using UnityEngine;
using System.Collections;
using iGUI;

namespace Voltage.Witches.Screens
{
	public class GeneratePasswordDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUIButton btn_generate,btn_popup_close;

		[HideInInspector]
		public iGUIImage popup_close;

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
			btn_generate.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit; 
		}
		
		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				if(button != btn_popup_close)
				{
					button.colorTo(Color.grey,0f);
				}
				else
				{
					popup_close.colorTo(Color.grey,0f);
				}
			}
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			if(button != btn_popup_close)
			{
				button.colorTo(Color.grey,0f);
			}
			else
			{
				popup_close.colorTo(Color.grey,0f);
			}
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			if(button != btn_popup_close)
			{
				button.colorTo(Color.white,0.3f);
			}
			else
			{
				popup_close.colorTo(Color.white,0.3f);
			}
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOver)
		{
			if(isOver)
			{
				if(button == btn_generate)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
				else if(button == btn_popup_close)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
			}
			
			if(button != btn_popup_close)
			{
				button.colorTo(Color.white,0.3f);
			}
			else
			{
				popup_close.colorTo(Color.white,0.3f);
			}
		}
	}
}