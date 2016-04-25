
using System;
using System.Collections.Generic;

namespace Voltage.Story.Variables
{
	using Voltage.Common.Logging;
	using Voltage.Story.Mapper;
	
	
	public abstract class VariableMapper : IMapping<string> 
	{
		
		public Dictionary<string,Func<object>> Map { get; protected set; }	// NOTE: not really protected set
		protected ILogger Logger { get; private set; }
		
		public VariableMapper (ILogger logger)
		{
			Map = new Dictionary<string, Func<object>>();
			
			Logger = logger;
		}

		public virtual bool TryGetValue(string key, out object value)
		{
			if (string.IsNullOrEmpty(key) || !Map.ContainsKey(key))
			{
				value = null;
				return false;
			}

			Func<object> func = Map[key];
			try
			{
				value = func();
			}
			catch (Exception)
			{
				Logger.Log("Error during variable translation for key: " + key, LogLevel.WARNING);
				throw;
			}

			return true;
		}

		public virtual bool TryGetValue(string key, out int value)
		{
			object obj;
			bool success = TryGetValue(key, out obj);

			value = (success) ? (int)obj : 0;
			return success;
		}

		public virtual bool TryGetValue<T> (string key, out T value)
		{
			object obj;
			bool success = TryGetValue(key, out obj);

			value = (success) ? (T)obj : default(T);
			return success;
		}
	}
}


