
using System;
using System.Collections.Generic;
using Voltage.Witches.Screens;
using Voltage.Witches.Controllers;
using UnityEngine;
using Voltage.Witches.Views;
using System.Collections;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.UI
{
	using Voltage.Witches.Models;
	using Voltage.Witches.Shop;
	
	public class UIRibbonController 
    {
		private IUIRibbonView _screen;

		private readonly Player _player;
		private readonly IShopDialogueController _shopDialogueController;
		private int _staminaCount = 0;
		private int _focusCount = 0;
		private MasterConfiguration _masterConfig;

        private readonly IScreenFactory _screenFactory;

		public UIRibbonController (Player player, IShopDialogueController shopDialogueController, IScreenFactory factory, MasterConfiguration masterConfig, IUIRibbonView view=null)
		{
			if(player == null || shopDialogueController == null)
			{
				throw new ArgumentNullException();
			}

            _screenFactory = factory;

            _shopDialogueController = shopDialogueController;
            _player = player;
            _masterConfig = masterConfig;


			if(view != null)
			{
				_screen = view;
//				InitializeScreen ();
			}
			else
			{
				// FIXME: the convention I believe is to create views on Show()
				_screen = _screenFactory.GetScreen<UIRibbonView> ();
				if(_screen == null)
				{
					throw new ArgumentNullException();
				}
			}

            InitializeScreen ();
		}

		private void InitializeScreen()
		{
            _screen.Init(isOpen: false);

			_screen.SetStamina (_player.Stamina);
			_screen.SetFocus (_player.Focus);

			_screen.SetPremiumCurrency (_player.CurrencyPremium);
			_screen.SetRegularCurrency (_player.Currency);

			HandlePlayerStaminaUpdated (this, new EventArgs ());
			HandlePlayerFocusUpdated (this, new EventArgs ());
			Subscribe ();
		}

        public virtual void Dispose()
        {
            UnSubscribe();

			if(_shopDialogueController != null)
			{
				_shopDialogueController.Dispose();
			}
			if (_screen != null) 
			{
				_screen.Dispose ();
				_screen = null;
			}
        }

        public virtual void Show()
        {
            // create view if it doesn't exist, DOESN'T support views passed in thru constructor
//            if (_screen == null)
//            {
//                _screen = _screenFactory.GetScreen<UIRibbonView> ();
//                InitializeScreen ();
//            }

            if (_screen != null)
            {
                _screen.Show();
            }
        }

        public virtual void Hide()
        {
            if (_screen != null)
            {
                _screen.Hide();
            }
        }

		private void Subscribe()
		{
			_screen.OnShopButtonSelected += HandleOnShopButtonSelected;
			_player.CurrencyUpdate += HandlePlayerRegularCurrencyUpdated;
			_player.CurrencyUpdate += HandlePlayerPremiumCurrencyUpdated;
			_player.StaminaUpdate += HandlePlayerStaminaUpdated;
			_player.FocusUpdate += HandlePlayerFocusUpdated;
			_screen.OnRibbonEnabled += HandleOnRibbonEnabled;
		}

		private void UnSubscribe()
		{
			_player.CurrencyUpdate -= HandlePlayerRegularCurrencyUpdated;
			_player.CurrencyUpdate -= HandlePlayerPremiumCurrencyUpdated;
			_player.StaminaUpdate -= HandlePlayerStaminaUpdated;
			_player.FocusUpdate -= HandlePlayerFocusUpdated;

			if(_screen != null) 
			{
				_screen.OnShopButtonSelected -= HandleOnShopButtonSelected;
				_screen.OnRibbonEnabled -= HandleOnRibbonEnabled;
			}
		}

		public void EnableToggleButton (bool enable)
		{
            if (_screen != null)
            {
                _screen.EnableToggleButtonInput (enable);
            }
		}

		public void EnableShopButton (bool enable)
		{
            if (_screen != null)
            {
                _screen.EnableShopButtonInput (enable);
            }
		}


		private void HandleOnShopButtonSelected ()
		{
            if (_screen != null)
            {
                _screen.EnableShopButtonInput (false);
                _screen.EnableToggleButtonInput (false);
            }

			_shopDialogueController.Show (ShopDisplayType.STARSTONES, () => 
              	{
                    if (_screen != null)
                    {
                        _screen.MakePassive(false);                 // HACK: need to resolve the interface for enabling/disabling ribbon buttons w/ support for passive and disabling while animating
                        _screen.EnableShopButtonInput(true);
                        _screen.EnableToggleButtonInput (true);     
                    }

					if(OnShopClosed != null)
					{
						OnShopClosed();
					}
				}	
			);
		}

		private void HandleOnRibbonEnabled ()
		{
			RecalculateStamina ();
			RecalculateFocus ();
		}
		
		private void HandlePlayerRegularCurrencyUpdated (object sender, EventArgs e)
		{
            if (_screen != null)
            {
                _screen.SetRegularCurrency (_player.Currency);
            }
		}

		private void HandlePlayerPremiumCurrencyUpdated (object sender, EventArgs e)
		{
            if (_screen != null)
            {
                _screen.SetPremiumCurrency (_player.CurrencyPremium);
            }
		}


		private bool _showStaminaTimer = true;

		public void ShowStaminaTimer(bool value)
		{
			_showStaminaTimer = value;
		}


		private void RecalculateStamina()
		{

			_staminaCount = _player.Stamina;
			_screen.SetStamina (_staminaCount);
			
			if (_staminaCount < _masterConfig.Max_Tickets && _showStaminaTimer) 
			{
				_screen.SetNextUpdate(CountDownType.STAMINA, _player.StaminaNextUpdate);
			}
			else
			{
				_screen.HideStaminaTimer();
			}
		}

		private void HandlePlayerStaminaUpdated (object sender, EventArgs e)
		{
			RecalculateStamina ();
		}

		private void RecalculateFocus()
		{
			_focusCount = _player.Focus;
			_screen.SetFocus (_focusCount);
			
			if (_focusCount < _masterConfig.Max_Focus) 
			{
				_screen.SetNextUpdate(CountDownType.FOCUS, _player.FocusNextUpdate);
			}
			else
			{
				_screen.HideFocusTimer ();
			}
		}

		private void HandlePlayerFocusUpdated (object sender, EventArgs e)
		{
			RecalculateFocus ();
		}

        public void MakePassive(bool value)
        {
            if (_screen != null)
            {
                _screen.MakePassive(value);
            }
        }

		public void HideStaminaTimer()
		{
			_screen.HideStaminaTimer ();
		}

		public event Action OnOpenEvent
		{
			add 
			{
				_screen.OnOpenRibbon += value;
			}
			remove
			{
				_screen.OnOpenRibbon -= value;
			}
		}

		public event Action OnCloseEvent
		{
			add 
			{
				_screen.OnCloseRibbon += value;
			}
			remove
			{
				_screen.OnCloseRibbon -= value;
			}
		}

		public event Action OnShopOpen
		{
			add
			{
				_screen.OnShopButtonSelected += value;
			}
			remove
			{
				if(_screen != null) 
				{
					_screen.OnShopButtonSelected -= value;
				}
			}
		}

		public event Action OnShopClosed;
		


		public void OpenRibbon()								// would prefer this to be in a tutorial specific ribbon controller
		{
			_screen.OpenRibbon ();
		}

		public void CloseRibbon()								// would prefer this to be in a tutorial specific ribbon controller
		{
			_screen.CloseRibbon ();
		}

		public void ManuallySetStamina(int staminaCount)		// would prefer this to be in a tutorial specific ribbon controller
		{
			_screen.SetStamina (staminaCount);
		}


	}
	
}




