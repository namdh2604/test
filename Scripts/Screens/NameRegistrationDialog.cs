using UnityEngine.UI;
using System;
using iGUI;
using Voltage.Witches.Events;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

//HACK Needs to be replaced with UGUI
namespace Voltage.Witches.Screens
{
	using Debug = UnityEngine.Debug;

	public class NameRegistrationDialog : AbstractDialog
	{
		[HideInInspector]
        public iGUITextfield first_input, last_input;

		[HideInInspector]
        public iGUIContainer btn_ok;

		[HideInInspector]
		public iGUIContainer tutorial_register_name_parts;

		[HideInInspector]
        public iGUIButton btn_galaxy_med;

        [HideInInspector]
        public iGUILabel errorMsg;

		private Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		private IGUIHandler _buttonHandler;

        public string FirstName { get { return first_input.value; } }
        public string LastName { get { return last_input.value; } }
			
		protected void Awake ()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
		}

		protected void Start ()
		{
			// HACK to reposition dialog for any given aspect ratio
			AdjustDialogPositionToAspectRatio ();

			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement>()
				{
					{btn_galaxy_med, btn_ok}
				};
				
			btn_galaxy_med.clickDownCallback += ClickInit;
				
			first_input.valueChangeCallback += UpdateTextField;
			last_input.valueChangeCallback += UpdateTextField;
		}

		private void AdjustDialogPositionToAspectRatio()
		{
			// HACK: adjust name dialog position to account for iPad aspect ratio
			Double aspectRatio = System.Math.Round((Screen.width / (Double)Screen.height), 1);
			Double iPadAspectRatio = System.Math.Round((4 / 3D), 1);		// 1.3
			float posY = aspectRatio > iPadAspectRatio ? 0.5f : 0f;
			tutorial_register_name_parts.setPosition (new Vector2 (1, posY));
		}
			
		private void UpdateTextField(iGUIElement caller)
		{
            btn_ok.setEnabled(HasValidNameData());
		}
			
		private bool HasValidNameData()
		{
			return ((!string.IsNullOrEmpty(last_input.value)) && (!string.IsNullOrEmpty(first_input.value)));
		}
			
		//TODO Add in name validation stuff??
			
		private void ClickInit(iGUIElement element)
		{
			if ((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null)) {
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey, 0f);
			}
		}

        private bool HasInvalidCharacters()
        {
            string name = first_input.value + last_input.value;
            string accentedCharacters = @"àáâãÀÁÂÃèéêẽÈÉÊẼõÕçÇ\-";
            string validCharacters = @"a-zA-Z0-9 " + accentedCharacters;

            return Regex.IsMatch(name, @"[^" + validCharacters + "]+");
        }
			
		private void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if (isOverButton)
            {
				if (button == btn_galaxy_med)
                {
                    if (HasInvalidCharacters())
                    {
                        errorMsg.label.text = "Your name contains unsupported characters";
                        errorMsg.setEnabled(true);
                    }
                    else
                    {
                        SubmitResponse((int)DialogResponse.OK);
                    }
				}
			}
				
			_buttonArtMap[button].colorTo(Color.white, 0.3f);
		}	

		public override void MakePassive(bool value)
		{
			base.MakePassive(value);

			first_input.passive = value;
			last_input.passive = value;
		}
	}
}