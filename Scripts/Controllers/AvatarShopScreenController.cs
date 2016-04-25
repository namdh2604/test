using System;
using System.Collections.Generic;
using UnityEngine;
using Voltage.Witches.Configuration.JSON;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;

using Voltage.Witches.Screens.AvatarShop;
using Voltage.Witches.Screens.Closet;
using Voltage.Witches.UI;
using Voltage.Witches.Shop;
using Voltage.Witches.Events;
using Voltage.Witches.Controllers.Factories;

namespace Voltage.Witches.Controllers
{
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Shop;

	using Voltage.Common.Metrics;
	using Voltage.Witches.Metrics;

	using Voltage.Witches.Screens.Dialogues;
	using Voltage.Witches.Bundles;

    public class AvatarShopItemViewModel
    {
        public Clothing Clothing { get; set; }
        public bool Owned { get; set; }
    }


	public class AvatarShopScreenController : ScreenController 
	{
		private IScreenFactory _screenFactory;
		private Player _player;
        private readonly Inventory _inventory;

        private NewAvatarShopScreen _screen;

        private Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> _clothing;

		private MasterConfiguration _masterConfig;
		private IControllerRepo _repo;

        private readonly ShopController _shopController;
        private readonly IShopDialogueController _shopDialogueController;

        private UIRibbonController _ribbonController;

        private List<AvatarShopItemViewModel> _purchaseableClothing;
        private int _availableClosetSpace;

		private readonly IClosetSorter _closetSorter;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;

		public AvatarShopScreenController(ScreenNavigationManager controller, IScreenFactory screenFactory, Player player, Inventory inventory, IControllerRepo repo, 
										  ShopController shopController, IShopDialogueController shopDialogueController, MasterConfiguration masterConfig, 
										  IClosetSorter closetSorter, 
										  IAvatarThumbResourceManager thumbResourceManager, // FIXME: this should be passed into the view, NOT the controller
										  ScreenClothingCategory activeCategory = ScreenClothingCategory.All) 		
		: base(controller)
		{
			_screenFactory = screenFactory;
			_player = player;
            _inventory = inventory;

			_masterConfig = masterConfig;
			_repo = repo;

			_shopController = shopController;
            _shopDialogueController = shopDialogueController;

			_closetSorter = closetSorter;
			_thumbResourceManager = thumbResourceManager;

			InitializeClothing(_closetSorter);
            UpdateAvailableSpace();

            IsLoaded = false;
			InitializeView(activeCategory);
            _ribbonController = new UIRibbonController(_player, _shopDialogueController, _screenFactory, masterConfig);
            _ribbonController.OnShopOpen += HandleRibbonPurchaseStart;
            _ribbonController.OnShopClosed += HandleRibbonPurchaseEnd;
		}

        private void HandleEvents(object source, EventArgs args)
        {
            ShopScreenEventArgs realArgs = args as ShopScreenEventArgs;
            if (realArgs.Type == "Home")
            {
                GoHome();
            }
            else if (realArgs.Type == "Closet")
            {
                GoToCloset();
            }
            else if (realArgs.Type == "Purchase")
            {
                MakePassive(true);
                ClothingPurchaseRequestArgs purchaseRequest = realArgs as ClothingPurchaseRequestArgs;
                var dialog = GetAvatarBuyDialog(purchaseRequest.Item);
                dialog.Display((response) => HandleItemPurchase(response, purchaseRequest.Item));
            }
        }

        void HandleItemPurchase(int answer, Clothing clothing)
        {
            PurchaseResponse response = (PurchaseResponse)answer;
            if ((response == PurchaseResponse.NormalPurchase) || (response == PurchaseResponse.PremiumPurchase))
            {
                if (_availableClosetSpace <= 0)
                {
                    HandleNotEnoughSpaceWhenPurchasing();
                }
                else
                {
                    bool isPremium = (response == PurchaseResponse.PremiumPurchase);
                    HandlePurchaseRequest(clothing, isPremium);
                }
            }
            else if (response == PurchaseResponse.Not_Enough)
            {
                HandleGetMoreStarStonesDialog();
            }
            else
            {
                // Request ended
                MakePassive(false);
            }
        }

        #region ScreenController
        public override void Show()
        {
            base.Show();
            _ribbonController.Show();
        }

        public override void Hide()
        {
            _ribbonController.Hide();
            base.Hide();
        }

        public override void Dispose()
        {
			if (_ribbonController != null) 
			{
				_ribbonController.OnShopOpen -= HandleRibbonPurchaseStart;
				_ribbonController.OnShopClosed -= HandleRibbonPurchaseEnd;
				_ribbonController.Dispose ();
				_ribbonController = null;
			}

            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _screen.OnAction -= HandleEvents;
                _screen.Dispose();
                _screen = null;
            }
        }

		protected override IScreen GetScreen()
		{
            return _screen;
		}

        #endregion

		public override void MakePassive(bool value)
		{
            if (_screen != null)
            {
                _screen.MakePassive(value);
            }

            if (_ribbonController != null)
            {
                _ribbonController.MakePassive(value);
            }
		}

        private void HandleRibbonPurchaseStart()
        {
            MakePassive(true);
        }

        private void HandleRibbonPurchaseEnd()
        {
            MakePassive(false);
        }

		private bool DoesNotHaveItem(Item avatarItem)
		{
			return (_inventory.GetCount(avatarItem) <= 0);
		}


		private void InitializeClothing(IClosetSorter sorter)
        {
            _purchaseableClothing = GetItemsFromMaster();
			_clothing = sorter.CreateClothingDictionaryFromViewModel(_purchaseableClothing);	// FIXME: pass in sorter
        }

		private void UpdateClothingOwnership(Clothing clothing, IClosetSorter sorter)
        {
            var viewModel = _purchaseableClothing.Find(value => value.Clothing == clothing);
            viewModel.Owned = true;

            List<AvatarShopItemViewModel> items = GetItemsFromMaster();
			_clothing = sorter.CreateClothingDictionaryFromViewModel(items);	
        }

		List<AvatarShopItemViewModel> GetItemsFromMaster()
		{
			ItemRawParser parser = new ItemRawParser(_masterConfig);
			var data = _masterConfig.Items_Master;
			var avatarItemList = new List<AvatarShopItemViewModel>();
			foreach(var pair in data)
			{
				var config = pair.Value;
				if(config.ItemCategory == ItemCategory.CLOTHING)
				{
					AvatarItemData item = config.Item as AvatarItemData;
                    Clothing clothing = parser.CreateAvatarItem(item) as Clothing;

                    // the shop is only interested in items that can be purchased
                    if (clothing.CurrencyType != PURCHASE_TYPE.NONE)
                    {
                        AvatarShopItemViewModel model = new AvatarShopItemViewModel();
                        model.Clothing = clothing;
                        model.Owned = (_inventory.GetCount(clothing) > 0);

    					avatarItemList.Add(model);
                    }
				}
			}

			return avatarItemList;
		}
		
		void InitializeView(ScreenClothingCategory activeCategory)
		{
            _screen = _screenFactory.GetScreen<NewAvatarShopScreen>();
			_screen.Init(this, _clothing, activeCategory, HandleScreenLoadComplete, _thumbResourceManager);
            _screen.OnAction += HandleEvents;
		}

        protected bool IsLoaded { get; private set; }

        private void HandleScreenLoadComplete()
        {
            IsLoaded = true;
        }

		private void UpdateAvailableSpace()
		{
			var allItems = _inventory.GetAllItemsByCategory(ItemCategory.CLOTHING);
            int itemCount = allItems.Count;

            _availableClosetSpace = _player.ClosetSpace - itemCount;
		}
			
		public IDialog GetPrePurchaseLoadingDialog()
		{
			return _screenFactory.GetDialog<iGUISmartPrefab_ConfirmPurchaseLoadDialog>();
		}
		
		public IDialog GetPostPurchaseLoadingDialog()
		{
			return _screenFactory.GetDialog<iGUISmartPrefab_ConfirmCompleteLoadDialog>();
		}

		public IDialog GetSystemDialog(string message)
		{
			var dialog = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialog.SetText(message);
			return dialog;
		}

		void HandleRealMoneyDialogResponse(int answer)
		{
            MakePassive(false);
		}

		void HandleSystemDialogResponse(int answer)
		{
            MakePassive(false);
		}
		

		private void CompleteStarstoneTransaction(bool isSuccessful, ShopItemData item, iGUISmartPrefab_ConfirmCompleteLoadDialog dialog)
		{
            dialog.EndLoading();
			if(isSuccessful)
			{
				var sysDialog = GetSystemDialog("Thank you for your business!");
				sysDialog.Display(HandleRealMoneyDialogResponse);
                _player.UpdatePremiumCurrency(item.premium_qty);
			}
			else
			{
				var purchaseErrorDialog = _screenFactory.GetDialog<iGUISmartPrefab_PurchaseErrorDialog>();
				purchaseErrorDialog.SetErrorMessage("Sorry, there was an error in your purchase");
				purchaseErrorDialog.Display(HandleRealMoneyDialogResponse);
			}
		}

        private void HandlePurchaseRequest(Clothing clothing, bool isPremium)
        {
            var dialog = GetPostPurchaseLoadingDialog() as iGUISmartPrefab_ConfirmCompleteLoadDialog;
            dialog.Display(null);
            dialog.BeginLoading();
            Action<bool> responseHandler = (success) => HandlePurchaseComplete(success, clothing, dialog, isPremium);
            if (isPremium)
            {
                _shopController.SpendPremium(clothing.Id, responseHandler);
            }
            else
            {
                _shopController.SpendCoins(clothing.Id, responseHandler);
            }
        }

        private void HandlePurchaseComplete(bool isSuccessful, Clothing clothing, iGUISmartPrefab_ConfirmCompleteLoadDialog dialog, bool isPremium)
        {
            dialog.EndLoading();
            if (isSuccessful)
            {
//                bool spaceAvailable = (_availableClosetSpace > 0);
//                var purchaseDialog = GetPartPurchasedDialog(clothing, spaceAvailable);
//                purchaseDialog.Display(HandleSuccessfulPurchaseResponse);

                if (isPremium)
                {
                    _player.UpdatePremiumCurrency(-clothing.PremiumPrice);
                    SendBoughtAvatarItemMetric(clothing, PurchaseType.PREMIUM);
                }
                else
                {
                    _player.UpdateCurrency(-clothing.CoinPrice);
                    SendBoughtAvatarItemMetric(clothing, PurchaseType.COIN);
                }

//                if (spaceAvailable)
//                {
                    AddClothingToInventory(clothing);
//                }

                // update the clothing listing
				UpdateClothingOwnership(clothing, _closetSorter);
                _screen.RefreshItemOwnership();


                // this ends the request -- note that the previous option,
                // where the user had the ability to go to the closet screen,
                // also could send them to the mailbox if they had no closet space -- this may need to be reimplemented,
                // in which case the make passive call would need to be moved
                MakePassive(false);
            }
            else
            {
                var sysDialog = GetSystemDialog("Sorry, there was an error in your purchase");
                sysDialog.Display(HandleSystemDialogResponse);
            }
        }

//        private void HandleSuccessfulPurchaseResponse(int answer)
//        {
//            switch((AvatarPurchasedResponse)answer)
//            {
//                case AvatarPurchasedResponse.MAILBOX:
//                    GoToMailbox();
//                    break;
//                case AvatarPurchasedResponse.WEAR:
//                    GoToCloset();
//                    break;
//            }
//
//            MakePassive(false);
//        }

		public IDialog GetCategorySelectDialog()
		{
			return _screenFactory.GetDialog<iGUISmartPrefab_AvatarShopCategoryDialog>();
		}

		public IDialog GetAvatarBuyDialog(Clothing avatarItem)
		{
			SendDisplayDialogueBuyAvatarItemMetric (avatarItem);

			var dialog = _screenFactory.GetDialog<AvatarClothingBuyDialogue>();
            dialog.Hide();

			bool canBuy = DoesNotHaveItem((avatarItem as Item));
			dialog.Init(avatarItem, _player.CurrencyPremium, _player.Currency, canBuy, _thumbResourceManager);
			return dialog;
		}

		public IDialog GetPartPurchasedDialog(IClothing avatarItem, bool spaceAvailable)
		{
			var dialog = _screenFactory.GetDialog<iGUISmartPrefab_AvatarPartBuyCompleteDialog> ();
			dialog.SetUpDialog(avatarItem, spaceAvailable);
			return dialog;
		}

        private void HandleNotEnoughSpaceWhenPurchasing()
        {
            // the user attempted to buy something but had no closet space to do so
            // he can now choose whether to buy closet space or not
            var dialog = _screenFactory.GetDialog<iGUISmartPrefab_InsufficientSpaceDialog>();
            dialog.Display(HandleNotEnoughSpaceResponse);
        }

		void HandleNotEnoughSpaceResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
				case DialogResponse.OK:
                    // user wishes to purchase more closet space
					var dialog = GetExpandClosetDialog();
					dialog.Display(HandleExpandClosetResponse);
					break;
                case DialogResponse.Cancel:
                default:
                    MakePassive(false);
                    break;
			}
		}

		void HandleExpandClosetResponse(int answer)
		{
			switch((ExpandClosetResponse)answer)
			{
    			case ExpandClosetResponse.Buy:
    				PurchaseClosetSpace();
    				break;
    			case ExpandClosetResponse.Not_Enough:
    				HandleGetMoreStarStonesDialog();
    				break;
			    case ExpandClosetResponse.Cancel:
                    MakePassive(false);
				    break;
			}
		}

		public void HandleGetMoreStarStonesDialog()
		{
            var dialog = GetNotEnoughStarstonesDialog();
            dialog.Display(HandleNotEnoughStarStonesResponse);
		}

        private void HandleNotEnoughStarStonesResponse(int answer)
        {
            switch((DialogResponse)answer)
            {
                case DialogResponse.Cancel:
                    MakePassive(false);
                    break;
                case DialogResponse.OK:
					ShowCurrencyPurchaseDialog();
                    break;
            }
        }

        private void HandleCurrencyPurchase(int answer)
        {
            switch ((CurrencyPurchaseResponse)answer)
            {
                case CurrencyPurchaseResponse.CLOSE:
                    MakePassive(false);
                    break;
            }
        }

		public void ShowCurrencyPurchaseDialog()
		{
			_shopDialogueController.Show (ShopDisplayType.STARSTONES, OnFinishTransaction);
		}

		public void OnFinishTransaction()
		{
			MakePassive (false);
		}

		public IDialog GetNotEnoughStarstonesDialog()
		{
			return _screenFactory.GetDialog<iGUISmartPrefab_NotEnoughStonesDialog>();
		}
		
		public IDialog GetExpandClosetDialog()
		{
			var dialog = _screenFactory.GetDialog<iGUISmartPrefab_ExpandClosetDialog>();
			dialog.HasCurrency((_player.CurrencyPremium > 0));
			SendDisplayDialogueExpandSpaceMetric();

			return dialog;
		}

		public IDialog GetInsufficientSpaceDialog()
		{
			return _screenFactory.GetDialog<iGUISmartPrefab_InsufficientSpaceDialog>();
		}

		public void PurchaseClosetSpace()
		{
            _shopController.SpendPremium(MasterConfiguration.ADDITIONAL_CLOSET_SLOTS_ITEM_ID, CompleteClosetSpacePurchase);
		}

		void CompleteClosetSpacePurchase(bool isSuccessful)
		{
			if (isSuccessful)
			{
				_player.AddClosetSpace();
				_player.UpdatePremiumCurrency(-1);
				UpdateAvailableSpace();
				var dialog = GetExpansionFinishedDialog();
                dialog.Display((response) => MakePassive(false));
				
				SendExpandSpaceMetric();
			}
			else
			{
				HandlePurchaseFailure();
			}
		}

		public IDialog GetExpansionFinishedDialog()
		{
			return _screenFactory.GetDialog<iGUISmartPrefab_ExpandFinishedDialog>();
		}

		void HandlePurchaseFailure()
		{
			var dialog = GetSystemDialog("Sorry, there was an error in your purchase");
			dialog.Display(HandleSystemDialogResponse);
		}

        protected void AddClothingToInventory(Clothing clothing)
        {
            _inventory.Add(clothing, 1);
            _availableClosetSpace -= 1;
        }

		public void GoHome()
		{
			Manager.GoToExistingScreen("/Home");
		}

		public void GoToCloset()
		{
            NewClosetScreenControllerFactory closetScreenFactory = _repo.Get<NewClosetScreenControllerFactory>();
            NewClosetScreenController nextScreen = closetScreenFactory.Create(_player);
            Manager.ReplaceCurrent(nextScreen);
		}

		public void GoToShopList()
		{
			GoToCloset();
		}

		public void GoToMailbox()
		{
			IScreenController nextScreen = _repo.Get<MailboxScreenController>();
			Manager.Add(nextScreen);
		}

		private void SendDisplayDialogueExpandSpaceMetric()	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"expand_cost", 1}	// value looks hardcoded, make sure it stays in sync
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.CLOSET_DISPLAY_DIALOGUE_EXPAND_SPACE, data);
		}

		private void SendExpandSpaceMetric()	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"starstones_paid", 1}	// value looks hardcoded, make sure it stays in sync
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.CLOSET_EXPAND_SPACE, data);
		}

		private void SendDisplayDialogueBuyAvatarItemMetric(IClothing clothing)	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"parts_id", clothing.Name},
				{"parts_num", 1},
				{"parts_starstones_cost", clothing.PremiumPrice},
				{"parts_coins_cost", clothing.CoinPrice}
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.AVATARSHOP_DISPLAY_DIALOGUE_BUY_ITEM, data);
		}

		private enum PurchaseType
		{
			PREMIUM,
			COIN
		}

		private void SendBoughtAvatarItemMetric(IClothing clothing, PurchaseType type)	// TODO: eventually move out to its own class
		{
			int starstonesPaid = 0;
			int coinsPaid = 0;

			if(type == PurchaseType.PREMIUM)
			{
				starstonesPaid = clothing.PremiumPrice;
			}
			else if (type == PurchaseType.COIN)
			{
				coinsPaid = clothing.CoinPrice;
			}

			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"parts_id", clothing.Name},
				{"parts_num", 1},
				{"starstones_paid", starstonesPaid},
				{"coins_paid", coinsPaid}
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.AVATARSHOP_BOUGHT_ITEM, data);
		}
	}
}