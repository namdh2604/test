
using System;
using System.Collections.Generic;

namespace Unit.Common.Metrics
{
    using NUnit.Framework;

	using Voltage.Common.Metrics;


    [TestFixture]
    public class TestCompositeMetricManager
    {
        [Test]
        public void Instantiate()
        {
			IMetricManager spy = new MetricManagerSpy ();
			IMetricManager composite = new CompositeMetricManager (spy, spy);

			Assert.That (composite, Is.TypeOf<CompositeMetricManager> ());
        }


		[Test]
		public void LogLevel_TwoManagers_CallsTwice()
		{
			MetricManagerSpy spy = new MetricManagerSpy ();
			IMetricManager composite = new CompositeMetricManager (spy, spy);

			composite.LogEvent ("TestEvent", null);
			
			Assert.That (spy.CallCount, Is.EqualTo(2));
		}



		private class MetricManagerSpy : IMetricManager
		{
			public string Event { get; private set; }
			public IDictionary<string,object> Parameters { get; private set; }
			public int CallCount { get; private set; }
			
			public MetricManagerSpy()
			{
				Event = string.Empty;
				Parameters = new Dictionary<string,object>();

				CallCount = 0;
			}

			public void LogEvent(string eventName)
			{
				LogEvent(eventName, null);
			}
			
			public void LogEvent (string eventName, IDictionary<string,object> parms)
			{
				Event = eventName;
				Parameters = parms;

				CallCount++;
			}
		}
    }
}