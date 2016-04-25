
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.Reset
{
	using Voltage.Common.Logging;

	using Voltage.Story.Reset;
	using Voltage.Witches.Models;
	

	public class WitchesRouteResetter : IStoryResetter
	{
		
		protected readonly Player _player;
		private readonly List<string> _availableScenesOnReset;

		
		public WitchesRouteResetter(Player player, params string[] availableScenesOnReset)
		{
			if(player == null)
			{
				throw new ArgumentNullException();
			}

			_player = player;

			_availableScenesOnReset = (availableScenesOnReset != null ? new List<string>(availableScenesOnReset) : new List<string>());
		}
		
		public virtual void Reset()
		{
			AmbientLogger.Current.Log ("Resetting Player", LogLevel.INFO);

			_player.CompleteRoute ();
			SetAvailableScenes ();
		}
		
		private void SetAvailableScenes()	
		{
			foreach (string scene in _availableScenesOnReset)	
			{
				_player.AddAvailableScene(scene);
			}
		}

	}
}




