using System.Collections.Generic;

using Ninject.Modules;

using Voltage.Witches.Models;
using Voltage.Story.StoryDivisions;
using Voltage.Story.Expressions;
using Voltage.Story.Variables;

using Voltage.Witches.Screens;
using Voltage.Witches.Controllers;
using Voltage.Witches.Controllers.Factories;
using Voltage.Witches.Story;
using Voltage.Story.General;
using Voltage.Story.Text;
using Voltage.Story.User;
using Voltage.Witches.Models.MissionRequirements;


namespace Voltage.Witches.DI
{
	using Ninject;

	using Voltage.Witches.Configuration;
    using Voltage.Witches.Configuration.JSON;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Common.Logging;
	using Voltage.Witches.Shop;
    using Voltage.Witches.Services;
    using Voltage.Story.Effects;

	using Voltage.Common.IAP;
	using Voltage.Witches.IAP;

	using Voltage.Story.Reset;
	using Voltage.Witches.Story.Reset;

	using Voltage.Story.General;
	using Voltage.Witches.Story.StoryDivisions.Factory;

	using Scene = Voltage.Story.StoryDivisions.Scene;
	
	using Voltage.Story.Configurations;

	using Voltage.Story.StoryDivisions;

	using Voltage.Witches.Factory;

	using Voltage.Witches.User;

	using Voltage.Witches.Bundles;

    public class SceneDI : NinjectModule
    {
        ScreenNavigationManager _manager;
        Player _player;
        VariableMapper _variableContext;
        IEffectResolver _effectResolver;
        IScreenFactory _screenFactory;
        IControllerRepo _controllerRepo;
        IFilesystemService _filesystem;
		MasterConfiguration _masterConfig;

		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
		private readonly INetworkMonitor _networkMonitor;

		private readonly MasterStoryData _storyData;
		private readonly IFactory<string,Scene> _sceneFactory;
		private readonly IFactory<string,SceneHeader> _sceneHeaderFactory;
		private readonly ITransactionProcessor _transactionProcessor;
		private readonly IBuildNumberService _buildNumberService;
		private readonly Inventory _inventory;
		private readonly IAssetBundleManager _bundleManager;

		private List<string> _availableScenesOnReset = new List<string>
		{
			"Prologue/Prologue/Answers and More Questions",
//			"Prologue/Prologue/The Huntress",
//			"Prologue/Prologue/The Aristocrat",
//			"Prologue/Prologue/The Councilman",
//			"Prologue/Prologue/The Soldier",
		};

	

		public class ServiceLocatorData
		{
			public Player Player { get; set; }
			public MasterConfiguration MasterConfig { get; set; }
			public MasterStoryData StoryData { get; set; }
			public ScreenNavigationManager NavManager { get; set; }
			public IScreenFactory ScreenFactory { get; set; }
//			public IControllerRepo Repo { get; set; }
			public INetworkTimeoutController<WitchesRequestResponse> NetworkController { get; set; }
			public IFilesystemService FileSystemService { get; set; }
			public IEffectResolver EffectResolver { get; set; }
			public VariableMapper VariableMapper { get; set; }
			public IFactory<string,Scene> SceneFactory { get; set; }
			public IFactory<string,SceneHeader> SceneHeaderFactory { get; set; }
			public ITransactionProcessor TransactionProcessor { get; set; }
			public IBuildNumberService BuildNumberService { get; set; }
			public INetworkMonitor NetworkMonitor { get; set; }
			public Inventory Inventory { get; set; }
			public IAssetBundleManager AssetBundleManager { get; set; }
		}

		public SceneDI(ServiceLocatorData data, IControllerRepo repo)
		{
			_player = data.Player;
			_masterConfig = data.MasterConfig;
			_storyData = data.StoryData;
			_manager = data.NavManager;
			_screenFactory = data.ScreenFactory;
			_networkController = data.NetworkController;
			_filesystem = data.FileSystemService;
			_effectResolver = data.EffectResolver;
			_variableContext = data.VariableMapper;
			_sceneFactory = data.SceneFactory;
			_sceneHeaderFactory = data.SceneHeaderFactory;
			_transactionProcessor = data.TransactionProcessor;
			_buildNumberService = data.BuildNumberService;
			_networkMonitor = data.NetworkMonitor;
			_inventory = data.Inventory;
			_bundleManager = data.AssetBundleManager;

//			_controllerRepo = data.Repo;
			_controllerRepo = repo;


		}




        public override void Load()
        {
			Bind<Inventory> ().ToConstant (_inventory);

			Bind<IAssetBundleManager>().ToConstant(_bundleManager);
			Bind<IAvatarThumbResourceManager>().To<AvatarThumbResourceManager>().InSingletonScope();

			Bind<ScreenNavigationManager> ().ToConstant (_manager).InSingletonScope();
            Bind<IPlayer>().ToConstant(_player);
            Bind<Player>().ToConstant(_player);
            Bind<IEffectResolver>().ToConstant(_effectResolver);
            Bind<VariableMapper>().ToConstant(_variableContext);
            Bind<IScreenFactory>().ToConstant(_screenFactory);
            Bind<IFilesystemService>().ToConstant(_filesystem);
           
			Bind<MasterStoryData> ().ToConstant (_storyData);

            Bind<IAudioController>().ToProvider(new SceneObjectProvider<AudioController>("SoundManager"));
            Bind<StoryMusicPlayer>().ToSelf().WithConstructorArgument("lookupTable", _storyData.MusicMap);

//            Bind<StoryMapScreenController>().ToSelf();

            Bind<ISceneViewModelFactory>().To<SceneViewModelFactory>();
            Bind<ILockClassifier>().To<LockClassifier>();
            Bind<IExpressionClassifier>().To<ExpressionClassifier>();
            Bind<IVariableExpressionClassifier>().To<VariableExpressionClassifier>();

            Bind<IControllerRepo>().ToConstant(_controllerRepo);

            Bind<IItemFactory>().To<ItemFactory>();
            Bind<IItemRawParser>().To<ItemRawParser>();

			Bind<IStoryResetter> ().To<WitchesRouteResetter> ().WithConstructorArgument("availableScenesOnReset", _availableScenesOnReset.ToArray());	// TODO: start moving dependencies out of here and into ConfigurationDependencies
			Bind<WitchesOptionsResetter>().ToSelf().WithConstructorArgument("availableScenesOnReset", _availableScenesOnReset.ToArray());

			Bind<IFactory<string,Scene>> ().ToConstant (_sceneFactory);
			Bind<IStoryLoaderFactory> ().To<StoryLoaderFactory> ();
			
//			Bind<IStoryLoaderFactory>().To<StoryLoaderFactory>().WithConstructorArgument("masterConfig", _masterConfig.GameResources);

            Bind<IStoryMapScreenControllerFactory>().To<StoryMapScreenControllerFactory>();
            Bind<IWitchesStoryPlayerScreenControllerFactory>().To<WitchesStoryPlayerScreenControllerFactory>().WithConstructorArgument("resourceConfig", _masterConfig.GameResources);
            Bind<WitchesStoryPlayerTutorialScreenControllerFactory>().ToSelf().WithConstructorArgument("resourceConfig", _masterConfig.GameResources);
//            Bind<IStoryPlayerDialogController>().To<TutorialDialogController>().WhenInjectedInto<WitchesStoryPlayerTutorialScreenControllerFactory>();
			Bind<NormalStoryPlayerDialogController>().ToSelf().WhenInjectedInto<WitchesStoryPlayerTutorialScreenControllerFactory>().WithConstructorArgument ("isInTutorial", true);
            Bind<IStoryPlayerDialogController>().To<NormalStoryPlayerDialogController>().WhenInjectedInto<WitchesStoryPlayerScreenControllerFactory>();
            Bind<IIngredientsSelectScreenControllerFactory>().To<IngredientsSelectScreenControllerFactory>();
            Bind<IInventoryScreenControllerFactory>().To<InventoryScreenControllerFactory>();


			Bind<HomeScreenControllerFactory>().ToSelf().InSingletonScope();

			Bind<MasterConfiguration> ().ToConstant (_masterConfig);
			
            Bind<Voltage.Witches.Login.BonusManager>().ToSelf().InSingletonScope();

			
			Bind<ILogger> ().To<UnityLogger> ();


			Bind<INetworkTimeoutController<WitchesRequestResponse>> ().ToConstant (_networkController);
			Bind<INetworkMonitor> ().ToConstant (_networkMonitor);


			Bind<ShopController> ().ToSelf ().WithConstructorArgument("networkController", _networkController);
			Bind<IShopController> ().To<ShopController> ();
//			Bind<ShopController> ().ToSelf ().WithConstructorArgument("networkController", (context) => context.Kernel.Get<WitchesNetworkController>());	// TODO: maybe best to push out to ConfigureDependencies
//			Bind<ITransactionProcessor> ().To<UnibillProcessor> ().InSingletonScope();				// TODO: maybe best to push out to ConfigureDependencies	
//			Bind<ITransactionProcessor> ().To<PaymentPassThruProcessor> ();	// TEMPORARY: Add IsProduction to Versioning Class
			Bind<ITransactionProcessor> ().ToConstant (_transactionProcessor);


			Bind<IFactory<string,SceneHeader>>().ToConstant(_sceneHeaderFactory);
			Bind<ISceneHeaderFactory> ().ToConstant (_sceneHeaderFactory as ISceneHeaderFactory);
            Bind<IParser<ExpressionState>>().To<ExpressionParser>();
            Bind<IParser<string>>().To<VariableTextParser>();
            Bind<IMissionRequirementParser>().To<MissionRequirementParser>();
			Bind<MailboxScreenController>().ToSelf().WithConstructorArgument("repo",_controllerRepo);

			Bind<IBuildNumberService> ().ToConstant(_buildNumberService);


			Bind<MainLoadingScreenControllerFactory> ().ToSelf ().WithConstructorArgument ("restorePlayerController", default(RestorePlayerController));	// should not have restore controller


			Bind<IRecipeFactoryNew>().To<RecipeFactoryNew>();
			Bind<ISpellbookFactoryNew>().To<SpellbookFactoryNew>();	
			Bind<IFactory<string,IRecipe>> ().To<DefaultRecipeFactory> ().WithConstructorArgument ("recipeConfig", _masterConfig.Recipes_Configuration); //.WithConstructorArgument("potionsConfig", _masterConfig.Potions_Configuration).WithConstructorArgument("ingredientCategories", _masterConfig.Categories);
			Bind<IFactory<string,ISpellbook>> ().To<DefaultSpellbookFactory> ().WithConstructorArgument("bookConfig", _masterConfig.Books_Configuration);


			Bind<INoStaminaController> ().To<NoStaminaController> ();
            Bind<ShopDialogueController>().ToSelf().InSingletonScope().WithConstructorArgument("shopItemsConfig", _masterConfig.Shop_Items);
            Bind<IShopDialogueController>().ToMethod((c) => c.Kernel.Get<ShopDialogueController>());
		}
	}
}

