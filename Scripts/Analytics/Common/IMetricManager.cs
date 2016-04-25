
//using System;
using System.Collections.Generic;

namespace Voltage.Common.Metrics
{


	public interface IMetricManager 
	{
        void LogEvent(string eventName);
		void LogEvent(string eventName, IDictionary<string,object> parms);	// Alternatively, eventName could be of Type MetricEvent
	}




}


