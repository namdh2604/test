
using System;
using System.Collections.Generic;

namespace Voltage.Common.Metrics
{


	public class CompositeMetricManager : IMetricManager
	{
		private readonly IEnumerable<IMetricManager> _metricManagers;

		public IEnumerable<IMetricManager> ChildrenManagers { get { return _metricManagers; } }
		
		public CompositeMetricManager (params IMetricManager[] managers)
		{
//			if(managers == null)	// OR allow passing in no managers, in which case an empty list is used 
//			{
//				throw new ArgumentNullException("CompositeMetricManager::Ctor >>>");
//			}

			_metricManagers = (managers != null ? managers : new IMetricManager[0]);
		}

        public void LogEvent(string eventName)
        {
            LogEvent(eventName, new Dictionary<string, object>());
        }
		
		public void LogEvent (string eventName, IDictionary<string,object> parms)
		{
			foreach(IMetricManager manager in _metricManagers)
			{
				manager.LogEvent(eventName, parms);
			}
		}
	}

}




