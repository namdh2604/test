using UnityEngine;
using System.Collections;
using Voltage.Witches.IAP;
using com.amazon.device.iap.cpt;
using System;
using Voltage.Common.IAP;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;
using Voltage.Witches.Exceptions;

public class AmazonIAPProcessor : ITransactionProcessor
{
	private string _requestIdString;
	private IAmazonIapV2 _iapService;
	private TransactionCallback _transactionCallback;

	public AmazonIAPProcessor(IIAPAuth iapAuth)
	{
		_iapService = AmazonIapV2Impl.Instance;
		_iapService.AddPurchaseResponseListener(PurchaseEventHandler);
	}

	public void  Initialize(Action<string> onInit)
	{
		// No Initialization required, return call back
		if (onInit != null)
		{
			onInit(string.Empty);
		}
	}

	private string _productId;
	public void Purchase(string productID, Action<TransactionReceipt> onSuccess, Action<TransactionFailedReceipt> onFailure)
	{
		_productId = productID;
		SkuInput request = new SkuInput();
		request.Sku = _productId;
		_iapService.Purchase(request);
		_transactionCallback = new TransactionCallback(onSuccess, onFailure);
	}
		

	const string IAP_SUCCESSFUL = "SUCCESSFUL";
	private void PurchaseEventHandler(com.amazon.device.iap.cpt.PurchaseResponse args)
	{
		if (args.Status == IAP_SUCCESSFUL) {
			LogReceipt (args);

			AmazonUserData userData = args.AmazonUserData;
			string amazon_user_id = userData.UserId;
			string receiptId = args.PurchaseReceipt.ReceiptId;
			string productId = args.PurchaseReceipt.Sku;

			Dictionary<string, object> receipt_data = new Dictionary<string, object> ();
			receipt_data.Add ("amazon_user_id", amazon_user_id);
			receipt_data.Add ("receipt_id", receiptId);
			string receipt = Json.Serialize (receipt_data);

			if (productId == _productId)
			{
				var callback = _transactionCallback.onSuccess;
				callback (new TransactionReceipt (productId, amazon_user_id, receipt));
			}
			else 
			{
				throw new WitchesException (string.Format("IAP item id {0} coming back from Amazon is unknown.", productId));
			}
		}
		else 
		{
			var callback = _transactionCallback.onFailure;
			callback(new TransactionFailedReceipt(_productId, args.Status));
		}
	}

	private void LogReceipt(com.amazon.device.iap.cpt.PurchaseResponse args)
	{
		string receiptId = args.PurchaseReceipt.ReceiptId;
		AmazonUserData userData = args.AmazonUserData;
		string requestId = args.RequestId;
		long cancelDate = args.PurchaseReceipt.CancelDate;
		long purchaseDate = args.PurchaseReceipt.PurchaseDate;
		string sku = args.PurchaseReceipt.Sku;
		string productType = args.PurchaseReceipt.ProductType;
		string status = args.Status;
		Debug.Log (string.Format ("Amazon Receipt: requestId: {0}, receiptId: {1}, cancelDate: {2}, purchaseDate: {3}, sku: {4}, productType: {5}, status: {6}  AmazonUserData: {7}", 
			requestId, receiptId, cancelDate, purchaseDate, sku, productType, status, userData.ToJson ()));
		
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
