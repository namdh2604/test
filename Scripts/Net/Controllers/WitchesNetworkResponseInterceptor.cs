
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Logging;
	using Voltage.Common.Net;

    public class WitchesNetworkResponseInterceptor : INetworkTimeoutController<WitchesRequestResponse>
    {

		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private readonly IDictionary<string,string> _responseMap;

		public WitchesNetworkResponseInterceptor(INetworkTimeoutController<WitchesRequestResponse> networkController, IDictionary<string,string> responseMap)	// TEMPORARY use of IScreenFactory
		{
			if(networkController == null || responseMap == null)
			{
				throw new ArgumentNullException();
			}
			
			_networkController = networkController;
			_responseMap = responseMap;
		}

		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			return _networkController.Send (url, parms, (response) => InterceptResponse (url, response, onSuccess), onFailure, timeout);
		}
		
		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			return _networkController.Receive (url, parms, (response) => InterceptResponse (url, response, onSuccess), onFailure, timeout);
		}


		private void InterceptResponse(string url, WitchesRequestResponse response, Action<WitchesRequestResponse> onSuccess)
		{
			if(_responseMap.ContainsKey(url))
			{
				WitchesRequestResponse newResponse = new WitchesRequestResponse()
				{
					Request = response.Request,
					WWW = response.WWW,
					Text = _responseMap[url],
				};

				onSuccess (newResponse);
			}
			else
			{
				AmbientLogger.Current.Log ("URL Not Found: " + url, LogLevel.INFO);
				onSuccess (response);
			}
		}

    }
    
}




