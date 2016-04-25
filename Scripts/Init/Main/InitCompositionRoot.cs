

using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Ninject;
	using Ninject.Modules;

	using Voltage.Common.Logging;
	using Voltage.Common.Net;


	using iGUI;





    public class InitCompositionRoot
    {
		private INinjectModule _configurationModule;

//		public InitCompositionRoot()
        public InitCompositionRoot(iGUIContainer contentPane, iGUIContainer dialoguePane, iGUIContainer overlayPane)
		{
			_configurationModule = new ConfigurationDependencies (contentPane, dialoguePane, overlayPane);
		}


        public void Execute (Action<IWitchesData, INinjectModule> callback)
		{
            Action<IWitchesData> callbackWrapper = delegate(IWitchesData data) {
                if (callback != null)
                {
                    callback(data, _configurationModule);
                }
            };
			IKernel kernel = GetIOCContainer(_configurationModule);
			var sequence = kernel.Get<WitchesStartupSequence>();
			sequence.Start(callbackWrapper);
		}


		private IKernel GetIOCContainer(params INinjectModule[] modules)
		{
			if(modules != null)
			{
				NinjectSettings settings = new NinjectSettings();
				settings.LoadExtensions = false;
				settings.UseReflectionBasedInjection = true;
				return new StandardKernel(settings, modules);
			}

			throw new ArgumentNullException ("InitCompositionRoot::GetDIContainer >>> ");
		}

    }
    


}




