
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Story.General;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Voltage.Common.Metrics;


	public class EnvironmentData
	{
		[JsonProperty(PropertyName = "base_url")]
		public string URL { get; set; }
		
		[JsonProperty(PropertyName = "latest")]
		public bool IsLatest { get; set; }		
		
		[JsonProperty(PropertyName = "metrics")]
		public IDictionary<string,IDictionary<string,object>> Metrics { get; set; }	// public IDictionary<string,IDictionary<string,string>> Metrics { get; set; }		// public IList<MetricConfig> Metrics { get; set; }

		[JsonProperty(PropertyName = "obb_path")]
		 public string OBBPath { get; set; }

		public EnvironmentData()
		{
			URL = string.Empty;
			IsLatest = false;
			Metrics = new Dictionary<string, IDictionary<string, object>>();

			OBBPath = string.Empty;
		}
	}
    
	public interface IEnvironmentParser
	{
		EnvironmentData Parse (string text);
	}

	public class EnvironmentParser : IParser<EnvironmentData>, IEnvironmentParser	// TODO: drop IEnvironmentParser
	{
		public EnvironmentData Parse (string text)
		{
			return JsonConvert.DeserializeObject<EnvironmentData> (text);
		}
	}
}
