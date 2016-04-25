using System;

namespace Voltage.Witches.Events
{
	public class ButtonPressedEventArgs : GUIEventArgs
	{
		public ButtonPressedEventArgs(int index)
		{
			Index = index;
		}

		public int Index { get; protected set; }
	}
}

