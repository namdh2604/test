
using System;
using System.Collections.Generic;

namespace Integration.Story.Text
{
    using NUnit.Framework;

	using Voltage.Common.Logging;

	using Voltage.Story.Text;
	using Voltage.Story.Mapper;


    [TestFixture]
    public class TestTextParser
    {
        [Test]
        public void CreateTextParser()
        {
			IMapping<string> variableMapper = new VariableTestMapper ();

			VariableTextParser textParser = new VariableTextParser (variableMapper, new ConsoleLogger());
			Assert.That (textParser, Is.Not.Null);
			Assert.That (textParser.VariableMapper, Is.SameAs(variableMapper));
        }

		[Test]
		public void ParseString()
		{
			string json = @"
			[
				""Hello, My name is '"",
				{
					""_class"": ""Variable"",
					""text"": ""MC/First""
				},
				""'""
			]
			";

			IMapping<string> variableMapper = new VariableTestMapper ();
			
			VariableTextParser textParser = new VariableTextParser (variableMapper, new ConsoleLogger());
			Assert.That (textParser.Parse (json), Is.StringMatching ("Hello, My name is 'jon'"));
		}


		[Test]
		public void ParseNumeral()
		{
			string json = @"
			[
				""Hello, I like Bryant: "",
				{
					""_class"": ""Variable"",
					""text"": ""Characters/Tyrone Bryant/Affinity""
				}
			],
			";
			
			IMapping<string> variableMapper = new VariableTestMapper ();
			
			VariableTextParser textParser = new VariableTextParser (variableMapper, new ConsoleLogger());
			Assert.That (textParser.Parse (json), Is.StringMatching ("Hello, I like Bryant: 7"));
		}


		[Test]
		public void ParseMultiple()
		{
			string json = @"
			[
				""Hello, My name is "",
				{
					""_class"": ""Variable"",
					""text"": ""MC/Last""
				},
				"" and I like Bryant: "",
				{
					""_class"": ""Variable"",
					""text"": ""Characters/Tyrone Bryant/Affinity""
				}
			]
			";
			
			IMapping<string> variableMapper = new VariableTestMapper ();
			
			VariableTextParser textParser = new VariableTextParser (variableMapper, new ConsoleLogger());
			Assert.That (textParser.Parse (json), Is.StringMatching ("Hello, My name is doe and I like Bryant: 7"));
		}

		[Test]
		public void ParseNotFound()
		{
			string json = @"
			[
				""Hello, My name is '"",
				{
					""_class"": ""Variable"",
					""text"": ""MC/Nickname""
				},
				""' and I'm not found""
			]
			";
			
			IMapping<string> variableMapper = new VariableTestMapper ();
			
			VariableTextParser textParser = new VariableTextParser (variableMapper, new ConsoleLogger());
			Assert.That (textParser.Parse (json), Is.EqualTo ("Hello, My name is '[ERROR]' and I'm not found"));	// Is.StringMatching
		}




		public class VariableTestMapper : IMapping<string>
		{
			public IDictionary<string,object> Map = new Dictionary<string,object> 
			{
				{"MC/First", "jon"},
				{"MC/Last", "doe"},
				{"Characters/Tyrone Bryant/Affinity", 7},
			};

			public bool TryGetValue(string key, out object value)
			{
				object v;
				if (Map.TryGetValue(key, out v))
				{
					value = v;
					return true;
				}

				value = null;
				return false;
			}

			public bool TryGetValue(string key, out int value)
			{
				object v;
				bool success = TryGetValue(key, out v);

				value = (success) ? (int)v : 0;

				return success;
			}
			
			public bool TryGetValue<T> (string key, out T value)
			{
				object v;
				if(Map.TryGetValue(key, out v))
				{
					value = (T)Convert.ChangeType(v, typeof(T));		// value = (T)v;
					return true;
				}
				else
				{
					value = default(T);
					return false;
				}
			}
		}
    }
}