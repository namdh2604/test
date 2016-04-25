using UnityEngine;
using System.Collections;
using iGUI;

namespace Voltage.Witches.Screens
{
	public class ForceUpdateDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIButton btn_galaxy_med;

		private IGUIHandler _buttonHandler;

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
			btn_galaxy_med.clickDownCallback += ClickInit;
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
				if(pressedButton == btn_galaxy_med)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
			}

			pressedButton.colorTo(Color.white,0.3f);
		}
	}
}