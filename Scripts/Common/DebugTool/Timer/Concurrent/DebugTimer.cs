
using System;
using System.Collections.Generic;

namespace Voltage.Common.DebugTool.Timer.Concurrent.WIP
{
	using Voltage.Common.Logging;
	using System.Diagnostics;

	public class DebugTimer : IDebugTimer	// UNTESTED
	{
		private readonly string _name;
		public string Name { get { return _name; } }

		private readonly Stopwatch _timer;
		public double ElapsedTimeInSec { get { return _timer.Elapsed.TotalSeconds; }	}	// maybe not?

		public DebugTimer (string name)
		{
			_name = (!string.IsNullOrEmpty(name) ? name : string.Empty);
			_timer = new Stopwatch ();
		}
		
		public void Start ()
		{
			_timer.Start ();
		}
		
		public void Stop()
		{
			_timer.Stop ();
		}

	}

	public class DebugTimerLogger : IDebugTimer
	{
		private readonly IDebugTimer _timer;
		private readonly ILogger _logger;

		public string Name { get { return _timer.Name; } }	
		public double ElapsedTimeInSec { get { return _timer.ElapsedTimeInSec; } }

		public DebugTimerLogger (IDebugTimer timer, ILogger logger)
		{
			if(timer == null || logger == null)
			{
				throw new ArgumentNullException();
			}

			_timer = timer;
			_logger = logger;
		}

		public void Start()
		{
			_logger.Log ("TIMER START: " + Name, LogLevel.INFO);
		}

		public void Stop()
		{
			_logger.Log (string.Format("TIMER STOP: {0} > {1}", Name, ElapsedTimeInSec), LogLevel.INFO);	// _timer.ElapsedTimeInSec
		}


	}
    
}




