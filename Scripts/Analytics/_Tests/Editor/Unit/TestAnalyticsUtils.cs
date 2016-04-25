
using System;
using System.Collections.Generic;

namespace Unit.Witches.Metrics
{
    using NUnit.Framework;

	using Voltage.Witches.Metrics;

    [TestFixture]
    public class TestAnalyticsUtils
    {
        [Test]
        public void Parse_String_ReturnString()
        {
			Dictionary<string,object> data = new Dictionary<string,object>
			{
				{"player", "foobar"},
			};

			Assert.That (AnalyticsUtils.ParseCollectionParametersToJson (data) ["player"], Is.StringMatching("foobar"));
        }


		[Test]
		public void Parse_Int_ReturnString()	// NOTE: may want to rename function to ParseParametersToString instead
		{
			Dictionary<string,object> data = new Dictionary<string,object>
			{
				{"num", 3},
			};
			
			Assert.That (AnalyticsUtils.ParseCollectionParametersToJson (data) ["num"], Is.StringMatching("3"));
		}


		[Test]
		public void Parse_List_ReturnJson()
		{
			Dictionary<string,object> data = new Dictionary<string,object>
			{
				{"list", new List<string> {"hello", "world"}},
			};

			string expected = 
			@"[
			  ""hello"",
			  ""world""
			]";

			Assert.That (AnalyticsUtils.ParseCollectionParametersToJson(data) ["list"], Is.StringMatching(expected));
		}

		[Test]
		public void Parse_Dict_ReturnJson()
		{
			Dictionary<string,object> data = new Dictionary<string,object>
			{
				{"dict", new Dictionary<string,string> {{"hello","world"},{"foo","bar"}}}
			};
			
			string expected = 
@"{
  ""hello"": ""world"",
  ""foo"": ""bar""
}";
			
			Assert.That (AnalyticsUtils.ParseCollectionParametersToJson (data) ["dict"], Is.EqualTo(expected));
		}


    }
}