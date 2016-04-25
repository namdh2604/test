
using System;
using System.Collections;

namespace Voltage.Witches.DI
{
	using Ninject;
	using Ninject.Activation;

	using Voltage.Common.Logging;

	// provides a single instance of a ScreenNavigationManager when used in a module shared across kernels
	public class ScreenNavigationManagerProvider : Provider<ScreenNavigationManager>	
	{
		private ScreenNavigationManager _navManager;	

		protected override ScreenNavigationManager CreateInstance(IContext context)		
		{
			if(_navManager == null) 
			{
				_navManager = new ScreenNavigationManager ();
			}
				
			return _navManager;
		}
			
	}


}



