
namespace Voltage.Witches.Controllers.Factories
{
    using Voltage.Witches.Screens;
    using Voltage.Witches.Models;
    using Voltage.Witches.Configuration;
	using Voltage.Witches.Shop;

    public interface IInventoryScreenControllerFactory
    {
        InventoryScreenController Create(int activeView);
    }

    public class InventoryScreenControllerFactory : IInventoryScreenControllerFactory
    {
        private readonly ScreenNavigationManager _navManager;
        private readonly IScreenFactory _factory;
        private readonly Player _player;
        private readonly Inventory _inventory;
        private readonly IControllerRepo _repo;
        private readonly MasterConfiguration _config;
		private readonly ShopDialogueController _shopDialogController;

        public InventoryScreenControllerFactory(ScreenNavigationManager navManager, IScreenFactory factory, Player player, Inventory inventory, 
			IControllerRepo repo, MasterConfiguration config, ShopDialogueController shopDialogController)
        {
            _navManager = navManager;
            _factory = factory;
            _player = player;
            _inventory = inventory;
            _repo = repo;
            _config = config;
			_shopDialogController = shopDialogController;
        }

        public InventoryScreenController Create(int activeView)
        {
			return new InventoryScreenController(_navManager, _factory, _player, _inventory, activeView, _repo, _config, _shopDialogController);
        }
    }
}

