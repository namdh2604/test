using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using iGUI;

namespace Voltage.Witches.Screens
{
	using Voltage.Witches.Events;

	public class GeneratePasswordSuccessDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIButton btn_email,btn_popup_close;
		
		[HideInInspector]
		public iGUILabel id_label,password_label,time_limit_label;

		[HideInInspector]
		public iGUITextfield email_input;

		[HideInInspector]
		public iGUIContainer email;

		IGUIHandler _buttonHandler;
		public GUIEventHandler EmailInput;

		public string UserID { get; protected set; }
		public string Password { get; protected set; }

		private string _emailRegex = @"\A[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\Z";
		private static string _defaultMessage = "This password will become invalid in 7 days or once it has been used.";

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
			btn_email.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit;

			email_input.focusCallback += ClearDefault;
			email_input.valueChangeCallback += UpdateText;

			UpdateTimeLimit();
			UpdateId();
			UpdatePassword();
			email.setEnabled(isValidEmail());
		}

		public void SetIDAndPassword(string id,string password)
		{
			UserID = id;
			Password = password;
		}

		void UpdateTimeLimit()
		{
			//This will display the default message
			time_limit_label.label.text = _defaultMessage;

			//This will display the password time limit relative to the current time when the password was received
//			var date = DateTime.Now.ToLocalTime();
//			date = date.AddDays((double)7);
//
//			string time = string.Format("{0}:{1}",date.Hour,date.Minute);
//			string dateText = string.Format("{0}/{1}/{2}",date.Month.ToString("D2"),date.Day.ToString("D2"),date.Year.ToString("D2"));
//			var currentText = time_limit_label.label.text;
//
//			currentText = currentText.Replace("00:00",time);
//			currentText = currentText.Replace("MM/DD/YY", dateText);
//
//			time_limit_label.label.text = currentText;
		}

		void UpdateId()
		{
			var label = id_label.label.text;

			label = label.Replace("NNNNNNNN", UserID);
			id_label.label.text = label;
		}

		void UpdatePassword()
		{
			var label = password_label.label.text;
			
			label = label.Replace("MMMM", Password);
			password_label.label.text = label;
		}

		public void CloseDialog()
		{
			SubmitResponse((int)DialogResponse.OK);
		}

		void ClearDefault(iGUIElement caller)
		{
			if(caller == email_input)
			{
				email_input.setValue(string.Empty);
				email_input.focusCallback -= ClearDefault;
			}
		}

		void UpdateText(iGUIElement caller)
		{
			if(caller == email_input)
			{
				Debug.LogWarning(email_input.value);
			}

			email.setEnabled(isValidEmail());
		}

		bool isValidEmail()
		{
			if(email_input.value != "Input e-mail address")
			{
				var value = email_input.value;
				bool isEmail = Regex.IsMatch(value,_emailRegex,RegexOptions.IgnoreCase);
				return isEmail;
			}

			return false;
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
				if(button == btn_email)
				{
					if(EmailInput != null)
					{
						EmailInput(this, new GUIEventArgs());
					}
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