using System;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;

namespace Voltage.Witches.Controllers
{
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Shop;

	public class ShopListScreenController : ScreenController
	{
		private IScreenFactory _factory;
		private Player _player;

		private iGUISmartPrefab_ShopListScreen _screen;

		private MasterConfiguration _masterConfig;
		private IControllerRepo _repo;

		public ShopController ShopController { get; protected set; }

		private ShopItemData _iapItem;
		private IDialog _completingPurchase;
		private iGUISmartPrefab_CurrencyPurchaseDialog _iapDialog;

		public ShopListScreenController(ScreenNavigationManager controller, IScreenFactory factory, Player player, IControllerRepo repo, MasterConfiguration masterConfig): base(controller)
		{
			_factory = factory;
			_player = player;

			_masterConfig = masterConfig;

			_repo = repo;

			ShopController = _repo.Get<ShopController>();

			InitializeView();
		}

        public override void Dispose()
        {
            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
        }

		protected override IScreen GetScreen()
		{
			if(_screen != null)
			{
				return _screen;
			}
			else
			{
				_screen = _factory.GetScreen<iGUISmartPrefab_ShopListScreen>();
				_screen.Init(_player, this);
				return _screen;
			}
		}

		public override void MakePassive(bool value)
		{
			_screen.MakePassive(value);
		}

		void InitializeView()
		{
			_screen = _factory.GetScreen<iGUISmartPrefab_ShopListScreen>();
			_screen.Init(_player, this);
		}

		public void SetEnabled(bool value)
		{
			_screen.SetEnabled(value);
		}
		
		public void Unload()
		{
			_screen.SetEnabled(false);
		}

		public void GoHome()
		{
			Manager.GoToExistingScreen("/Home");
		}

		public void GoToAvatarShop()
		{
            IScreenController nextScreen = _repo.Get<AvatarShopScreenController>();
			Manager.Add(nextScreen);
		}

		public IDialog GetStarstoneShopDialog()
		{
			var currencyPurchaseDialogue = _factory.GetDialog<iGUISmartPrefab_CurrencyPurchaseDialog>();
			currencyPurchaseDialogue.ChangeActiveState(0);
			currencyPurchaseDialogue.Init (_masterConfig.Shop_Items);
			_iapDialog = currencyPurchaseDialogue as iGUISmartPrefab_CurrencyPurchaseDialog;
			return currencyPurchaseDialogue;
		}

		public IDialog GetStaminaShopDialog()
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_CurrencyPurchaseDialog>();
			dialog.ChangeActiveState(1);
			dialog.Init(_masterConfig.Shop_Items);
			_iapDialog = dialog as iGUISmartPrefab_CurrencyPurchaseDialog;
			return dialog;
		}

		public IDialog GetPrePurchaseLoadingDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_ConfirmPurchaseLoadDialog>();
		}
		
		public IDialog GetPostPurchaseLoadingDialog()
		{
			return _factory.GetDialog<iGUISmartPrefab_ConfirmCompleteLoadDialog>();
		}
		
		public IDialog GetSystemDialog(string message)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialog.SetText (message);
			return dialog;
		}
		
		public void AddItemToPlayer(ShopItemData item)
		{
			if(!item.name.Contains("Stamina"))
			{
				UnityEngine.Debug.LogWarning("Add " + item.premium_qty.ToString() + " starstones to the player");
                _player.UpdatePremiumCurrency(item.premium_qty);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Add " + item.premium_qty.ToString() + " stamina potions once it has been included properly");
				_player.UpdateStaminaPotion(item.premium_qty);
			}
		}
		
		void HandleSystemDialogResponse(int answer)
		{
			UnityEngine.Debug.Log("Close system dialog");
			if(_iapDialog != null)
			{
				_iapDialog.ToggleActiveElements(true);
			}
		}
		
		public void InitiatePremiumTransaction(ShopItemData itemData)
		{
			_iapDialog.ToggleActiveElements(false);
			_iapItem = itemData;
			_completingPurchase = GetPostPurchaseLoadingDialog();
			_completingPurchase.Display(null);
			(_completingPurchase as iGUISmartPrefab_ConfirmCompleteLoadDialog).BeginLoading();
			Action<bool> responseHandler = delegate(bool obj) { CompleteTransaction(obj); };
			ShopController.InitiatePremiumPurchase(_iapItem,responseHandler);
		}
		
		void CompleteTransaction(bool isSuccessful)
		{
			(_completingPurchase as iGUISmartPrefab_ConfirmCompleteLoadDialog).EndLoading();
			_completingPurchase = null;
			if(isSuccessful)
			{
				var dialog = GetSystemDialog("Thank you for your business!");
				dialog.Display(HandleSystemDialogResponse);
				AddItemToPlayer(_iapItem);
			}
			else
			{
				UnityEngine.Debug.LogWarning("There was a major fail in your purchase");
				var dialog = GetSystemDialog("Sorry, there was an error in your purchase");
				dialog.Display(HandleSystemDialogResponse);
			}
			_iapItem = null;
		}
	}
}