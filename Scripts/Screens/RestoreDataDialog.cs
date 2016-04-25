using iGUI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Screens
{
	public class RestoreDataDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUITextfield user_id_input, password_input;
		
		[HideInInspector]
		public iGUIContainer btn_restore_grp;

		[HideInInspector]
		public iGUIImage invalid_message,restore_text;

		[HideInInspector]
		public iGUIButton btn_restore,btn_popup_close;
		
		public GUIEventHandler UserIDPassInput;

		Coroutine _invalidMessage;

		IGUIHandler _buttonHandler;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected void Start()
		{
			restore_text.passive = true;

			user_id_input.focusCallback += ClearDefault;
			password_input.focusCallback += ClearDefault;

			user_id_input.valueChangeCallback += UpdateTextField;
			password_input.valueChangeCallback += UpdateTextField;

			btn_restore_grp.setEnabled(AreMyTextFieldsNotEmpty());

			btn_restore.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit;

			_buttonArtMap = new Dictionary<iGUIButton,iGUIElement> ()
			{
				{btn_restore,btn_restore_grp},
				{btn_popup_close,btn_popup_close}
			};
		}

		void ClearDefault(iGUIElement caller)
		{
			if((caller == user_id_input) && (user_id_input.value == "UserID"))
			{
				user_id_input.value = string.Empty;
				user_id_input.focusCallback -= ClearDefault;
			}
			else if((caller == password_input) && (password_input.value == "Pass"))
			{
				password_input.value = string.Empty;
				password_input.focusCallback -= ClearDefault;
			}
		}
		
		void UpdateTextField(iGUIElement caller)
		{
			if(caller == password_input)
			{
				Debug.Log(password_input.value);
			}
			else if(caller == user_id_input)
			{
				Debug.Log(user_id_input.value);
			}
			
			btn_restore_grp.setEnabled(AreMyTextFieldsNotEmpty());
		}
		
		bool AreMyTextFieldsNotEmpty()
		{
			if(_invalidMessage != null)
			{
				return false;
			}

			if((!string.IsNullOrEmpty(password_input.value)) && (!string.IsNullOrEmpty(user_id_input.value)))
			{
				if((password_input.value == "Pass") || (user_id_input.value == "UserID"))
				{
					return false;
				}

				var passwordLength = password_input.value.Length;
				var userLength = user_id_input.value.Length;

				if((passwordLength != 4) && (userLength != 8))
				{
					return false;
				}

				return true;
			}

			return false;
		}

		public void FlashInvalidText()
		{
			if((!invalid_message.enabled) && (_invalidMessage == null))
			{
				_invalidMessage = StartCoroutine(FlashingText());
				btn_restore_grp.setEnabled(AreMyTextFieldsNotEmpty());
			}
		}

		IEnumerator FlashingText()
		{
			invalid_message.setEnabled(true);
			var scale = invalid_message.scale;
			invalid_message.scaleTo(1.15f, 0.5f, iTweeniGUI.EaseType.easeOutBounce);
			yield return new WaitForSeconds(0.75f);
			var current = invalid_message.scale;
			invalid_message.scaleTo(current,scale,0.5f,iTweeniGUI.EaseType.easeInBounce);
			yield return new WaitForSeconds(0.75f);
			invalid_message.setEnabled(false);
			_invalidMessage = null;
		}

		public void HandleServerCallback(bool isSuccess)
		{
			if(isSuccess)
			{
				SubmitResponse((int)DialogResponse.OK);
			}
			else
			{
				FlashInvalidText();
			}
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
		
		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey,0f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_restore)
				{
					if(UserIDPassInput != null)
					{
						UserIDPassInput(this, new GUIEventArgs());
					}
				}
				else if(button == btn_popup_close)
				{
					if(_invalidMessage != null)
					{
						StopCoroutine(_invalidMessage);
					}
					SubmitResponse((int)DialogResponse.Cancel);
				}
			}
			
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
	}
}