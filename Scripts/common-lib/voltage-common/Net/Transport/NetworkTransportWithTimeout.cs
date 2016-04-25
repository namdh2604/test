
using System;
using System.Collections.Generic;

namespace Voltage.Common.Net
{

	public interface ITimeout
	{
		int TimeoutDelay { get; }	// probably should make a set instead or explicitly state its in seconds
		void SetTimeout (int seconds);
	}

	public abstract class NetworkTransportWithTimeout : INetworkTransportLayer, ITimeout
	{
		protected const int DEFAULT_TIMEOUT = 90;		// in seconds
		public int TimeoutDelay { get; private set; }

		public void SetTimeout (int seconds)
		{
			TimeoutDelay = ConvertToMilliseconds(seconds);
		}
		
		protected readonly INetworkRequest _request;

		public abstract float Progress { get; }
		
		public NetworkTransportWithTimeout (INetworkRequest request, int timeout=DEFAULT_TIMEOUT)
		{
			_request = request;
			TimeoutDelay = ConvertToMilliseconds(timeout);
		}
		
		public abstract void Send();
		
		
		protected int ConvertToMilliseconds(int seconds)
		{
			return seconds * 1000;
		}
		
		protected bool HasValidRequest
		{
			get
			{
				return _request != null && !string.IsNullOrEmpty(_request.URL);
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("{0} <{1}>", this.GetType(), HasValidRequest ? _request.ToString() : string.Empty);
		}
		
	}
    
}




