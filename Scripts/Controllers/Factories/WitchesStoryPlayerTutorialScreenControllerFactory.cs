using System;

namespace Voltage.Witches.Controllers.Factories
{
    using Voltage.Witches.Models;
    using Voltage.Story.StoryPlayer;
    using Voltage.Story.StoryDivisions;
    using Voltage.Story.Effects;
    using Voltage.Story.Reset;
    using Voltage.Story.Configurations;
    using Voltage.Witches.Story;
	using Voltage.Witches.Screens;

	using Voltage.Common.Net;
	using Voltage.Witches.Net;

    public class WitchesStoryPlayerTutorialScreenControllerFactory : IWitchesStoryPlayerScreenControllerFactory
    {
        private readonly Player _player;
        private readonly string _resourceConfig;
        private readonly MasterStoryData _storyData;
        private readonly IControllerRepo _controllerRepo;
        private readonly IEffectResolver _effectResolver;
        private readonly IStoryResetter _storyResetter;
        private readonly StoryParser _storyDependencyParser;
		private readonly IScreenFactory _screenFactory;
        private readonly NormalStoryPlayerDialogController _normalDialogController;
		private readonly TutorialDialogController _tutorialDialogController;
        private readonly StoryMusicPlayer _musicPlayer;

		private readonly INetworkTimeoutController<WitchesRequestResponse> _networkController;

        public WitchesStoryPlayerTutorialScreenControllerFactory(Player player, MasterStoryData storyData, IScreenFactory screenFactory,
            string resourceConfig, IEffectResolver effectResolver, IStoryResetter storyResetter, StoryParser storyDependencyParser, IControllerRepo repo,
			NormalStoryPlayerDialogController normalDialogController, TutorialDialogController tutorialDialogController,
			INetworkTimeoutController<WitchesRequestResponse> networkController, StoryMusicPlayer musicPlayer)
        {
            _player = player;
            _storyData = storyData;
            _resourceConfig = resourceConfig;

            _controllerRepo = repo;
            _effectResolver = effectResolver;
            _storyResetter = storyResetter;
            _storyDependencyParser = storyDependencyParser;

			_screenFactory = screenFactory;
			_normalDialogController = normalDialogController;
			_tutorialDialogController = tutorialDialogController;

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

			IStoryPlayerDialogController dialogController;

			if (dialogType == WitchesStoryPlayerScreenController.TUTORIAL_DIALOG) 
			{
				dialogController = _tutorialDialogController;
			} else
			{
				dialogController = _normalDialogController;
			}


            return new WitchesStoryPlayerTutorialScreenController(data, showInterface, dialogController, _musicPlayer);
        }
    }
}

