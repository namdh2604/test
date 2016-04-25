
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Witches.Configuration;

    public class LocalDataController
    {
		public LocalData Data { get; private set; }

		private readonly ILocalDataFetcher _dataFetcher;

		public LocalDataController(ILocalDataFetcher dataFetcher)	// TODO: errorhandling
		{
			if (dataFetcher == null)
			{
				throw new ArgumentNullException();
			}

			_dataFetcher = dataFetcher;
		}

		public void Execute(Action<Exception> callback)
		{
			_dataFetcher.Fetch((e, data) => OnFetchLocalData(e, data, callback));
		}

		private void OnFetchLocalData(Exception e, LocalData data, Action<Exception> callback)
		{
            if (e != null)
            {
                callback(e);
                return;
            }

			Data = data;

			callback(null);
		}

    }

    
}




