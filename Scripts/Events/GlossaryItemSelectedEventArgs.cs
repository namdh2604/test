using System;

namespace Voltage.Witches.Events
{
	public class GlossaryItemSelectedEventArgs : GUIEventArgs
	{
		public GlossaryItemSelectedEventArgs(int index, string itemKey)
		{
			ItemKey = itemKey;
			Index = index;
		}

		public string ItemKey { get; protected set; }
		public int Index { get; protected set; }
	}
}