
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Dependencies
{
	using Ninject;
	using Ninject.Modules;

	using Voltage.Common.Logging;

	using Voltage.Witches.Models;
	using Voltage.Story.StoryDivisions;

	using Voltage.Story.Mapper;
	using Voltage.Witches.Story.Variables;
	using Voltage.Story.General;
	using Voltage.Story.Expressions;
	

	using Voltage.Story.User;

	using Voltage.Story.Text;

	using Voltage.Witches.DI;
	using Voltage.Story.Configurations;


    public class WitchesPlayerDependencies : NinjectModule
    {
		private readonly ILogger _logger;

		private readonly Player _player;
		private readonly MasterStoryData _storyData;



		public WitchesPlayerDependencies (Player player, MasterStoryData storyData) //: this (player, null, new List<NPCModel>()) {}
		{
			_logger = AmbientLogger.Current;
			_player = player;
			_storyData = storyData;

		}



		public override void Load ()
		{

			Bind<ILogger>().ToConstant(_logger);

			Bind<MasterStoryData>().ToConstant(_storyData);

			Bind<Player>().ToConstant(_player).InSingletonScope();
			Bind<IPlayer>().ToMethod(context => context.Kernel.Get<Player>());	// Bind<IPlayer>().To<Player>();


			Bind<List<NPCModel>>().ToConstant(_storyData.NPCs).InSingletonScope();			// TODO: remove of NPCModel (not worth the class)
			Bind<IEnumerable<NPCModel>>().ToMethod((context) => context.Kernel.Get<List<NPCModel>>());


		
			Bind<WitchesVariableMapper>().ToSelf().InSingletonScope().WithConstructorArgument("npcs", Kernel.Get<IEnumerable<NPCModel>>());		// FIXME: strange need to pass in argument, IEnumerable<NPCModel> not bound correctly?
			Bind<IMapping<string>>().ToMethod(context => context.Kernel.Get<WitchesVariableMapper>());	// Bind<IMapping<string>>().To<WitchesVariableMapper>();
						


			Bind<ExpressionParser>().ToSelf();
			Bind<IParser<ExpressionState>>().To<ExpressionParser>();

			Bind<ExpressionFactory>().ToSelf();
			Bind<IExpressionFactory>().To<ExpressionFactory>();

			Bind<VariableTextParser>().ToSelf();
			Bind<IParser<string>>().To<VariableTextParser>();

		}

    }
    
}




