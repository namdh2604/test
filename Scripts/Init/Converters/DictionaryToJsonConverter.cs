
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Converters
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Voltage.Common.Converters;


	public class DictionaryToJsonConverter<T,U> : IConverter<IDictionary<T,U>, string>
	{
		public string Convert(IDictionary<T,U> original)
		{
			if(original != null && original.Count > 0)
			{
				return JsonConvert.SerializeObject(original);
			}
			else
			{
				return "{}";
			}
		}
	}




    
}




