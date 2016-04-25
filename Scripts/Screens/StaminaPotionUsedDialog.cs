using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Screens
{
	public class StaminaPotionUsedDialog : AbstractDialog 
	{
		[HideInInspector]
		public iGUIButton btn_galaxy_med;

		[HideInInspector]
		public iGUIImage ok_text;

		[HideInInspector]
		public iGUILabel refill_stamina_potion_label;

		IGUIHandler _buttonHandler;

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
			ok_text.passive = true;
			btn_galaxy_med.clickDownCallback += ClickInit;
		}

		void ClickInit(iGUIElement element)
		{
			if(_buttonHandler.IsActive)
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				button.getTargetContainer().colorTo(Color.grey, 0f);
			}
		}

		void HandleMovedAway(iGUIButton button)
		{
			button.getTargetContainer().colorTo(Color.white, 0.3f);
		}

		void HandleMovedBack(iGUIButton button)
		{
			button.getTargetContainer().colorTo(Color.grey, 0f);
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				SubmitResponse((int)DialogResponse.OK);
			}

			button.getTargetContainer().colorTo(Color.white, 0.3f);
		}
	}
}