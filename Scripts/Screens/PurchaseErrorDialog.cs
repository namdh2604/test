using UnityEngine;
using iGUI;
using System.Collections;

namespace Voltage.Witches.Screens
{
	public class PurchaseErrorDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUIButton btn_ok;

		[HideInInspector]
		public iGUIContainer btn_ok_grp;

		[HideInInspector]
		public iGUILabel sorry_popup_label;

		[HideInInspector]
		public iGUIImage ok_text;

		string _errorMessage;
		IGUIHandler _buttonHandler;

		public void SetErrorMessage(string errorMessage)
		{
			_errorMessage = errorMessage;
		}

		protected virtual void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected virtual void Start()
		{
			btn_ok.clickDownCallback += ClickInit;
			if(!string.IsNullOrEmpty(_errorMessage))
			{
				sorry_popup_label.label.text = _errorMessage;
			}
			ok_text.passive = true;
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				btn_ok_grp.colorTo(Color.grey,0f);
			}
		}

		void HandleMovedAway(iGUIButton button)
		{
			btn_ok_grp.colorTo(Color.white,0.3f); 
		}

		void HandleMovedBack(iGUIButton button)
		{
			btn_ok_grp.colorTo(Color.grey,0f);
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_ok)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
			}

			btn_ok_grp.colorTo(Color.white,0.3f); 
		}
	}
}