using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Events;
using Voltage.Witches.Controllers;
using Voltage.Witches.Models;
using Voltage.Witches.Views;

namespace Voltage.Witches.Screens
{
	using Debug = UnityEngine.Debug;

	public class StoryCompleteScreen : BaseScreen 
	{
		[HideInInspector]
		public iGUIButton btn_again;

		[HideInInspector]
		public iGUIContainer btn_play_again;

		StoryCompleteScreenController _controller;

		IGUIHandler _buttonHandler;

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
			btn_again.clickDownCallback += ClickInit;
		}

		public void Init(Player player,StoryCompleteScreenController controller)
		{
			_controller = controller;
		}
		
		protected override IScreenController GetController()
		{
			return _controller;
		}
		
		public void SetEnabled(bool value)
		{
			screenFrame.setEnabled(value);
			gameObject.SetActive(value);
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
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
				if(button == btn_again)
				{
					_controller.GoBackHome ();
				}
			}

			button.getTargetContainer().colorTo(Color.white, 0.3f);
		}
	}
}