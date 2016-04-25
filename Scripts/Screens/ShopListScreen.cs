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

	public class ShopListScreen : BaseScreen
	{
		Player _player;
		ShopListScreenController _controller;

		[HideInInspector]
		public Placeholder interfaceShell;

		[HideInInspector]
		public iGUIButton home_button,starstone_btn,stamina_btn,ingredientsshop_btn,avatarshop_btn,
						  ingredientsgacha_btn,avatargacha_btn;

		[HideInInspector]
		public List<iGUIButton> pressableButtons;

		private Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		private iGUISmartPrefab_InterfaceShell _interface;

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
			_interface = LoadPlaceholder().GetComponent<iGUISmartPrefab_InterfaceShell>();
			_interface.SetLayout(InterfaceLayout.Home_Button_Only);
			_interface.SetCounts(_player);
			_interface.BeginCountDown(CountDownType.FOCUS, _player.FocusNextUpdate);
			_interface.BeginCountDown(CountDownType.STAMINA, _player.StaminaNextUpdate);

			pressableButtons = new List<iGUIButton>()
			{
				_interface.btn_home,starstone_btn,stamina_btn,avatarshop_btn
			};

			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement> ()
			{
				{_interface.btn_home,_interface.home},
				{starstone_btn,starstone_btn},
				{stamina_btn,stamina_btn},
				{avatarshop_btn,avatarshop_btn}
			};

			ConfigureButtons();

			//TODO Remove this once the ingredients shop is to be implemented
			{
				ingredientsshop_btn.style.normal.background = ingredientsgacha_btn.style.normal.background;
				ingredientsshop_btn.setRotation(13f);
			}
		}

		public override void MakePassive (bool value)
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

		iGUIElement LoadPlaceholder()
		{
			var element = interfaceShell.SwapForSmartObject() as iGUIContainer;
			element.setLayer(15);
			return element;
		}

		void ConfigureButtons()
		{
			for(int i = 0; i < pressableButtons.Count; ++i)
			{
				var button = pressableButtons[i];
				button.clickDownCallback += ClickInit;
			}
		}

		public void Init(Player player, ShopListScreenController controller)
		{
			_player = player;
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

		void HandleCurrencyDialogResponse(int answer)
		{
			_buttonHandler.Activate();
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

		void HandleReleasedButtonEvent(iGUIButton pressedButton, bool isOverButton)
		{
			if(isOverButton)
			{
				if(pressedButton == _interface.btn_home)
				{
					_controller.GoHome();
				}
				else if(pressedButton == avatarshop_btn)
				{
					_controller.GoToAvatarShop();
				}
				else if(pressedButton == stamina_btn)
				{
					IDialog dialog = _controller.GetStaminaShopDialog();
					(dialog as iGUISmartPrefab_CurrencyPurchaseDialog).OnPurchaseRequest += HandlePremiumTransaction;
					dialog.Display(HandleCurrencyDialogResponse);
					_buttonHandler.Deactivate();
				}
				else if(pressedButton == starstone_btn)
				{
					IDialog dialog = _controller.GetStarstoneShopDialog();
					(dialog as iGUISmartPrefab_CurrencyPurchaseDialog).OnPurchaseRequest += HandlePremiumTransaction;
					dialog.Display(HandleCurrencyDialogResponse);
					_buttonHandler.Deactivate();
				}
				else if(pressedButton == ingredientsshop_btn)
				{
					//TODO go to the ingredients shop??
				}
				else if(pressedButton == ingredientsgacha_btn)
				{
					//TODO make the call happen for gacha
				}
				else if(pressedButton == avatargacha_btn)
				{
					//TODO make the call happen for gacha
				}
			}
			_buttonArtMap[pressedButton].colorTo(Color.white,0.3f);
		}
		
		void HandleMovedBack(iGUIButton pressedButton)
		{
			_buttonArtMap[pressedButton].colorTo(Color.grey,0f);
		}
		
		void HandleMovedAway(iGUIButton pressedButton)
		{
			_buttonArtMap[pressedButton].colorTo(Color.white,0.3f);
		}
		
		void HandlePremiumTransaction (object sender, GUIEventArgs e)
		{
			Debug.Log(sender.ToString() + " made request");
			PremiumPurchaseRequestEventArgs requestArgs = e as PremiumPurchaseRequestEventArgs;
			Debug.Log(requestArgs.Shop_Item.name);
			
			_controller.InitiatePremiumTransaction(requestArgs.Shop_Item);
		}
	}
}