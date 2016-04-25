using System;

namespace Voltage.Witches.User
{
    public class PlayerFocusManager : PlayerResourceManager
    {
        private readonly PlayerDataStore _dataStore;

        public PlayerFocusManager(float updateFrequency, int maxFocus, PlayerDataStore dataStore)
            : base(maxFocus, updateFrequency)
        {
            _dataStore = dataStore;
        }

        public int Focus { 
            get { return _dataStore.focus; } 
            protected set { _dataStore.focus = value; }
        }

		public void AddFocus(int amount)
		{
			_dataStore.focus += amount;
		}

        public override int Amount {
            get { return Focus; }
            protected set { Focus = value; }
        }

        public override DateTime LastUpdate {
            get { return _dataStore.focusLastUpdate; }
            protected set { _dataStore.focusLastUpdate = value; }
        }
    }
}

