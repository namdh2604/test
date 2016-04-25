using System;
using System.Net;
using System.Collections.Generic;
using Voltage.Common.Logging;
using UnityEngine;
using ILogger = Voltage.Common.Logging.ILogger;

namespace Voltage.Common.Net
{


	public class NetworkLoggingController : INetworkTimeoutController<WWWNetworkPayload>//, INetworkController<WWWNetworkPayload>
	{
		private const int DEFAULT_TIMEOUT = 30;

		public ILogger Logger { get; private set; }


		public NetworkLoggingController (ILogger logger) 
		{
			Logger = logger;
		}

//		public virtual void Send (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure)
//		{
//			Send (url, parms, onSuccess, onFailure, DEFAULT_TIMEOUT);
//		}
		public virtual INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout=DEFAULT_TIMEOUT)
		{
			INetworkRequest request = new NetworkRequest (url, parms);

			Action<WWWNetworkPayload> logSuccess = (response) => LogSuccessfulRequest(response, request, onSuccess);
			Action<WWWNetworkPayload> logFailure = (response) => LogFailedRequest(response, request, onFailure);

			// Check if online
//			new WWWPostRequest (request, logSuccess, logFailure, timeout).Send ();
			UnityPostRequest post = new UnityPostRequest (request, logSuccess, logFailure, timeout);
			post.Send ();

			Logger.Log (string.Format ("Request <{0}> Sent", request.ToString()), LogLevel.INFO);

			return post;
		}

//		public virtual void Receive (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure)
//		{
//			Receive (url, parms, onSuccess, onFailure, DEFAULT_TIMEOUT);
//		}
		public virtual INetworkTransportLayer Receive (string url, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure)
		{
			return Receive (url, null, onSuccess, onFailure, DEFAULT_TIMEOUT);
		}
		public virtual INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout=DEFAULT_TIMEOUT)
		{
			INetworkRequest request = new NetworkRequest (url, parms);

			Action<WWWNetworkPayload> logSuccess = (response) => LogSuccessfulRequest(response, request, onSuccess);
			Action<WWWNetworkPayload> logFailure = (response) => LogFailedRequest(response, request, onFailure);

			// Check if online
//			new WWWGetRequest (request, logSuccess, logFailure, timeout).Send ();
			UnityGetRequest get = new UnityGetRequest (request, logSuccess, logFailure, timeout);
			get.Send ();

			Logger.Log (string.Format ("Request <{0}> Sent", request.ToString()), LogLevel.INFO);

			return get;
		}

//		private void SendRequest (INetTransportLayer transportLayer, INetworkRequest request, Action<INetworkPayload> onSuccess, Action<INetworkPayload> onFailure)
//		{
//			Action<INetworkPayload> logSuccess = (response) => LogSuccessfulRequest(response, request, onSuccess);
//			Action<INetworkPayload> logFailure = (response) => LogFailedRequest(response, request, onFailure);
//
//			transportLayer.Send (request);
//		}


		private void LogSuccessfulRequest  (WWWNetworkPayload response, INetworkRequest request, Action<WWWNetworkPayload> callback)
		{
			Logger.Log (string.Format ("Request <{0}> Successful\n{1}", request.ToString(), response.Text), LogLevel.INFO);
			
			if(callback != null)
			{
				callback(response);
			}
		}
		
		
		private void LogFailedRequest (WWWNetworkPayload response, INetworkRequest request, Action<WWWNetworkPayload> callback)
		{
			Logger.Log (string.Format ("Request <{0}> Failed: [{2}]", request.ToString(), response.Text), LogLevel.WARNING);
			
			if(callback != null)
			{
				callback(response);
			}
		}


	}


//	public class NetworkTestConnectionControllerDecorator : INetworkTimeoutController<WWWNetworkPayload>
//	{
//		public INetworkTimeoutController<WWWNetworkPayload> NetworkController { get; private set; }
//		public Action OnOffline { get; private set; }
//
//		public NetworkTestConnectionControllerDecorator (INetworkTimeoutController<WWWNetworkPayload> networkController, Action onOffline)
//		{
//			if(networkController == null || onOffline == null)
//			{
//				throw new ArgumentNullException ("NetworkTestConnectionControllerDecorator::Ctor >>> ");
//			}
//
//			NetworkController = networkController;
//			OnOffline = onOffline;
//		}
//
//		public virtual void Send (string url, IDictionary<string,string> parms, Action<WWWNetworkPayload> onSuccess, Action<WWWNetworkPayload> onFailure, int timeout)
//		{
//			// Test for Connection
//			NetworkController.Send (url, parms, onSuccess, onFailure, timeout);
//		}
//	}



}

