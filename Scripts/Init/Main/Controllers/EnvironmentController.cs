
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Witches.Net;
    using Voltage.Witches.Exceptions;

	using Voltage.Common.Logging;

	public class EnvironmentController
    {
		public EnvironmentData Data { get; private set; }	// not a fan

		private readonly EnvironmentDataFetcher _dataFetcher;
		private readonly IBaseUrl _networkController;
		private readonly IStartupErrorController _errorController;

        public EnvironmentController(EnvironmentDataFetcher dataFetcher, IBaseUrl networkController, IStartupErrorController errorController)
		{
			if (dataFetcher == null || networkController == null || errorController == null)
			{
				throw new ArgumentNullException();
			}

			_dataFetcher = dataFetcher;
			_networkController = networkController;
			_errorController = errorController;
		}

		public void Execute(Action<Exception> callback)
		{
			_dataFetcher.Fetch((e, data) => OnFetch(e, data, callback));
		}

		protected virtual void OnFetch(Exception e, EnvironmentData data, Action<Exception> onComplete)
		{
            if (e != null)
            {
                onComplete(e);
                return;
            }

			Data = data;

			// TODO: check for server maintenance

			if (Data.IsLatest)
			{
				SetBaseURL(Data.URL);

				onComplete(null);
			}
			else
			{
                // This doesn't really seem like an 'error' since it is an expected event in the app's lifecycle
				ErrorUpdateVersion();
			}
		}

		private void SetBaseURL(string baseURL)
		{
			_networkController.BaseURL = "http://" + baseURL;
		}


		private void ErrorUpdateVersion()
		{
			_errorController.ErrorUpdateVersion();			// defer to errorhandler...show dialogue, stop loading, exit to app store
		}
    }
}




