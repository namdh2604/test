
namespace Voltage.Witches.Tutorial
{
    using Voltage.Witches.Shop;
    using Voltage.Witches.Screens;
    using Voltage.Witches.Configuration;
    using Voltage.Witches.Controllers;
    using Voltage.Witches.Models;
    using Voltage.Witches.Login;

    public class TutorialHomeScreenFactory
    {
        private readonly ScreenNavigationManager _navManager;
        private readonly IScreenFactory _screenFactory;
        private readonly IControllerRepo _repo;
        private readonly MasterConfiguration _masterConfig;
        private readonly ShopDialogueController _shopDialogController;
		private readonly HomeScreenFeatureLockHandler _lockHandler;
        private readonly BonusManager _bonusManager;

        public TutorialHomeScreenFactory(ScreenNavigationManager navManager, IScreenFactory screenFactory, 
            IControllerRepo repo, MasterConfiguration masterConfig, ShopDialogueController shopDialogController,
            HomeScreenFeatureLockHandler lockHandler, BonusManager bonusManager)
        {
            _navManager = navManager;
            _screenFactory = screenFactory;
            _repo = repo;
            _masterConfig = masterConfig;
            _shopDialogController = shopDialogController;
			_lockHandler = lockHandler;
            _bonusManager = bonusManager;
        }

        public TutorialHomeScreenController Create(Player player)
        {
            return new TutorialHomeScreenController(_navManager, _screenFactory, player,
                _repo, _masterConfig, _shopDialogController, _lockHandler, _bonusManager);
        }

    }
}

