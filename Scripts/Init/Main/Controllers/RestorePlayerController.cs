
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using System.Diagnostics;

	using Voltage.Story.General;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Witches.User;
    

	// {'password': '6kpx', 'user_id': u'78191790'}


	public class RestorePlayerController
    {
		private INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private IParser<PlayerDataStore> _playerDataParser;
        private IPlayerWriter _writer;
//		private IStartupErrorController _errorController;

        public RestorePlayerController(INetworkTimeoutController<WitchesRequestResponse> networkController, IParser<PlayerDataStore> playerDataParser, IPlayerWriter writer)
		{
//			Debug.Assert (networkController != null, "RestorePlayerController::Ctor >>> No NetworkController Given");
            if(networkController == null || playerDataParser == null || writer ==  null )
			{
				throw new ArgumentNullException();
			}

			_networkController = networkController;
			_playerDataParser = playerDataParser;
            _writer = writer;
		}

		public void RequestRestore(string playerID, string password, Action onSuccess, Action onFailure)
		{
//			try
			{
				if(ValidInput(playerID, password))
				{
					Dictionary<string,string> parms = new Dictionary<string,string>()
					{
						{"phone_id", playerID},
						{"password", password}
					};

					_networkController.Send(URLs.RESTORE_PLAYER, parms, (payload) => OnRestoreSuccess(payload, onSuccess), (payload) => OnRestoreFailed(payload, onFailure));
				}
				else
				{
					onFailure();	
				}
			}
//			catch (NullReferenceException e)
//			{
//				Console.WriteLine(e.ToString());	// temp
//				_errorController.ErrorOnRestorePlayer();
//			}
		}

		private void OnRestoreSuccess(WitchesRequestResponse payload, Action onSuccess)
		{
//			try
			{
				PlayerDataStore dataStore = _playerDataParser.Parse (payload.Text);		
                _writer.Save(dataStore);
				
				onSuccess ();
			}
//			catch (Exception e)
//			{
//				Console.WriteLine(e.ToString());	// temp
//				_errorController.ErrorOnRestorePlayer();
//			}
		}

		private void OnRestoreFailed(WitchesRequestResponse payload, Action onFailure)
		{
			onFailure ();
		}

		private bool ValidInput (string playerID, string password)
		{
			return !string.IsNullOrEmpty (playerID) && !string.IsNullOrEmpty (password);
		}



    }
    
}









