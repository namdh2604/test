
using System;
using System.Collections.Generic;

namespace Integration.Witches.Metrics
{
    using NUnit.Framework;

	using Voltage.Common.Logging;

	using Voltage.Witches.Models;
	using Voltage.Witches.User;

	using Voltage.Common.Metrics;
	using Voltage.Witches.Metrics;

    [TestFixture]
    public class TestWitchesMetricManager
    {
        [Test]
        public void Instantiate()
        {
			IMetricManager manager = new WitchesMetricManager (GetPlayer (), new MetricManagerSpy (), new ConsoleLogger ()); 
			Assert.That (manager, Is.TypeOf<WitchesMetricManager> ());
        }

		[Test]
		public void LogEvent_WithParameters_CommonParametersAdded()
		{
			MetricManagerSpy spy = new MetricManagerSpy ();
			IMetricManager manager = new WitchesMetricManager (GetPlayer (), spy, new ConsoleLogger ()); 

			IDictionary<string,object> expected = new Dictionary<string,object>
			{
				{"player_id", "12345"},
//				{"player_reg_date", "today"},
				{"player_scenes", "hi"},
				{"player_previous_scene", string.Empty},
				{"player_coins_balance", 3},
				{"player_starstones_balance", 7},
				{"eventParameter", "foobar"}
			};

			manager.LogEvent ("TestCommonParameter", new Dictionary<string,object>{ {"eventParameter", "foobar"} });

			Assert.That (spy.Parameters, Is.EqualTo (expected));
		}

		[Test]
		public void LogEvent_NoParameters_OnlyCommonParameters()
		{
			MetricManagerSpy spy = new MetricManagerSpy ();
			IMetricManager manager = new WitchesMetricManager (GetPlayer (), spy, new ConsoleLogger ()); 
			
			IDictionary<string,object> expected = new Dictionary<string,object>
			{
				{"player_id", "12345"},
//				{"player_reg_date", "today"},
				{"player_scenes", "hi"},
				{"player_coins_balance", 3},
				{"player_starstones_balance", 7},
				{"player_previous_scene", string.Empty},
			};
			
			manager.LogEvent ("TestCommonParameter", null);
			
			Assert.That (spy.Parameters, Is.EqualTo (expected));
		}


		[Test]
		public void LogEvent_WithConflictingCommonParameter_TakeGiven ()
		{
			MetricManagerSpy spy = new MetricManagerSpy ();
			IMetricManager manager = new WitchesMetricManager (GetPlayer (), spy, new ConsoleLogger ()); 
			
			IDictionary<string,object> expected = new Dictionary<string,object>
			{
				{"player_id", "12345"},
//				{"player_reg_date", "today"},
				{"player_scenes", "hi"},
				{"player_previous_scene", string.Empty},
				{"player_coins_balance", 9},
				{"player_starstones_balance", 7},
			};
			
			manager.LogEvent ("TestCommonParameter", new Dictionary<string,object>{ {"player_coins_balance", 9} });
			
			Assert.That (spy.Parameters, Is.EqualTo (expected));
		}


		[Test]
		public void LogEvent_NoEventGiven_NoAction()
		{
			MetricManagerSpy spy = new MetricManagerSpy ();
			IMetricManager manager = new WitchesMetricManager (GetPlayer (), spy, new ConsoleLogger ()); 

			Assert.Throws<ArgumentNullException> (() => manager.LogEvent (string.Empty, new Dictionary<string,object>{ {"eventParameter", "foobar"} }));
//			Assert.That (spy.Event, Is.Empty);
		}



		private class MetricManagerSpy : IMetricManager
		{
			public string Event { get; private set; }
			public IDictionary<string,object> Parameters { get; private set; }

			public MetricManagerSpy()
			{
				Event = string.Empty;
				Parameters = new Dictionary<string,object>();
			}

			public void LogEvent(string eventName)
			{
				LogEvent(eventName, null);
			}

			public void LogEvent (string eventName, IDictionary<string,object> parms)
			{
				Event = eventName;
				Parameters = parms;
			}
		}

		private Player GetPlayer ()
		{
			PlayerDataStore data = new PlayerDataStore
			{
				userID = "12345",
				currencyGame = 3,
				currencyPremium = 7,
				currentScene = "hi"
			};

			return new Player (data, null, null, null, null);
		}
    }
}