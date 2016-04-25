
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Net;

    public class WitchesNetworkController : INetworkTimeoutController<WWWNetworkPayload>
    {
		private const int DEFAULT_TIMEOUT = 30;

		private readonly INetworkTimeoutController<WWWNetworkPayload> _networkController;

		public string BaseURL { get; set; }

        public WitchesNetworkController(INetworkTimeoutController<WWWNetworkPayload> networkController, string baseURL="")
		{
			if(networkController == null)
			{
				throw new ArgumentNullException("WitchesNetworkController::Ctor >>> " );
			}

			_networkController = networkController;
			BaseURL = !string.IsNullOrEmpty(baseURL) ? baseURL : string.Empty;

			Voltage.Common.Logging.AmbientLogger.Current.Log (string.Format ("Created WitchesNetworkController [{0}]", baseURL), Voltage.Common.Logging.LogLevel.INFO);
		}


		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout=DEFAULT_TIMEOUT)
		{
			// TODO: Check for Connection

			url = FormURL (url);		

			return _networkController.Send (url, parms, onSuccess, onFailure, timeout);
		}

		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout=DEFAULT_TIMEOUT)
		{
			// TODO: Check for Connection

			url = FormURL (url);

			return _networkController.Receive (url, parms, onSuccess, onFailure, timeout);
		}	

		private string FormURL(string url)
		{
			return BaseURL + (url[0] == '/' ? url : "/" + url);	// NOTE: can throw exception is url is null, maybe DON'T bother with the '/' check
//			return BaseURL + "/" + url;
		}

    }
    
}




