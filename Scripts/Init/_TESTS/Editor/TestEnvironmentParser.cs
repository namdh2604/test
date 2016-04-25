
using System;
using System.Collections.Generic;

namespace Unit.Witches.DI
{
	using NUnit.Framework;

	using Voltage.Witches.DI;
	using Voltage.Common.Metrics;

    [TestFixture]
    public class TestEnvironmentParser
    {

		[Test]
		public void CreateIEnvironmentParser()
		{
			IEnvironmentParser parser = new EnvironmentParser ();
		
			Assert.That (parser, Is.TypeOf<EnvironmentParser> ());
		}

        [Test]
        public void ParseReturnsData()
        {
			string json = @"
				{
					""base_url"": ""123.45.678.910"",
					""latest"": true
				}
			";

			IEnvironmentParser parser = new EnvironmentParser ();

			EnvironmentData data = parser.Parse (json);
			Assert.That (data, Is.Not.Null);
        }

		[Test]
		public void ParseURLValid()
		{
			string json = @"
				{
					""base_url"": ""123.45.678.910"",
					""latest"": true
				}
			";
			
			IEnvironmentParser parser = new EnvironmentParser ();
			
			EnvironmentData data = parser.Parse (json);
			Assert.That (data.URL, Is.StringMatching ("123.45.678.910"));
		}

		[Test]
		public void ParseURLIsEmpty()
		{
			string json = @"
				{
					""base_url"": """",
					""latest"": true
				}
			";
			
			IEnvironmentParser parser = new EnvironmentParser ();
			
			EnvironmentData data = parser.Parse (json);
			Assert.That (data.URL, Is.Empty);
		}


		[Test]
		public void ParseLatestIsTrue()
		{
			string json = @"
				{
					""base_url"": ""123.45.678.910"",
					""latest"": true
				}
			";
			
			IEnvironmentParser parser = new EnvironmentParser ();
			
			EnvironmentData data = parser.Parse (json);
			Assert.That (data.IsLatest, Is.True);
		}

		[Test]
		public void ParseLatestIsFalse()
		{
			string json = @"
				{
					""base_url"": ""123.45.678.910"",
					""latest"": false
				}
			";
			
			IEnvironmentParser parser = new EnvironmentParser ();
			
			EnvironmentData data = parser.Parse (json);
			Assert.That (data.IsLatest, Is.False);
		}






//		[Test]
//		public void Parse_Json_HasFlurry()
//		{
//			IEnvironmentParser parser = new EnvironmentParser ();
//			
//			EnvironmentData data = parser.Parse (_json);
//			
//			Assert.That (data.Metrics.ContainsKey ("flurry"), Is.True);
//		}
//		
//		[Test]
//		public void Parse_Json_HasAdjust()
//		{
//			IEnvironmentParser parser = new EnvironmentParser ();
//			
//			EnvironmentData data = parser.Parse (_json);
//			
//			Assert.That (data.Metrics.ContainsKey ("adjust"), Is.True);
//		}

		[Test]
		public void Parse_Json_HasCorrectCountOfMetricConfigs()
		{
			IEnvironmentParser parser = new EnvironmentParser ();
			EnvironmentData data = parser.Parse (_json);

			Assert.That (data.Metrics.Count, Is.EqualTo (2));
		}

		
		[Test]
		public void Parse_Json_ReturnFlurryKey()
		{
			IEnvironmentParser parser = new EnvironmentParser ();
			
			string expected = "Z669CM3WKYWVQ4MD7NQT";
			
			EnvironmentData data = parser.Parse (_json);
			
//			Assert.That (data.Metrics["flurry"].Key, Is.StringMatching(expected));
			Assert.That (data.Metrics["flurry"]["key"], Is.StringMatching(expected));
		}
		
//		[Test]
//		public void Parse_Json_ReturnEmptyFlurryTokens()
//		{
//			IEnvironmentParser parser = new EnvironmentParser ();
//			
//			EnvironmentData data = parser.Parse (_json);
//			
//			Assert.That (data.Metrics["flurry"].Tokens, Is.Empty);
//		}
		
		
		[Test]
		public void Parse_Json_ReturnAdjustKey()
		{
			IEnvironmentParser parser = new EnvironmentParser ();
			
			string expected = "9dae96zp43la";
			
			EnvironmentData data = parser.Parse (_json);
			
//			Assert.That (data.Metrics["adjust"].Key, Is.StringMatching(expected));
			Assert.That (data.Metrics["adjust"]["key"], Is.StringMatching(expected));
		}
		
		[Test]
		public void Parse_Json_NestedDictionaryPopulated()	
		{
			IEnvironmentParser parser = new EnvironmentParser ();
			
//			IDictionary<string,string> expected = new Dictionary<string,string>
//			{
//				{"STAMINAPOTIONSSHOPBUY", "yzapr6"},
//				{"STARSTONESSHOPBUY", "un633m"},
//			};

			string expected = @"{
  ""STAMINAPOTIONSSHOPBUY"": ""yzapr6"",
  ""STARSTONESSHOPBUY"": ""un633m""
}";
			
			EnvironmentData data = parser.Parse (_json);

			Assert.That (data.Metrics["adjust"]["tokens"].ToString(), Is.StringMatching(expected));

		}




		private string _json = @"
		{
			""base_url"": ""172.16.100.205"",
			""latest"": true,
			""metrics"":
			{
				""flurry"": 
				{
					""key"": ""Z669CM3WKYWVQ4MD7NQT""
				},
				""adjust"":
				{
					""key"": ""9dae96zp43la"",
					""tokens"":
					{
						""STAMINAPOTIONSSHOPBUY"": ""yzapr6"",
						""STARSTONESSHOPBUY"": ""un633m"",
					}
				}
			}
		}
		";






//		[Test]
//		public void Parse_Json_TwoConfigs()
//		{
//			IEnvironmentParser parser = new EnvironmentParser ();
//
//			EnvironmentData data = parser.Parse (_json);
//
//			Assert.That (data.Metrics.Count, Is.EqualTo (2));
//		}
//
//		[Test]
//		public void Parse_Json_HasCorrectConfigs()
//		{
//			IEnvironmentParser parser = new EnvironmentParser ();
//
//			IList<MetricConfig> expected = new List<MetricConfig>
//			{
//				new MetricConfig
//				{
//					Type = "flurry",
//					Key = "Z669CM3WKYWVQ4MD7NQT"
//				},
//				new MetricConfig
//				{
//					Type = "adjust",
//					Key = "9dae96zp43la",
//					Tokens = new Dictionary<string,string>
//					{
//						{"STAMINAPOTIONSSHOPBUY", "yzapr6"},
//						{"STARSTONESSHOPBUY", "un633m"},
//					}
//				}
//			};
//
//			EnvironmentData data = parser.Parse (_json);
//			
//			Assert.That (data.Metrics, Is.EquivalentTo(expected));		// Is.EqualTo(expected).AsCollection
//		}
//
//
//
//
//
//		private string _json = @"
//		{
//			""base_url"": ""172.16.100.205"",
//			""latest"": true,
//			""metrics"":
//			[
//				{
//					""type"": ""flurry"",
//					""key"": ""Z669CM3WKYWVQ4MD7NQT""
//				},
//				{
//					""type"": ""adjust"",
//					""key"": ""9dae96zp43la"",
//					""tokens"":
//					{
//						""STAMINAPOTIONSSHOPBUY"": ""yzapr6"",
//						""STARSTONESSHOPBUY"": ""un633m"",
//					}
//				}
//			]
//		}
//		";


	
    }
}