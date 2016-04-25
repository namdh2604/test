
using System;
using System.Collections.Generic;

namespace Voltage.Story.Text
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Voltage.Common.Logging;
	using Voltage.Story.General;
	using Voltage.Story.Mapper;


    public class VariableTextParser : IParser<string>
    {
		public ILogger Logger { get; private set; }

		public IMapping<string> VariableMapper { get; private set; }

		public VariableTextParser (IMapping<string> variableMapper, ILogger logger)
		{
			VariableMapper = variableMapper;
			Logger = logger;
		}

		public string Parse(string json)
		{
			string text = string.Empty;	// "[MISSING]"

			if(!string.IsNullOrEmpty(json) && VariableMapper != null)
			{
				JArray tokenArray = JArray.Parse(json);
				if(tokenArray != null)				// NOTE: maybe don't need to test for null, but may need to catch JArray.Parse exception
				{
					foreach(JToken innerToken in tokenArray)
					{
						if(innerToken is JObject)
						{
							try
							{
								// NOTE: presently only 'Variable' is supported
								string value;
								if(VariableMapper.TryGetValue<string>(innerToken["text"].ToString(), out value))
								{
									text += value;
								}
								else
								{
									text += "[ERROR]";
								}
							}
							catch (KeyNotFoundException e)
							{
								Logger.Log (e.ToString(), LogLevel.WARNING);
							}
							catch (NullReferenceException e)
							{
								Logger.Log (e.ToString(), LogLevel.WARNING);
							}
						}
						else
						{
							text += innerToken.ToString();
						}
					}
				}

			}

            return text.Trim();
		}


    }
    
}




