using System;
using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class OutfitSelectedEventArgs : EventArgs
	{
        public string Name { get; protected set; }

        public OutfitSelectedEventArgs(string name)
		{
            Name = name;
		}
	}
}