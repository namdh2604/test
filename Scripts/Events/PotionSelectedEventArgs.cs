using System;
using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class PotionSelectedEventArgs : GUIEventArgs
	{
		public PotionSelectedEventArgs(Potion potion)
		{
			Potion = potion;
		}

		public Potion Potion { get; protected set; }
	}
}