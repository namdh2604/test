using System;
using System.Collections.Generic;
using Voltage.Witches.Screens.Closet;

namespace Voltage.Witches.Shop
{
	using Voltage.Common.Logging;

	using Voltage.Witches.Configuration;
	using Voltage.Witches.Screens;

	using Voltage.Witches.Events;
	using Voltage.Witches.Models;


	public enum ShopDisplayType
	{
		STARSTONES = 0,
		POTION = 1,
	}

	public interface IShopDialogueController
	{
		void Show (ShopDisplayType displayType, Action onClose); // Action onSuccess, Action onCancel);	
		void Dispose ();
	}


    public class ShopDialogueController : IShopDialogueController
    {
		private readonly Player _player;
		private readonly IShopController _shopController;
		private readonly IScreenFactory _screenFactory;


		private CurrencyPurchaseDialog _shopDialogue;
		private iGUISmartPrefab_ConfirmCompleteLoadDialog _processingDialogue;	// may actually prefer to pass it thru...
//		private IGUIHandler _caller;	// TODO: replace IGUIHandler with interface not specific to iGUI...maybe don't need this

        private readonly MasterConfiguration _masterConfig;
        private readonly StarterPackEvaluator _starterPackEval;
		private readonly ShopItemsConfiguration _shopItemsConfig;
		private readonly Inventory _inventory;

		private int _availableClosetSpace;

        public ShopDialogueController(Player player, ShopItemsConfiguration shopItemsConfig, IShopController shopController, IScreenFactory screenFactory, 
			MasterConfiguration masterConfig, StarterPackEvaluator starterPackEval, Inventory inventory)
		{
			if(player == null || shopItemsConfig == null || shopController == null || screenFactory == null)
			{
				throw new ArgumentNullException();

			}

			_player = player;
			_shopController = shopController;
			_screenFactory = screenFactory;

            _masterConfig = masterConfig;
            _starterPackEval = starterPackEval;
			_shopItemsConfig = shopItemsConfig;
			_inventory = inventory;
		}

		public void Show(ShopDisplayType displayType, Action onClose) //, IGUIHandler caller)	// TODO: replace IGUIHandler with interface not specific to iGUI
		{
			_shopDialogue = GetShopDialogue (displayType);
			_shopDialogue.OnPurchaseRequest += HandleShopItemSelection; //(sender,eventArgs) => HandleShopItemSelection (sender, eventArgs, _shopDialogue);

			Action<int> onComplete = (result) => 
			{ 
				if(onClose != null)
				{
					onClose();
				}
			};

			_shopDialogue.Display (onComplete);
		}

		private iGUISmartPrefab_CurrencyPurchaseDialog _dialogue;
		private CurrencyPurchaseDialog GetShopDialogue(ShopDisplayType displayType)
		{
			_dialogue = _screenFactory.GetDialog<iGUISmartPrefab_CurrencyPurchaseDialog> ();
			_dialogue.ChangeActiveState ((int)displayType);
			HandleShopDialogDisplay (_dialogue);
			return _dialogue;
		}

		private void HandleShopDialogDisplay(CurrencyPurchaseDialog dialogue)
		{
			if (_starterPackEval.IsAvailable ()) 
			{
				dialogue.PrepShopDialog(true, _player);
				dialogue.SubscribeButtonHandlerForStarterPack ();
				dialogue.ActivateStarterPackShopView ();
				dialogue.InitStarterPack (_shopItemsConfig);
			}
			else
			{
				dialogue.PrepShopDialog(false, _player);
				dialogue.ActivateRegularShopView();
				dialogue.Init (_shopItemsConfig);
			}
		}

		public void GiveStarterPackToPlayer(ShopItemData starterPack)
		{
			UnityEngine.Debug.Log("Giving Starter Pack");
			_player.PurchaseStarterPack();
			_player.UpdateStaminaPotion(starterPack.bundle_items.Stamina);
			_player.UpdatePremiumCurrency(starterPack.bundle_items.Starstone);
			UpdateInventoryFromBundle(starterPack.bundle_items);
		}

		private void UpdateInventoryFromBundle(BundleItems bundleItems)
		{

			foreach (string itemId in bundleItems.Avatar) 
			{
				var parser = new Voltage.Witches.Configuration.JSON.ItemRawParser(_masterConfig);
				AvatarItemData config = _masterConfig.Items_Master[itemId].Item as AvatarItemData;

				Clothing avatarItem = parser.CreateAvatarItem (config) as Clothing;

				AddClothingToInventory(avatarItem);
			}
		}

		private void AddClothingToInventory(Item avatarItem)
		{
			_inventory.Add(avatarItem, 1);
		}

		private void HandleShopItemSelection (object sender, GUIEventArgs eventArg)
		{
			PremiumPurchaseRequestEventArgs request = eventArg as PremiumPurchaseRequestEventArgs;

			if(request != null)
			{
				_shopDialogue.ToggleActiveElements (false);
				ShowProcessingDialogue ();

				AmbientLogger.Current.Log (string.Format ("{0} initiated purchase of {1}", sender.ToString (), request.Shop_Item.name), LogLevel.INFO);
				InitiateTransaction(request.Shop_Item);
			}
			else
			{
				throw new ArgumentNullException("ShopDialogueController::HandleShopItemSelection >>> request is null");
			}
		}


		private void ShowProcessingDialogue()	
		{
			_processingDialogue = _screenFactory.GetDialog<iGUISmartPrefab_ConfirmCompleteLoadDialog>();
			_processingDialogue.Display (null);
			_processingDialogue.BeginLoading ();
		}

		private void HideProcessingDialogue()
		{
			_processingDialogue.EndLoading ();
		}

		private void EnableShopDialogue()
		{
			_shopDialogue.ToggleActiveElements (true);
		}


		private void InitiateTransaction (ShopItemData shopItem)
		{
			_shopController.InitiatePremiumPurchase (shopItem, (response) => OnTransactionResult(response, shopItem));
		}

		private void OnTransactionResult (bool successful, ShopItemData shopItem)
		{
			AmbientLogger.Current.Log (string.Format ("Purchase '{0}' Successful? {1}", shopItem.name, successful), LogLevel.INFO);

			HideProcessingDialogue ();
			if(successful)
			{
				ShowSystemDialog("Thank you for your business!", EnableShopDialogue);
				AddItemToPlayer(shopItem);
			}
			else
			{
				ShowSystemDialog("Sorry, there was an error in your purchase", EnableShopDialogue);
			}
		}

		private void AddItemToPlayer(ShopItemData shopItem)
		{
			if (shopItem.name.Contains ("Stamina")) 
			{
				_player.UpdateStaminaPotion (shopItem.premium_qty);
			} 
			else if (shopItem.name.Contains ("Starter Pack")) 
			{
				GiveStarterPackToPlayer (shopItem);	
			}
			else
			{
				_player.UpdatePremiumCurrency(shopItem.premium_qty);
			}
		}

		private void ShowSystemDialog(string message, Action onComplete=null)
		{
			var dialogue = _screenFactory.GetDialog<iGUISmartPrefab_SystemPopupDialog>();
			dialogue.SetText (message);

			dialogue.Display ((response) => {
				AmbientLogger.Current.Log (string.Format("System Dialogue \"{0}\" Closed", message), LogLevel.INFO);
				if(onComplete != null) onComplete();
			});
		}

		public void Dispose()
		{
			if(_shopDialogue != null)
			{
				_shopDialogue.Dispose();
			}

			if(_processingDialogue != null)
			{
				_processingDialogue.Dispose();
			}
		}

        // current implementation doesn't require returning a bool result, but calling controller may need to know
        public void ShowStarterPackDialogue(Action<bool> onComplete)
		{
            if (_starterPackEval.IsAvailable())
            {
                BuyStarterPackDialog buyStarterPackDialog = _screenFactory.GetDialog<BuyStarterPackDialog>();
                buyStarterPackDialog.Init(_player.TimeToDisableStarterPack);

                buyStarterPackDialog.Display((response) => StarterPackDialogHandler(response, onComplete));
            }
            else
            {
                AmbientLogger.Current.Log("starter pack not available!", LogLevel.WARNING);
                if (onComplete != null)
                {
                    onComplete(false);
                }
            }
		}

        private void StarterPackDialogHandler(int response, Action<bool> onComplete)
        {
            BuyStarterPackDialog.ButtonType button = (BuyStarterPackDialog.ButtonType)response;

            switch (button)
            { 
                case BuyStarterPackDialog.ButtonType.BUY:
                    ShowProcessingDialogue();
					ShopItemData starterPack = _masterConfig.Shop_Items.StarterPack;        // design says there will only ever be one starter pack

                    Action<bool> onPurchaseComplete = (isSuccessful) =>
                    {
                        HideProcessingDialogue();

                        if(isSuccessful)
                        {
                            ShowSystemDialog("Thank you for your business!", () => onComplete(isSuccessful));
							GiveStarterPackToPlayer(starterPack);
                        }
                        else
                        {
                            ShowSystemDialog("Sorry, there was an error in your purchase", () => onComplete(isSuccessful));
                        }
                    };

                    _shopController.InitiatePremiumPurchase(starterPack, onPurchaseComplete);

                    break;
                case BuyStarterPackDialog.ButtonType.CLOSE:
                default:
                    onComplete(false);
                    break;
            }

        }

    }

}



