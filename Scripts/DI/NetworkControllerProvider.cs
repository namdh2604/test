
using System;
using System.Collections;

namespace Voltage.Witches.DI
{
	using Ninject;
	using Ninject.Activation;

	using Voltage.Common.Net;
	using Voltage.Witches.Net;

	using Voltage.Witches.Services;
	using Voltage.Common.Logging;

	public class NetworkControllerProvider : Provider<WitchesBaseNetworkController>	//<INetworkTimeoutController<WitchesRequestResponse>>
	{
		private readonly string _baseURL;

		private WitchesBaseNetworkController _networkController;	

		public NetworkControllerProvider(string baseURL)
		{
			if (string.IsNullOrEmpty (baseURL)) 
			{
				throw new ArgumentNullException ();
			}

			_baseURL = baseURL;
		}

	
		protected override WitchesBaseNetworkController CreateInstance(IContext context)		
		{
			if(_networkController == null) 
			{
				// FIXME: too many decorators
				_networkController = new WitchesBaseNetworkController(new WitchesNetworkLoggingController(new UnityLogger(), new WitchesNetworkResponseController()), context.Kernel.Get<IBuildNumberService>(), _baseURL);
			}

			return _networkController;
		}
			
	}


}





// FIXME: too many decorators
//			Bind<INetworkTimeoutController<WitchesRequestResponse>>().To<WitchesNetworkResponseController>().WhenInjectedInto<WitchesNetworkLoggingController>();
//			Bind<INetworkTimeoutController<WitchesRequestResponse>>().To<WitchesNetworkLoggingController>().WhenInjectedInto<WitchesBaseNetworkController>();
//			Bind<WitchesBaseNetworkController>().ToSelf().InSingletonScope().WithConstructorArgument("baseURL", BASE_URL);		// singleton scope won't work across kernels