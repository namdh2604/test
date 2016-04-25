
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	public interface IErrorHandler
	{
		void HandleError(int code);
	}

	public interface IStartupErrorController	// HACK: 
	{
		void ErrorUpdateVersion();
		void ErrorServerMaintenance();
		void ErrorGetEnvironment();
		void ErrorCreatePlayer();
		void ErrorGetMasterConfig();
		void ErrorOnRestorePlayer();
		void ErrorOnShopSetup ();
	}


    public class StartupErrorController : IStartupErrorController
    {
		private readonly StartupDisplayController _displayController;

		public StartupErrorController(StartupDisplayController displayController)
		{
			if(displayController == null)
			{
				throw new ArgumentNullException();
			}

			_displayController = displayController;
		}

		public void ErrorUpdateVersion()
		{
			// make sure show LoadingScreeen?

			// show dialogue, stop loading
//			_displayController.StopLoadingScreen ();
			_displayController.ShowLoadingScreenDialogue (StartupDisplayController.DialogueType.ERROR_FORCEUPDATE);

			// exit to app store on user input
		}
		
		public void ErrorServerMaintenance()
		{
			// make sure show LoadingScreeen?

			// show dialogue, stop loading
			_displayController.StopLoadingScreen ();
			_displayController.ShowLoadingScreenDialogue (StartupDisplayController.DialogueType.ERROR_MAINTENANCE);

			// exit on user input
		}

		public void ErrorGetEnvironment()
		{
			// make sure show LoadingScreeen?

			_displayController.StopLoadingScreen ();
			_displayController.ShowLoadingScreenDialogue (StartupDisplayController.DialogueType.ERROR_INIT);
		}

		public void ErrorCreatePlayer()
		{

		}

		public void ErrorGetMasterConfig()
		{

		}

		public void ErrorOnRestorePlayer()
		{

		}

		public void ErrorOnShopSetup()
		{

		}

    }
    
}




