using System;

namespace Voltage.Witches.Scheduling.Tasks
{
    using Voltage.Story.User;
    using Voltage.Witches.Services;

    public class FocusUpdateTask : IRepeatableTask
    {
        private readonly IPlayer _player;
        private readonly int _intervalSeconds;
        private DateTime _nextUpdate;

        public FocusUpdateTask(IPlayer player, int intervalSeconds)
        {
            _player = player;
            _intervalSeconds = intervalSeconds;
        }

        public void Execute()
        {
            if (_player.FocusNextUpdate <= TimeService.Current.UtcNow)
            {
                _player.UpdateFocus();
            }

            DateTime nextInterval = TimeService.Current.UtcNow.AddSeconds(_intervalSeconds);
            _nextUpdate = (nextInterval < _player.FocusNextUpdate) ? nextInterval : _player.FocusNextUpdate;
        }

        public DateTime NextExecutionTime {
            get { return _nextUpdate; }
        }

        public string Name { get { return "FocusUpdateTask"; } }
    }
}

