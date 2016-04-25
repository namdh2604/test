
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{
	using Voltage.Common.Logging;
	using Voltage.Common.Net;

	public interface IMaintenanceTrigger
	{
		event EventHandler OnMaintenanceEvent;
	}

	public class WitchesNetworkMaintenanceController : INetworkTimeoutController<WitchesRequestResponse>, IMaintenanceTrigger
    {

		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		public event EventHandler OnMaintenanceEvent;	// maybe???

		public WitchesNetworkMaintenanceController(INetworkTimeoutController<WitchesRequestResponse> networkController)
		{
			if(networkController == null)
			{
				throw new ArgumentNullException();
			}

			_networkController = networkController;
		}


		public INetworkTransportLayer Send (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			return _networkController.Send (url, parms, onSuccess, (response) => CheckForMaintenance(response, onFailure), timeout);
		}
		
		public INetworkTransportLayer Receive (string url, IDictionary<string,string> parms, Action<WitchesRequestResponse> onSuccess, Action<WitchesRequestResponse> onFailure, int timeout=30)
		{
			return _networkController.Receive (url, parms, onSuccess, (response) => CheckForMaintenance(response, onFailure), timeout);
		}

		private void CheckForMaintenance(WitchesRequestResponse response, Action<WitchesRequestResponse> onFailure)
		{
			if(response.Status == WitchesRequestStatus.MAINTENANCE)
			{
				OnServerMaintenance();
			}
			else
			{
				onFailure(response);
			}
		}

		private void OnServerMaintenance()
		{
			AmbientLogger.Current.Log ("Server Maintenance", LogLevel.WARNING);
			OnMaintenanceEvent (this, new EventArgs ());
		}
    }
    
}




