
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Logging;
	using Voltage.Witches.Screens;

	using Voltage.Common.Net;
	using UnityEngine;

	public class WitchesNetworkConnectionCheckController : INetworkTimeoutController<WitchesRequestResponse>
    {
		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private readonly INetworkMonitor _networkMonitor;
		private readonly IScreenFactory _screenFactory;
		
		public WitchesNetworkConnectionCheckController(INetworkTimeoutController<WitchesRequestResponse> networkController, INetworkMonitor networkMonitor, IScreenFactory screenFactory=null) //: base(screenFactory)
		{
			if(networkController == null || networkMonitor == null || screenFactory == null)
			{
				throw new ArgumentNullException();
			}
			
			_networkController = networkController;
			_networkMonitor = networkMonitor;
			_screenFactory = screenFactory;
		}


		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			Func<INetworkTransportLayer> request = () => _networkController.Send (url, parms, onSuccess, onFailure, timeout);
			return SendRequest (request);
		}
		
		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			Func<INetworkTransportLayer> request = () => _networkController.Receive (url, parms, onSuccess, onFailure, timeout);
			return SendRequest (request);
		}

		private INetworkTransportLayer SendRequest(Func<INetworkTransportLayer> request)
		{
			if(_networkMonitor.IsConnected)			// Network.HavePublicAddress()
			{
//				AmbientLogger.Current.Log ("WitchesNetworkConnectionCheckController::SendRequest >>> Has Connection...", LogLevel.INFO);
				return request();
			}
			else
			{
//				AmbientLogger.Current.Log ("WitchesNetworkConnectionCheckController::SendRequest >>> No Connection...", LogLevel.WARNING);
				ShowRetryDialogue(request, false);

				return default(INetworkTransportLayer);	// FIXME: Not great...
			}
		}


		private void ShowRetryDialogue(Func<INetworkTransportLayer> request, bool allowClose=true)
		{
			var dialogue = _screenFactory.GetDialog<iGUISmartPrefab_RetryCloseDialog> ();
			dialogue.btn_close.setEnabled (allowClose);
//			dialogue.message = 
			dialogue.Display ((choice) => 
			{
				switch((DialogResponse)choice)
				{
					case DialogResponse.OK:
					default:
						SendRequest(request); break;
				}
			});
		}




    }
    
}




