namespace Voltage.Witches.Server
{
	public class ConfigRequest : IServerRequest
	{
		const string ADDRESS = "/config/{0}";
		const string CURRENT = "current";

		string _version;

		public ConfigRequest(string version)
		{
		}
	}
}

