
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Story.Reset
{
	using Voltage.Story.Reset;
	using Voltage.Witches.Models;

	using Voltage.Common.Net;

	public sealed class WitchesOptionsResetter : WitchesRouteResetter
	{
		public WitchesOptionsResetter(Player player, params string[] availableScenesOnReset) : base (player, availableScenesOnReset) {}

		public override void Reset ()
		{
			_player.ClearAllAvailableScenes ();
			base.Reset ();
		}
    }
    
}




