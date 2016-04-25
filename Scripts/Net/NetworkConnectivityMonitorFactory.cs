using System;

namespace Voltage.Witches.Net
{
    using Voltage.Witches.Scheduling;
    using Voltage.Witches.Models;

    public class NetworkConnectivityMonitorFactory
    {
		private readonly MonitoringNetworkController _networkController;
        private readonly ITaskScheduler _scheduler;

		public NetworkConnectivityMonitorFactory(MonitoringNetworkController networkController, ITaskScheduler scheduler)
        {
            _networkController = networkController;
            _scheduler = scheduler;
        }

        public NetworkConnectivityMonitor Create(WitchesNetworkedPlayer player)
        {
            return new NetworkConnectivityMonitor(_networkController, player, _scheduler);
        }
    }
}

