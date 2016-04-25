
using System;
using System.Collections.Generic;

namespace Voltage.Common.Metrics
{

	public abstract class AmbientMetricManager 
	{
		private static IMetricManager _current;
		
		public static IMetricManager Current
		{
			get
			{
				return _current;
			}
			set
			{
				if(value == null)
				{
					Console.WriteLine ("AmbientMetricManager::Current >>> Setting to default");
					value = new DefaultMetricManager();
				}
				
				_current = value;
			}
		}
		
		private AmbientMetricManager(){}
		static AmbientMetricManager()
		{
			_current = new DefaultMetricManager ();
		}
		
		private class DefaultMetricManager : IMetricManager
		{
            public void LogEvent(string eventName)
            {
                LogEvent(eventName, new Dictionary<string, object>());
            }

			public void LogEvent (string eventName, IDictionary<string,object> parms)
			{
				Console.WriteLine ("LogEvent: " + eventName); 	// Do nothing???
			}
		}
	}
    
}




