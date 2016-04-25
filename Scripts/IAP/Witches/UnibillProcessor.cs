// TODO #5.0# Move this over to Unity IAP

//using System;
//using System.Collections.Generic;
//
//namespace Voltage.Witches.IAP
//{
//	using Unibill;
//	using Voltage.Common.IAP;
//
//	using Voltage.Common.Logging;
//
//	using Newtonsoft.Json;
//	using Newtonsoft.Json.Linq;
//
//
////	public class UnibillerConfig
////	{
////		public string GooglePlayPublicKey { get; set; }
////	}
//
//
//    public class UnibillProcessor : ITransactionProcessor
//    {
//		private Action<TransactionReceipt> _onSuccess;
//		private Action<TransactionFailedReceipt> _onFailure;
//
//		private readonly IIAPAuth _iapAuth;
////		private readonly UnibillerConfig _unibillerConfig;
//
//
//		public UnibillProcessor (IIAPAuth iapAuth)
//		{
//			if(iapAuth == null)
//			{
//				throw new ArgumentNullException();
//			}
//
//			_iapAuth = iapAuth;
//			InitBiller ();
//		}
//
//		private void InitBiller()
//		{
////			if (UnityEngine.Resources.Load ("unibillInventory.json") == null) 
////			{
////				throw new NullReferenceException("UnibillProcessor::Ctor >>> " );
////			}
//			if(Unibiller.Initialised)
//			{
//				Unibiller.ResetDelegates ();
//			}
//
////			Unibiller.onTransactionsRestored += onTransactionsRestored;
//			Unibiller.onPurchaseFailed += OnPurchaseFailed;
//			Unibiller.onPurchaseCompleteEvent += OnPurchaseSuccess;
////			Unibiller.onPurchaseDeferred += onDeferred;
//		}
//
//
//		public void Initialize(Action<UnibillState> callback)
//		{
////			if(!Unibiller.Initialised)
//			{
//				Unibiller.onBillerReady += callback;
//
//				List<ProductDefinition> products = GetProductDefinitions ();
//				IDictionary<string,object> unibillerConfigSettings = GetConfigSettings ();	// TODO: replace with UnibillerConfig
//
//				Unibiller.Initialise (products, unibillerConfigSettings); 	// MODIFIED
//
//				#if UNITY_IOS
//				var appleExtensions = Unibiller.getAppleExtensions();
//				appleExtensions.onAppReceiptRefreshed += x => {
//					Console.WriteLine (x);
//					Console.WriteLine ("Refreshed app receipt!");
//				};
//				
//				appleExtensions.onAppReceiptRefreshFailed += () => {
//					Console.WriteLine("Failed to refresh app receipt.");
//				};
//				#endif
//			}
////			else
////			{
////				callback(UnibillState.SUCCESS);
////			}
//		}
//
//		private IDictionary<string,object> GetConfigSettings()
//		{
//			return new Dictionary<string,object>
//			{
//				{"GooglePlayPublicKey", _iapAuth.GetKey()},
//			};
//		}
//
////		private void ResetDelegates()
////		{
////			Unsubscribe<Action<UnibillState>> (Unibiller.onBillerReady);	// not possible, events are not firstclass citizens
////		}
////
////		private void Unsubscribe<T>(Delegate eventDelegate)
////		{
////			foreach(Delegate d in eventDelegate.GetInvocationList())
////			{
////				eventDelegate -= (T)d;
////			}
////		}
//
//
//
//
//
//
//		private List<ProductDefinition> GetProductDefinitions()	// TEMPORARY...eventually pass thru constructor
//		{
//			List<ProductDefinition> products = new List<ProductDefinition> ()
//			{
//				new ProductDefinition("com.voltage.ent.witch.001", PurchaseType.Consumable),
//				new ProductDefinition("com.voltage.ent.witch.002", PurchaseType.Consumable),	
//				new ProductDefinition("com.voltage.ent.witch.003", PurchaseType.Consumable),
//				new ProductDefinition("com.voltage.ent.witch.004", PurchaseType.Consumable),
//				new ProductDefinition("com.voltage.ent.witch.005", PurchaseType.Consumable),
//				new ProductDefinition("com.voltage.ent.witch.006", PurchaseType.Consumable),
//				new ProductDefinition("com.voltage.ent.witch.101", PurchaseType.Consumable),
//				new ProductDefinition("com.voltage.ent.witch.102", PurchaseType.Consumable),
//				new ProductDefinition("com.voltage.ent.witch.103", PurchaseType.Consumable),
//				new ProductDefinition("com.voltage.ent.witch.104", PurchaseType.Consumable),
//			};
//
//			return products;
//		}
//
//
//
//		public void Purchase(string productID, Action<TransactionReceipt> onSuccess, Action<TransactionFailedReceipt> onFailure)
//		{
//			_onSuccess = onSuccess;
//			_onFailure = onFailure;
//
//			Unibiller.initiatePurchase(productID);
//		}
//
//
//		private void OnPurchaseSuccess(PurchaseEvent purchaseEvent) 
//		{
//			AmbientLogger.Current.Log (string.Format ("Purchase: {0} Successful! [{1}]\n{2}]", purchaseEvent.PurchasedItem.name, purchaseEvent.PurchasedItem.Id, purchaseEvent.Receipt), LogLevel.INFO);
//
//
//			string receipt = SanitizeUnibillReceipt (purchaseEvent);
//
//
//
//			TransactionReceipt transactionReceipt = new TransactionReceipt (purchaseEvent.PurchasedItem.Id, purchaseEvent.TransactionId, receipt);
//			_onSuccess (transactionReceipt);
//		}
//
//
//		private string SanitizeUnibillReceipt(PurchaseEvent purchaseEvent)
//		{
////			AmbientLogger.Current.Log("Pre-Parsed: " + purchaseEvent.Receipt, LogLevel.INFO);
//			string receipt;
//#if UNITY_IOS
//			receipt = purchaseEvent.Receipt;
//#else
//			JToken purchaseEventToken = JToken.Parse (purchaseEvent.Receipt);
//			receipt = purchaseEventToken["json"].ToString();
//#endif
//
////			AmbientLogger.Current.Log("Parsed: " + receipt, LogLevel.INFO);
//			return receipt;
//		}
//
//
//
//		private void OnPurchaseFailed(PurchaseFailedEvent purchaseEvent) 
//		{
//			AmbientLogger.Current.Log (string.Format ("Purchase: {0} FAILED! [{1}]\nReason: {2}\nMessage: {3}]", purchaseEvent.PurchasedItem.name, purchaseEvent.PurchasedItem.Id, purchaseEvent.Reason.ToString(), purchaseEvent.Message), LogLevel.WARNING);
//
//			TransactionFailedReceipt receipt = new TransactionFailedReceipt (purchaseEvent.PurchasedItem.Id, purchaseEvent.Message);
//			_onFailure (receipt);
//		}
//
//
//
//    }
//
//
//    
//}
//
//
//
////		private string TranslateID(string id)	// TEMPORARY
////		{
////			switch(id)
////			{
////				case "54da8ad76f983f60ee01f98c" : return "com.voltage.ent.witch.001";
////				case "54da8ad76f983f60ee01f98d" : return "com.voltage.ent.witch.002";
////				case "54da8ad76f983f60ee01f98e" : return "com.voltage.ent.witch.003";
////				case "54da8ad76f983f60ee01f98f" : return "com.voltage.ent.witch.004";
////				case "54da8ad76f983f60ee01f990" : return "com.voltage.ent.witch.005";
////				case "54da8ad86f983f60ee01f991" : return "com.voltage.ent.witch.006";
////				case "54da8ad86f983f60ee01f992" : return "com.voltage.ent.witch.101";
////				case "54da8ad86f983f60ee01f993" : return "com.voltage.ent.witch.102";
////				case "54da8ad86f983f60ee01f994" : return "com.voltage.ent.witch.103";
////				case "54da8ad86f983f60ee01f995" : return "com.voltage.ent.witch.104";
////				default : throw new ArgumentException();
////			}
////
////		}
//
//
