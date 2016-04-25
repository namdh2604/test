
using System;
using System.Collections.Generic;

namespace Voltage.Common.Android.ExpansionFile
{
	using Voltage.Common.Logging;

	public class PassThruOBBFetcher : IExpansionFileFetcher
    {
		public void Fetch(Action<Exception> callback)
		{
			AmbientLogger.Current.Log (string.Format("PassThruOBBFetcher::Fetch..."), LogLevel.INFO);
			callback(null);
		}

    }
    
}




