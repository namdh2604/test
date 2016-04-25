
using System;
using System.Collections.Generic;

namespace Unit.Witches.Net
{
    using NUnit.Framework;
	using System.Net;

	using Voltage.Common.Logging;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;

    [TestFixture]
    public class TestWitchesNetworkController
    {
		[Test]
		[ExpectedException("System.ArgumentNullException")]
		public void InstantiateNoControllerThrowException()
		{
			new WitchesNetworkController (null);
		}


        [Test]
        public void InstantiateNoBaseURL()
        {
			MockNetworkController baseController = new MockNetworkController();

			INetworkTimeoutController<WWWNetworkPayload> controller = new WitchesNetworkController (baseController);

			Assert.That (controller, Is.TypeOf<WitchesNetworkController>());
        }


		[TestCase("")]
		[TestCase("123")]
		public void InstantiateWithBaseURL(string baseUrl)
		{
			MockNetworkController baseController = new MockNetworkController();
			
			WitchesNetworkController controller = new WitchesNetworkController (baseController, baseUrl);

			Assert.That (controller.BaseURL, Is.StringMatching (baseUrl));
		}


//		[TestCase("", "123")]
//		[TestCase("123", "890")]
		[TestCase("http://www.google.com", "http://www.microsoft.com")]
		public void SetBaseURL(string baseUrl, string newUrl)
		{
			MockNetworkController baseController = new MockNetworkController();
			
			WitchesNetworkController controller = new WitchesNetworkController (baseController, baseUrl);

			controller.BaseURL = newUrl;

			Assert.That (controller.BaseURL, Is.StringMatching (newUrl));
		}



		[TestCase("http://www.google.com", "/search", Result="http://www.google.com/search")]
//		[TestCase("http://www.google.com", "search", Result="http://www.google.com/search")]
//		[TestCase("http://www.google.com", "//search", Result="http://www.google.com//search")]									// presently allowed
//		[TestCase("http://www.google.com/", "search", Result="http://www.google.com//search")]									// presently allowed
//		[TestCase("http://www.google.com/", "/search", Result="http://www.google.com//search")]									// presently allowed
//		[TestCase("http://www.google.com", "http://www.google.com", Result="http://www.google.com/http://www.google.com")]		// presently allowed
//		[TestCase("", "/search", Result="/search")]
		public string Receive_WithSlash_ValidURL(string baseUrl, string getURL)
		{
			MockNetworkController baseController = new MockNetworkController();
			
			WitchesNetworkController controller = new WitchesNetworkController (baseController, baseUrl);

			string url = string.Empty;
			controller.Receive(getURL, null, (response) => url = response.Request.URL, (response) => {throw new Exception();});

			return url;
		}

		[TestCase("http://www.google.com", "/search", Result="http://www.google.com/search")]
//		[TestCase("http://www.google.com", "search", Result="http://www.google.com/search")]
//		[TestCase("http://www.google.com", "//search", Result="http://www.google.com//search")]									// presently allowed
//		[TestCase("http://www.google.com/", "search", Result="http://www.google.com//search")]									// presently allowed
//		[TestCase("http://www.google.com/", "/search", Result="http://www.google.com//search")]									// presently allowed
//		[TestCase("http://www.google.com", "http://www.google.com", Result="http://www.google.com/http://www.google.com")]		// presently allowed
//		[TestCase("", "/search", Result="/search")]
		public string Send_WithSlash_ValidURL(string baseUrl, string getURL)
		{
			MockNetworkController baseController = new MockNetworkController();
			
			WitchesNetworkController controller = new WitchesNetworkController (baseController, baseUrl);
			
			string url = string.Empty;
			controller.Send(getURL, null, (response) => url = response.Request.URL, (response) => {throw new Exception();});
			
			return url;
		}



		[Test]
		public void GetRequestSent()
		{
			MockNetworkController baseController = new MockNetworkController();
			
			WitchesNetworkController controller = new WitchesNetworkController (baseController);
			
			var transportLayer = controller.Receive("http://www.google.com", null, (response) => {}, (response) => {throw new Exception();});
			
			Assert.That (transportLayer, Is.TypeOf<TestTransportLayer>());
		}

		[Test]
		public void PostRequestSent()
		{
			MockNetworkController baseController = new MockNetworkController();
			
			WitchesNetworkController controller = new WitchesNetworkController (baseController);

			var transportLayer = controller.Send("http://www.google.com", null, (response) => {}, (response) => {throw new Exception();});
	
			Assert.That (transportLayer, Is.TypeOf<TestTransportLayer>());
		}








		public class MockNetworkController : INetworkTimeoutController<WWWNetworkPayload>
		{
			public const int DEFAULT_TIMEOUT=30;

			public virtual INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout=DEFAULT_TIMEOUT)
			{
				onSuccess (new WWWNetworkPayload
				{
					Request = new NetworkRequest(url, parms),
//					Status = HttpStatusCode.OK,
					Text = @"
					{	
						""hello"" : ""world"",
					}
					"
				});

				return new TestTransportLayer ();
			}
			
			public virtual INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout=DEFAULT_TIMEOUT)
			{
				onSuccess (new WWWNetworkPayload
				{
					Request = new NetworkRequest(url, parms),
//					Status = HttpStatusCode.OK,
					Text = @"
					{	
						""hello"" : ""world"",
					}
					"
				});

				return new TestTransportLayer ();
			}	
		}

		public class TestTransportLayer : INetworkTransportLayer
		{
			public void Send() {}
			public float Progress { get { return 0; } }
		}

    }
}