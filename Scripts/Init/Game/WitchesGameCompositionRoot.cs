
using System;
using System.Collections.Generic;

namespace Voltage.Witches.DI
{
	using Ninject;
	using Ninject.Modules;

	using iGUI;

	using Voltage.Common.Logging;
	using Voltage.Common.DebugTool.Timer;



	public class WitchesGameCompositionRoot
	{

		private INinjectModule _gameModule;


		public WitchesGameCompositionRoot(iGUIContainer contentPane, iGUIContainer dialoguePane, IWitchesData data)
		{
			_gameModule = new WitchesGameDependencies(data);
		}

        public void Execute(INinjectModule parentModule)
		{
//			AmbientLogger.Current.Log("Game Composition Root", LogLevel.INFO);

			GetIOCContainer(parentModule, _gameModule).Get<WitchesGameStartSequence>().Start();
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




//public class WitchesGameStartSequence
//{
//	private IWitchesData _witchesData;
//
//	public GameResumer GameResumer { get; private set; }
//
//	public WitchesGameStartSequence (Player player, GameResumer gameResumer)
//	{
//		GameResumer = gameResumer;
//	}
//
//
//	public void Start (IWitchesData data)
//	{
//		_witchesData = data;
//
////			HideLoadingScreen ();
//
//		GameResumer.Resume (_witchesData.Player.RouteProgress);
//	}
//
//	private void HideLoadingScreen()
//	{
//
//	}
//
//}



