
using System;
using System.Collections.Generic;

namespace Voltage.Common.Metrics
{
	public interface IMetricManagerFactory
	{
		IMetricManager Create (IDictionary<string,IDictionary<string,object>> metricConfigs);	// IDictionary<string,object> or IDictionary<string,string> or MetricConfigurations ???
	}

//	public class MetricConfigurations
//	{
//		public IDictionary<string,IDictionary<string,object>> Configurations { get; set; }
//	}

}


namespace Voltage.Witches.Metrics
{
	using Voltage.Common.Metrics;

	using Voltage.Witches.Models;
	using Voltage.Common.Logging;
	using com.adjust.sdk;

	using Newtonsoft.Json;			// TODO: remove and replace with parser
	using Newtonsoft.Json.Linq;
//	using UnityEngine;

    public class WitchesMetricManagerFactory : IMetricManagerFactory
    {
		private readonly Player _player;
		private readonly ILogger _logger;

		public WitchesMetricManagerFactory (Player player, ILogger logger)	// TODO: pass in parser
		{
			if(player == null || logger == null)
			{
				throw new ArgumentNullException("WitchesMetricManagerFactory::Ctor >>>");
			}

			_player = player;
			_logger = logger;
		}



		public IMetricManager Create (IDictionary<string,IDictionary<string,object>> metricConfigs)
		{
			List<IMetricManager> managers = GetManagersFromConfig (metricConfigs);

//			if(managers.Count > 0)
			{
				CompositeMetricManager composite = new CompositeMetricManager (managers.ToArray());

				return new WitchesMetricManager (_player, composite, _logger);
			}
//			else
//			{
//				_logger.Log ("WitchesMetricManagerFactory::Create >>> No Configured Managers", LogLevel.WARNING);
//				return null;	// return null ???
//			}
		}


		private List<IMetricManager> GetManagersFromConfig (IDictionary<string,IDictionary<string,object>> metricConfigs)
		{
			List<IMetricManager> managers = new List<IMetricManager> ();
			
			foreach(KeyValuePair<string,IDictionary<string,object>> kvp in metricConfigs)
			{
				switch(kvp.Key)
				{
					case "flurry":
						managers.Add(GetFlurryManager(kvp.Value)); break;	// NOTE: can be an issue if key exists but value is empty
						
					case "adjust":
						managers.Add(GetAdjustManager(kvp.Value)); break;	// NOTE: can be an issue if key exists but value is empty

					default:
						throw new NotSupportedException(string.Format("WitchesMetricManagerFactory::Create >>> {0} manager not supported", kvp.Key));
				}
			}

			return managers;
		}



		private IMetricManager GetFlurryManager(IDictionary<string,object> parms)
		{
//			if(parms != null && parms.ContainsKey("key"))
			{
				string key = parms["key"].ToString();	// CAN THROW EXCEPTION!

				return new WitchesFlurryMetricManager(key);
			}
//			else
//			{
//				throw new ArgumentNullException("WitchesMetricManagerFactory::GetFlurryManager >>> No Key Found");
//			}
		}

		private IMetricManager GetAdjustManager(IDictionary<string,object> parms)
		{
//			if(parms != null && parms.ContainsKey("key") && parms.ContainsKey("is_production"))
			{
				string key = parms["key"].ToString(); 	// CAN THROW EXCEPTION!
				IDictionary<string,string> tokens = ParseTokens(parms);

				bool isProduction = (bool)parms["is_production"];	// CAN THROW EXCEPTION!
				AdjustEnvironment environment = GetAdjustEnvironment(isProduction);

				return new WitchesAdjustMetricManager(key, environment, AdjustLogLevel.Info, tokens);

//				throw new PlatformNotSupportedException("WitchesMetricManagerFactory::GetAdjustManager >>> Platform not Supported");
			}
//			else
//			{
//				throw new ArgumentNullException("WitchesMetricManagerFactory::GetAdjustManager >>> No Key Found");
//			}
		}

		private AdjustEnvironment GetAdjustEnvironment(bool isProduction)
		{
			if(isProduction)
			{
				return AdjustEnvironment.Production;
			}
			else
			{
				return AdjustEnvironment.Sandbox;
			}
		}


		private IDictionary<string,string> ParseTokens(IDictionary<string,object> parms)
		{
			if(parms.ContainsKey("tokens"))
			{
				return JsonConvert.DeserializeObject<Dictionary<string,string>>(parms["tokens"].ToString());	// TODO: replace with parser
			}
			else
			{
				throw new ArgumentException("WitchesMetricManagerFactory::ParseTokens >>> No Tokens");
			}
		}

    }
    
}







