namespace Voltage.Common.Serialization
{
	public interface ICachable<T> 
	{
		bool Cache (T data);
		T RetrieveCache();
	}
	
	public interface ICachable
	{
		bool Cache();
		bool LoadCache();
	}
}

