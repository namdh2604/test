using System;

namespace Voltage.Witches.DI
{
	using Voltage.Common.Net;
	using Voltage.Witches.Net;

	using Voltage.Story.General;
	using Voltage.Witches.User;
    using Voltage.Witches.Exceptions;


	public class NewPlayerFetcher
	{
		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private readonly IParser<PlayerDataStore> _parser;		

        public NewPlayerFetcher(INetworkTimeoutController<WitchesRequestResponse> networkController, IParser<PlayerDataStore> parser)
		{
            if(networkController == null || parser == null)
			{
				throw new ArgumentNullException();
			}
			
			_networkController = networkController;
			_parser = parser;
		}
		
		public void Fetch(Action<Exception, PlayerDataStore> callback)
		{
            _networkController.Send(URLs.POST_CREATE_USER, null, (response) => RequestNewPlayerSuccess(response, callback), (response) => RequestNewPlayerFailed(response, callback)); 
		}
		
		private void RequestNewPlayerSuccess (WitchesRequestResponse response, Action<Exception, PlayerDataStore> callback)
		{
            try
            {
    			PlayerDataStore playerData = _parser.Parse(response.Text);	
                callback(null, playerData);
            }
            catch (Exception e)
            {
                callback(e, null);
            }
		}
		
		private void RequestNewPlayerFailed(WitchesRequestResponse payload, Action<Exception, PlayerDataStore> callback)
		{
            callback(new InvalidPlayerDataException(), null);
		}
		
	}
}




