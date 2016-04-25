using System;

namespace Voltage.Witches.User
{
    using Voltage.Witches.Services;
    using Voltage.Witches.Exceptions;

    public abstract class PlayerResourceManager
    {
        public event EventHandler ResourceUpdate;

        public PlayerResourceManager(int maxAmount, float updateFrequency)
        {
            if (updateFrequency == 0)
            {
                throw new WitchesException("Resource update frequency cannot be 0");
            }

            _maxAmount = maxAmount;
            _updateFrequency = updateFrequency;
        }

        public void Update()
        {
            DateTime currentTime = TimeService.Current.UtcNow;

            TimeSpan elapsed = currentTime.Subtract(LastUpdate);
            float totalMinutes = (float)elapsed.TotalMinutes;

            int numWindows = (int)Math.Floor(totalMinutes / _updateFrequency);

            int prevValue = Amount;

            UpdateAmount(numWindows);
            if ((Amount != prevValue) || (IsMaxed()))
            {
                if (!IsMaxed())
                {
                    LastUpdate = LastUpdate.AddMinutes(_updateFrequency * numWindows);
                }
                else
                {
                    LastUpdate = currentTime;
                }
            }
        }

        public void Consume()
        {
            if (Amount <= 0)
            {
                throw new WitchesException("No resource available to consume");
            }

            // Updates are re-calculated after the initial deduction from max.
            // If this did not happen, users could essentially go over the max by consuming just before the next scheduled update
            if (IsMaxed())
            {
                LastUpdate = TimeService.Current.UtcNow;
            }

            UpdateAmount(-1);
        }

        public bool IsMaxed()
        {
            return (Amount >= _maxAmount);
        }

        protected void UpdateAmount(int difference)
        {
            int initial = Amount;

            UpdateAmountInternal(difference);

            if ((Amount != initial) && (ResourceUpdate != null))
            {
                ResourceUpdate(this, new EventArgs());
            }
        }

        private void UpdateAmountInternal(int difference)
        {
            int result = Amount + difference;

            if (result < 0)
            {
                throw new WitchesException("Attempted to set resource amount negative");
            }

            if (result > _maxAmount)
            {
                result = _maxAmount;
            }

            Amount = result;
        }

        public void SetAmount(int amount, int nextUpdateSeconds)
        {
            DateTime initialTime = LastUpdate;
            int difference = amount - Amount;

            UpdateAmountInternal(difference);

            if (IsMaxed())
            {
                LastUpdate = TimeService.Current.UtcNow;
            }
            else
            {
                LastUpdate = TimeService.Current.UtcNow.AddSeconds(nextUpdateSeconds) - new TimeSpan(0, (int)_updateFrequency, 0);
            }

            if (((difference != 0) || (LastUpdate != initialTime)) && (ResourceUpdate != null))
            {
                ResourceUpdate(this, new EventArgs());
            }
        }

        public abstract int Amount { get; protected set; }
        public abstract DateTime LastUpdate { get; protected set; }

        protected readonly int _maxAmount;
        protected readonly float _updateFrequency;
        public DateTime NextUpdate { get { return LastUpdate.AddMinutes(_updateFrequency); } }

        public int GetSecondsUntilMaxed()
        {
            if (IsMaxed())
            {
                return 0;
            }

            DateTime maxDate = NextUpdate;

            int numToRegenerate = _maxAmount - Amount;
            if (numToRegenerate > 1)
            {
                maxDate = maxDate.AddMinutes(_updateFrequency * (numToRegenerate - 1));
            }

            return (int)(maxDate.Subtract(DateTime.UtcNow).TotalSeconds);
        }
    }
}

