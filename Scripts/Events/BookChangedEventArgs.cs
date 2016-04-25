using System;

namespace Voltage.Witches.Events
{
	public class BookChangedEventArgs : EventArgs
	{
		public BookChangedEventArgs(int index)
		{
			Index = index;
		}

		public int Index { get; protected set; }
	}
}

