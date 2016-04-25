
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Metrics
{
	using Voltage.Common.Logging;
	using Voltage.Witches.Models;
	using Voltage.Common.Metrics;

    public class WitchesMetricManager : IMetricManager
    {
		private readonly Player _player;
		private readonly IMetricManager _metricManager;
		private readonly ILogger _logger;

		public IMetricManager DecoratedManager { get { return _metricManager; } }

		public WitchesMetricManager (Player player, IMetricManager metricManager, ILogger logger)
		{
			if(player == null || metricManager == null || logger == null)
			{
				throw new ArgumentNullException("WitchesMetricManager::Ctor >>> " );
			}

			_player = player;
			_metricManager = metricManager;
			_logger = logger;
		}

        public void LogEvent(string eventName)
        {
            LogEvent(eventName, new Dictionary<string, object>());
        }

		public void LogEvent(string eventName, IDictionary<string,object> parms)
		{
			if(!string.IsNullOrEmpty(eventName))
			{
				parms = AddCommonMetrics(parms);
				_metricManager.LogEvent(eventName, parms);

				LogMetricCall(eventName, parms);
			}
			else
			{
//				_logger.Log ("WitchesMetricManager::LogEvent >>> No Event Given", LogLevel.WARNING);	// or throw an exception
				throw new ArgumentNullException("WitchesMetricManager::LogEvent >>> No Event Given");
			}
		}

		private IDictionary<string,object> AddCommonMetrics (IDictionary<string,object> parms)
		{
			// NOTE: keys match event parameter name
			IDictionary<string,object> playerData = new Dictionary<string,object>
			{
				{"player_id", _player.UserID},
//				{"player_reg_date", _player.RegistrationDate},
				{"player_scenes", _player.CurrentScene},
				{"player_previous_scene", GetPriorScene() },
				{"player_coins_balance", _player.Currency},
				{"player_starstones_balance", _player.CurrencyPremium},
			};

			IDictionary<string,object> data = (parms != null ? parms : new Dictionary<string,object> ());

			foreach(KeyValuePair<string,object> kvp in playerData)
			{
				if(!data.ContainsKey(kvp.Key))
				{
					data.Add(kvp.Key, kvp.Value);
				}
//				else
//				{
//					_logger.Log (string.Format("WitchesMetricManager::AddCommonMetrics >>> Already contains key [{0}], taking provided", kvp.Key), LogLevel.WARNING);
//				}
			}

			return data;
		}

		private string GetPriorScene()
		{
			int completedSceneCount = _player.CompletedScenes.Count;

			return (completedSceneCount > 0 ? _player.CompletedScenes[completedSceneCount-1] : string.Empty);
		}

		private void LogMetricCall(string eventName, IDictionary<string,object> parms)
		{
			string parameters = string.Empty;
			foreach(var kvp in parms)
			{
				parameters += string.Format("\t{0}: {1}\n", kvp.Key, kvp.Value.ToString());
			}
			
			_logger.Log (string.Format ("METRIC: {0}\n{1}", eventName, parameters), LogLevel.INFO);
		}

    }
    
}




