//
//using System;
//
//namespace Unit.Witches.DI
//{
//	using NUnit.Framework;
//
//	using Voltage.Witches.DI;
//
//    [TestFixture]
//    public class TestLoginParser
//    {
//
//		[Test]
//		public void CreateILoginParser()
//		{
//			ILoginParser parser = new LoginParser ();
//		
//			Assert.That (parser, Is.TypeOf<LoginParser> ());
//		}
//
//        [Test]
//        public void ParseReturnsData()
//        {
//			string json = @"
//				{
//					""phone_id"": ""12345""
//				}
//			";
//
//			ILoginParser parser = new LoginParser ();
//
//			LoginData data = parser.Parse (json);
//			Assert.That (data, Is.Not.Null);
//
//        }
//
//		[Test]
//		public void ParseIDValid()
//		{
//			string json = @"
//				{
//					""phone_id"": ""12345""
//				}
//			";
//			
//			ILoginParser parser = new LoginParser ();
//			
//			LoginData data = parser.Parse (json);
//			Assert.That (data.ID, Is.StringMatching ("12345"));
//		}
//
//
//		[Test]
//		public void ParseEmptyResultsInNullID()
//		{
//			string json = @"
//				{
//					""phone_id"": """"
//				}
//			";
//			
//			ILoginParser parser = new LoginParser ();
//			
//			LoginData data = parser.Parse (json);
//			Assert.That (data.ID, Is.Empty);
//		}
//    }
//}