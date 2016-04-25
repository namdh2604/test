//
//using System;
//using System.Collections.Generic;
//
//namespace Voltage.Witches.Mock.Net
//{
//	using System.Net;
//
//	using Voltage.Common.Logging;
//	using Voltage.Common.Net;
//
//
//	public class WitchesMockNetworkController : INetworkTimeoutController<WWWNetworkPayload>
//	{
//		public ILogger Logger { get; private set; }
//		
//		public bool IsSuccessful { get; set; }
//		
//		public WitchesMockNetworkController (bool successful, ILogger logger)
//		{
//			Logger = logger;
//			IsSuccessful = successful;
//		}
//		
//		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout=0)
//		{
//			switch(url)
//			{
//				default: 					Failed(url, parms, onFailure); break;
//			}
//
//			return new TestTransportLayer ();
//		}
//		
//		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout=0)
//		{
//			switch(url)
//			{
//				case "http://a.mock.com":	MockGetA (url, parms, onSuccess, onFailure); break;
//				case "http://b.mock.com":	MockGetB (url, parms, onSuccess, onFailure); break;
//				case "http://c.mock.com":	MockGetTimeout (url, parms, onFailure); break;
//				case "https://www.google.com": MockGetA (url, parms, onSuccess, onFailure); break;
//				default: 					Failed(url, parms, onFailure); break;
//			}
//
//			return new TestTransportLayer ();
//		}
//
//		public class TestTransportLayer : INetworkTransportLayer
//		{
//			public void Send() {}
//			public float Progress { get { return 0; } }
//		}
//
//		
//		private void MockGetA (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure) //(Action onSuccess)
//		{
//			if (IsSuccessful)
//			{
//				WWWNetworkPayload payload = new WWWNetworkPayload ();
//				payload.Request = new NetworkRequest(url, parms);
//				payload.Status = HttpStatusCode.OK;
//				payload.Text = "hello world";
//				
//				onSuccess (payload);
//			}
//			else
//			{
//				WWWNetworkPayload payload = new WWWNetworkPayload ();
//				payload.Request = new NetworkRequest(url, parms);
//				payload.Status = HttpStatusCode.BadRequest;
//				payload.Text = "Bad Request";
//				
//				onFailure (payload);
//			}
//		}
//		
//		private void MockGetB (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure) //(Action onSuccess)
//		{
//			if (IsSuccessful)
//			{
//				WWWNetworkPayload payload = new WWWNetworkPayload ();
//				payload.Request = new NetworkRequest(url, parms);
//				payload.Status = HttpStatusCode.OK;
//				payload.Text = "foo bar";
//				
//				onSuccess (payload);
//			}
//			else
//			{
//				WWWNetworkPayload payload = new WWWNetworkPayload ();
//				payload.Request = new NetworkRequest(url, parms);
//				payload.Status = HttpStatusCode.BadRequest;
//				payload.Text = "Bad Request";
//				
//				onFailure (payload);
//			}
//		}
//		
//		
//		private void MockGetTimeout (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onFailure)
//		{
//			WWWNetworkPayload payload = new WWWNetworkPayload ();
//			payload.Request = new NetworkRequest(url, parms);
//			payload.Status = HttpStatusCode.RequestTimeout;
//			payload.Text = "timed out";
//			
//			onFailure (payload);
//		}
//		
//		
//		private void Failed (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onFailure) //(Action onSuccess)
//		{
//			WWWNetworkPayload payload = new WWWNetworkPayload ();
//			payload.Request = new NetworkRequest(url, parms);
//			payload.Status = HttpStatusCode.BadRequest;
//			payload.Text = "bad request";
//			
//			onFailure (payload);
//		}
//		
//	}
//    
//}
//
//
//
//
