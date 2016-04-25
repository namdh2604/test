
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Converters
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	
	using Voltage.Common.Converters;
	
	
	public class ListToJsonConverter<T>
	{
		public string Convert(List<T> original)
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