using System;

namespace Voltage.Witches.Controllers.Factories
{
    using Voltage.Witches.Models;
    using Voltage.Story.StoryPlayer;
    using Voltage.Story.StoryDivisions;
    using Voltage.Story.Effects;
	using Voltage.Story.Reset;
//	using Voltage.Witches.Screens;
	using Voltage.Story.Configurations;
    using Voltage.Witches.Story;
	using Voltage.Witches.Screens;

	using Voltage.Common.Net;
	using Voltage.Witches.Net;

    public interface IWitchesStoryPlayerScreenControllerFactory
    {
		WitchesStoryPlayerScreenController Create(StoryPlayerSettings settings, bool showInterface, Action onFailure, string dialogType);
    }

    public class WitchesStoryPlayerScreenControllerFactory : IWitchesStoryPlayerScreenControllerFactory
    {

        private readonly Player _player;
        private readonly string _resourceConfig;
		private readonly MasterStoryData _storyData;
		private readonly IScreenFactory _screenFactory;
//		private readonly ScreenNavigationManager _screenNavManager;
		private readonly IControllerRepo _controllerRepo;
		private readonly IEffectResolver _effectResolver;
		private readonly IStoryResetter _storyResetter;
        private readonly StoryParser _storyDependencyParser;
        private readonly IStoryPlayerDialogController _dialogController;

		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;
        private readonly StoryMusicPlayer _musicPlayer;

		public WitchesStoryPlayerScreenControllerFactory (Player player, MasterStoryData storyData, string resourceConfig, IScreenFactory screenFactory,
            IEffectResolver effectResolver, IStoryResetter storyResetter, StoryParser storyDependencyParser, IControllerRepo repo, IStoryPlayerDialogController dialogController,
            INetworkTimeoutController<WitchesRequestResponse> networkController, StoryMusicPlayer musicPlayer)
		{
			_player = player;
			_storyData = storyData;
			_resourceConfig = resourceConfig;

			_screenFactory = screenFactory;

			_controllerRepo = repo;
			_effectResolver = effectResolver;
			_storyResetter = storyResetter;
            _storyDependencyParser = storyDependencyParser;
            _dialogController = dialogController;

			_networkController = networkController;
            _musicPlayer = musicPlayer;
		}

		public WitchesStoryPlayerScreenController Create(StoryPlayerSettings settings, bool showInterface, Action onFailure, string dialogType)
		{
			StoryPlayerScreenControllerData data = new StoryPlayerScreenControllerData
			{
				Player = _player,
				Settings = settings,
				StoryData = _storyData,
				GameResourceConfiguration = _resourceConfig,
				OnFailure = onFailure,
				ControllerRepo = _controllerRepo,
				EffectResolver = _effectResolver,
				StoryResetter = _storyResetter,
				StoryParser = _storyDependencyParser,
				ScreenFactory = _screenFactory,
				NetworkController = _networkController,
			};

			return new WitchesStoryPlayerScreenController (data, _dialogController, _musicPlayer, showInterface);
		}

    }
}
