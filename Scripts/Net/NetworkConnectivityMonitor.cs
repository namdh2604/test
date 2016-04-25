using System;
using System.Collections.Generic;
using Voltage.Witches.User;
using Voltage.Witches.Models;
using Voltage.Witches.Scheduling;
using Voltage.Witches.Scheduling.Tasks;

namespace Voltage.Witches.Net
{
    public class NetworkConnectivityMonitor
    {
		private readonly MonitoringNetworkController _networkController;
        private readonly WitchesNetworkedPlayer _player;
        private readonly ITaskScheduler _scheduler;

        private IRepeatableTask _pingTask;

		public NetworkConnectivityMonitor(MonitoringNetworkController networkController, WitchesNetworkedPlayer player, ITaskScheduler scheduler)
        {
            _networkController = networkController;
            _player = player;
            _scheduler = scheduler;

            _networkController.ConnectionStatusEvent += HandleConnectionReset;
        }

        private void HandleConnectionReset(object sender, EventArgs e)
        {
            if (_networkController.IsConnected)
            {
                // network just came back online
                // stop the polling task if one exists
                if (_pingTask != null)
                {
                    _scheduler.CancelTask(_pingTask);
                    _pingTask = null;
                }
                _player.SyncResources();
            }
            else
            {
                // network disconnected -- set up a task to poll for when a connection is available
                _pingTask = new NetworkPollingTask(_networkController);
                _scheduler.ScheduleRecurring(_pingTask);
            }
        }
    }
}

