using UnityEngine;
using System.Collections;
using iGUI;

namespace Voltage.Witches.Screens
{
	public class StoryResetDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUIButton btn_reset,btn_cancel;

		[HideInInspector]
		public iGUILabel story_reset_label;

		IGUIHandler _buttonHandler;

		private string _message = "If you reset all story progress and Affinity will be erased and cannot be recovered later.";

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
			btn_cancel.clickDownCallback += ClickInit;

			story_reset_label.label.text = _message;
			story_reset_label.label.tooltip = string.Empty;
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
				else if(button == btn_cancel)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
			}

			button.colorTo(Color.white,0.3f);
		}
	}
}