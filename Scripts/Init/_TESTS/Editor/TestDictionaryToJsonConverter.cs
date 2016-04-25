
using System;
using System.Collections.Generic;

namespace Unit.Witches.Converters
{
    using NUnit.Framework;

	using Voltage.Witches.Converters;

	using System.Linq;

    [TestFixture]
    public class TestDictionaryToJsonConverter
    {
        [Test]
        public void Constructor()
        {
			var converter = new DictionaryToJsonConverter<string,int> ();

			Assert.That (converter, Is.TypeOf<DictionaryToJsonConverter<string,int>> ());
        }

		[Test]
		public void Convert_ValidDic_ValidJson()
		{
			var converter = new DictionaryToJsonConverter<string,int> ();

			string json = converter.Convert (_dic);

			Assert.That (json, Is.StringMatching (_expected));
		}

		[Test]
		public void Convert_EmptyDic_EmptyJson()
		{
			var converter = new DictionaryToJsonConverter<string,int> ();

			string json = converter.Convert (_emptyDic);

			Assert.That (json, Is.StringMatching(_expectedEmpty));

		}

//		[Test]
//		public void Convert_InvalidDic_ValidJsonEmptyDic()
//		{
//
//		}


		private string _expected = @"{""A"":0,""B"":10,""C"":20}";
		private string _expectedEmpty = "{}";

		private IDictionary<string,int> _dic = new Dictionary<string,int>
		{
			{"A", 0},
			{"B", 10},
			{"C", 20}
		};

		private IDictionary<string,int> _emptyDic = new Dictionary<string,int>();
		
    }
}