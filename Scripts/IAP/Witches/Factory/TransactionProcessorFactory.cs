
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage.Witches.IAP
{
	using Voltage.Common.Logging;

	using Voltage.Witches.Services;
	using Voltage.Story.General;
	using Voltage.Common.IAP;
    using Voltage.Witches.Exceptions;

	public class TransactionProcessorFactory
    {
		#pragma warning disable 414
		private readonly IIAPAuth _iapAuth;
		#pragma warning restore 414

		public TransactionProcessorFactory(IIAPAuth iapAuth)
		{
			if(iapAuth == null)
			{
				throw new ArgumentNullException();
			}

			_iapAuth = iapAuth;
		}
			
		public void Create (ClientEnvironment environment, Action<Exception, ITransactionProcessor> callback)		// onFailure ???
		{
			ITransactionProcessor processor	= GetIAPProcessor();
			switch(environment)	// FIXME: Would like to keep as much of the environmental setup in the DI Composition Root
			{
				case ClientEnvironment.PRODUCTION:
				case ClientEnvironment.STAGING:
					processor.Initialize ((error) => HandleProcessorInit (error, processor, callback));
					break;
				case ClientEnvironment.CUSTOM:
				case ClientEnvironment.DEVELOPMENT:
				default:
					AmbientLogger.Current.Log ("TransactionProcessorFactory::Create >>> Using PaymentPassThruProcessor", LogLevel.INFO);
					callback(null, new PaymentPassThruProcessor()); break;
			}
		}

		private ITransactionProcessor GetIAPProcessor()
		{
			ITransactionProcessor processor;
			if (Application.platform == RuntimePlatform.Android) 
			{
				string OS_TYPE = AndroidOSDetector.DetectAndroidOSType ();
				if (OS_TYPE == AndroidOSDetector.AMAZON) {
					processor = new AmazonIAPProcessor (_iapAuth);
				} else {
					processor = new UnityIAPProcessor (_iapAuth);
				}
			} 
			else 
			{
				processor = new UnityIAPProcessor (_iapAuth);
			}
			return processor;
		}

		private void HandleProcessorInit(string errorMsg, ITransactionProcessor processor, Action<Exception, ITransactionProcessor> callback)
        {
            if (!string.IsNullOrEmpty(errorMsg))
            {
                callback(new WitchesException(errorMsg), null);
                return;
            }

            callback(null, processor);
        }
    }
    
}




