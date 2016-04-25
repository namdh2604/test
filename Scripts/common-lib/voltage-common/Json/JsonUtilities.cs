using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Voltage.Common.Json
{
    using Voltage.Common.Logging;

	public static class JsonUtilities
	{
		public static List<JToken> TokenizeJsons(params string[] jsons)
		{
			List<JToken> tokens = new List<JToken>();
			
			if (jsons != null)
			{
				foreach (string json in jsons)
				{
					tokens.Add(TokenizeJson(json));	// tokens = new List<string>(jsons).ConvertAll((j) => (JToken.Parse(j)));
				}
			}
			
			return tokens;
		}
		
		public static JToken TokenizeJson (string json)
		{
			if (!string.IsNullOrEmpty(json))
			{
				return JToken.Parse(json);
			}

			return default(JToken);
		}

        public static List<T> ConvertJsonToListOf<T>(Func<string, JToken, T> createDelegate, IDictionary<string, string> jsons, JToken config, string errorFmt = "")
		{
			List<T> list = new List<T>();
            List<string> failedCreations = new List<string>();

            foreach (var keyval in jsons)
            {
                string filePath = keyval.Key;
                string rawJson = keyval.Value;
                T constructedObject = default(T);
                try
                {
                    constructedObject = createDelegate(rawJson, config);
                }
                catch (Exception e)
                {
                    failedCreations.Add(filePath);
                    if (!string.IsNullOrEmpty(errorFmt))
                    {
                        string errorString = string.Format(errorFmt, filePath, e.Message);
                        AmbientLogger.Current.Log(errorString, LogLevel.WARNING);
                    }
                    continue; // compile a list of all errors, rather than stopping on the first one
                }

                list.Add(constructedObject);
            }

            if (failedCreations.Count > 0)
            {
                string failurePaths = string.Join(", ", failedCreations.ToArray());
                AmbientLogger.Current.Log("failures constructing: " + failurePaths, LogLevel.WARNING);
            }
			
			return list;
		}


		public static JToken FindToken(this JToken token, string key, string value)
		{
			return token.FindToken((t) => 
    		{
				JToken tryToken = t.Value<string>(key) ?? null;
				if (tryToken != null)
				{
					return t[key].ToString() == value;
				}
				
				return false;
			}); 
		}

		public static JToken FindToken (this JToken token, Predicate<JToken> predicate)
		{
			JToken foundToken = null;

			WalkToken(token, (t) => 
			{
				if (predicate(t))
				{
					foundToken = t;
				}
			});

			return foundToken;
		}

		public static List<JToken> FindAllTokens(this JToken token, Predicate<JToken> predicate)
		{
			List<JToken> foundTokens = new List<JToken> ();
			WalkToken(token, (t) => 
			{
				if(predicate(t))
				{
					foundTokens.Add(t);
				}
			});
			
			return foundTokens;
		}

	
		public static void WalkToken(JToken token, Action<JObject> action)	// TODO: shortcircuit search, currently runs through everything
		{
			if (token.Type == JTokenType.Object)
			{
				action((JObject)token);
				
				foreach (JProperty child in token.Children<JProperty>())
				{
					WalkToken(child.Value, action);
				}
			}
			else if (token.Type == JTokenType.Array)
			{
				foreach (JToken child in token.Children())
				{
					WalkToken(child, action);
				}
			}
		}
	}
}

