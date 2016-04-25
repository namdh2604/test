

namespace Voltage.Witches.Services
{
	public enum ClientEnvironment
	{
//		NONE = 0,
		DEVELOPMENT = 0,
		STAGING,
		PRODUCTION,
		CUSTOM,
	}

    public interface IVersionService
    {
		ClientEnvironment Environment { get; }
		string BuildNumber { get; }
    }
    
}



