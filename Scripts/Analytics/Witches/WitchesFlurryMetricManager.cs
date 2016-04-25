
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Metrics
{
	using Voltage.Common.Metrics;

	using Analytics;
	using UnityEngine;

	using Voltage.Common.Logging;


	public sealed class WitchesFlurryMetricManager : IMetricManager
	{

//		private IList<string> TimedEvents = new List<string>
//		{
//			MetricEvent.TUTORIAL_STARTED,
//			MetricEvent.SCENE_READ,
//		};
//
//		private IList<string> EndTimedEvents = new List<string>
//		{
//			MetricEvent.TUTORIAL_COMPLETED,
//			MetricEvent.SCENE_COMPLETED,
//		};


		private IAnalytics _flurryService;
		
		public WitchesFlurryMetricManager (string apiKey)	// List<string> validEvents
		{
			_flurryService = Flurry.Instance;
			_flurryService.SetLogLevel(Analytics.LogLevel.All);

			switch(Application.platform)
			{
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.IPhonePlayer:
					FlurryIOS.SetDebugLogEnabled (true);
					FlurryIOS.SetShowErrorInLogEnabled (true);
//					FlurryIOS.SetLogLevel(Analytics.LogLevel.CriticalOnly);
					FlurryIOS.SetCrashReportingEnabled (true);
					_flurryService.StartSession(apiKey, string.Empty); break;

				case RuntimePlatform.Android:
					_flurryService.StartSession(string.Empty, apiKey); break;

				default:
					throw new PlatformNotSupportedException("WitchesFlurryMetricManager::Ctor >>> Platform not Supported");
			}

		}

        public void LogEvent(string eventName)
        {
            LogEvent(eventName, new Dictionary<string, object>());
        }
		
		public void LogEvent (string eventName, IDictionary<string,object> parms)
		{
//			try
//			{
				switch(eventName)
				{
					case MetricEvent.STORY_SCENE_READ:
//						Console.WriteLine ("Flurry (begin timed): " + eventName);
//						_flurryService.EndLogEvent (eventName, AnalyticsUtils.ParseCollectionParametersToJson(parms)); break;

					case MetricEvent.STORY_SCENE_COMPLETED:
//						Console.WriteLine ("Flurry (end timed): " + eventName);
//						_flurryService.EndLogEvent (eventName, AnalyticsUtils.ParseCollectionParametersToJson(parms)); break;

					default: 
//						AmbientLogger.Current.Log ("Flurry (normal): " + eventName, Voltage.Common.Logging.LogLevel.INFO);
						_flurryService.LogEvent (eventName, AnalyticsUtils.ParseCollectionParametersToJson(parms)); break;
				}
//			}
//			catch(Exception e)
//			{
//				Debug.LogError(string.Format ("WitchesFlurryMetricManager::LogEvent >>> EXCEPTION: {0}", e.Message));
//				throw e;
//			}

		}
	}

    
}






