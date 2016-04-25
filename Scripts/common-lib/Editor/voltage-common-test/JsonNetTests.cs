using System;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Integration.ThirdParty
{
	[TestFixture]
	public class JsonNetTest
	{
		private string ParseJson (string json, string key)
		{
			JObject jsonObj = JObject.Parse (json);
			JToken token;
			if(jsonObj.TryGetValue(key, out token))
		   	{
				Console.WriteLine("Token: " + token.ToString());
				return token.ToString();
			}
			else
			{
				return string.Empty;
			}
		}

		[Test]
		public void GetPath()
		{
			Console.WriteLine("path is: " + System.Environment.GetEnvironmentVariable("MONO_PATH"));
		}

		[Test]
		public void TestNormalString ()
		{
			string normal = "{\"hello\": \"world\"}";

			Assert.That (new TestDelegate (() => ParseJson (normal, "hello")), Throws.Nothing); //  & Is.EqualTo("world"));
			Assert.That (() => ParseJson (normal, "hello"), Is.EqualTo ("world"));
		}

		[Test]
		public void TestLiteralString()
		{
			string literal = @"{""hello"": ""world""}";

			Assert.That (new TestDelegate (() => ParseJson (literal, "hello")), Throws.Nothing); //  & Is.EqualTo("world"));
			Assert.That (() => ParseJson (literal, "hello"), Is.EqualTo ("world"));
		}


		[Test]
		public void TestLiteralEscapedString()
		{
			string literal = "{\n  \"_class\": \"Dialogue\",\n  \"speaker\": null,\n  \"text\": [\n    \"The minutes flew by in the <i>exact way</i> that tortoises don't.\"\n  ]\n}";

			Assert.That (new TestDelegate (() => ParseJson (literal, "_class")), Throws.Nothing); //  & Is.EqualTo("world"));
			Assert.That (() => ParseJson (literal, "_class"), Is.EqualTo ("Dialogue"));
		}
	}
}
