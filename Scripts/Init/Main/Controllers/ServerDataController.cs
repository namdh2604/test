
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
//	using Voltage.Common.Startup;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Configuration.JSON;


    public class ServerDataController //: IStartupDataTask<ServerData>
    {
		public ServerData Data { get; private set; }	// not a fan

        private readonly MasterConfigFetcher _masterConfigDataFetcher;

		public ServerDataController(MasterConfigFetcher masterConfigDataFetcher)
		{
			if (masterConfigDataFetcher == null)
			{
				throw new ArgumentNullException();
			}

			_masterConfigDataFetcher = masterConfigDataFetcher;
		}


		public void Execute(Action<Exception> callback)
		{
			_masterConfigDataFetcher.Fetch((e, masterConfig) => OnReceivedMasterConfigText(e, masterConfig, callback));
		}

		private void OnReceivedMasterConfigText(Exception e, MasterConfiguration masterConfig, Action<Exception> callback)
		{
            if (e != null)
            {
                callback(e);
                return;
            }

            try
            {
                Data = new ServerData
                {
                    MasterConfig = masterConfig
                };

                callback(null);
            }
            catch (Exception ex)
            {
                callback(ex);
            }
		}

		public class ServerData
		{
			public MasterConfiguration MasterConfig { get; set; }
		}
    }


    
}




