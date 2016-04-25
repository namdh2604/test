
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage.Witches.Shop
{
	using Voltage.Common.Net;
	
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Models;
	using Voltage.Witches.Controllers;
	
	using Voltage.Witches.Net;
	
	using Voltage.Common.Metrics;
	using Voltage.Witches.Metrics;
	
	using Voltage.Common.IAP;
	using Voltage.Witches.IAP;
	using Voltage.Common.Logging;
	
	using Voltage.Witches.Screens;
	
	public interface IShopController
	{
		void SpendCoins (string itemID, Action<bool> onComplete);
		void SpendPremium (string itemID, Action<bool> onComplete);
		void InitiatePremiumPurchase (ShopItemData data, Action<bool> onComplete);
		void BuyFocus(Action<bool> onComplete);
	}
	
	
	public class ShopController : IShopController
	{
		public INetworkTimeoutController<WitchesRequestResponse> NetworkController { get; private set; }
		
		public Player Player { get; private set; }
		public MasterConfiguration MasterConfig { get; private set; }
		//		public IURLFactory UrlController { get; private set; }
		
		private readonly ILogger _logger;
		private readonly ITransactionProcessor _transactionProcessor;
		private readonly IScreenFactory _screenFactory;	//TEMPORARY
		private readonly INetworkMonitor _networkMonitor;

		private readonly string OS_TYPE;
		
		public bool IsReady { get { return true; } }	// TODO...eventually push out to composition root...should also factor in online status
		
		
		public ShopController (INetworkTimeoutController<WitchesRequestResponse> networkController, Player player, MasterConfiguration masterConfig, ITransactionProcessor transactionProcessor, IScreenFactory screenFactory, INetworkMonitor networkMonitor) 
		{
			if(networkController == null || player == null || masterConfig == null ) //|| transactionProcessor == null)
			{
				throw new ArgumentNullException("ShopController::Ctor >>> ");
			}
			
			NetworkController = networkController;
			Player = player;
			MasterConfig = masterConfig;
			
			_transactionProcessor = transactionProcessor;
			
			_logger = AmbientLogger.Current; 
			
			_screenFactory = screenFactory;			// TEMPORARY
			_networkMonitor = networkMonitor;		// TEMPORARY

			#if  UNITY_IOS || UNITY_IPHONE
				OS_TYPE = "ios";
			#elif UNITY_ANDROID
				if (Application.platform == RuntimePlatform.Android) {
					OS_TYPE = AndroidOSDetector.DetectAndroidOSType ();
				}
			#else
				OS_TYPE = "other";
			#endif
		}
		
		
		public void BuyFocus(Action<bool> onComplete)
		{
			var userId = Player.UserID;
			var ticketType = "1";
			
			Dictionary<string,string> parameters = new Dictionary<string,string> ()
			{
				{"phone_id",userId},
				{"ticket_type",ticketType}
			};
			
			NetworkController.Send(URLs.BUY_FOCUS, parameters,(response) => SpendPremiumSuccess (response, onComplete),(response) => SpendPremiumFailed (response, onComplete));
		}
		
		public void SpendCoins(string itemID, Action<bool> onComplete)	
		{
			if(!string.IsNullOrEmpty(itemID) && MasterConfig.Items_Master != null)
			{
				if (MasterConfig.Items_Master.ContainsKey(itemID))
				{
					var item = MasterConfig.Items_Master[itemID];
					string userID = Player.UserID;		//"e2e4a8c5-378d-11e4-9bcc-0c4de9a17bc6";
					UnityEngine.Debug.LogWarning("USER ID :: " + userID);
					
					string itemType = string.Empty;
					string item_id = string.Empty;
					
					string quantity = "1";
					switch(item.ItemCategory)
					{
					case ItemCategory.CLOTHING:
						itemType = "1";
						item_id = itemID;
						break;
					case ItemCategory.INGREDIENT:
						itemType = "0";
						item_id = itemID;
						break;
					case ItemCategory.POTION:
						itemType = "0";
						item_id = itemID;
						break;
					case ItemCategory.OUTFIT:
						itemType = "2";
						item_id = itemID;
						break;
						//					default: 
					}
					
					Dictionary<string,string> parameters = new Dictionary<string,string>()
					{
						{"phone_id",userID},
						{"item_type",itemType},
						{"item_id",item_id},
						{"quantity",quantity}
					};
					
					NetworkController.Send(URLs.BUY_WITH_COINS, parameters,(response) => SpendCoinSuccess (response, onComplete),(response) => SpendCoinFailed (response, onComplete));
				}
				else
				{
					ErrorPerformingAction (new KeyNotFoundException("ShopController::SpendCoins >>>"), onComplete);
				}
				
			}
			else
			{
				ErrorPerformingAction (new KeyNotFoundException("ShopController::SpendCoins >>>"), onComplete);
			}
		}
		
		private void SpendCoinSuccess(WitchesRequestResponse response, Action<bool> onComplete)
		{
			onComplete(true);
		}
		private void SpendCoinFailed(WitchesRequestResponse response, Action<bool> onComplete)
		{
			onComplete(false);
		}
		
		public void SpendPremium(string itemID, Action<bool> onComplete)
		{
			if (string.IsNullOrEmpty(itemID) || (MasterConfig.Items_Master == null))
			{
				ErrorPerformingAction(new ArgumentNullException("ShopController::SpendPremium >>>"), onComplete);
				return;
			}
			
			
			string userID = Player.UserID;                  //"e2e4a8c5-378d-11e4-9bcc-0c4de9a17bc6";
//			UnityEngine.Debug.LogWarning("USER ID :: " + userID);
			string itemType = string.Empty;
			string item_id = string.Empty;
			string quantity = "1";
			
			if (itemID == MasterConfiguration.ADDITIONAL_CLOSET_SLOTS_ITEM_ID)
			{
				itemType = "3";
				item_id = string.Empty;
			}
			else
			{
				if (!MasterConfig.Items_Master.ContainsKey(itemID))
				{
					ErrorPerformingAction (new KeyNotFoundException("ShopController::SpendPremium >>>"), onComplete);
					return;
				}
				
				var item = MasterConfig.Items_Master[itemID];
				
				switch(item.ItemCategory)
				{
				case ItemCategory.CLOTHING:
					itemType = "1";
					item_id = itemID;
					break;
				case ItemCategory.INGREDIENT:
					itemType = "0";
					item_id = itemID;
					break;
				case ItemCategory.POTION:
					itemType = "0";
					item_id = itemID;
					break;
				case ItemCategory.OUTFIT:
					itemType = "2";
					item_id = itemID;
					break;
				}
			}
			
			Dictionary<string,string> parameters = new Dictionary<string,string>()
			{
				{"phone_id",userID},
				{"item_type",itemType},
				{"item_id",item_id},
				{"quantity",quantity}
			};
			
			NetworkController.Send(URLs.BUY_WITH_STONES,parameters, (response) => SpendPremiumSuccess(response,onComplete), (response) => SpendPremiumFailed (response, onComplete));
		}
		
		private void SpendPremiumSuccess(WitchesRequestResponse response, Action<bool> onComplete)
		{
			onComplete(true);
		}
		private void SpendPremiumFailed(WitchesRequestResponse response, Action<bool> onComplete)
		{
			onComplete(false);
		}
		
		
		
		public void InitiatePremiumPurchase(ShopItemData data, Action<bool> onComplete)		// TODO: transition logic from homescreen view to here
		{
			if(data != null)
			{
				_logger.Log(string.Format("Purchasing {0} [{1}]", data.name, data.product_id), LogLevel.INFO);
				SendStartedTransactionMetric (data);
				
				_transactionProcessor.Purchase(data.product_id, (receipt) => OnSuccessfulPurchase (receipt, data, onComplete), (receipt) => OnFailedPurchase (receipt, onComplete));
			}
			else
			{
				ErrorPerformingAction (new ArgumentNullException("ShopController::InitiatePremiumPurchase >>>"),onComplete);
			}
		}
		
		private void OnSuccessfulPurchase(TransactionReceipt receipt, ShopItemData itemData, Action<bool> onComplete)
		{
			SendCompletedTransactionMetric (itemData);
			
			UpdateServerWithPurchase (receipt, Player, OS_TYPE, onComplete);
		}
		
		private void OnFailedPurchase (TransactionFailedReceipt receipt, Action<bool> onComplete)	// Cancel heads here too???
		{
			onComplete (false);
		}
		
		
		
		private void UpdateServerWithPurchase (TransactionReceipt receipt, Player player, string os_type, Action<bool> onComplete)	// Move into NetworkedPlayer??? or something else?
		{
			Dictionary<string,string> parameters = new Dictionary<string, string> ()
			{
				{"phone_id", player.UserID},
				{"premium_id",receipt.ItemID},
				{"device_os", os_type},
				{"receipt",receipt.Receipt},
				{"transaction_id", receipt.TransactionID}
			};
			
			
			var maintenanceTrigger = new WitchesNetworkMaintenanceController (NetworkController);	// TEMPORARY
			maintenanceTrigger.OnMaintenanceEvent += OnMaintenance;									// TEMPORARY
			var retryController = new WitchesNetworkRetryController (maintenanceTrigger, _screenFactory);	// TEMPORARY
			var networkController = new WitchesNetworkConnectionCheckController (retryController, _networkMonitor, _screenFactory);	// TEMPORARY
			networkController.Send (URLs.BUY_PREMIUM, parameters, (response) => onComplete (true), (response) => onComplete (false));
		}
		
		
		private void OnMaintenance (object sender, EventArgs args)	// TEMPORARY
		{
			//			AmbientLogger.Current.Log ("ShopController::OnMaintenance", LogLevel.INFO);
			
			var dialogue = _screenFactory.GetDialog<iGUISmartPrefab_MessageNoInputDialog>();
			dialogue.message.label.text = "SERVER NOT RESPONDING";	// "Server is in Maintenance, Please Try Again Later."
			dialogue.Display (null);
		}
		
		
		
		
		
		
		
		private void ErrorPerformingAction(Exception exception, Action<bool> onComplete)
		{
			// TODO: display error dialogue
			UnityEngine.Debug.LogError(exception.Message);
			onComplete(false);
		}
		
		
		
		private void SendStartedTransactionMetric(ShopItemData itemData) // TODO: eventually move out to its own class
		{
			string idKey = string.Empty;
			string eventName = string.Empty;
			string priceKey = string.Empty;
			
			if(itemData.name.Contains("Stamina"))
			{
				idKey = "staminapotions_id";
				priceKey = "staminapotions_cost";
				eventName = MetricEvent.SHOP_STAMINA_POTION_CONFIRM_BUTTON;
			}
			else
			{
				idKey = "starstones_id";
				priceKey = "starstones_cost";
				eventName = MetricEvent.SHOP_STARSTONE_CONFIRM_BUTTON;
			}
			
			IDictionary<string,object> data = new Dictionary<string,object>
			{
				{idKey, itemData.name },
				{priceKey, itemData.price}
			};
			
			AmbientMetricManager.Current.LogEvent(eventName, data);
		}
		
		
		
		private void SendCompletedTransactionMetric(ShopItemData itemData) // TODO: eventually move out to its own class
		{
			string idKey = string.Empty;
			string eventName = string.Empty;
			
			if(itemData.name.Contains("Stamina"))
			{
				idKey = "staminapotions_id";
				eventName = MetricEvent.SHOP_STAMINA_POTION_BOUGHT;
			}
			else
			{
				idKey = "starstones_id";
				eventName = MetricEvent.SHOP_STARSTONE_BOUGHT;
			}
			
			IDictionary<string,object> data = new Dictionary<string,object>
			{
				{idKey, itemData.name },
				{"iap_paid", itemData.price}
			};
			
			AmbientMetricManager.Current.LogEvent(eventName, data);
		}
		
	}
	
}


























