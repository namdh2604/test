namespace Voltage.Common.Serialization
{
	public interface ISerializer<T>
	{
		bool Serialize(T data, string fullpath);
	}

	public interface IDeserializer<T>
	{
		T Deserialize (string filepath);
	}
}





