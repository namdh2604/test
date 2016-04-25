
using System;
using System.Collections.Generic;

namespace Voltage.Common.DebugTool.Timer.Concurrent.WIP
{


    public abstract class AmbientDebugTimerFactory : IDebugTimerFactory	// WIP!!!!
    {
		private static IDebugTimerFactory _current;
		public static IDebugTimerFactory Current
		{
			get
			{
				return _current;
			}
			set
			{
				if(value == null)
				{
					Console.WriteLine ("AmbientDebugTimerFactory::Current >>> Setting to default");
					value = new DefaultDebugTimerFactory();
				}
				
				_current = value;
			}
		}
		
		private AmbientDebugTimerFactory(){}
		static AmbientDebugTimerFactory()
		{
			_current = new DefaultDebugTimerFactory ();
		}

		public IDebugTimer Start(string name) 
		{
			return _current.Start (name);
		}

		public void StopAll () 
		{
			_current.StopAll ();
		}



		private class DefaultDebugTimerFactory : IDebugTimerFactory
		{
			public IDebugTimer Start(string name){ return default(IDebugTimer); }	// not great
			public void StopAll () {}
		}
    }
    
}




