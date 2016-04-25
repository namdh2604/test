
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Dependencies
{
	using Ninject;
	using Ninject.Activation;
	using Ninject.Modules;
	
	using Voltage.Common.Logging;
	using Voltage.Story.General;
	
	using Voltage.Story.StoryPlayer;

	using Voltage.Story.Models.Nodes;
	using Voltage.Story.Models.Nodes.Controllers;
	using Voltage.Story.StoryDivisions;
	
	using Voltage.Story.Views;
	
	using Voltage.Witches.Story.StoryPlayer;

	using Voltage.Story.Effects;
	using Voltage.Witches.Story.Effects;

	using Voltage.Witches.Models;
    using Voltage.Witches.Controllers;


	using Voltage.Witches.Story.Models.Nodes.Controllers;
    using Voltage.Story.Text;

	using Voltage.Witches.Net;

	using Voltage.Witches.Screens;
	using Voltage.Witches.Shop;

	using Voltage.Witches.User;
    using Voltage.Witches.Story;


	using StoryPlayerUIScreen = Voltage.Witches.UI.StoryPlayerUIScreen;

	
	public class WitchesStoryPlayerDependencies : NinjectModule
	{
		private readonly ILayoutDisplay _layoutDisplay;
        private readonly StoryMusicPlayer _musicPlayer;
		private readonly StoryPlayerUIScreen _screenController;
		private readonly WitchesStoryPlayerScreenController _witchesStoryPlayerScreenController;		// HACK to get story exit calls from bit node
        private readonly IScreenFactory _screenFactory;

		private readonly StoryPlayerSettings _settings;

		public Action OnFinish { get; set; }
		public Action OnFailure { get; set; }

        private readonly IEffectResolver _effectResolver;
		private readonly INoStaminaController _noStaminaController;
		private readonly IShopDialogueController _shopDialogueController;


		public WitchesStoryPlayerDependencies(ILayoutDisplay layoutDisplay, StoryMusicPlayer musicPlayer, StoryPlayerUIScreen screenController, IEffectResolver effectResolver, Action onSceneFinish, Action onFailure, 
											  INoStaminaController noStaminaController, IShopDialogueController shopDialogueController, IScreenFactory screenFactory, StoryPlayerSettings settings,
		                                      WitchesStoryPlayerScreenController witchesStoryPlayerScreenController 										// HACK to get story exit calls from bit node
		                                      ): base() 
		{
			if(layoutDisplay == null || screenController == null)
			{
				throw new ArgumentNullException();
			}

			_layoutDisplay = layoutDisplay;
            _musicPlayer = musicPlayer;
			_screenController = screenController;
            _screenFactory = screenFactory;

			_settings = settings;

            _effectResolver = effectResolver;
			OnFinish = onSceneFinish;
			OnFailure = onFailure;

			_noStaminaController = noStaminaController;
			_shopDialogueController = shopDialogueController;

			_witchesStoryPlayerScreenController = witchesStoryPlayerScreenController;
		}
		
		public override void Load()
		{
			Bind<ILayoutDisplay>().ToConstant(_layoutDisplay);
            Bind<StoryMusicPlayer>().ToConstant(_musicPlayer);
			Bind<StoryPlayerUIScreen> ().ToConstant (_screenController);
			Bind<WitchesStoryPlayerScreenController> ().ToConstant (_witchesStoryPlayerScreenController);
            Bind<IScreenController>().ToMethod(c => c.Kernel.Get<WitchesStoryPlayerScreenController>());

            Bind<IEffectResolver>().ToConstant(_effectResolver);

            Bind<IScreenFactory>().ToConstant(_screenFactory);

			Bind<StoryPlayerSettings>().ToConstant(_settings);


			Bind<INoStaminaController> ().ToConstant (_noStaminaController);		// To<NoStaminaController> ();
			Bind<IShopDialogueController> ().ToConstant (_shopDialogueController);	// To<ShopDialogueController> ();
			
			BindNodeControllers (OnFailure);
			
			// TODO: Bind Variable/IMapper here too? Presently requires separate player module WitchesPlayerDependencies
			Bind<WitchesStoryPlayer> ().ToSelf().WithConstructorArgument ("onFinish", OnFinish);
			Bind<IStoryPlayer> ().ToMethod (context => context.Kernel.Get<WitchesStoryPlayer> ()); // Bind<IStoryPlayer> ().To<WitchesStoryPlayer> (); 
		}



		public void BindNodeControllers(Action onFailure)
		{
			Bind<SceneNodeController> ().ToSelf ();
			Bind<DialogueNodeController> ().ToSelf ();
			Bind<OptionNodeController> ().ToSelf ();
			Bind<ConditionalNodeController> ().ToSelf ().WithConstructorArgument("onFailure", onFailure);
			Bind<BranchNodeController> ().ToSelf ();
			Bind<WitchesLockNodeController> ().ToSelf ();
			Bind<WitchesUnlockNodeController> ().ToSelf ();
            Bind<WitchesBitNodeController> ().ToSelf ().WithConstructorArgument("onFailure", new Action(_witchesStoryPlayerScreenController.GoToStoryMap));	// FIXME: double check what existing 'onFailure' does
			Bind<WitchesSelectionNodeController> ().ToSelf ();
			
			Bind<IDictionary<Type,INodeController>>().ToProvider (new NodeControllerProvider());
		}

		protected class NodeControllerProvider : IProvider<IDictionary<Type, INodeController>>
		{
			public object Create(IContext context)
			{
				return new Dictionary<Type,INodeController>
				{
					{typeof(Voltage.Story.StoryDivisions.Scene), context.Kernel.Get<SceneNodeController>()},
					{typeof(DialogueNode), context.Kernel.Get<DialogueNodeController>()},
                    {typeof(EstablishingNode), context.Kernel.Get<EstablishingNodeController>()},
					{typeof(SelectionNode), context.Kernel.Get<WitchesSelectionNodeController>()},
                    {typeof(EINode), context.Kernel.Get<EventIllustrationNodeController>()},
					{typeof(OptionNode), context.Kernel.Get<OptionNodeController>()},
					{typeof(ConditionalNode), context.Kernel.Get<ConditionalNodeController>()},
					{typeof(BranchNode), context.Kernel.Get<BranchNodeController>()},
					{typeof(LockNode), context.Kernel.Get<WitchesLockNodeController>()},
					{typeof(UnlockNode), context.Kernel.Get<WitchesUnlockNodeController>()},
					{typeof(BitNode), context.Kernel.Get<WitchesBitNodeController>()},
				};
			}
			
			public Type Type
			{
				get { return typeof(IDictionary<Type, INodeController>); }
			}
		}
		
		
		
	}
	
}









