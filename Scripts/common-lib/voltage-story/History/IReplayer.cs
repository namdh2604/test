namespace Voltage.Story.History
{
	public interface IReplayer<T,U> 
	{
		T GetRecordAt(U state);
	}
}

