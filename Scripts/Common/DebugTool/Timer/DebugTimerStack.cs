
using System;
using System.Collections.Generic;

namespace Voltage.Common.DebugTool.Timer
{
	using System.Diagnostics;

    public class DebugTimerStack : IDebugTimer	// WIP
    {
		private Stack<TimerEvent> _timerStack = new Stack<TimerEvent> ();

		public DebugTimerStack()
		{
			throw new NotImplementedException ("UNTESTED");
//			_timerStack = new Stack<TimerEvent> ();
		}

		public void Start (string name)
		{
			TimerEvent timerEvent = new TimerEvent (name);
			timerEvent.Timer.Start ();
			_timerStack.Push (timerEvent);
		}

		public void Stop ()	// FIXME: NEED TO STOP ASSOCIATED TIMEREVENT (IE, START)...NOT just whatever is at top of stack...may require update to interface
		{
			throw new NotImplementedException ("Needs to stop corresponding TimerEvent, not just whatever is at the top of the stack");
//			if(_timerStack.Count > 0)
//			{
//				TimerEvent timerEvent = _timerStack.Pop ();
//				timerEvent.Timer.Stop ();
//			}
		}


		public void StopAll()
		{
			while(_timerStack.Count > 0) // _timerStack.Clear()
			{
				Stop ();
			}
		}


		public override string ToString()
		{
			return string.Format ("{0} > {1}sec", GetCurrentTimerName(), GetCurrentTimerElapsedTimeInSec());
		}

		private string GetCurrentTimerName()
		{
			if(_timerStack.Count > 0)
			{
				TimerEvent timerEvent = _timerStack.Peek();
				return timerEvent.Name;
			}
			else
			{
				return string.Empty;
			}
		}


		private string GetCurrentTimerElapsedTimeInSec()
		{
			if(_timerStack.Count > 0)
			{
				TimerEvent timerEvent = _timerStack.Peek();

				TimeSpan elapsedTime = timerEvent.Timer.Elapsed;
				return elapsedTime.TotalSeconds.ToString ("F");
			}
			else
			{
				return string.Empty;
			}
		}

		private struct TimerEvent
		{
			public string Name;
			public Stopwatch Timer;

			public TimerEvent(string name)
			{
				Name = (!string.IsNullOrEmpty(name) ? name : string.Empty);
				Timer = new Stopwatch();
			}
		}

    }
    
}




