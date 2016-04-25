//
//using System;
//using System.Collections.Generic;
//
//namespace Integration.Witches.Metrics
//{
//    using NUnit.Framework;
//
//	using Voltage.Common.Logging;
//	using Voltage.Witches.Models;
//	using Voltage.Witches.Metrics;
//	using Voltage.Witches.DI;
//
//    [TestFixture]
//    public class TestWitchesMetricManagerFactory
//    {
//        [Test]
//        public void CreateWitchesMetricManager()
//        {
//			WitchesMetricManagerFactory factory = new WitchesMetricManagerFactory (GetPlayer (), new ConsoleLogger ());
//			EnvironmentData data = new EnvironmentParser ().Parse (_json);
//
//			var manager = factory.Create (data.Metrics);
//
//			Assert.That (manager, Is.TypeOf<WitchesMetricManager> ());
//        }
//
//
//		private Player GetPlayer ()
//		{
////			PlayerDataStore data = new PlayerDataStore
////			{
////
////			};
//			IDictionary<string,object> data = new Dictionary<string,object>
//			{
//				{"userID", "12345"},
//				{"registrationDate", "today"},
//				//				{"current_scene"},
//				{"currency", 3},
//				{"currencyPremium", 7},
//			};
//			
//			return new Player (data);
//		}
//
//
//		private string _json = @"
//		{
//			""base_url"": ""172.16.100.205"",
//			""latest"": true,
//			""metrics"":
//			{
//				""flurry"": 
//				{
//					""key"": ""Z669CM3WKYWVQ4MD7NQT""
//				},
//				""adjust"":
//				{
//					""key"": ""9dae96zp43la"",
//					""tokens"":
//					{
//						""STAMINAPOTIONSSHOPBUY"": ""yzapr6"",
//						""STARSTONESSHOPBUY"": ""un633m"",
//					}
//				}
//			}
//		}
//		";
//    }
//}