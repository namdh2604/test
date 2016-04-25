using System;
using Voltage.Witches.Screens;
using Voltage.Witches.Models;
using Voltage.Story.StoryDivisions;
using Voltage.Witches.UI;
using Voltage.Witches.Shop;

namespace Voltage.Witches.Controllers.Factories
{
	using Voltage.Witches.Configuration;
	using Voltage.Story.StoryDivisions;
	using Voltage.Story.Configurations;
    using Voltage.Witches.Story;
    using Voltage.Witches.User;
	using Voltage.Witches.Bundles;

    public interface IStoryMapScreenControllerFactory
    {
        //StoryMapScreenController Create(Player player, Action<int> storyMapLoaded);
        StoryMapUGUIScreenController CreateUGUI(Player player, Action<int> storyMapLoaded);
    }

    public class StoryMapScreenControllerFactory : IStoryMapScreenControllerFactory
    {
        private ScreenNavigationManager _screenManager;
        private IScreenFactory _screenFactory;
        private ISceneViewModelFactory _sceneFactory;
        private ISceneHeaderFactory _sceneHeaderFactory;
        private IControllerRepo _screenRepo;

		private MasterConfiguration _masterConfig;
		private MasterStoryData _masterStory;

		private readonly IShopDialogueController _shopDialogController;
        private readonly INoStaminaController _noStaminaController;
        private readonly IStoryLoaderFactory _storyFactory;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;
        private readonly FavorabilityMilestoneController _favorabilityController;


		public StoryMapScreenControllerFactory(ScreenNavigationManager screenManager, IScreenFactory screenFactory, ISceneViewModelFactory sceneFactory, IControllerRepo screenRepo, 
											   MasterConfiguration masterConfig, MasterStoryData masterStory, ISceneHeaderFactory headerFactory, IStoryLoaderFactory storyFactory, 
            								   IShopDialogueController shopDialogController, INoStaminaController noStaminaController,
											   IAvatarThumbResourceManager thumbResourceManager, FavorabilityMilestoneController favorabilityController)
        {
            _screenManager = screenManager;
            _screenFactory = screenFactory;
            _sceneFactory = sceneFactory;
            _sceneHeaderFactory = headerFactory;
            _screenRepo = screenRepo;

			_masterConfig = masterConfig;
			_masterStory = masterStory;

            _storyFactory = storyFactory;
			_shopDialogController = shopDialogController;
            _noStaminaController = noStaminaController;
			_thumbResourceManager = thumbResourceManager;
            _favorabilityController = favorabilityController;
        }


        public StoryMapUGUIScreenController CreateUGUI(Player player, Action<int> storyMapLoaded)
        {
            IStoryLoaderFacade storyLoader = _storyFactory.CreateFacade(player);
            return new StoryMapUGUIScreenController(_screenManager, _screenFactory, _shopDialogController, _noStaminaController, _screenRepo,
                storyLoader, _sceneHeaderFactory, _sceneFactory, player, _masterConfig, _masterStory, storyMapLoaded, _thumbResourceManager, _favorabilityController);
        }
    }
}

