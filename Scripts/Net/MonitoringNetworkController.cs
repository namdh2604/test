using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
    using Voltage.Common.Net;

	public interface INetworkMonitor
	{
		bool IsConnected { get; }
        event EventHandler ConnectionStatusEvent;
	}

    public interface IMonitoringNetworkController : INetworkTimeoutController<WitchesRequestResponse>, INetworkMonitor
    {
    }

    public class MonitoringNetworkController : IMonitoringNetworkController
    {
		private readonly INetworkTimeoutController<WitchesRequestResponse> _wrappedController;

		public MonitoringNetworkController(INetworkTimeoutController<WitchesRequestResponse> wrappedController)
        {
            IsConnected = true;
            _wrappedController = wrappedController;
        }

        public event EventHandler ConnectionStatusEvent;
        public bool IsConnected { get; protected set; }

		public virtual INetworkTransportLayer Send(string url, IDictionary<string, string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
        {
            return _wrappedController.Send(url, parms,
                (result) => ProcessResult(true, result, onSuccess),
                (result) => ProcessResult(false, result, onFailure), timeout);
        }

		public virtual INetworkTransportLayer Receive(string url, IDictionary<string, string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
        {
            return _wrappedController.Receive(url, parms, 
                (result) => ProcessResult(true, result, onSuccess),
                (result) => ProcessResult(false, result, onFailure), timeout);
        }

		private void ProcessResult(bool success, WitchesRequestResponse result, Action<WitchesRequestResponse> callback)
        {
            if (IsConnected != success)
            {
                IsConnected = success;
                if (ConnectionStatusEvent != null)
                {
                    ConnectionStatusEvent(this, null);
                }
            }

            if (callback != null)
            {
                callback(result);
            }
        }

    }
}

