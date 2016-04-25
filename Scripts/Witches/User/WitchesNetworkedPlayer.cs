
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace Voltage.Witches.Models
{
	using Voltage.Witches.User;

	using Voltage.Common.Net;
	using Voltage.Witches.Net;
    using Voltage.Witches.Services;
	using Voltage.Common.Logging;
	using Voltage.Witches.Converters;
    using Voltage.Witches.Models.Avatar;

	using System.Linq;

	using Voltage.Witches.Configuration;
	using Voltage.Witches.Login;

	public class WitchesNetworkedPlayer : PersistentPlayer// HACK:should be an interface/decorator
	{
		private readonly ILogger _logger;

        private readonly IMonitoringNetworkController _networkController;

		private readonly DictionaryToJsonConverter<string,int> _affinityJsonConverter; 
		private readonly DictionaryToJsonConverter<string,string> _choiceJsonConverter; 
        private readonly PlayerStaminaManager _staminaManager;
        private readonly PlayerFocusManager _focusManager;

        private int _outstandingStaminaUpdateRequests;
        private int _offlineStaminaRequests;

        private int STAMINA_TIMEOUT = 10;
        private int FOCUS_TIMEOUT = 10;

        public WitchesNetworkedPlayer (IMonitoringNetworkController networkController, ILogger logger, List<Spellbook> books, PlayerDataStore dataStore, IPlayerWriter playerWriter, 
		                               DictionaryToJsonConverter<string,int> affinityJsonConverter, DictionaryToJsonConverter<string,string> choiceJsonConverter, 
			PlayerStaminaManager staminaManager, PlayerFocusManager focusManager) : base (books, dataStore, playerWriter, staminaManager, focusManager)	
		{
			if (networkController == null || logger == null)
			{
				throw new ArgumentNullException("WitchesNetworkPlayer::Ctor >>>");
			}

			
			_logger = logger;
			_networkController = networkController;
			_affinityJsonConverter = affinityJsonConverter;
			_choiceJsonConverter = choiceJsonConverter;

            _staminaManager = staminaManager;
            _focusManager = focusManager;

            _outstandingStaminaUpdateRequests = 0;
            _offlineStaminaRequests = 0;
		}

		public override void SetPlayerName(string firstName, string lastName)
		{
			base.SetPlayerName(firstName, lastName);
			IDictionary<string,string> data = GetBaseUpdateData();
			data.Add("first_name", firstName);
			data.Add("last_name", lastName);

			_networkController.Send(URLs.INPUT_NAMES, data, ((payload) => HandleNameInputs(true)), ((payload) => HandleNameInputs(false)));
		}

		public override void DeductStamina()		// NOTE: will need to consider interface if callback required
		{
			base.DeductStamina();
            if (_networkController.IsConnected)
            {
                UpdateServerStaminaDeducted();
            }
            else
            {
                _offlineStaminaRequests += 1;
            }

			CachePlayerState ();
		}

        public override void StartScene(string sceneId)
        {
            base.StartScene(sceneId);
            IDictionary<string, string> data = GetBaseUpdateData();
            data["scene_path"] = sceneId;

            // TODO: Currently fire and forget, most likely needs to block before allowing user to continue
            _networkController.Send(URLs.START_SCENE, data, null, null);
        }
		
		public override void CompleteScene(Action<bool> onComplete)				// NOTE: will need to consider interface if callback required
		{
            IDictionary<string,string> data = GetBaseUpdateData ();
            data.Add ("pendingStaminaPotions", GetBonusStaminaPotions().ToString());

            Action<WitchesRequestResponse> onSuccess = (_) => HandleSceneComplete(true, onComplete);
            Action<WitchesRequestResponse> onFailure = (_) => HandleSceneComplete(false, onComplete);

            _networkController.Send(URLs.UPDATE_COMPLETED_SCENE, data, onSuccess, onFailure);
		}

        protected override void HandleSceneComplete(bool success, Action<bool> onComplete)
        {
            base.HandleSceneComplete(success, onComplete);

            if (success)
            {
                CachePlayerState();
            }
        }

		public override void CompleteRoute ()
		{
			base.CompleteRoute ();
			UpdateServerPlayerReset ();
		}

        public override void UpdateStamina()
        {
            // HACK -- update the last updated timestamp so the task won't be fired multiple times prior to returning
            // on successful return, the last update will be overwritten anyhow
            int fauxNextUpdate = (int)(StaminaNextUpdate.Subtract(DateTime.Now) + new TimeSpan(0, 0, STAMINA_TIMEOUT + 1)).TotalSeconds;
            _staminaManager.SetAmount(_staminaManager.Amount, fauxNextUpdate);
            IDictionary<string, string> data = GetBaseUpdateData();
            _networkController.Send(URLs.UPDATE_STAMINA, data, HandleStaminaUpdate, HandleStaminaUpdate, STAMINA_TIMEOUT);
        }

        public override void UpdateFocus()
        {
            int fauxNextUpdate = (int)(FocusNextUpdate.Subtract(DateTime.Now) + new TimeSpan(0, 0, FOCUS_TIMEOUT + 1)).TotalSeconds;
            _focusManager.SetAmount(_focusManager.Amount, fauxNextUpdate);
            IDictionary<string, string> data = GetBaseUpdateData();
            _networkController.Send(URLs.UPDATE_FOCUS, data, HandleFocusUpdate, HandleFocusUpdate, FOCUS_TIMEOUT);
        }

        public override void Refresh()
        {
            SyncResources();
        }

        public void SyncResources()
        {
            IDictionary<string, string> data = GetBaseUpdateData();
            data.Add("stamina", Stamina.ToString());
            data.Add("focus", Focus.ToString());
            data.Add("offlineStamina", _offlineStaminaRequests.ToString());
			data.Add("pendingStaminaPotions", GetBonusStaminaPotions().ToString());
            // No more outstanding requests -- regardless of what the server has, we're telling it our state
            _outstandingStaminaUpdateRequests = 0;
            _offlineStaminaRequests = 0;

            _networkController.Send(URLs.SYNC_RESOURCES, data, HandleResourceUpdate, (_) => HandleFailure("SyncResources"));
        }

		private int GetBonusStaminaPotions()
		{
			int bonusStaminaPotions = 0;

				// bonusItem can be empty if user has already received the items and not eligible for next bonus yet
				if (BonusItems.Count() > 0 && BonusItems [BonusManager.BONUS_INDEX].ID == MasterConfiguration.STAMINA_POTION_ID) 
				{
					bonusStaminaPotions = BonusItems [BonusManager.BONUS_INDEX].Quantity;
				}
			return bonusStaminaPotions;
		}

        private void HandleResourceUpdate(WWWNetworkPayload payload)
        {
            string response = payload.WWW.text;
            JObject jsonResponse = JObject.Parse(response);

            int stamina = jsonResponse.Value<int>("stamina");
            int nextStamUpdateDelta = jsonResponse.Value<int>("stamina_next_update_delta");
            int staminaPotions = jsonResponse.Value<int>("stamina_potion");

			staminaPotions -= GetBonusStaminaPotions ();

            this.UpdateStaminaPotion(staminaPotions - this.StaminaPotions);
            ProcessStaminaUpdate(stamina, nextStamUpdateDelta);

            int focus = jsonResponse.Value<int>("focus");
            int nextFocusUpdateDelta = jsonResponse.Value<int>("focus_next_update_delta");
            _focusManager.SetAmount(focus, nextFocusUpdateDelta);

            Serialize();
        }

        private void ProcessStaminaUpdate(int newStamina, int nextUpdateDelta)
        {
            if (_outstandingStaminaUpdateRequests > 0)
            {
                newStamina -= _outstandingStaminaUpdateRequests;
                _outstandingStaminaUpdateRequests = 0;
            }
            _staminaManager.SetAmount(newStamina, nextUpdateDelta);
            Serialize();
        }

        private void HandleStaminaUpdate(WWWNetworkPayload payload)
        {
			if ((payload == null) || (payload.WWW == null))
			{
				// Need to handle the time out case because we are not handling the timeout.
				// timeout result in a null WWW.
				AmbientLogger.Current.Log("Update Stamina Timeout", LogLevel.WARNING);
				return;
			}
            string response = payload.WWW.text;
            JObject jsonResponse = JObject.Parse(response);

            int stamina = jsonResponse.Value<int>("stamina");
            int nextUpdateDelta = jsonResponse.Value<int>("stamina_next_update_delta");

            ProcessStaminaUpdate(stamina, nextUpdateDelta);
        }

        private void HandleFocusUpdate(WWWNetworkPayload payload)
        {
			if ((payload == null) || (payload.WWW == null))
			{
				// Need to handle the time out case because we are not handling the timeout.
				// timeout result in a null WWW.
				AmbientLogger.Current.Log("Update Focus Timeout", LogLevel.WARNING);
				return;
			}
            string response = payload.WWW.text;
            JObject jsonResponse = JObject.Parse(response);

            int focus = jsonResponse.Value<int>("focus");
            int nextUpdateDelta = jsonResponse.Value<int>("focus_next_update_delta");

            _focusManager.SetAmount(focus, nextUpdateDelta);
            Serialize();
        }

        private void HandleFailure(string methodName)
        {
            string fmt = "{0}::{1} >>> Failed!";
            _logger.Log(string.Format(fmt, GetType().ToString(), methodName), LogLevel.WARNING);
        }

		private void UpdateServerStaminaDeducted()
		{
			IDictionary<string,string> data = GetBaseUpdateData ();
			data.Add ("scene_id", CurrentScene);
			data.Add ("node_id", CurrentNodeID);
			data.Add ("pendingStaminaPotions", GetBonusStaminaPotions().ToString());

            _outstandingStaminaUpdateRequests += 1;

			// TODO: add in ability to react to failed response from server and rollback/reset state
            _networkController.Send(URLs.UPDATE_PLAYERSTORY_STATE, data, 
                ((payload) => HandleServerStaminaDeducted(true)), 
                ((payload) => HandleServerStaminaDeducted(false)));
		}

        private void HandleServerStaminaDeducted(bool success)
        {
            if (_outstandingStaminaUpdateRequests > 0)
            {
                _outstandingStaminaUpdateRequests -= 1;
            }

            string result = (success) ? "Success!" : "Failed!";
            LogLevel level = (success) ? LogLevel.INFO : LogLevel.WARNING;

            _logger.Log("WitchesNetworkedPlayer::UpdateServerStaminaDeducted >>> " + result, level);
        }

		private void HandleNameInputs(bool success)
		{
			string result = (success) ? "Success!" : "Failed!";
			LogLevel level = (success) ? LogLevel.INFO : LogLevel.WARNING;
			_logger.Log("WitchesNetworkedPlayer::HandleNameInputs >>> " + result, level);
		}

		private IDictionary<string,string> GetBaseUpdateData()	
		{
			IDictionary<string,string> newChoices = GetOnlyNewChoices ();

			return new Dictionary<string,string>
			{
				{"phone_id", UserID},
				{"affinities", ParseAffinityToJson(CharacterAffinities) },
				{"stamina_potions", StaminaPotions.ToString() },
				{"choices", ParseChoicesToJson(newChoices)}, 
			};
		}

		private IDictionary<string,string> GetOnlyNewChoices()	// TODO: maybe replace with some player diff module
		{
			Dictionary<string,string> allChoices = GetAllSelectedChoices ();
			Dictionary<string,string> priorChoices = _priorPlayerState.sceneChoices;

			IEnumerable<KeyValuePair<string,string>> newChoices = allChoices.Where (kvp => !priorChoices.Contains (kvp));
			return newChoices.ToDictionary(kvp=>kvp.Key, kvp=>kvp.Value);
		}

		private PlayerDataStore _priorPlayerState = new PlayerDataStore();
		private void CachePlayerState()
		{
			_priorPlayerState = this.CloneDataStore ();
		}

		private string ParseAffinityToJson(IDictionary<string,int> affinity)
		{
			return _affinityJsonConverter.Convert (affinity);
		}

		private string ParseChoicesToJson(IDictionary<string,string> choices)
		{
			return _choiceJsonConverter.Convert (choices);
		}

		private void UpdateServerPlayerReset()
		{
			IDictionary<string,string> data = new Dictionary<string,string>
			{
				{"phone_id", UserID}
			};
			
			_networkController.Send (URLs.PLAYER_RESET, data, (payload) => AmbientLogger.Current.Log ("WitchesRouteResetter::SendPlayerReset >>> Success", LogLevel.INFO), (payload) => AmbientLogger.Current.Log ("WitchesRouteResetter::SendPlayerReset >>> Failed", LogLevel.WARNING));
		}

        public override void UpdateOutfit(Outfit outfit)
        {
            string clothingJson = JsonConvert.SerializeObject(outfit.GetClothingValues());

            Dictionary<string, string> parameters = new Dictionary<string, string>() {
                { "phone_id", UserID },
                { "new_outfit",  clothingJson }
            };
            _networkController.Send(URLs.UPDATE_OUTFIT, parameters, (payload) => AmbientLogger.Current.Log("Set outfit", LogLevel.INFO), 
                (payload) => AmbientLogger.Current.Log("Set outfit failure", LogLevel.WARNING));
        
            base.UpdateOutfit(outfit);
        }

		public override void SetTutorialProgress(int step, string name)
		{	base.SetTutorialProgress (step, name);
			string step_string = step.ToString ();

			Dictionary<string, string> parameters = new Dictionary<string, string> () 
			{
				{ "phone_id", UserID },
				{"tutorial_name", name},
				{"tutorial_progress", step_string}
			};
			_networkController.Send (URLs.TUTORIAL_PROGRESS, parameters, (payload) => AmbientLogger.Current.Log ("update tutorial progress: " + name, LogLevel.INFO), 
			                         (payload) => AmbientLogger.Current.Log ("update tutorial progress: " + name, LogLevel.WARNING));
		}


		public override void FinishTutorial()
		{
			base.FinishTutorial ();

			Dictionary<string, string> parameters = new Dictionary<string, string> () 
			{
			{ "phone_id", UserID },
			};
			_networkController.Send (URLs.FINISH_TUTORIAL, parameters, (payload) => AmbientLogger.Current.Log ("finish tutorial", LogLevel.INFO), 
		                        (payload) => AmbientLogger.Current.Log ("finish tutorial failure", LogLevel.WARNING));
		}

		public override void RefillStamina()
		{	
			base.RefillStamina ();
			IDictionary<string,string> refillStamina =  new Dictionary<string,string>
			{
				{"phone_id", UserID},
			};
			_networkController.Send(URLs.REFILL_STAMINA, refillStamina, HandleStaminaUpdate, HandleStaminaUpdate);
		}

	}

}




