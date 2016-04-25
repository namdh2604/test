using System;

namespace Voltage.Witches.Scheduling.Tasks
{
    using Voltage.Story.User;
    using Voltage.Witches.Services;

    public class StaminaUpdateTask : IRepeatableTask
    {
        private readonly IPlayer _player;
        private readonly int _intervalSeconds;
        private DateTime _nextUpdate;

        public StaminaUpdateTask(IPlayer player, int intervalSeconds)
        {
            _player = player;
            _intervalSeconds = intervalSeconds;
        }

        public void Execute()
        {
            if (_player.StaminaNextUpdate <= TimeService.Current.UtcNow)
            {
                _player.UpdateStamina();
            }

            DateTime nextInterval = TimeService.Current.UtcNow.AddSeconds(_intervalSeconds);

            _nextUpdate = (nextInterval < _player.StaminaNextUpdate) ? nextInterval : _player.StaminaNextUpdate;
			if (_nextUpdate < TimeService.Current.UtcNow)
			{
				// This condition is here if we ever get a situation where the _player.StaminaNextUpdate is currupted, this will stop the game from 
				// spamming the server.  We have seen this happen in the past with players and have not fully identify the caused.
				_nextUpdate = TimeService.Current.UtcNow.AddSeconds (30);
			}
        }

        public DateTime NextExecutionTime {
            get { return _nextUpdate; }
        }

        public string Name { get { return "StaminaUpdateTask"; } }
    }
}

