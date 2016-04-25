using System;

namespace Voltage.Witches.Scheduling.Tasks
{
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
    using Voltage.Witches.Services;

    public class NetworkPollingTask : IRepeatableTask
	{
		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
        private readonly int _interval;

		public NetworkPollingTask(INetworkTimeoutController<WitchesRequestResponse> networkController, int interval=5)
		{
			_networkController = networkController;
            NextExecutionTime = TimeService.Current.UtcNow;
            _interval = interval;
		}

		public void Execute()
		{
            _networkController.Receive(URLs.PING, null, null, null, 10);
            NextExecutionTime = NextExecutionTime.Add(TimeSpan.FromSeconds(_interval));
		}

        public DateTime NextExecutionTime { get; protected set; }

        public string Name { get { return "NetworkPollingTask"; } }
	}
}

