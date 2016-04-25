namespace Voltage.Story.History
{
	public interface IRecorder<T,U>
	{
		bool Record(T save, U state, bool overwrite);
	}
}

