
namespace Voltage.Common.DebugTool.Timer.Concurrent.WIP
{
	public interface IDebugTimerFactory // or IDebugTimer
	{
		IDebugTimer Start(string name);
		void StopAll ();
	}


	public interface IDebugTimer
	{
		void Start ();
		void Stop();

		string Name { get; }	// maybe?
		double ElapsedTimeInSec { get; }	// maybe?
	}
	

}


