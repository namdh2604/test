using System;
using System.Net;
using System.Collections.Generic;

namespace Voltage.Common.Net
{
//	public interface INetworkController
//	{
//		void Send<T>(string url, IDictionary<string,string> parms, Action<T> onSuccess, Action<HttpStatusCode,T> onFailure);
//		void Receive<T>(string url, Action<T> onSuccess, Action<HttpStatusCode,T> onFailure);
//	}


//	public interface INetworkController<T> where T: INetworkPayload
//	{
//		// return INetworkTransport
//		void Send (string url, IDictionary<string,string> parms, Action<T> onSuccess, Action<T> onFailure);
//
//		// return INetworkTransport
//		void Receive (string url, IDictionary<string,string> parms, Action<T> onSuccess, Action<T> onFailure);		
//	}


	public interface INetworkTimeoutController<T> where T: INetworkPayload
	{
//		float DefaultTimeout { get; }

		INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<T> onSuccess, Action<T> onFailure, int timeout=30);

		INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<T> onSuccess, Action<T> onFailure, int timeout=30);

//		void Download (string url, IDictionary<string,string> parms, Action<IUnityPayload> onSuccess, Action<INetworkPayload> onFailure, int timeout);
	}

}

