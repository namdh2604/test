using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Screens
{
	public class AvatarPartBuyCompleteDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUILabel avatarname_text, unlock_text_1;

		[HideInInspector]
		public iGUIContainer main_screen_finish_buying_1,main_screen_finish_buying_2,prompt_button_container,btn_left,btn_right;

		[HideInInspector]
		public iGUIImage mailbox_text, wear_text;

		[HideInInspector]
		public iGUIImage avatar_parts_MA;

		[HideInInspector]
		public iGUIButton prompt_button, left_button, right_button;

		private bool _hasSpace;
		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		IGUIHandler _buttonHandler;

		public IClothing AvatarItem { get; protected set; }

		public void SetUpDialog(IClothing avatarItem, bool spaceAvailable)
		{
			AvatarItem = avatarItem;
			_hasSpace = spaceAvailable;
		}

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected void Start()
		{
			main_screen_finish_buying_1.setEnabled(_hasSpace);
			main_screen_finish_buying_2.setEnabled((!_hasSpace));
			
			mailbox_text.setEnabled((!_hasSpace));
			wear_text.setEnabled(_hasSpace);
			
			avatarname_text.label.text = AvatarItem.Name;
			unlock_text_1.label.text = AvatarItem.Name;
			
			SetUpImage();
			SetButtonMap();
			AssignCallbacks();
		}
		
		void SetUpImage()
		{
			avatar_parts_MA.setPositionAndSize(new Rect (0.5f, 0.5f, 1f, 1f));
			var texture = Resources.Load<Texture2D>(AvatarItem.IconFilePath);
			if(texture != null)
			{
				var sizes = new Vector2 (texture.width, texture.height);
				var rect = new Rect(avatar_parts_MA.positionAndSize);
				var evenlySized = (sizes.x == sizes.y);
				if(!evenlySized)
				{
					rect = GetScaledRect(sizes,rect);
				}
				
				avatar_parts_MA.image = texture;
				avatar_parts_MA.setPositionAndSize(rect);
				avatar_parts_MA.setEnabled(true);
			}
			else
			{
				avatar_parts_MA.setEnabled(false);
			}
		}

		Rect GetScaledRect(Vector2 sizes, Rect rect)
		{
			var width = rect.width;
			var height = rect.height;
			var widthIsBigger = (sizes.x > sizes.y) ? true : false;
			if(widthIsBigger)
			{
				height = (sizes.y / sizes.x);
			}
			else
			{
				width = (sizes.x / sizes.y);
			}
			
			rect.width = width;
			rect.height = height;
			return rect;
		}

		void SetButtonMap()
		{
			_buttonArtMap = new Dictionary<iGUIButton,iGUIElement> ()
			{
				{prompt_button,prompt_button_container},
				{left_button,btn_left},
				{right_button,btn_right}
			};
		}

		void AssignCallbacks()
		{
			prompt_button.clickDownCallback += ClickInit;
			left_button.clickDownCallback += ClickInit;
			right_button.clickDownCallback += ClickInit;
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
				if(button == prompt_button)
				{
					main_screen_finish_buying_1.setEnabled(true);
					main_screen_finish_buying_2.setEnabled(false);
				}
				else if(button == left_button)
				{
					SubmitResponse((int)AvatarPurchasedResponse.CLOSE);
				}
				else if(button == right_button)
				{
					if(!_hasSpace)
					{
						SubmitResponse((int)AvatarPurchasedResponse.MAILBOX);
					}
					else
					{
						SubmitResponse((int)AvatarPurchasedResponse.WEAR);
					}
				}
			}

			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}
	}

	public enum AvatarPurchasedResponse
	{
		WEAR = 0,
		MAILBOX = 1,
		CLOSE = 2
	}
}