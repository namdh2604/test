
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Logging;
	using Voltage.Common.Net;

	public class WitchesNetworkLoggingController : INetworkTimeoutController<WitchesRequestResponse>
    {
		private readonly ILogger _logger;
		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;

		public WitchesNetworkLoggingController (ILogger logger, INetworkTimeoutController<WitchesRequestResponse> networkController) 
		{
			if(logger == null || networkController == null)
			{
				throw new ArgumentNullException();
			}

			_logger = logger;
			_networkController = networkController;
		}


		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			Action<WitchesRequestResponse> logSuccess = (response) => LogSuccessfulResponse (response, onSuccess);
			Action<WitchesRequestResponse> logFailure = (response) => LogFailedResponse (response, onFailure);

			return _networkController.Send (url, parms, logSuccess, logFailure, timeout);
		}
		
		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			Action<WitchesRequestResponse> logSuccess = (response) => LogSuccessfulResponse (response, onSuccess);
			Action<WitchesRequestResponse> logFailure = (response) => LogFailedResponse (response, onFailure);
			
			return _networkController.Receive (url, parms, logSuccess, logFailure, timeout);
		}



		private void LogSuccessfulResponse  (WitchesRequestResponse response, Action<WitchesRequestResponse> callback)
		{
			_logger.Log (string.Format ("Request <{0}> Successful\n{1}", response.Request.ToString(), response.Text), LogLevel.INFO);
			
			if(callback != null)
			{
				callback(response);
			}
		}
		
		
		private void LogFailedResponse (WitchesRequestResponse response, Action<WitchesRequestResponse> callback)
		{
			_logger.Log (string.Format ("Request <{0}> Failed\n{1}", response.Request.ToString(), response.Text), LogLevel.WARNING);
			
			if(callback != null)
			{
				callback(response);
			}
		}

    }

    
}




