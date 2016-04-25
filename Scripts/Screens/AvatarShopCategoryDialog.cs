using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Screens
{
	public class AvatarShopCategoryDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIButton button_01,button_02,button_03,button_04,button_05,button_06,button_07,button_08,button_09,button_10,all_button,drawer_front;

		IGUIHandler _buttonHandler;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;

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
			var pressableButtons = new iGUIButton[]{ button_01,button_02,button_03,button_04,button_05,button_06,button_07,button_08,button_09,button_10,all_button,drawer_front };

			AssignCallbacks(pressableButtons);
			SetButtonArtMap(pressableButtons);
		}

		void AssignCallbacks(iGUIButton[] pressableButtons)
		{
			for(int i = 0; i < pressableButtons.Length; ++i)
			{
				var button = pressableButtons[i];
				button.clickDownCallback += ClickInit;
			}
		}

		void SetButtonArtMap(iGUIButton[] pressableButtons)
		{
			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement>();

			for(int i = 0; i < pressableButtons.Length; ++i)
			{
				var button = pressableButtons[i];
				_buttonArtMap[button] = button.getTargetContainer();
			}
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				iGUIButton button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey,0f);
			}
		}
		
		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey,0f);
		}
		
		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
		
		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == button_01)
				{
					SubmitResponse((int)ShopCategory.HATS);
				}
				else if(button == button_02)
				{
					SubmitResponse((int)ShopCategory.SKIN);
				}
				else if(button == button_03)
				{
					SubmitResponse((int)ShopCategory.TOPS);
				}
				else if(button == button_04)
				{
					SubmitResponse((int)ShopCategory.BOTTOMS);
				}
				else if(button == button_05)
				{
					SubmitResponse((int)ShopCategory.SHOES);
				}
				else if(button == button_06)
				{
					SubmitResponse((int)ShopCategory.HAIRSTYLES);
				}
				else if(button == button_07)
				{
					SubmitResponse((int)ShopCategory.INTIMATES);
				}
				else if(button == button_08)
				{
					SubmitResponse((int)ShopCategory.JACKETS_COATS);
				}
				else if(button == button_09)
				{
					SubmitResponse((int)ShopCategory.DRESSES);
				}
				else if(button == button_10)
				{
					SubmitResponse((int)ShopCategory.ACCESSORIES);
				}
				else if(button == all_button)
				{
					SubmitResponse((int)ShopCategory.ALL);
				}
				else if(button == drawer_front)
				{
					SubmitResponse((int)ShopCategory.CLOSE);
				}
			}
			
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
	}

	public enum ShopCategory
	{
		ALL = 0,
		HATS = 1,
		HAIRSTYLES = 2,
		SKIN = 3,
		INTIMATES = 4,
		TOPS = 5,
		JACKETS_COATS = 6,
		BOTTOMS = 7,
		DRESSES = 8,
		SHOES = 9,
		ACCESSORIES = 10,
		CLOSE = 11,
	}
}