
using System;
//using System.Collections.Generic;

namespace Voltage.Common.DebugTool.Timer
{
	using System.Diagnostics;

    public sealed class DebugTimer : IDebugTimer	// TODO: eventually deprecated by DebugTimerStack!
    {
		private readonly Stopwatch _stopwatch;	// STACK

		private string _activityName;

		public DebugTimer()
		{
			_stopwatch = new Stopwatch ();
			_activityName = string.Empty;
		}

		public void Start(string name)	// (bool reset=true)
		{
			if(IsRunning)
			{
				Stop ();
			}

			_activityName = (!string.IsNullOrEmpty(name) ? name : string.Empty);
			_stopwatch.Start ();
		}

		public void Stop() // (bool reset=true)
		{
			_stopwatch.Stop ();
			Reset ();
		}

		private void Reset()	// could be public
		{
			_activityName = string.Empty;
			_stopwatch.Reset ();
		}

		public bool IsRunning { get { return _stopwatch.IsRunning; } }

		public override string ToString()
		{
			return string.Format ("{0} > {1}sec", _activityName, GetElapsedTimeInSec());
		}

		private string GetElapsedTimeInSec()
		{
			TimeSpan elapsedTime = _stopwatch.Elapsed;
			return elapsedTime.TotalSeconds.ToString ("F");
		}

    }

    
}




