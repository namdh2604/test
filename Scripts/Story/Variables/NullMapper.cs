namespace Voltage.Story.Variables
{
	using Voltage.Story.Mapper;

	// HACK -- Dummy class to make DI-bindings work. This is because of a circular dependency between the story (and the variable mapper) and the player
	// The variable mapper should not be necessary to construct a story OR a player, but until this dependency is removed, this class is needed
	public class NullMapper : IMapping<string>
	{

		public bool TryGetValue(string key, out object value)
		{
			value = null;
			return false;
		}

		public bool TryGetValue(string key, out int value)
		{
			value = 0;
			return false;
		}

		public bool TryGetValue<T>(string key, out T value)
		{
			value = default(T);
			return false;
		}
	}
}

