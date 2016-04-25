
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Voltage.Story.General;

	using Voltage.Witches.Services;
	using Voltage.Common.Startup;
	using Voltage.Common.IAP;
	using Voltage.Witches.IAP;

	public class StartupShopController
    {
		public ITransactionProcessor Data { get; private set; }

		private readonly IVersionService _versionService;
		private readonly TransactionProcessorFactory _transactionProcessorFactory;

		private readonly IStartupErrorController _errorController;

		public StartupShopController(IVersionService versionService, TransactionProcessorFactory transactionProcessorFactory, IStartupErrorController errorController)
		{
			if(transactionProcessorFactory == null || versionService == null || errorController == null)
			{
				throw new ArgumentNullException();
			}

			_versionService = versionService;
			_transactionProcessorFactory = transactionProcessorFactory;
			_errorController = errorController;
		}

		public void Execute(Action<Exception> callback)
		{
            try
            {
    			ClientEnvironment env = GetEnvironment();

    			_transactionProcessorFactory.Create(env, (e, processor) => OnProcessorReady(e, processor, callback));
            }
            catch (Exception e)
            {
                callback(e);
            }
		}


		private void OnProcessorReady(Exception e, ITransactionProcessor processor, Action<Exception> callback)
		{
            if (e != null)
            {
                callback(e);
                return;
            }

			Data = processor;

            callback(null);
		}

		private ClientEnvironment GetEnvironment()
		{
			return _versionService.Environment;
		}

		private void OnError()
		{
			_errorController.ErrorOnShopSetup ();
		}
    }
    
}




