
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Events
{	
	public class CallbackEventArgs : EventArgs
	{
		public CallbackEventArgs(Action callback)
		{
			Callback = callback;
		}

		public Action Callback { get; set; }
	}
}




