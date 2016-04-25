using System;
using System.Collections.Generic;
using Voltage.Common.IAP;
using UnityEngine.Purchasing;
using Voltage.Witches.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Voltage.Witches.IAP
{
	public class UnityIAPProcessor : IStoreListener, ITransactionProcessor
	{
		private IStoreController _storeController;
		private IExtensionProvider _storeExtensionProvider;

		private IIAPAuth _iapAuth;

		private Action<string> _onInit;

		private Dictionary<string, TransactionCallback> _inflightPurchases;

		public UnityIAPProcessor(IIAPAuth iapAuth)
		{
			_iapAuth = iapAuth;
			_inflightPurchases = new Dictionary<string, TransactionCallback>();
		}

		public void Initialize(Action<string> onInit)
		{
			if (IsInitialized())
			{
				if (onInit != null)
				{
					onInit(string.Empty);
				}
				return;
			}

			ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			builder.Configure<IGooglePlayConfiguration>().SetPublicKey(_iapAuth.GetKey());

			builder.AddProduct("com.voltage.ent.witch.001", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.002", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.003", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.004", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.005", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.006", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.101", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.102", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.103", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
			builder.AddProduct("com.voltage.ent.witch.104", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });
            builder.AddProduct("com.voltage.ent.witch.201", ProductType.Consumable, new IDs() { GooglePlay.Name, AppleAppStore.Name });     // Starter Pack

			_onInit = onInit;
			UnityPurchasing.Initialize(this, builder);
		}


		#region ITransactionProcessor
		public void Purchase(string productID, Action<TransactionReceipt> onSuccess, Action<TransactionFailedReceipt> onFailure)
		{
			if ((onSuccess == null) || (onFailure == null))
			{
				throw new WitchesException("Success and failure callbacks must be specified for purchasing. Product ID: " + productID);
			}

			string errMsg = string.Empty;
			if (!IsInitialized())
			{
				errMsg = "Attempted to purchase: " + productID + " before engine was initialized";
			}

			if (_inflightPurchases.ContainsKey(productID))
			{
				errMsg = "An existing transaction for " + productID + " is still in progress. Please wait";
			}

			if (!string.IsNullOrEmpty(errMsg))
			{
				onFailure(new TransactionFailedReceipt(productID, errMsg));
				return;
			}

			_inflightPurchases[productID] = new TransactionCallback(onSuccess, onFailure);
			_storeController.InitiatePurchase(productID);
		}
		#endregion


		#region IStoreListener
		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			_storeController = controller;
			_storeExtensionProvider = extensions;

			if (_onInit != null)
			{
				_onInit(string.Empty);
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			UnityEngine.Debug.LogWarning("Billing failed to initialize: " + error.ToString());
			if (_onInit != null)
			{
				_onInit(error.ToString());
			}
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			// find the callback
			string productID = e.purchasedProduct.definition.id;
			string receipt = e.purchasedProduct.hasReceipt ? e.purchasedProduct.receipt : string.Empty;
			string purchaseInfo = GetPurchaseInfoFromReceipt(receipt);
			UnityEngine.Debug.LogWarning("full receipt is: " + receipt);
			UnityEngine.Debug.LogWarning("receipt is: " + purchaseInfo);

			if (_inflightPurchases.ContainsKey(productID))
			{
				var callback = _inflightPurchases[productID].onSuccess;
				_inflightPurchases.Remove(productID);
				callback(new TransactionReceipt(productID, e.purchasedProduct.transactionID, purchaseInfo));
				UnityEngine.Debug.LogWarning("transaction found that wasn't initiated");
			}

			return PurchaseProcessingResult.Complete;
		}

		public static string GetPurchaseInfoFromReceipt(string receipt)
		{
			JObject receiptJson = JObject.Parse(receipt);
			return receiptJson.Value<string>("Payload");
		}

		public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
		{
			string productID = item.definition.id;
			var callback = _inflightPurchases[productID].onFailure;
			callback(new TransactionFailedReceipt(productID, r.ToString()));
			_inflightPurchases.Remove(productID);
		}
		#endregion

		private bool IsInitialized()
		{
			return (_storeController != null) && (_storeExtensionProvider != null);
		}

		// called from IAppleExtensions
		private void OnTransactionsRestored(bool success)
		{
			UnityEngine.Debug.Log("Transactions restored");
		}

		// Apple's "Ask to buy" functionality
		private void OnDeferred(Product item)
		{
			UnityEngine.Debug.Log("Purchase deferred: " + item.definition.id);
		}

		private class TransactionCallback
		{
			public Action<TransactionReceipt> onSuccess;
			public Action<TransactionFailedReceipt> onFailure;

			public TransactionCallback(Action<TransactionReceipt> onSuccess, Action<TransactionFailedReceipt> onFailure)
			{
				this.onSuccess = onSuccess;
				this.onFailure = onFailure;
			}
		}

	}
}

