
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DebugTool
{
	using Voltage.Witches.Models;

    public class DebugPlayer // : Player
    {
//		private readonly Player _player;

		public DebugPlayer(Player player) 
		{
			if(player == null)
			{
				throw new ArgumentNullException("DebugPlayer::Ctor");
			}

//			_player = player;
		}

		public void ClearAllAvailableScenes()
		{
//			_player.ClearAllAvailableScenes ();
		}
    }
    
}




