using UnityEngine;
using System.Collections;
using iGUI;

namespace Voltage.Witches.Screens
{
	public class ResetStoryConfirmDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIButton btn_reset,btn_popup_close;
		
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
			btn_reset.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit; 
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
		
		void HandleMovedBack(iGUIButton button)
		{
			button.colorTo(Color.grey,0f);
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			button.colorTo(Color.white,0.3f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOver)
		{
			if(isOver)
			{
				if(button == btn_reset)
				{
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