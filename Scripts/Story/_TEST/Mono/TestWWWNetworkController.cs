//using UnityEngine;
//using System;
//using System.Collections.Generic;
//
//namespace Integration.Mono.Common.Net
//{
//	using Voltage.Common.Logging;
//
//	using Voltage.Common.Net;
////	using Voltage.Witches.Mock.Net;
//
//
//    public class TestWWWNetworkController : MonoBehaviour
//    {
//		private void OnGUI()
//		{
//			if(GUILayout.Button ("Get"))
//			{
//				GetGoogle();
//			}
//			if(GUILayout.Button ("Post"))
//			{
//				PostTest();
//			}
//			if(GUILayout.Button ("Get Mock"))
//			{
//				GetMock();
//			}
//			if(GUILayout.Button ("Download Progress"))
//			{
//				DownloadProgress();
//			}
//		}
//
//		private INetworkTransportLayer Request { get; set; }
//
//		private void GetGoogle()
//		{
//			ILogger logger = new UnityLogger();
//
//			INetworkTimeoutController<WWWNetworkPayload> controller = new NetworkLoggingController(logger);
//
//			Request = controller.Receive ("https://www.google.com", null, (payload) => logger.Log (payload.Text, LogLevel.INFO), (payload) => logger.Log (payload.Text, LogLevel.INFO));
//		}
//
//		private void Update()
//		{
//			if (Request != null && Request.Progress < 1f)
//			{
//				Debug.Log (Request.Progress);
//			}
//		}
//
//
//		private void GetMock()
//		{
//			ILogger logger = new UnityLogger();
//			
//			INetworkTimeoutController<WWWNetworkPayload> controller = new WitchesMockNetworkController(true, new UnityLogger());
//			
//			Request = controller.Receive ("https://www.google.com", null, (payload) => logger.Log (payload.Text, LogLevel.INFO), (payload) => logger.Log (payload.Text, LogLevel.INFO));
//		}
//
//
//		private void DownloadProgress ()
//		{
//			ILogger logger = new UnityLogger();
//			
//			INetworkTimeoutController<WWWNetworkPayload> controller = new NetworkLoggingController(logger);
//			
//			Request = controller.Receive ("file:///Users/michael.chang/Downloads/command_line_tools_for_osx_10.9_for_xcode_6.1.dmg", null, (payload) => logger.Log ("fin", LogLevel.INFO), (payload) => logger.Log ("failed", LogLevel.INFO));
//		}
//
//
//		private void PostTest()
//		{
//			ILogger logger = new UnityLogger();
//			
//			INetworkTimeoutController<WWWNetworkPayload> controller = new NetworkLoggingController(logger);
//			
//			Request = controller.Receive ("http://postcatcher.in/catchers/554c08f6978887030000607d", new Dictionary<string,string>{{"foo","bar"}}, OnSuccess, (payload) => logger.Log (payload.Text, LogLevel.INFO));
//
//		}
//
//		private void OnSuccess (INetworkPayload payload)
//		{
//			UnityEngine.Debug.Log ("Success");
//		}
//
//    }
//
//}
//
//
