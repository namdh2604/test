
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Metrics
{
	using Voltage.Common.Metrics;

	using UnityEngine;
	using com.adjust.sdk;		// using AdjustUtil = com.adjust.sdk.AdjustUtil;

	using Voltage.Common.Logging;

	public sealed class WitchesAdjustMetricManager : IMetricManager
	{
		private readonly IDictionary<string,string> _tokenMap;
		private readonly Adjust _adjust;
		
		public WitchesAdjustMetricManager(string token, AdjustEnvironment environment, AdjustLogLevel logLevel, IDictionary<string,string> tokenMap)
		{
			if(string.IsNullOrEmpty(token) || tokenMap == null)
			{
				throw new ArgumentNullException("WitchesAdjustMetricManager::Ctor >>>");
			}

			_tokenMap = tokenMap;

			GameObject go = Resources.Load<GameObject> ("Adjust");
			go.GetComponent<Adjust>().startManually = true;	

			_adjust = (MonoBehaviour.Instantiate (go) as GameObject).GetComponent<Adjust>();

			AdjustConfig adjustConfig = new AdjustConfig (token, environment);
			adjustConfig.setLogLevel (logLevel);
//			adjustConfig.setAttributionChangedDelegate (this.attributionChangedDelegate);
			
			Adjust.start (adjustConfig);
		}

        public void LogEvent(string eventName)
        {
            LogEvent(eventName, new Dictionary<string, object>());
        }
		
		public void LogEvent (string eventName, IDictionary<string,object> parms)
		{
//			LogAdjustDetails ();

			if(eventName == MetricEvent.SHOP_STARSTONE_BOUGHT || eventName == MetricEvent.SHOP_STAMINA_POTION_BOUGHT)
			{
				Adjust.trackEvent(CreateRevenueEvent(_tokenMap[eventName], parms));
			}

			if(eventName == MetricEvent.TUTORIAL_STARTED || eventName == MetricEvent.TUTORIAL_COMPLETED)
			{
				Adjust.trackEvent (CreateEvent(_tokenMap[eventName]));
			}
		}

		private AdjustEvent CreateRevenueEvent(string eventToken, IDictionary<string,object> parms)
		{
			AmbientLogger.Current.Log ("Adjust - Creating Revenue Event for: " + eventToken, LogLevel.INFO);

			double price = GetPrice (parms);

			AdjustEvent adjustEvent = new AdjustEvent (eventToken);
			adjustEvent.setRevenue (price, "USD");

			return adjustEvent;
		}

		private AdjustEvent CreateEvent(string eventToken)
		{
			AmbientLogger.Current.Log ("Adjust - Creating Event for: " + eventToken, LogLevel.INFO);
			return new AdjustEvent (eventToken);
		}


		private double GetPrice(IDictionary<string,object> parms)
		{
			object priceObj;
			if(parms != null && parms.TryGetValue("iap_paid", out priceObj))
			{
				return Convert.ToDouble(priceObj);	// NOTE: Can throw cast/format exception!
			}

			return -1;
		}



		private void LogAdjustDetails()
		{
			string token = !string.IsNullOrEmpty (_adjust.appToken) ? _adjust.appToken : string.Empty;
			AmbientLogger.Current.Log (string.Format ("Adjust: {0}, {1}, {2}", token, _adjust.environment.ToString (), _adjust.logLevel.ToString ()), LogLevel.INFO);
		}
	}
    
}


//		private IDictionary<string,string> _tokenMap = new Dictionary<string,string>		// TODO: pass in as a dependency
//		{
//			// Revenue
//			{MetricEvent.STAMINA_POTION_BOUGHT, "yzapr6"},
//			{MetricEvent.STARSTONE_BOUGHT, "un633m"},
//
//			// Non-Revenue
//			{MetricEvent.TUTORIAL_STARTED, ""},
//			{MetricEvent.TUTORIAL_COMPLETED, ""},
//		};




