namespace Voltage.Common.Serialization
{
	public interface ISavable<T>
	{
		T SavableState();
	}
	
	public interface ISavableState<T>
	{
		T CreateInstance();
	}
}

