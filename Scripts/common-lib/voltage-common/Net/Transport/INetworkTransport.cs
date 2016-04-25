
using System;
using System.Collections.Generic;

namespace Voltage.Common.Net
{

	public interface INetworkTransportLayer
	{
		void Send();
		float Progress { get; }
	}


    
}



