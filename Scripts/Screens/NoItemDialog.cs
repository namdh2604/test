using iGUI;
using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Screens
{
	public class NoItemDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUIButton btn_popup_close, btn_shop;

		[HideInInspector]
		public iGUILabel main_header_label;

		private string _category;
		IGUIHandler _buttonHandler;

		public void SetCategory(string category)
		{
			_category = category;
		}

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
			btn_shop.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit;

			var text = main_header_label.label.text;
			text = text.Replace("CAT", _category.ToUpper());
			main_header_label.label.text = text;
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

		void HandleMovedAway(iGUIButton button)
		{
			button.colorTo(Color.white,0.3f);
		}

		void HandleMovedBack(iGUIButton button)
		{
			button.colorTo(Color.grey,0f);
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_shop)
				{
					SubmitResponse((int)NoItemDialogResponse.GO_TO_SHOP);
				}
				else if(button == btn_popup_close)
				{
					SubmitResponse((int)NoItemDialogResponse.CLOSE);
				}
			}

			button.colorTo(Color.white,0.3f);
		}
	}

	public enum NoItemDialogResponse
	{
		CLOSE = 0,
		GO_TO_SHOP = 1
	}
}