using System;

namespace Voltage.Witches.User
{
	using Voltage.Witches.Exceptions;

    public class PlayerStaminaManager : PlayerResourceManager
    {
        private readonly PlayerDataStore _dataStore;

        public PlayerStaminaManager(float updateFrequency, int maxStamina, PlayerDataStore dataStore)
            : base(maxStamina, updateFrequency)
        {
            _dataStore = dataStore;
        }

        public int Stamina {
            get { return _dataStore.stamina; }
            protected set { _dataStore.stamina = value; }
        }

        public override int Amount {
            get { return Stamina; }
            protected set { Stamina = value; }
        }

        public override DateTime LastUpdate {
            get { return _dataStore.staminaLastUpdate; }
            protected set { _dataStore.staminaLastUpdate = value; }
        }

		public void GiveStamina (int amount)	
		{
			if(amount > 0)
			{
				UpdateAmount (amount);
			}
			else
			{
				throw new WitchesException("PlayerStaminaManager::GiveStamina >>> Invalid Operation");
			}
		}

		public void RefillStamina()
		{
			GiveStamina(_maxAmount);
		}
    }
}
