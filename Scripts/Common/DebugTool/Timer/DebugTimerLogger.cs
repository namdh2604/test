
using System;
using System.Collections.Generic;

namespace Voltage.Common.DebugTool.Timer
{
	using Voltage.Common.Logging;

    public sealed class DebugTimerLogger : IDebugTimer
    {
		private readonly DebugTimer _timer;		// IDebugTimer
		private readonly ILogger _logger;

		public DebugTimerLogger(DebugTimer timer, ILogger logger)	// IDebugTimer
		{
			if(timer == null || logger == null)
			{
				throw new ArgumentNullException("DebugTimerLogger::Ctor");
			}

			_timer = timer;
			_logger = logger;
		}

		public void Start(string name)
		{
			if(_timer.IsRunning)
			{
				LogOnStop();
			}

			_timer.Start (name);
			LogOnStart ();
		}


		public void Stop()
		{
			LogOnStop ();
			_timer.Stop ();

		}

		private void LogOnStart ()
		{
			_logger.Log (string.Format("DEBUG TIMER START: {0}", _timer.ToString()), LogLevel.INFO);
		}

		private void LogOnStop ()
		{
			_logger.Log (string.Format("DEBUG TIMER END: {0}", _timer.ToString()), LogLevel.INFO);
		}

    }
    
}




