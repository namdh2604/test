
using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Modules;

namespace Voltage.Witches.DI
{
	using Voltage.Witches.Controllers;

	using Voltage.Story.User;
	using Voltage.Witches.Models;
	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Variables;
	using Voltage.Witches.Story.Variables;
	using Voltage.Witches.Configuration;
    using Voltage.Story.Effects;
    using Voltage.Witches.Story.Effects;

	using Voltage.Story.Configurations;
	using Voltage.Story.General;
	using Voltage.Witches.Story.StoryDivisions.Factory;
	using Voltage.Witches.Story.StoryDivisions.Parsers;
	using Voltage.Witches.Story;

	using Scene = Voltage.Story.StoryDivisions.Scene;

	using Voltage.Story.Expressions;
	using Voltage.Witches.Models.MissionRequirements;

	using Voltage.Common.IAP;

	using Voltage.Common.Net;
	using Voltage.Witches.Net;

	using Voltage.Witches.Services;

	using Voltage.Witches.Bundles;



	public class WitchesGameDependencies : NinjectModule
	{
		private IWitchesData _data;

        // FIXME: need to finalize where this dependency is injected
        public const double STARTER_PACK_DURATION_IN_DAYS = 3D; //0.001D;  

		public WitchesGameDependencies(IWitchesData data)
		{
            if(data == null)
			{
				throw new ArgumentNullException("WitchesGameDependencies::Ctor >>> " );
			}

			_data = data;

			if(UnityEngine.Debug.isDebugBuild)
			{
				Voltage.Witches.DebugTool.DebugAccessor.Instance.Player = data.Player;	// FIXME: until DebugPlayer decorator is added
				Voltage.Witches.DebugTool.DebugAccessor.Instance.StoryData = data.MasterStoryData;
			}

		}



		public override void Load()
		{
			// being explicit here
			Bind<ControllerRepo>().ToSelf().InSingletonScope()
				.WithConstructorArgument("networkController", c => c.Kernel.Get<INetworkTimeoutController<WitchesRequestResponse>>())
				.WithConstructorArgument("networkMonitor", c => c.Kernel.Get<MonitoringNetworkController>())
				.WithConstructorArgument("inventory", c => c.Kernel.Get<Inventory>())
				.WithConstructorArgument("bundleManager", c => c.Kernel.Get<IAssetBundleManager>());
			Bind<IControllerRepo>().ToMethod(c => c.Kernel.Get<ControllerRepo>());

			Bind<Inventory>().ToSelf().InSingletonScope();

			Bind<GameResumer> ().ToSelf ();

			Bind<Player>().ToConstant(_data.Player);
			Bind<IPlayer> ().ToMethod (c => c.Kernel.Get<Player> ());	// .To<Player>()

			Bind<MasterConfiguration>().ToConstant(_data.MasterConfig);
			Bind<MasterStoryData>().ToConstant (_data.MasterStoryData);

			Bind<VariableMapper>().To<WitchesVariableMapper>().WithConstructorArgument("npcs", _data.MasterStoryData.NPCs);
			Bind<IEffectResolver>().To<WitchesEffectResolver>().WithConstructorArgument("npcs", _data.MasterStoryData.NPCs);

			Bind<ITransactionProcessor>().ToConstant(_data.TransactionProcessor);

			Bind<IFactory<string,Scene>>().To<SceneFactory>(); //.InSingletonScope();
			Bind<SceneHeaderFileSystemAdapter> ().ToSelf ();
			Bind<IFactory<string,SceneHeader>> ().To<SceneHeaderFactory> ().WithConstructorArgument("filesystemService", context => context.Kernel.Get<SceneHeaderFileSystemAdapter>()); //.InSingletonScope();
			Bind<IParser<SceneHeader>>().To<SceneHeaderParser>();
//			Bind<IParser<ExpressionState>>().To<ExpressionParser>();
//			Bind<IMissionRequirementParser>().To<MissionRequirementParser>();

            Bind<IItemFactory>().To<ItemFactory>();



		}
	}
    
}


