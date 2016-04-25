using System;
using System.Collections.Generic;
using System.Collections;


namespace Voltage.Witches.Shop
{
	using Voltage.Witches.Models;
//	using Voltage.Witches.User;
    using Voltage.Witches.Configuration;

	public class StarterPackEvaluator 
	{
		private readonly Player _player;
        private readonly MasterConfiguration _masterConfig;

        public StarterPackEvaluator(Player player, MasterConfiguration masterConfig)    //, double durationInMinutes)
		{
			// TODO: guard clause

			_player = player;
            _masterConfig = masterConfig;
		}


        public bool IsAvailable()   
        {
            return !HadPurchased() && PackForSale() && PackEnabled();
        }

            
        private bool HadPurchased()
        {
            return _player.StarterPackPurchased;
        }

        private bool PackEnabled()
        {
            if (!_player.StarterPackTriggered)
            {
                return false;
            }

            return _player.TimeToDisableStarterPack > DateTime.UtcNow;
        }

        private bool PackForSale()
        {
            return _masterConfig.Shop_Items.StarterPack != null;
        }

	}
}
