
using System;
using System.Collections.Generic;
using Ninject.Modules;

using Voltage.Witches.Configuration.JSON;
using Voltage.Witches.Configuration;
using Voltage.Witches.Models;
using Voltage.Story.StoryDivisions;
using Voltage.Story.General;
using Voltage.Story.Expressions;
using Voltage.Story.Mapper;
using Voltage.Story.Variables;
using Voltage.Witches.Models.MissionRequirements;
using Voltage.Witches.User;
using UnityEngine;
using Voltage.Witches.Notifications;
using Voltage.Witches.Models.Avatar;
using Voltage.Witches.Exceptions;

namespace Voltage.Witches.DI
{
	using Ninject;

	using Voltage.Common.Net;
	using Voltage.Common.Logging;

	using Voltage.Witches.Screens;
	using Voltage.Witches.Net;

	using iGUI;
    using Voltage.Witches.Services;
    using Voltage.Witches.Story;

	using Voltage.Story.StoryPlayer;
    using Voltage.Witches.Scheduling;

	using Voltage.Witches.Converters;
	using Voltage.Story.Configurations;

	using Voltage.Witches.Story.StoryDivisions.Factory;
	using Voltage.Witches.Story.StoryDivisions.Parsers;

	using Voltage.Common.IAP;
	using Voltage.Witches.IAP;

	using Voltage.Witches.Resetter;

	using Voltage.Witches.Login;

	using Voltage.Story.General;
	using Voltage.Common.Android.ExpansionFile;

    using Voltage.Witches.Init;

    using Voltage.Witches.Bundles;

	using Voltage.Witches.Crypto;
	using System.Collections.ObjectModel;



	public class ConfigurationDependencies : NinjectModule
	{

		private iGUIContainer _contentPane;
		private iGUIContainer _dialoguePane;
        private iGUIContainer _overlayPane;


		// TODO: break out BASE_URL and USE_LOCAL_PLAYER to a local configuration
#if UNITY_EDITOR
//        private const string BASE_URL = "http://127.0.0.1:8000";
        private const string BASE_URL = "http://curses.en.my-romance.com";
#else
        private const string BASE_URL = "http://curses.en.my-romance.com";
#endif



		private const bool USE_LOCAL_PLAYER_IN_DEBUG = false;

		private bool IsLocalPlayer	// maybe DON'T want to compile local player into build
		{
#if UNITY_EDITOR
			get { return USE_LOCAL_PLAYER_IN_DEBUG; }
#else
			get { return false; }
#endif
		}



#if DEBUG     
        private const bool DATA_ENCRYPTED = false;
#else
        private const bool DATA_ENCRYPTED = true;
#endif

		public const string GOOGLEPLAY_PUBLICKEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlA1mnAkRlKAOjhe238V0GD8buk1u2eTB1Ew2qAn/WI57oyFjw5cV6Qh5Wh8PH38/wV62NFfVp071NR8Wm2yhPwXzse3DLUdqPW4FKxNhRzck6q12MnSmUergqh2qflB/hkMOLj8mn5nix0xYcbyq0/PYjDVNccPZnLEaJpmYdcKWO5V7cNH6wB+bzkFKbAFqEmZtNAEkSYaW7ZNoJzhKQZjfB1xdgKN7x2DDgvcL6p12fdbx9M2pEvzyJZAqJ7tMWlgeptGqIO11MQUDx+wou1OVTczyscSxMqMjqG/HGT7gQtREsmJM3w0DrQeD8IuUS6eofuwExaUes+bM9+OazQIDAQAB";

		private const float PAUSE_TIMEOUT_DURATION_IN_MIN = 10.0f;


        private string _jsonPlayerDataPath;		// NOTE: unfortunately can't be const
        private string _encryptedPlayerDataPath;
        public const string FILE_NAME = "voltage_kisses_and_curses_player";
        public const string FILE_TYPE = "witches";


		private  NetworkControllerProvider _networkControllerProvider;		// retaining reference to a single instance, for use with other kernels

		// this needs to persist across kernels as some game dependencies are bound to it during initialization (e.g., AndroidBackButtonHandler)
		// may need to reset the path (pop all init controllers) before entering home screen
		private ScreenNavigationManagerProvider _navManagerProvider;						


        public ConfigurationDependencies (iGUIContainer contentPane, iGUIContainer dialoguePane, iGUIContainer overlayPane)
		{
			if(contentPane == null || dialoguePane == null)
			{
				throw new ArgumentNullException("ConfigurationDependencies::Ctor >>> " );
			}

			_contentPane = contentPane;
			_dialoguePane = dialoguePane;
            _overlayPane = overlayPane;

            _jsonPlayerDataPath = string.Format("{0}/{1}.json", Application.persistentDataPath, FILE_NAME);		// /Users/michael.chang/Library/Application Support/Voltage Entertainment/Curses Dev/voltage_kisses_and_curses_player.json
            _encryptedPlayerDataPath = string.Format("{0}/{1}.{2}", Application.persistentDataPath, FILE_NAME, FILE_TYPE );

			_networkControllerProvider = new NetworkControllerProvider(BASE_URL);
			_navManagerProvider = new ScreenNavigationManagerProvider ();
		}




		public override void Load()
		{
            #if DEBUG
            bool useDebugMode = true;
            #else
            bool useDebugMode = false;
            #endif


			////////////////////////
			///  Serialization	 ///
			////////////////////////

			Bind<CryptoKeyStore> ().ToSelf().InSingletonScope ();
			Bind<ICryptoKeyStore>().ToMethod(context => context.Kernel.Get<CryptoKeyStore>()).InSingletonScope();	// ensuring that only one CryptoKeyStore is used
            Bind<ICryptoService>().To<RijndaelCryptoService>().InSingletonScope();

            const int DATA_KEY = 10;    // for support of hashed fields...if this key changes any hashed fields will break!
            Bind<JSONHashedPlayerDataSerializer>().ToSelf().WithConstructorArgument("key", DATA_KEY).WithConstructorArgument("applyHash", false);
            Bind<PlayerWriter>().ToSelf()
                .WithConstructorArgument("serializer", context => context.Kernel.Get<JSONHashedPlayerDataSerializer>()) // still leveraging JSONHashedPlayerDataSerializer to handle hashed fields
                .WithConstructorArgument("path", _jsonPlayerDataPath);

            Bind<IPlayerDataSerializer>().To<JSONPlayerDataSerializer>();
            Bind<CryptoPlayerWriter>().ToSelf().WithConstructorArgument("path", _encryptedPlayerDataPath);

            Bind<IPlayerWriter>().To<WitchesPlayerWriter>()
                .WithConstructorArgument("encryptedPath", _encryptedPlayerDataPath)
                .WithConstructorArgument("jsonPath", _jsonPlayerDataPath)
                .WithConstructorArgument("encrypt", DATA_ENCRYPTED);


			////////////////////////
			///  	Providers	 ///
			////////////////////////

            Bind<UGUIScreenFactory>().ToProvider(new SceneObjectProvider<UGUIScreenFactory>("ScreenRoot", true));
			Bind<ITaskScheduler> ().ToProvider (new MonoBehaviourProvider<UnityTaskScheduler> ("Scheduler")).InSingletonScope();
            Bind<UnityLifeCycleManager>().ToProvider(new MonoBehaviourProvider<UnityLifeCycleManager>("LifeCycleManager")).InSingletonScope();
            Bind<IAssetBundleManager>().ToProvider(new MonoBehaviourProvider<AssetBundleManager>("AssetBundleManager", true)).InSingletonScope();
            Bind<IAvatarResourceManager>().To<AvatarResourceManager>();
            Bind<NotificationManager>().ToProvider(new SceneObjectProvider<NotificationManager>("NotificationManager"));
            Bind<AudioController>().ToProvider(new SceneObjectProvider<AudioController>("SoundManager")).InSingletonScope();

			Bind<IAvatarThumbResourceManager>().To<AvatarThumbResourceManager>().InSingletonScope();


            Bind<IScreenFactory>().To<ScreenFactory>()
                .WithConstructorArgument("contentRoot", _contentPane)
                .WithConstructorArgument("dialogRoot", _dialoguePane)
                .WithConstructorArgument("overlayRoot", _overlayPane);

			Bind<ScreenNavigationManager>().ToProvider(_navManagerProvider);

			Bind<ScreenFactoryData> ().ToSelf ();
			
			Bind<ILogger> ().To<UnityLogger> ();

            Bind<GameErrorHandler>().ToSelf().WithPropertyValue("UseDebugMode", useDebugMode);

			// Startup Initialization 
#if UNITY_ANDROID
			Bind<WitchesStartupSequence>().ToSelf().WithConstructorArgument("environmentController", context => context.Kernel.Get<AndroidEnvironmentController>());
			Bind<AndroidEnvironmentController> ().ToSelf ();
//			Bind<NetworkLoggingController> ().ToSelf ();
			Bind<OBBFetcherFactory> ().ToSelf ().WithConstructorArgument ("publicKey", GOOGLEPLAY_PUBLICKEY); //.WithConstructorArgument("networkController", context => context.Kernel.Get<NetworkLoggingController>());
			Bind<IFactory<string,IExpansionFileFetcher>> ().ToMethod (context => context.Kernel.Get<OBBFetcherFactory> ()); 
#else
			Bind<WitchesStartupSequence>().ToSelf();
			Bind<EnvironmentController> ().ToSelf ();
#endif

			Bind<IEnvironmentParser> ().To<EnvironmentParser> ();
            Bind<EnvironmentDataFetcher>().ToSelf();
			Bind<UnityFileSystemService> ().ToSelf ();
			Bind<IBuildNumberService> ().To<BuildNumberService> ().WithConstructorArgument("filesystem", context => context.Kernel.Get<UnityFileSystemService>());
			Bind<StartupDisplayController> ().ToSelf ();
			Bind<IStartupErrorController> ().To<StartupErrorController> ();
			Bind<PlayerDataController> ().ToSelf ().WithConstructorArgument ("path", _jsonPlayerDataPath);
            Bind<NewPlayerFetcher>().ToSelf();
			Bind<IParser<PlayerDataStore>> ().To<ServerPlayerDataParser> ();
			Bind<StartupDataController> ().ToSelf ();
			Bind<ServerDataController> ().ToSelf ();
            Bind<MasterConfigFetcher>().ToSelf();
			Bind<LocalDataController> ().ToSelf();
            Bind<ILocalDataFetcher>().To<LocalDataFetcher>();
			Bind<IParser<MasterStoryData>> ().To<MasterStoryDataParser> ();
			Bind<StartupShopController> ().ToSelf ();
			Bind<IVersionService> ().To<VersionService> ();
			Bind<IIAPAuth> ().To<IAPAuth> ().WithConstructorArgument ("key", GOOGLEPLAY_PUBLICKEY);
            Bind<TransactionProcessorFactory>().ToSelf();
			Bind<IPlayerPreferences> ().To<PlayerPreferences> ();


			
			Bind<RestorePlayerController> ().ToSelf ();

			Bind<StartupSceneSetupController> ().ToSelf ().WithConstructorArgument ("timeoutDurationInMin", PAUSE_TIMEOUT_DURATION_IN_MIN);;
			Bind<IResetter> ().To<WitchesGameResetter> ().WithConstructorArgument("kernel", context => context.Kernel);

#if UNITY_EDITOR
			Bind<IMoviePlayer> ().To<EditorIntroMoviePlayer> ();
#else
			Bind<IMoviePlayer> ().To<MobileIntroMoviePlayer> ();
#endif

			Bind<IPlayerFactory> ().To<WitchesPlayerFactory> ().WithConstructorArgument("localPlayer", IsLocalPlayer);

			Bind<DictionaryToJsonConverter<string,int>> ().ToSelf ();
			Bind<DictionaryToJsonConverter<string,string>> ().ToSelf ();


			Bind<ILoginController> ().To<WitchesLoginController> ();
            Bind<BonusManager>().ToSelf().InSingletonScope();


			// Networking
			Bind<WitchesBaseNetworkController>().ToProvider(_networkControllerProvider); //.InSingletonScope();
			Bind<INetworkTimeoutController<WitchesRequestResponse>>().ToMethod(c => c.Kernel.Get<WitchesBaseNetworkController>());
			Bind<IBaseUrl>().ToMethod(c => c.Kernel.Get<WitchesBaseNetworkController>());

			Bind<INetworkTimeoutController<WitchesRequestResponse>>().ToMethod(c => c.Kernel.Get<WitchesBaseNetworkController>()).WhenInjectedInto<MonitoringNetworkController>();	// being explicit here, but not necessary...or could add WithConstructorArgument to MonitoringNetworkController bind
			Bind<MonitoringNetworkController>().ToSelf().InSingletonScope();
			Bind<INetworkMonitor>().ToMethod(c => c.Kernel.Get<MonitoringNetworkController>());

			Bind<NetworkConnectivityMonitorFactory> ().ToSelf ();





			Bind<IIngredientConfigParser>().To<IngredientConfigParser>();
			Bind<IIngredientCategoryParser>().To<IngredientCategoryParser>();
			Bind<IItemParser>().To<ItemParser>();
			Bind<IBookParser>().To<SpellbookRefParser>();
			Bind<IRecipeRefParser>().To<RecipeRefParser>();

			Bind<IPlayerSpellbookConfigParser>().To<PlayerSpellbookConfigParser>();
			Bind<IPlayerRecipeConfigParser>().To<PlayerRecipeConfigParser>();

			Bind<ISpellbookFactory>().To<SpellbookFactory>();
			Bind<IRecipeFactory>().To<RecipeFactory>();


           



			Bind<IPlayerStateParser>().To<PlayerStateParser>();
			Bind<IGameConfigParser>().To<GameConfigParser>();
			Bind<IItemRawParser>().To<ItemRawParser>();

			Bind<IMasterDataParser>().To<MasterDataConfigParser>();
			Bind<IGamePropertiesConfigParser>().To<GamePropertiesConfigParser>();
			Bind<IAffinityConfigParser>().To<AffinityConfigParser>();
			Bind<IIngredientsMasterConfigParser>().To<IngredientsMasterConfigParser>();
			Bind<IIngredientConfigurationParser>().To<IngredientConfigurationParser>();

			Bind<IAvatarItemsConfigParser>().To<AvatarItemsConfigParser>();
			Bind<IRecipesConfigParser>().To<RecipesConfigParser>();
			Bind<IPotionsConfigParser>().To<PotionsConfigParser>();

			Bind<IPlayerStateParserNew>().To<PlayerStateParserNew>();
			Bind<IPlayerSpellbookConfigurationParser>().To<PlayerSpellbookConfigurationParser>();
			Bind<IBooksConfigParser>().To<BooksConfigParser>();
			Bind<ISpellbookRefConfigParser>().To<SpellbookRefConfigParser>();

//          Bind<ISceneHeaderFactory>().To<SceneHeaderFactory>();
//			Bind<IParser<SceneHeader>> ().To<SceneHeaderParser> ();

            Bind<IMissionRequirementParser>().To<MissionRequirementParser>();
			Bind<IParser<ExpressionState>>().To<ExpressionParser>();
			Bind<IExpressionFactory>().To<ExpressionFactory>();
			// enforce singleton so that we can modify the variable parser after player initialization
			Bind<ExpressionFactory>().ToSelf().InSingletonScope();
			Bind<IMapping<string>>().To<NullMapper>();

			Bind<IRecipeFactoryNew>().To<RecipeFactoryNew>();
			Bind<ISpellbookFactoryNew>().To<SpellbookFactoryNew>();

#if UNITY_ANDROID && !UNITY_EDITOR
            Bind<IFilesystemService>().To<AndroidFilesystemService>();
#else
            Bind<IFilesystemService>().To<FilesystemService>();
#endif
            Bind<IScenePathParser>().To<ScenePathParser>();
            Bind<ISceneDiscoveryService>().To<SceneDiscoveryService>().InSingletonScope();
		}
	}

}







//			Bind<INetworkTimeoutController<WWWNetworkPayload>> ().To<WitchesNetworkController> ().When (request => !request.Target.Member.DeclaringType.Name.Contains("WitchesNetworkController"));
//			Bind<INetworkTimeoutController<WWWNetworkPayload>> ().ToMethod ((context) => context.Kernel.Get<WitchesNetworkController>()); // Bind<INetworkTimeoutController<WWWNetworkPayload>> ().To<WitchesNetworkController> ();







