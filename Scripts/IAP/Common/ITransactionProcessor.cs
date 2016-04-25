
using System;
using System.Collections.Generic;

namespace Voltage.Common.IAP
{

	public interface ITransactionProcessor
	{
		void Purchase (string productID, Action<TransactionReceipt> onSuccess, Action<TransactionFailedReceipt> onFailure);
		void Initialize(Action<string> onInit);
//		bool IsReady { get; }
	}


	public class TransactionReceipt
	{
		public string ItemID { get; private set; }
		public string TransactionID { get; private set; }
		public string Receipt { get; private set; }
		
		public TransactionReceipt(string itemID, string transactionID, string receipt="")
		{
			ItemID = itemID;
			TransactionID = transactionID;
			Receipt = receipt;
		}
	}
	
	public class TransactionFailedReceipt
	{
		public string ItemID { get; private set; }
		public string Message { get; private set; }
		
		public TransactionFailedReceipt (string itemID, string msg)
		{
			ItemID = itemID;
			Message = msg;
		}
	}
    
}




