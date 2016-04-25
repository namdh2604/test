
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Witches.Configuration;

	using Voltage.Witches.Configuration.JSON;
    using Voltage.Witches.Exceptions;

	using Voltage.Witches.User;
	using UnityEngine;

    public class MasterConfigFetcher
    {
		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private readonly IMasterDataParser _masterParser;

        public MasterConfigFetcher(INetworkTimeoutController<WitchesRequestResponse> networkController, IMasterDataParser masterParser)
		{
            if(networkController == null || masterParser == null)
			{
				throw new ArgumentNullException();
			}

			_networkController = networkController;
			_masterParser = masterParser;
		}

		public void Fetch(Action<Exception, MasterConfiguration> callback)
		{
            _networkController.Receive (URLs.GET_MASTER, null, (response) => OnDataReceivedSuccess(response, callback), (response) => OnDataReceivedFailed(response, callback));
		}

		private void OnDataReceivedSuccess(WitchesRequestResponse response, Action<Exception, MasterConfiguration> callback)
		{
            try
            {
    			MasterConfiguration masterConfig = _masterParser.Construct(response.Text);
                callback(null, masterConfig);
            }
            catch (Exception e)
            {
                callback(e, null);
            }
		}

		private void OnDataReceivedFailed(WitchesRequestResponse response, Action<Exception, MasterConfiguration> callback)
		{
            callback(new MasterConfigurationException(), null);
		}
    }
    
}




