using System;
using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class ClosetItemChangedEventArgs : EventArgs
	{
		public ClosetItemChangedEventArgs(int index, Clothing clothing)
		{
			Index = index;
			Clothing = clothing;
		}

		public Clothing Clothing { get; protected set; }

		public int Index { get; protected set; }
	}
}