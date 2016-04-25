using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace Voltage.Witches.Login
{
    using Voltage.Witches.User;
    using Voltage.Witches.Net;
    using Voltage.Common.Net;
    using Voltage.Witches.Exceptions;

	using Voltage.Witches.DI;

	using Voltage.Common.Logging;

    public class WitchesLoginController : ILoginController
    {
        private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
        private readonly IPlayerDataSerializer _serializer;
        private readonly IPlayerWriter _playerWriter;
        
        public WitchesLoginController(INetworkTimeoutController<WitchesRequestResponse> networkcontroller, IPlayerDataSerializer serializer, IPlayerWriter playerWriter)
        {
            if(networkcontroller == null)
            {
                throw new ArgumentNullException();
            }
            
            _networkController = networkcontroller;
            _serializer = serializer;
            _playerWriter = playerWriter;

        }
        
		public void Execute(StartupData startupData, Action<Exception, bool> callback)
        {
            try
            {
				string playerJson = _serializer.Serialize(startupData.PlayerData);
                IDictionary<string,string> data = new Dictionary<string,string>
                {
					{"phone_id", startupData.PlayerData.userID},
                    {"device", GetDevice()},
                    {"player_json", playerJson},
					{"has_bonus", HasBonus(startupData.PlayerData).ToString()}
                };

				_networkController.Send(URLs.KPI_LOGIN, data, (response) => HandleLoginResponse(response, callback, startupData), (response) => HandleLoginFailure(response, callback));
            }
            catch (Exception e)
            {
                callback(e, false);
            }
        }

		private bool HasBonus(PlayerDataStore playerData)
		{
            // can't use BonusManager here as Player isn't constructed yet
			return (playerData.bonusItems != null) && (playerData.bonusItems.Count > 0);	// or check for exact size (ie, 4), but that value would need to be shared
		}

        private string GetDevice()
        {
            return Application.platform.ToString();
        }
        
		private void HandleLoginResponse(WitchesRequestResponse response, Action<Exception, bool> callback, StartupData startupData)
        {
            try
            {
				ProxyLoginResponse loginResponse = JsonConvert.DeserializeObject<ProxyLoginResponse>(response.Text);

				if(loginResponse.RestoreData != null)
				{
					AmbientLogger.Current.Log("WitchesLoginController::HandleLoginResponse >>> Restoring Player", LogLevel.INFO);
					startupData.PlayerData = loginResponse.RestoreData;
				}

				ModifyPlayerDataBonusInPlace(startupData.PlayerData, loginResponse.BonusItems);
               
				callback(null, loginResponse.TutorialFlag);
            }
            catch (Exception e)
            {
                callback(e, false);
            }
        }

		// modifies PlayerDataStore inplace
		private void ModifyPlayerDataBonusInPlace(PlayerDataStore playerData, IEnumerable<BonusItem> bonusItems)
		{
            playerData.bonusItems = (bonusItems != null ? new List<BonusItem>(bonusItems) : new List<BonusItem>());

            // should we serialize PlayerDataStore directly?
            // note the current state is serialized out from the in game state that gets passed along
            _playerWriter.Save(playerData);
		}


        private void HandleLoginFailure(WitchesRequestResponse response, Action<Exception, bool> callback)
        {
            callback(new LoginFailureException(), false);
        }



		private sealed class ProxyLoginResponse
		{
			[JsonProperty(PropertyName="tutorial_flag")]
			public bool TutorialFlag { get; set; }

			[JsonProperty(PropertyName="login_bonus_items")]
			public List<BonusItem> BonusItems { get; set; }

			[JsonProperty(PropertyName="restore_json")]
			public PlayerDataStore RestoreData { get; set; }
		}
    }


}







