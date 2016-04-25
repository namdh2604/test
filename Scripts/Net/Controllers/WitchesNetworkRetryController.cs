
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Logging;
	using Voltage.Common.Net;

	using Voltage.Witches.Screens;
	


	public class WitchesNetworkRetryController : INetworkTimeoutController<WitchesRequestResponse>	// AbstractRetryController, 
    {
		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private readonly IScreenFactory _screenFactory;

		public WitchesNetworkRetryController(INetworkTimeoutController<WitchesRequestResponse> networkController, IScreenFactory screenFactory=null) //: base(screenFactory) 
		{
			if(networkController == null || screenFactory == null)
			{
				throw new ArgumentNullException();
			}

			_networkController = networkController;
			_screenFactory = screenFactory;
		}


		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
//			AmbientLogger.Current.Log ("WitchesNetworkRetryController::Send...", LogLevel.INFO);
			RequestInfo info = CreateInfo (url, parms, onSuccess, onFailure, timeout, Method.POST);
			return CreateRequest (info).Invoke ();
		}
		
		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
//			AmbientLogger.Current.Log ("WitchesNetworkRetryController::Receive...", LogLevel.INFO);
			RequestInfo info = CreateInfo (url, parms, onSuccess, onFailure, timeout, Method.GET);
			return CreateRequest (info).Invoke ();
		}

		private void OnErrorResponse (WitchesRequestResponse response, RequestInfo requestInfo)	// NOTE: Won't beable to return Progress for consecutive retries!!!!
		{
			switch(response.Status)
			{
				case WitchesRequestStatus.TIMED_OUT:		// ShowRetry(); break;
				case WitchesRequestStatus.REQUEST_ERROR:
				case WitchesRequestStatus.CLIENT_ERROR:
				case WitchesRequestStatus.SERVER_ERROR:		ShowRetryDialogue (response, requestInfo); break;
				case WitchesRequestStatus.FAILED:			requestInfo.OnFailure(response); break;

				default: throw new ArgumentException();
			}
		}

		private void ShowRetryDialogue(WitchesRequestResponse response, RequestInfo info, bool allowClose=true)
		{
//			AmbientLogger.Current.Log ("Showing Retry/Cancel", LogLevel.INFO);
			
			Func<INetworkTransportLayer> request = CreateRequest (info);
			
			var dialogue = _screenFactory.GetDialog<iGUISmartPrefab_RetryCloseDialog> ();
			dialogue.btn_close.setEnabled (allowClose);
//			dialogue.message =
			dialogue.Display ((choice) => 
			{
				switch((DialogResponse)choice)
				{
					case DialogResponse.OK: 	
						request(); break;
						
					case DialogResponse.Cancel:
					default:
						info.OnFailure(response); break;
				}
			});
		}


		private Func<INetworkTransportLayer> CreateRequest(RequestInfo info)
		{
			if (info.Method == Method.GET) 
			{
				return () => _networkController.Receive(info.URL, info.Parameters, info.OnSuccess, (response) => OnErrorResponse (response, info), info.Timeout);
			}
			else
			{
				return () => _networkController.Send(info.URL, info.Parameters, info.OnSuccess, (response) => OnErrorResponse (response, info), info.Timeout);
			}
		}


		private RequestInfo CreateInfo(string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout, Method method)
		{
			return new RequestInfo ()
			{
				URL = url,
				Parameters = parms,
				OnSuccess = onSuccess,
				OnFailure = onFailure,
				Timeout = timeout,
				Method = method,
			};
		}
		
		
		private class RequestInfo
		{
			public string URL { get; set; }
			public IDictionary<string,string> Parameters { get; set; }
			public Action<WitchesRequestResponse> OnSuccess { get; set; }
			public Action<WitchesRequestResponse> OnFailure { get; set; }
			public int Timeout { get; set; }
			public Method Method { get; set; }
		}
		
		
		private enum Method
		{
			GET,
			POST
		}

    }

}




