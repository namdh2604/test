


namespace Voltage.Story.Mapper
{
	public interface IMapping<T,U>
	{
		T Retrieve (U key);
	}

	public interface IMapping<U> 	// FIXME: maybe add generic types in declaration instead of at call
	{
//		T Retrieve<T> (U key);
		bool TryGetValue(U key, out object value);
		bool TryGetValue(U key, out int value);
		bool TryGetValue<T> (U key, out T value);
	}
	

}
