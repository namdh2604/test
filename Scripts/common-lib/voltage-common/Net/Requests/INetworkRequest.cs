using System.Collections.Generic;

namespace Voltage.Common.Net
{
	public interface INetworkRequest
	{
		string URL { get; }
		Dictionary<string,string> Parameters { get; }		// not needed for GET, move its own Interface???
//		Method Method { get; }
	}

//	public enum Method
//	{
//		GET,
//		POST
//	}

}



