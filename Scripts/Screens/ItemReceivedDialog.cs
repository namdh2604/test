using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Screens
{
	public class ItemReceivedDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIContainer ingredients_grp,ei_grp,starstone_grp,coin_grp,potion_grp,avatar_grp;

		[HideInInspector]
		public iGUIImage surprint_wardrobe,surprint_potion,surprint_coin,surprint_starstone,surprint_ei,surprint_ingredients,go_to_closet_text,
						 go_to_inventory_text_potions,ok_text_coin,ok_text_stone,go_to_glossary_text,go_to_inventory_text_ingredients;

		[HideInInspector]
		public iGUIButton btn_close,btn_center;

		private enum LayoutType
		{
			INGREDIENT = 0,
			EI = 1,
			STARSTONE = 2,
			COIN = 3,
			POTION = 4,
			AVATAR =5
		}

		LayoutType _myLayout;
		Item _myItem;
		iGUIImage _activeButtonText;

		IGUIHandler _buttonHandler;

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected virtual void Start()
		{
			ToggleContainers();
			SetActiveText();
			Debug.Log("Send item to get added via server");
			btn_close.clickDownCallback += ClickInit;
			if(_myItem.Category != ItemCategory.ILLUSTRATION)
			{
				btn_center.clickDownCallback += ClickInit;
			}
		}

		public void SetItem(Item item)
		{
			_myItem = item;
			SetLayout();
		}

		void SetLayout()
		{
			switch(_myItem.Category)
			{
				case ItemCategory.CLOTHING:
					_myLayout = LayoutType.AVATAR;
					break;
				case ItemCategory.INGREDIENT:
					_myLayout = LayoutType.INGREDIENT;
					break;
				case ItemCategory.POTION:
					_myLayout = LayoutType.POTION;
					break;
				case ItemCategory.COINS:
					_myLayout = LayoutType.COIN;
					break;
				case ItemCategory.ILLUSTRATION:
					_myLayout = LayoutType.EI;
					break;
				case ItemCategory.STARSTONES:
					_myLayout = LayoutType.STARSTONE;
					break;
			}
		}

		void ToggleContainers()
		{
			var isIngredient = (_myLayout == LayoutType.INGREDIENT);
			var isEI = (_myLayout == LayoutType.EI);
			var isStarstone = (_myLayout == LayoutType.STARSTONE);
			var isCoin = (_myLayout == LayoutType.COIN);
			var isPotion = (_myLayout == LayoutType.POTION);
			var isAvatar = (_myLayout == LayoutType.AVATAR);

			ingredients_grp.setEnabled(isIngredient);
			surprint_ingredients.setEnabled(isIngredient);
			ei_grp.setEnabled(isEI);
			surprint_ei.setEnabled(isEI);
			starstone_grp.setEnabled(isStarstone);
			surprint_starstone.setEnabled(isStarstone);
			coin_grp.setEnabled(isCoin);
			surprint_coin.setEnabled(isCoin);
			potion_grp.setEnabled(isPotion);
			surprint_potion.setEnabled(isPotion);
			avatar_grp.setEnabled(isAvatar);
			surprint_wardrobe.setEnabled(isAvatar);

			if((isStarstone) || (isCoin))
			{
				btn_close.setEnabled(false);
			}

			if(isEI)
			{
				btn_center.setColor(Color.grey);
			}
		}

		void SetActiveText()
		{
			switch(_myLayout)
			{
				case LayoutType.AVATAR:
					_activeButtonText = go_to_closet_text;
					break;
				case LayoutType.COIN:
					_activeButtonText = ok_text_coin;
					break;
				case LayoutType.EI:
					_activeButtonText = go_to_glossary_text;
					break;
				case LayoutType.INGREDIENT:
					_activeButtonText = go_to_inventory_text_ingredients;
					break;
				case LayoutType.POTION:
					_activeButtonText = go_to_inventory_text_potions;
					break;
				case LayoutType.STARSTONE:
					_activeButtonText = ok_text_stone;
					break;
			}
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				button.colorTo(Color.grey,0f);
				if((_activeButtonText != null) && (button != btn_close))
				{
					_activeButtonText.colorTo(Color.grey,0f);
				}
			}
		}

		void HandleMovedAway(iGUIButton button)
		{
			button.colorTo(Color.white, 0.3f);
			if((_activeButtonText != null) && (button != btn_close))
			{
				_activeButtonText.colorTo(Color.white,0.3f);
			}
		}

		void HandleMovedBack(iGUIButton button)
		{
			button.colorTo(Color.grey,0f);
			if((_activeButtonText != null) && (button != btn_close))
			{
				_activeButtonText.colorTo(Color.grey,0f);
			}
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_close)
				{
					SubmitResponse((int)ItemReceivedResponse.CLOSE);
				}
				else if(button == btn_center)
				{
					switch(_myLayout)
					{
					case LayoutType.AVATAR:
						SubmitResponse((int)ItemReceivedResponse.GO_TO_CLOSET);
						break;
					case LayoutType.COIN:
						SubmitResponse((int)ItemReceivedResponse.CLOSE);
						break;
					case LayoutType.EI:
						SubmitResponse((int)ItemReceivedResponse.GO_TO_GLOSSARY);
						break;
					case LayoutType.INGREDIENT:
						SubmitResponse((int)ItemReceivedResponse.GO_TO_INVENTORY);
						break;
					case LayoutType.POTION:
						SubmitResponse((int)ItemReceivedResponse.GO_TO_INVENTORY);
						break;
					case LayoutType.STARSTONE:
						SubmitResponse((int)ItemReceivedResponse.CLOSE);
						break;
					}
				}
			}

			button.colorTo(Color.white, 0.3f);
			if((_activeButtonText != null) && (button != btn_close))
			{
				_activeButtonText.colorTo(Color.white,0.3f);
			}
		}
	}

	public enum ItemReceivedResponse
	{
		CLOSE = 0,
		GO_TO_INVENTORY = 1,
		GO_TO_GLOSSARY = 2,
		GO_TO_CLOSET = 3
	}
}