
using System;


namespace Voltage.Common.DebugTool.Timer
{

    public interface IDebugTimer	// maybe replace with Timer.Concurrent
    {
		void Start(string name);
		void Stop();
//		void Pause();

//		string Print();
    }
    
	public abstract class AmbientDebugTimer
	{
		private static IDebugTimer _current;
		
		public static IDebugTimer Current
		{
			get
			{
				return _current;
			}
			set
			{
				if(value == null)
				{
					Console.WriteLine ("AmbientDebugTimer::Current >>> Setting to default");
					value = new DefaultDebugTimer();
				}
				
				_current = value;
			}
		}
		
		private AmbientDebugTimer(){}
		static AmbientDebugTimer()
		{
			_current = new DefaultDebugTimer ();
		}
		
		private class DefaultDebugTimer : IDebugTimer
		{
			public void Start(string name){}
			public void Stop () {}
		}
	}
}



