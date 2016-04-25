
using System;
using System.Collections.Generic;

namespace Unit.ThirdParty.FlurryUnityPlugin
{
    using NUnit.Framework;
	using Analytics;

    [TestFixture]
    public class TestFlurryPlugin
    {

		private string _keys;
		private string _values;

		private Dictionary<string,string> _dic;

		[SetUp]
		public void Init()
		{
//			_keys = string.Empty;
//			_values = string.Empty;

			_dic = new Dictionary<string,string>
			{
				{"hello", "world"},
				{"foo", "bar"}
			};
		}


        [Test]
		public void ToKeyValueForPlugin_ValidKeyListNoEmptyString()
        {
			FlurryIOS.ToKeyValueForPlugin (_dic, out _keys, out _values);

			string expected = "hello\nfoo";

			Assert.That (_keys, Is.StringMatching(expected));
        }

		[Test]
		public void ToKeyValueForPlugin_ValidValueListNoEmptyString()
		{
			FlurryIOS.ToKeyValueForPlugin (_dic, out _keys, out _values);
			
			string expected = "world\nbar";
			
			Assert.That (_values, Is.StringMatching(expected));
		}


		[Test]
		public void ToKeyValueForPlugin_EmptyDic_EmptyKeyList()
		{
			var dic = new Dictionary<string,string> ();

			FlurryIOS.ToKeyValueForPlugin (dic, out _keys, out _values);

			Assert.That (_keys, Is.Empty);
		}

		[Test]
		public void ToKeyValueForPlugin_EmptyDic_EmptyValueList()
		{
			var dic = new Dictionary<string,string> ();
			
			FlurryIOS.ToKeyValueForPlugin (dic, out _keys, out _values);
			
			Assert.That (_values, Is.Empty);
		}




    }


}