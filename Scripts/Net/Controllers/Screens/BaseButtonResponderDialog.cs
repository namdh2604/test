
using System;
using System.Collections.Generic;


namespace Voltage.Witches.Screens
{
	using UnityEngine;
	using iGUI;

	public class BaseButtonResponderDialog : AbstractDialog
	{
		
		IGUIHandler _buttonHandler;
		
		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}
		
//		protected void Start()
//		{
//			yes_button.clickDownCallback += ClickInit;
//			no_button.clickDownCallback += ClickInit;
//		}
		
		private IDictionary<iGUIButton,int> _buttonResponseMap = new Dictionary<iGUIButton,int> ();
		
		protected void InitButtonCallbacks(params iGUIButton[] buttons)
		{
			if(buttons != null)
			{
//				foreach(iGUIButton button in buttons)
				for(int i=0; i < buttons.Length; i++)
				{
					buttons[i].clickDownCallback += ClickInit;
					_buttonResponseMap.Add (buttons[i], i);
				}
			}
		}
		
		
		void ClickInit(iGUIElement caller)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)caller;
				_buttonHandler.SelectButton(button);
				button.getTargetContainer().colorTo(Color.grey,0f);
			}
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			button.getTargetContainer().colorTo(Color.white,0.3f);
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			button.getTargetContainer().colorTo(Color.grey,0f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				SubmitResponse(_buttonResponseMap[button]);
				
//				if(button == yes_button)
//				{
//					SubmitResponse((int)DialogResponse.OK);
//				}
//				else if(button == no_button)
//				{
//					SubmitResponse((int)DialogResponse.Cancel);
//				}
			}
			
			button.getTargetContainer().colorTo(Color.white,0.3f);
		}
		
	}
    
}




