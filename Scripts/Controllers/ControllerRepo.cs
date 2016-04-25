using Voltage.Witches.Models;
using Voltage.Story.Variables;
using Voltage.Witches.Screens;
using Voltage.Witches.Controllers;

using Ninject;
using Ninject.Modules;
using Voltage.Witches.DI;

namespace Voltage.Witches.Controllers
{
	using Voltage.Witches.Configuration;
    using Voltage.Witches.Services;
    using Voltage.Story.Effects;
	using Voltage.Witches.Net;
	using Voltage.Common.Net;

	using Voltage.Story.Configurations;
	using Voltage.Story.General;

	using Voltage.Story.StoryDivisions;

	using Scene = Voltage.Story.StoryDivisions.Scene;

	using Voltage.Common.IAP;
	using Voltage.Witches.Bundles;


    public interface IControllerRepo
    {
        T Get<T>();
    }


//	public class ControllerRepoReferences
//	{
//		public ScreenNavigationManager NavManager { get; set; }
//		public Player Player { get; set; }
//		public IEffectResolver EffectResolver { get; set; }
//		public VariableMapper VariableMapper { get; set; }
//		public IScreenFactory ScreenFactory { get; set; }
//		public IFilesystemService FileSystemService { get; set; }
//		public MasterConfiguration MasterConfig { get; set; }
//		public WitchesNetworkController NetworkController { get; set; }
//		public Voltage.Story.StoryDivisions.Story Story { get; set; }
//	}


    public class ControllerRepo : IControllerRepo
    {
        private IKernel _kernel;
        NinjectModule _module;

//		public ControllerRepo(INinjectModule module)
//		public ControllerRepo(SceneDI.ServiceLocatorData data)	// ServiceLocatorData doesn't seem to be defined before ControllerRepo by Ninject
		public ControllerRepo(ScreenNavigationManager manager, Player player, IEffectResolver effectResolver, VariableMapper variableMapper, IScreenFactory screenFactory,
		                      IFilesystemService filesystem, MasterConfiguration masterConfig, INetworkTimeoutController<WitchesRequestResponse> networkController, INetworkMonitor networkMonitor,
		                      IFactory<string,Scene> sceneFactory, IFactory<string,SceneHeader> sceneHeaderFactory, MasterStoryData storyData, ITransactionProcessor transactionProcessor, IBuildNumberService numberService,
							  Inventory inventory, IAssetBundleManager bundleManager)
        {
			SceneDI.ServiceLocatorData data = new SceneDI.ServiceLocatorData
			{
				NavManager = manager,
				Player = player,
				EffectResolver = effectResolver,
				VariableMapper = variableMapper,
				ScreenFactory = screenFactory,
				FileSystemService = filesystem,
				MasterConfig = masterConfig,
				NetworkController = networkController,
				SceneFactory = sceneFactory,
				SceneHeaderFactory = sceneHeaderFactory,
				StoryData = storyData,
				TransactionProcessor = transactionProcessor,
				BuildNumberService = numberService,
				NetworkMonitor = networkMonitor,
				Inventory = inventory,
				AssetBundleManager = bundleManager,
			};

			_module = new SceneDI (data, this);
            NinjectSettings settings = new NinjectSettings();
            settings.LoadExtensions = false;
            settings.UseReflectionBasedInjection = true;
            _kernel = new StandardKernel(settings, _module);
        }

        public T Get<T>()
        {
            return _kernel.Get<T>();
        }

    }
}

