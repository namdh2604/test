using System;
using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class ItemReceivedArgs : GUIEventArgs
	{
		public Item ReceivedItem { get; protected set; }

		public ItemReceivedArgs(Item item)
		{
			ReceivedItem = item;
		}
	}
}