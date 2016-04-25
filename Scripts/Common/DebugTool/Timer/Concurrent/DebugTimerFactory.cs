
using System;
using System.Collections.Generic;

namespace Voltage.Common.DebugTool.Timer.Concurrent.WIP
{


	public class DebugTimerFactory : IDebugTimerFactory	// UNTESTED
	{
		private IList<IDebugTimer> _timerList = new List<IDebugTimer> ();

		public IDebugTimer Start (string name)
		{
			IDebugTimer timer = new DebugTimer (name);

			_timerList.Add (timer);
			timer.Start ();
			
			return timer;
		}
		
		public void StopAll()
		{
			foreach(IDebugTimer timer in _timerList)
			{
				timer.Stop();
			}
		}
	}
    
}




