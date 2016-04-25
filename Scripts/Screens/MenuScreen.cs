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

	public class MenuScreen : BaseScreen
	{
		[HideInInspector]
		public iGUIImage news_img,inventory_img,mail_img,glossary_img,login_bonus_img,ranking_img,options_img;

		[HideInInspector]
		public iGUIButton home_button,news,inventory,mail,glossary,login_bonus,ranking,options;

		[HideInInspector]
		public Placeholder interfaceShell;

		private iGUISmartPrefab_InterfaceShell _interface;
		Player _player;
		MenuScreenController _controller;

		IGUIHandler _buttonHandler;

		Dictionary<string,iGUIImage> _buttonArtMap;

		public void Init(Player player, MenuScreenController controller)
		{
			_player = player;
			_controller = controller;
			Debug.Log(_player.FullName);
		}

		public override void MakePassive(bool value)
		{
			base.MakePassive(value);

			if (value) 
			{
				_buttonHandler.Deactivate ();
			} 
			else 
			{
				_buttonHandler.Activate ();
			}
		}


		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		void Start()
		{
			_interface = LoadPlaceholder().GetComponent<iGUISmartPrefab_InterfaceShell>();
			_interface.SetLayout(Voltage.Witches.Views.InterfaceLayout.Home_Button_Only);

			_buttonArtMap = new Dictionary<string, iGUIImage> ()
			{
				{news.name,news_img},
				{inventory.name,inventory_img},
				{mail.name,mail_img},
				{glossary.name,glossary_img},
				{login_bonus.name,login_bonus_img},
				{ranking.name,ranking_img},
				{options.name,options_img},
				{_interface.btn_home.name,_interface.home}
			};

			{
				news.setEnabled(false);
				news_img.setEnabled(false);
				login_bonus.setEnabled(false);
				login_bonus_img.setEnabled(false);
				ranking.setEnabled(false);
				ranking_img.setEnabled(false);
				glossary.setEnabled(false);
				glossary_img.setEnabled(false);
			}

			_interface.btn_home.clickDownCallback += ClickInit;
			inventory.clickDownCallback += ClickInit;
			mail.clickDownCallback += ClickInit;
			glossary.clickDownCallback += ClickInit;
			options.clickDownCallback += ClickInit;
		}

		iGUIElement LoadPlaceholder()
		{
			var element = interfaceShell.SwapForSmartObject() as iGUIContainer;
			element.setLayer(15);
			return element;
		}

		protected override IScreenController GetController()
		{
			return _controller;
		}

		void HandleReleasedButtonEvent(iGUIButton pressedButton, bool isOverButton)
		{
			if(isOverButton)
			{
				if(pressedButton == _interface.btn_home)
				{
					_controller.GoHome();
				}
				else if(pressedButton == inventory)
				{
					_controller.GoToInventory();
				}
				else if(pressedButton == mail)
				{
					_controller.GoToMailBox();
				}
				else if(pressedButton == glossary)
				{
					_controller.GoToGlossary();
				}
				else if(pressedButton == options)
				{
					IDialog dialog = _controller.GetOptionsDialog();
					dialog.Display(HandleOptionsClosed);
					_buttonHandler.Deactivate();
				}
				else if(pressedButton == news)
				{
					//TODO News shit
				}
				else if(pressedButton == login_bonus)
				{
					//TODO Login shit
				}
				else if(pressedButton == ranking)
				{
					//TODO Ranking shit
				}
			}

			var img = _buttonArtMap[pressedButton.name];
			img.colorTo(Color.white,0.3f);
		}
		
		void HandleMovedBack(iGUIButton pressedButton)
		{
			var img = _buttonArtMap[pressedButton.name];
			img.colorTo(Color.grey,0f);
		}
		
		void HandleMovedAway(iGUIButton pressedButton)
		{
			var img = _buttonArtMap[pressedButton.name];
			img.colorTo(Color.white,0.3f);
		}
		
		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				iGUIButton pressedButton = (iGUIButton)element;
				_buttonHandler.SelectButton(pressedButton);

				var img = _buttonArtMap[pressedButton.name];
				img.colorTo(Color.grey,0f);
			}
		}

		void HandleOptionsClosed(int answer)
		{
			_buttonHandler.Activate();
		}

		public void SetEnabled(bool value)
		{
			screenFrame.setEnabled(value);
			gameObject.SetActive(value);
		}
	}
}