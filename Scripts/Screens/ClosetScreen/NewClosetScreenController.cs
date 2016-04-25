using System;
using Voltage.Witches.Screens;
using Voltage.Witches.Models;
using Voltage.Witches.Screens.Closet;

using System.Collections.Generic;

using Voltage.Witches.Shop;

using Voltage.Witches.UI; // For UIRibbonController

using Voltage.Witches.Models.Avatar; // For Outfit

using Voltage.Witches.Net; // For URLs
using Voltage.Common.Net; // For WWWNetworkPayload

using Voltage.Common.Metrics; // Metric Manager
using Voltage.Witches.Metrics; // Actual metric events

using Voltage.Witches.Configuration; // for master config

using Voltage.Witches.Events; // for PremiumPurchaseRequestEventArgs

using Voltage.Witches.Bundles;
using Voltage.Witches.Screens.Dialogs;

namespace Voltage.Witches.Controllers
{
	public class NewClosetScreenController : ScreenController
	{
		protected readonly IScreenFactory _screenFactory;	// private

		private readonly Player _player;

		// TODO: Turn this into a networked inventory class, which manages adding and removing, hiding the details of the network operations
		private readonly Inventory _inventory;

		private readonly ShopController _shopController;
        private readonly IShopDialogueController _shopDialogueController;

		private Dictionary<ScreenClothingCategory, List<Clothing>> _clothing;

		protected NewClosetScreen _screen;				// private

		private UIRibbonController _ribbonController;

		private Outfit _currentOutfit;

        private AvatarGenerator _avatarGenerator;

        private AvatarShopScreenControllerFactory _avatarShopScreenFactory;

		private readonly IClosetSorter _closetSorter;
        private readonly AvatarManifest _avatarManifest;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;

		public NewClosetScreenController(ScreenNavigationManager navManager, IScreenFactory screenFactory, Player player, Inventory inventory, 
            ShopController shopController, IShopDialogueController shopDialogueController, MasterConfiguration masterConfig, 
			AvatarShopScreenControllerFactory avatarShopScreenFactory, IClosetSorter closetSorter, AvatarManifest avatarManifest,
			IAvatarThumbResourceManager thumbResourceManager)		// FIXME: this should be passed into the view, NOT the controller
		: base(navManager)
		{
			_screenFactory = screenFactory;
			_player = player;
            _currentOutfit = _player.GetOutfit();
			_shopController = shopController;
            _shopDialogueController = shopDialogueController;
			_inventory = inventory;
            _avatarManifest = avatarManifest;
			_thumbResourceManager = thumbResourceManager;

            _avatarShopScreenFactory = avatarShopScreenFactory;

			_closetSorter = closetSorter;

            InitializeClothing();
            InitializeView();
            _ribbonController = new UIRibbonController(_player, _shopDialogueController, _screenFactory, masterConfig);
            _ribbonController.OnShopOpen += HandleRibbonPurchaseStart;
            _ribbonController.OnShopClosed += HandleRibbonPurchaseEnd;

			_player.OnInventoryUpdate += RefreshClothingData;
		}

        private void InitializeClothing()
        {
			List<Item> allClothing = _inventory.GetAllItemsByCategory(ItemCategory.CLOTHING);

			_clothing = _closetSorter.CreateClothingDictionaryFromData(allClothing); 

            int totalClothingPieces = _clothing[ScreenClothingCategory.All].Count;
            _player.AvailableClosetSpace = _player.ClosetSpace - totalClothingPieces;
        }

		protected virtual void InitializeView()
		{
            _screen = GetScreen() as NewClosetScreen;
			_screen.Init(this, _clothing, HandleScreenLoadComplete, _thumbResourceManager);
            _screen.OnAction += HandleEvents;

            _avatarGenerator = _screen.GetAvatarGenerator();

            RefreshClosetCount();
		}

        protected bool IsLoaded { get; private set; }

        private void HandleScreenLoadComplete()
        {
            IsLoaded = true;
        }

        private void DisplayModalDialog(IDialog dialog, Action<int> callback)
        {
            MakePassive(true);
            dialog.Display((value) => {
                callback(value);
            });
        }

        public override void MakePassive(bool value)
        {
            if (_ribbonController != null)
            {
                _ribbonController.MakePassive(value);
            }

            if (_screen != null)
            {
                _screen.MakePassive(value);
            }
        }

        private void RefreshClosetCount()
        {
            int totalClothingPieces = _clothing[ScreenClothingCategory.All].Count;
            _screen.SetClosetSpace(totalClothingPieces, _player.ClosetSpace);
        }


		private void RefreshClothingData()
		{
            List<Item> allClothing = _inventory.GetAllItemsByCategory(ItemCategory.CLOTHING);
			_clothing = _closetSorter.CreateClothingDictionaryFromData(allClothing); 

            int totalClothingPieces = _clothing[ScreenClothingCategory.All].Count;
            _player.AvailableClosetSpace = _player.ClosetSpace - totalClothingPieces;

            _screen.SetItems(_clothing);
            RefreshClosetCount();
		}

        private void HandleRibbonPurchaseStart()
        {
            MakePassive(true);
        }

        private void HandleRibbonPurchaseEnd()
        {
            MakePassive(false);
        }

        private void HandleEvents(object source, EventArgs args)
        {
            ClosetScreenEventArgs realArgs = args as ClosetScreenEventArgs;
            if (realArgs.Type == "Home")
            {
                HandleHomeNavigation();
            }
            else if (realArgs.Type == "Undress")
            {
                HandleUndress();
            }
            else if (realArgs.Type == "Clothing")
            {
                ClothingChangeEventArgs clothingArgs = args as ClothingChangeEventArgs;
                HandleClothingToggle(clothingArgs.item);
            }
            else if (realArgs.Type == "Archival")
            {
                ClothingArchivalEventArgs clothingArgs = args as ClothingArchivalEventArgs;
                HandleClothingDeletionRequest(clothingArgs.item);
            }
            else if (realArgs.Type == "ClosetExpansion")
            {
                HandleClosetExpansionRequest();
            }
            else if (realArgs.Type == "EmptyCategory")
            {
                ClosetEmptyEventArgs closetArgs = args as ClosetEmptyEventArgs;
                HandleEmptyCategory(closetArgs.Category);
            }
            else if (realArgs.Type == "Shop")
            {
                HandleShopNavigation();
            }
        }

		ScreenClothingCategory _currentEmptyCategory;
        private void HandleEmptyCategory(ScreenClothingCategory category)
        {
			_currentEmptyCategory = category;
            MakePassive(true);
            var dialog = _screenFactory.GetDialog<NoItemDialog>();
            dialog.SetCategory(_categoryNames[category]);
            dialog.Display(HandleCategoryNavigationResponse);
        }

        private void HandleCategoryNavigationResponse(int response)
        {
            NoItemDialogResponse dlgResponse = (NoItemDialogResponse)response;

            if (dlgResponse == NoItemDialogResponse.GO_TO_SHOP)
            {
				HandleShopNavigation(_currentEmptyCategory);
            }
            else
            {
                // end request
                MakePassive(false);
            }
        }

		private void HandleClothingDeletionRequest(Clothing requestedItem)
		{
            MakePassive(true);

			if (CanDeleteClothing(requestedItem))
			{
				DisplayRemovalConfirmationDialog(requestedItem);
			}
			else
			{
				// Cannot remove item because it is currently worn
				DisplayInvalidRemovalDialog();
			}
		}

		private void HandleItemRemovalResponse(Clothing clothing, int response)
		{
			DialogResponse answer = (DialogResponse)response;
			if (answer == DialogResponse.OK)
			{
				DeleteClothing(clothing);
			}
            else
            {
                // ends request
                MakePassive(false);
            }
		}

		private void DeleteClothing(Clothing clothing)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>()
			{
				{"phone_id", _player.UserID },
				{"avatar_item_id", clothing.Id }
			};

			_shopController.NetworkController.Send(URLs.REMOVE_AVATAR_ITEM, parameters, 
				(reponse) => ClothingDeletionSuccess(reponse, clothing), 
				(response) => ClothingDeletionFailure(response, clothing));
		}

		private void ClothingDeletionSuccess(WWWNetworkPayload obj, Clothing clothing)
		{
			_inventory.Remove(clothing, 1);

			SendItemDeletionMetric(clothing);

            var dialog = _screenFactory.GetDialog<iGUISmartPrefab_ArchiveFinishedDialog>();
            dialog.SetItemText(clothing);
            dialog.Display(_ => MakePassive(false));

			RefreshClothingData();
		}

		private void ClothingDeletionFailure(WWWNetworkPayload obj, Clothing clothing)
		{
            // ends request
            MakePassive(false);
		}

		private void SendItemDeletionMetric(Clothing item)
		{
			var data = new Dictionary<string, object>
			{
				{"parts_id", item.Name}
			};
			AmbientMetricManager.Current.LogEvent(MetricEvent.CLOSET_PLAYER_ARCHIVED_ITEM, data);
		}

        private void SendClosetExpansionMetric()
        {
            var data = new Dictionary<string, object>
            {
                {"starstones_paid", 1}
            };

            AmbientMetricManager.Current.LogEvent(MetricEvent.CLOSET_EXPAND_SPACE, data);
        }

        private void SendWearItemMetric(Clothing clothing)
        {
            IDictionary<string,object> data = new Dictionary<string,object> 
            {
                {"parts_id", clothing.Name} // value looks hardcoded, make sure it stays in sync
            };

            AmbientMetricManager.Current.LogEvent (MetricEvent.CLOSET_PLAYER_WEARS_ITEM, data);
        }

		private void DisplayRemovalConfirmationDialog(Clothing clothing)
		{
            var dialog = _screenFactory.GetDialog<ArchiveConfirmDialog>();
            dialog.Init(clothing, _thumbResourceManager);
            Action<int> responseHandler = (response) => HandleItemRemovalResponse(clothing, response);
            dialog.Display(responseHandler);
		}

		private void DisplayInvalidRemovalDialog()
		{
            IDialog dialog = _screenFactory.GetDialog<iGUISmartPrefab_CannotArchiveDialog>();
            dialog.Display(HandleRemovalFailResponse);
		}


        private void HandleRemovalFailResponse(int response)
        {
            MakePassive(false);
        }

        private void HandleClosetExpansionRequest()
        {
            MakePassive(true);
            var dialog = _screenFactory.GetDialog<iGUISmartPrefab_ExpandClosetDialog>();
            dialog.HasCurrency((_player.CurrencyPremium > 0));
            dialog.Display(HandleClosetExpansionPurchase);
        }

        private void HandleClosetExpansionPurchase(int response)
        {
            ExpandClosetResponse actualResponse = (ExpandClosetResponse)response;
            if (actualResponse == ExpandClosetResponse.Buy)
            {
                _shopController.SpendPremium(MasterConfiguration.ADDITIONAL_CLOSET_SLOTS_ITEM_ID, CompleteClosetSpacePurchase);
            }
            else if (actualResponse == ExpandClosetResponse.Not_Enough)
            {
                var dialog = _screenFactory.GetDialog<iGUISmartPrefab_NotEnoughStonesDialog>();
                dialog.Display(HandleStarstonePurchaseRequest);
            }
            else if (actualResponse == ExpandClosetResponse.Cancel)
            {
                // end the request
                MakePassive(false);
            }
        }

        private void HandleStarstonePurchaseRequest(int result)
        {
            DialogResponse response = (DialogResponse)result; 

            if (response == DialogResponse.OK)
            {
				ShowCurrencyPurchaseDialog();
            }
            else
            {
                // ends the request
                MakePassive(false);
            }
        }

		private void ShowCurrencyPurchaseDialog()
		{
			_shopDialogueController.Show (ShopDisplayType.STARSTONES, OnFinishTransaction);
		}

		private void OnFinishTransaction()
		{
			MakePassive (false);
		}

        private void HandlePremiumTransaction(object sender, GUIEventArgs e, AbstractDialog dlg)
        {
            PremiumPurchaseRequestEventArgs requestArgs = e as PremiumPurchaseRequestEventArgs;
            ShopItemData requestedItem = requestArgs.Shop_Item;
            _shopController.InitiatePremiumPurchase(requestedItem, (success) => CompleteStarstonePurchase(success, requestedItem, dlg));
        }

        private void CompleteClosetSpacePurchase(bool isSuccessful)
        {
            if (isSuccessful)
            {
                _player.AddClosetSpace();
                _player.UpdatePremiumCurrency(-1);
                SendClosetExpansionMetric();

                var dialog = _screenFactory.GetDialog<iGUISmartPrefab_ExpandFinishedDialog>();
                dialog.Display((_) => HandleClosetRequestFinished());
            }
            else
            {
                var dialog = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
                dialog.SetText("Sorry, there was an error in your purchase");
                dialog.Display((_) => MakePassive(false));
            }
        }

        private void HandleClosetRequestFinished()
        {
            RefreshClosetCount();
            MakePassive(false);
        }

        private void CompleteStarstonePurchase(bool isSuccessful, ShopItemData item, AbstractDialog dlg)
        {
            dlg.Dispose();

            if (isSuccessful)
            {
                var dialog = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
                dialog.SetText("Thank you for your business!");
                dialog.Display((_) => MakePassive(false));

                _player.UpdatePremiumCurrency(item.premium_qty);
                // Then need to update the screen
            }
            else
            {
                var dialog = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
                dialog.SetText("Sorry, there was an error in your purchase");
                dialog.Display((_) => MakePassive(false));
            }
        }

		private bool CanDeleteClothing(Clothing clothing)
		{
			return (!_currentOutfit.IsWearingItem(clothing.Layer_Name));
		}

		private static Dictionary<ScreenClothingCategory, string> _categoryNames = new Dictionary<ScreenClothingCategory, string>
		{
			{ ScreenClothingCategory.All, "All" },
			{ ScreenClothingCategory.Hats, "Hats" },
			{ ScreenClothingCategory.Hair, "Hairstyles" },
			{ ScreenClothingCategory.Skin, "Skin" },
			{ ScreenClothingCategory.Intimates, "Intimates" },
			{ ScreenClothingCategory.Tops, "Tops" },
			{ ScreenClothingCategory.Jackets, "Jackets & Coats" },
			{ ScreenClothingCategory.Bottoms, "Bottoms" },
			{ ScreenClothingCategory.Dresses, "Dresses" },
			{ ScreenClothingCategory.Shoes, "Shoes" },
			{ ScreenClothingCategory.Accessories, "Accessories" }
		};

        private void HandleClothingToggle(Clothing clothing)
        {
            MakePassive(true);
            string itemName = clothing.Layer_Name;
            if (_currentOutfit.IsWearingItem(itemName))
            {
                _currentOutfit.RemoveItem(itemName);
            }
            else
            {
                // Exception from [KC-1686] -- Wearing a fullbody dress should first remove all clothing
                string subCategory = _avatarManifest.GetSubCategoryForItem(itemName);
                if (subCategory == OutfitSubCat.FULLBODY_DRESS)
                {
                    _currentOutfit.RemoveAllClothing();
                }
                _currentOutfit.WearItem(itemName);
                SendWearItemMetric(clothing);
            }

            UpdateAvatar();
        }

		private void HandleUndress()
		{
			if (_currentOutfit.HasNoClothes())
			{
				// no changes are necessary -- avatar is already undressed
				return;
			}

            MakePassive(true);

			_currentOutfit.RemoveAllClothing();
			UpdateAvatar();
		}

		private void UpdateAvatar()
		{
			_screen.DisplayAvatarLoadingIndicator(true);
            _avatarGenerator.RegenerateImages(_currentOutfit, OnAvatarGenerationComplete);
		}

		private void OnAvatarGenerationComplete()
		{
			_player.UpdateOutfit(_currentOutfit);
			_screen.RefreshAvatar(HandleAvatarUpdate);
		}

        private void HandleAvatarUpdate()
        {
            _screen.DisplayAvatarLoadingIndicator(false);
            MakePassive(false);
        }

        private void HandleHomeNavigation()
        {
            Manager.GoToExistingScreen("/Home");
        }

		private void HandleShopNavigation(ScreenClothingCategory activeCategory = ScreenClothingCategory.All)
        {
            // TODO: should re-use an existing shop if one exists
            // TODO: should replace this screen with the shop?
			IScreenController nextScreen = _avatarShopScreenFactory.Create(activeCategory);
            Manager.ReplaceCurrent(nextScreen);
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
			_player.OnInventoryUpdate -= RefreshClothingData;

			if (_ribbonController != null) 
			{
				_ribbonController.OnShopOpen -= HandleRibbonPurchaseStart;
				_ribbonController.OnShopClosed -= HandleRibbonPurchaseEnd;
				_ribbonController.Dispose ();
				_ribbonController = null;
			}

			if (_screen != null)
			{
                _screen.OnAction -= HandleEvents;

				_screen.Dispose();
				_screen = null;
			}
		}

		protected override IScreen GetScreen()
		{
            if (_screen == null)
            {
                _screen = _screenFactory.GetScreen<NewClosetScreen>();
            }

            return _screen;
		}
		#endregion
	}
}
