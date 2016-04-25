using iGUI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Screens
{
	public class PotionUsedDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIButton btn_resume,btn_another;

		[HideInInspector]
		public iGUIImage nvr_heart_icon,nvr_heart_icon_2,tb_heart_icon,ap_heart_icon,mh_heart_icon,
						 rc_heart_icon;

		[HideInInspector]
		public iGUILabel game_effect_counter_1,game_effect_counter_2,game_effect_counter_3,
						 game_effect_counter_4,game_effect_counter_5,game_effect_counter_6,
						 new_affinity_counter_1,new_affinity_counter_2,new_affinity_counter_3,
						 new_affinity_counter_4,new_affinity_counter_5,new_affinity_counter_6;
		
		[HideInInspector]
		public iGUIContainer indicator,affinity_effect_1,affinity_effect_2,affinity_effect_3,
							 affinity_effect_4,affinity_effect_5,affinity_effect_6;

		private static Dictionary<string,iGUILabel> _effectLabelMap;
		private static Dictionary<string,iGUILabel> _newEffectLabelMap;
		private Dictionary<string,int> _currentPlayerValues; 

		public Potion UsedPotion { get; protected set; }

		IGUIHandler _buttonHandler;

		public void SetPotion(Potion potion)
		{
			UsedPotion = potion;
		}

		public void SetAffinityAndAlignment(Dictionary<string,int> playerValues)
		{
			_currentPlayerValues = playerValues;
		}

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
			SetUpLabelMappings();

			DisplayAffinityChanges();

			btn_resume.clickDownCallback += ClickInit;
			btn_another.clickDownCallback += ClickInit;
		}

		void SetUpLabelMappings()
		{
			_effectLabelMap = new Dictionary<string,iGUILabel>()
			{
				{"T",game_effect_counter_1},
				{"M",game_effect_counter_2},
				{"A",game_effect_counter_3},
				{"R",game_effect_counter_4},
				{"N",game_effect_counter_5},
			};
			
			_newEffectLabelMap = new Dictionary<string, iGUILabel> ()
			{
				{"T",new_affinity_counter_1},
				{"M",new_affinity_counter_2},
				{"A",new_affinity_counter_3},
				{"R",new_affinity_counter_4},
				{"N",new_affinity_counter_5},
			};
		}

		void DisplayAffinityChanges()
		{
//			var effects = UsedPotion.EffectList;
			var keys = _effectLabelMap.Keys.ToList();

			for(int i = 0; i < keys.Count; ++i)
			{
				var currentKey = keys[i];
				var label = _effectLabelMap[currentKey];
				label.style.alignment = TextAnchor.MiddleRight;
				label.refreshStyle();
				var totalLabel = _newEffectLabelMap[currentKey];
				totalLabel.setEnabled(false);
				var currentValue = _currentPlayerValues[currentKey];
				label.label.text = currentValue.ToString();
			}
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
				if(button == btn_resume)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
				else if(button == btn_another)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
			}
			
			button.colorTo(Color.white,0.3f);
		}
	}
}