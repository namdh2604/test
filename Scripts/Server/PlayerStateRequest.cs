namespace Voltage.Witches.Server
{
	public class PlayerStateRequest : IServerRequest
	{
		const string _address = "/player/{0}";

		public PlayerStateRequest()
		{
		}

		void OnComplete(string json)
		{
		}
	}
}

