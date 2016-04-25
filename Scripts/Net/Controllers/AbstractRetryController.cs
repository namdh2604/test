
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Logging;
	using Voltage.Common.Net;
	using Voltage.Witches.Screens;

//	public abstract class AbstractRetryController //: INetworkTimeoutController<WitchesRequestResponse>
//	{
//		private readonly IScreenFactory _screenFactory;
//		
//		public AbstractRetryController (IScreenFactory screenFactory)
//		{
//			if(screenFactory == null)
//			{
//				throw new ArgumentNullException();
//			}
//			
//			_screenFactory = screenFactory;
//		}
		
		
//		protected abstract Func<INetworkTransportLayer> CreateRequest (RequestInfo info);
		
//		protected void ShowRetryDialogue(WitchesRequestResponse response, RequestInfo info, bool allowClose=true)
//		{
//			AmbientLogger.Current.Log ("Showing Retry/Cancel", LogLevel.INFO);
//			
//			Func<INetworkTransportLayer> request = CreateRequest (info);
//			
//			var dialogue = _screenFactory.GetDialog<iGUISmartPrefab_RetryCloseDialog> ();
//			dialogue.btn_close.setEnabled (allowClose);
//			dialogue.Display ((choice) => 
//			{
//				switch((DialogResponse)choice)
//				{
//					case DialogResponse.OK: 	
//						request(); break;
//						
//					case DialogResponse.Cancel:
//					default:
//						info.OnFailure(response); break;
//				}
//			});
//		}
		
		
//		protected RequestInfo CreateInfo(string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout, Method method)
//		{
//			return new RequestInfo ()
//			{
//				URL = url,
//				Parameters = parms,
//				OnSuccess = onSuccess,
//				OnFailure = onFailure,
//				Timeout = timeout,
//				Method = method,
//			};
//		}
//		
//		
//		protected class RequestInfo
//		{
//			public string URL { get; set; }
//			public IDictionary<string,string> Parameters { get; set; }
//			public Action<WitchesRequestResponse> OnSuccess { get; set; }
//			public Action<WitchesRequestResponse> OnFailure { get; set; }
//			public int Timeout { get; set; }
//			public Method Method { get; set; }
//		}
//		
//		
//		protected enum Method
//		{
//			GET,
//			POST
//		}
		
//	}
    
}




