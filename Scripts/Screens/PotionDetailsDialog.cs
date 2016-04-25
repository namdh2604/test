using System;
using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Screens
{
	public class PotionDetailsDialog : AbstractDialog
	{
		[HideInInspector]
		public Placeholder potionPlaceholder;

		[HideInInspector]
		public iGUIButton btn_use,close_button;

		[HideInInspector]
		public iGUIImage master_label,superior_label,basic_label,master_liquid,superior_liquid,basic_liquid;

		[HideInInspector]
		public iGUILabel potions_counter_label,Potion_description_label,Potion_Name_Header,game_effect_counter_6,
						 game_effect_counter_5,game_effect_counter_4,game_effect_counter_3, 
						 game_effect_counter_2,game_effect_counter_1; 

		iGUISmartPrefab_InventoryPotion _potionView;
		Potion _selectedPotion;
		int _count;

        private Dictionary<string, iGUILabel> _characterEffectsMap;
        private List<string> _validChars = new List<string>() { "T", "A", "M", "R", "N" };

		IGUIHandler _buttonHandler;

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		public void SetPotion(Potion potion, int count)
		{
            SetupCharacterEffectsMap();
			_selectedPotion = potion;
			_count = count;
		}

		protected void Start()
		{
			btn_use.clickDownCallback += ClickInit;
			close_button.clickDownCallback += ClickInit;

			SetName();
			SetIcon();
			SetCounter();
			SetEffects();
		}

        private void SetupCharacterEffectsMap()
        {
            _characterEffectsMap = new Dictionary<string, iGUILabel>() {
                { "T", game_effect_counter_1 },
                { "A", game_effect_counter_2 },
                { "M", game_effect_counter_3 },
                { "R", game_effect_counter_4 },
                { "N", game_effect_counter_5 }
            };
        }

		void SetName()
		{
			var potionName = _selectedPotion.Name;
			Potion_Name_Header.label.text = potionName;
			Potion_description_label.label.text = _selectedPotion.Description;
		}

		void SetIcon()
		{
			var element = potionPlaceholder.SwapForSmartObject() as iGUIContainer;
			_potionView = element.GetComponent<iGUISmartPrefab_InventoryPotion>();
			_potionView.SetPotion(_selectedPotion,_count);
			_potionView.potions_number_badge.setEnabled(false);
		}

		void SetCounter()
		{
			potions_counter_label.label.text = GetQuantityString();
			potions_counter_label.style.alignment = TextAnchor.MiddleCenter;
		}

		void SetEffects()
		{
			var effects = _selectedPotion.EffectList;

            foreach (var character in _validChars)
            {
                int value = effects.ContainsKey(character) ? Convert.ToInt32(effects[character]) : 0;
                string convertedValue = value.ToString();
                if (value >= 0)
                {
                    convertedValue = "+" + convertedValue;
                }
                _characterEffectsMap[character].label.text = convertedValue;
            }
		}

		string GetQuantityString()
		{	
			return _count.ToString();
		}

		string StripUnnecessaryHeader(string potionName)
		{
			if((!potionName.Contains("Master")) && (!potionName.Contains("Superior")))
			{
				return potionName;
			}
			
			if(potionName.Contains("Master"))
			{
				var returnName = potionName.Replace("Master ","");
				return returnName;
			}
			else
			{
				var returnName = potionName.Replace("Superior ","");
				return returnName;
			}
		}

		Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r,g,b, 255);
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
				if(button == btn_use)
				{
					SubmitResponse((int)PotionDetailResponse.USE);
				}
				else if(button == close_button)
				{
					SubmitResponse((int)PotionDetailResponse.CLOSE);
				}
			}

			button.colorTo(Color.white,0.3f);
		}
	}

	public enum PotionDetailResponse
	{
		BREW = 0,
		USE = 1,
		CLOSE =2
	}
}