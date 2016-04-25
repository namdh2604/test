
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Common.JsonNet.Utils
{
	using Newtonsoft.Json;

	// http://stackoverflow.com/questions/14427596/convert-an-int-to-bool-with-json-net
	public class BoolConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(((bool)value) ? 1 : 0);
		}
		
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return reader.Value.ToString() == "1";
		}
		
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(bool);
		}
	}
    
}




