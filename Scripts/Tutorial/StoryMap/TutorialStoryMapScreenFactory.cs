using System;

namespace Voltage.Witches.Tutorial
{
    using Voltage.Witches.Controllers;
    using Voltage.Witches.Screens;
    using Voltage.Witches.Configuration;
    using Voltage.Story.Configurations;
    using Voltage.Story.StoryDivisions;
    using Voltage.Witches.Models;

	using Voltage.Witches.Shop;
	using Voltage.Witches.Tutorial.uGUI;
	using Voltage.Witches.User;
	using Voltage.Witches.Story;
	using Voltage.Witches.Bundles;

    public class TutorialStoryMapScreenFactory
    {
        private readonly ScreenNavigationManager _navManager;
        private readonly IScreenFactory _screenFactory;
        private readonly ISceneViewModelFactory _sceneViewModelFactory;
        private readonly IControllerRepo _repo;
        private readonly MasterConfiguration _masterConfig;
        private readonly MasterStoryData _masterStory;
        private readonly ISceneHeaderFactory _sceneHeaderFactory;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;
        private readonly FavorabilityMilestoneController _favorabilityController;

		IShopDialogueController _shopDialogueController;
		INoStaminaController _noStaminaController;
		IStoryLoaderFactory _storyLoaderFactory;

        public TutorialStoryMapScreenFactory(ScreenNavigationManager navManager, IScreenFactory screenFactory, ISceneViewModelFactory sceneViewModelFactory,
		                                     IShopDialogueController shopDialogueController, INoStaminaController noStaminaController, IStoryLoaderFactory storyLoaderFactory,
            								 IControllerRepo repo, MasterConfiguration masterConfig, MasterStoryData masterStory, ISceneHeaderFactory sceneHeaderFactory,
											 IAvatarThumbResourceManager thumbResourceManager, FavorabilityMilestoneController favorabilityController)
        {
            _navManager = navManager;
            _screenFactory = screenFactory;
            _sceneViewModelFactory = sceneViewModelFactory;
            _repo = repo;
            _masterConfig = masterConfig;
            _masterStory = masterStory;
            _sceneHeaderFactory = sceneHeaderFactory;

			_shopDialogueController = shopDialogueController;
			_noStaminaController = noStaminaController;
			_storyLoaderFactory = storyLoaderFactory;
			_thumbResourceManager = thumbResourceManager;
            _favorabilityController = favorabilityController;
        }

		public TutorialStoryMapScreenController Create(Player player)
        {
			IStoryLoaderFacade facade = _storyLoaderFactory.CreateFacade(player);

			return new TutorialStoryMapScreenController (_navManager, _screenFactory, _shopDialogueController, _noStaminaController, 
			                                             _repo, facade, _sceneHeaderFactory, _sceneViewModelFactory, player, 
                                                         _masterConfig, _masterStory, _thumbResourceManager, _favorabilityController);
        }
    }
}

