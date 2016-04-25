//
//using System;
//using System.Collections.Generic;
//
//namespace Integration.Common.Net
//{
//    using NUnit.Framework;
//
//	using Voltage.Common.Logging;
//
//	using UnityEngine;
//	using System.Net;
//	using Voltage.Common.Net;
//
//
//    [TestFixture]
//    public class TestMockNetworkTransportLayer
//    {
////        [Test]
////        public void MockWWW()
////        {
////			try
////			{
////			WWW test = new WWW ("https://www.google.com", null, new Dictionary<string,string> { {"CONTENT-TYPE", "charset=UTF-8"}});
////
////			}
////			catch (Exception e)
////			{
////				Console.WriteLine (e.ToString());
////			}
////        }
//
//
//		[Test]
//		public void GetSuccessful()
//		{
//			INetworkTimeoutController<INetworkPayload> controller = new TestMockController (true, new ConsoleLogger());
//
//			INetworkPayload payload = new NetworkPayload();
//			controller.Receive ("http://a.mock.com", null, (p) =>  payload = p, (p) => payload = p);
//
//			Assert.That (payload.Status, Is.EqualTo (HttpStatusCode.OK));
//		}
//
//		[Test]
//		public void GetBadRequest()
//		{
//			INetworkTimeoutController<INetworkPayload> controller = new TestMockController (false, new ConsoleLogger());
//			
//			INetworkPayload payload = new NetworkPayload();
//			controller.Receive ("http://a.mock.com", null, (p) =>  payload = p, (p) => payload = p);
//			
//			Assert.That (payload.Status, Is.EqualTo (HttpStatusCode.BadRequest));
//		}
//
//
//		[Test]
//		public void GetTimeout()
//		{
//			INetworkTimeoutController<INetworkPayload> controller = new TestMockController (false, new ConsoleLogger());
//			
//			INetworkPayload payload = new NetworkPayload();
//			controller.Receive ("http://c.mock.com", null, (p) =>  payload = p, (p) => payload = p);
//			
//			Assert.That (payload.Status, Is.EqualTo (HttpStatusCode.RequestTimeout));
//		}
//
//
//
//
//		private void OnSuccess(INetworkPayload payload)
//		{
//
//		}
//
//		private void OnFailure (INetworkPayload payload)
//		{
//
//		}
//
//
//
//
//		public class TestMockController : INetworkTimeoutController<INetworkPayload>
//		{
//			public ILogger Logger { get; private set; }
//
//			public bool IsSuccessful { get; set; }
//
//			public TestMockController (bool successful, ILogger logger)
//			{
//				Logger = logger;
//
//				IsSuccessful = successful;
//			}
//
//			public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<INetworkPayload> onSuccess, Action<INetworkPayload> onFailure, int timeout=0)
//			{
//				return new TestTransportLayer ();
//			}
//
//			public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<INetworkPayload> onSuccess, Action<INetworkPayload> onFailure, int timeout=0)
//			{
//				switch(url)
//				{
//					case "http://a.mock.com":	MockGetA (url, parms, onSuccess, onFailure); break;
//					case "http://b.mock.com":	MockGetB (url, parms, onSuccess, onFailure); break;
//					case "http://c.mock.com":	MockGetTimeout (url, parms, onFailure); break;
//					default: 					Failed(url, parms, onFailure); break;
//				}
//
//				return new TestTransportLayer ();
//			}
//
//			public class TestTransportLayer : INetworkTransportLayer
//			{
//				public void Send() {}
//				public float Progress { get { return 0; } }
//			}
//
//
//			private void MockGetA (string url, IDictionary<string,string> parms, Action<INetworkPayload> onSuccess, Action<INetworkPayload> onFailure) //(Action onSuccess)
//			{
//				if (IsSuccessful)
//				{
//					NetworkPayload payload = new NetworkPayload ();
//					payload.Request = new NetworkRequest(url, parms);
//					payload.Status = HttpStatusCode.OK;
//					payload.Text = "hello world";
//
//					onSuccess (payload);
//				}
//				else
//				{
//					NetworkPayload payload = new NetworkPayload ();
//					payload.Request = new NetworkRequest(url, parms);
//					payload.Status = HttpStatusCode.BadRequest;
//					payload.Text = "Bad Request";
//
//					onFailure (payload);
//				}
//			}
//
//			private void MockGetB (string url, IDictionary<string,string> parms, Action<INetworkPayload> onSuccess, Action<INetworkPayload> onFailure) //(Action onSuccess)
//			{
//				if (IsSuccessful)
//				{
//					NetworkPayload payload = new NetworkPayload ();
//					payload.Request = new NetworkRequest(url, parms);
//					payload.Status = HttpStatusCode.OK;
//					payload.Text = "foo bar";
//					
//					onSuccess (payload);
//				}
//				else
//				{
//					NetworkPayload payload = new NetworkPayload ();
//					payload.Request = new NetworkRequest(url, parms);
//					payload.Status = HttpStatusCode.BadRequest;
//					payload.Text = "Bad Request";
//					
//					onFailure (payload);
//				}
//			}
//
//
//			private void MockGetTimeout (string url, IDictionary<string,string> parms, Action<INetworkPayload> onFailure)
//			{
//				NetworkPayload payload = new NetworkPayload ();
//				payload.Request = new NetworkRequest(url, parms);
//				payload.Status = HttpStatusCode.RequestTimeout;
//				payload.Text = "timed out";
//				
//				onFailure (payload);
//			}
//
//
//			private void Failed (string url, IDictionary<string,string> parms, Action<INetworkPayload> onFailure) //(Action onSuccess)
//			{
//				NetworkPayload payload = new NetworkPayload ();
//				payload.Request = new NetworkRequest(url, parms);
//				payload.Status = HttpStatusCode.BadRequest;
//				payload.Text = "bad request";
//				
//				onFailure (payload);
//			}
//
//		}
//
//
//
//
//
//    }
//}
//
//
//
//
//
//
//
//
//
//
