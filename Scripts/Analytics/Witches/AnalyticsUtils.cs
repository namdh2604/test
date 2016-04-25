
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Metrics
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class AnalyticsUtils
	{
		public static Dictionary<string,string> ParseCollectionParametersToJson(IDictionary<string,object> parms)	// or rename ParseParametersToString ???
		{
			Dictionary<string,string> parameters = new Dictionary<string,string> ();
			
			foreach(KeyValuePair<string,object> kvp in parms)
			{
				string value = kvp.Value != null ? kvp.Value.ToString() : string.Empty;	// NOTE: also converts single elements to string!
				if (kvp.Value is ICollection)
				{
					value = JsonConvert.SerializeObject(kvp.Value, Formatting.Indented);
				}

				parameters.Add(kvp.Key, value);
			}
			
			return parameters;
		}
	}
    
}




