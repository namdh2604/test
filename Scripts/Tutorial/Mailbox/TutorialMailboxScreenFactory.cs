using Voltage.Witches.Shop;

namespace Voltage.Witches.Tutorial
{
    using Voltage.Witches.Controllers;
    using Voltage.Witches.Configuration;
    using Voltage.Witches.Models;
    using Voltage.Witches.Screens;

    public class TutorialMailboxScreenFactory
    {
        private readonly ScreenNavigationManager _navManager;
        private readonly IScreenFactory _screenFactory;
        private readonly IControllerRepo _repo;
        private readonly MasterConfiguration _masterConfig;
		private readonly ShopDialogueController _shopDialogController;

        public TutorialMailboxScreenFactory(ScreenNavigationManager navManager, IScreenFactory screenFactory,
			IControllerRepo repo, MasterConfiguration masterConfig, ShopDialogueController shopDialogController )
        {
            _navManager = navManager;
            _screenFactory = screenFactory;
            _repo = repo;
            _masterConfig = masterConfig;
			_shopDialogController = shopDialogController;
        }

        public TutorialMailboxController Create(Player player)
        {
            Inventory inventory = _repo.Get<Inventory>();

			return new TutorialMailboxController(_navManager, _screenFactory, player, inventory, _repo, _masterConfig, _shopDialogController);
        }
    }
}

