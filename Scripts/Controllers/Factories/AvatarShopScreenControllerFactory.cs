
using Voltage.Witches.Models;
using Voltage.Witches.Screens;
using Voltage.Witches.Shop;
using Voltage.Witches.Configuration;
using Voltage.Witches.Screens.Closet;
using Voltage.Witches.Bundles;

namespace Voltage.Witches.Controllers
{
    public class AvatarShopScreenControllerFactory
    {
        private readonly ScreenNavigationManager _navManager;
        private readonly IScreenFactory _screenFactory;
        private readonly Player _player;
        private readonly Inventory _inventory;
        private readonly IControllerRepo _repo;
        private readonly ShopController _shopController;
        private readonly IShopDialogueController _shopDialogueController;
        private readonly MasterConfiguration _masterConfig;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;

        public AvatarShopScreenControllerFactory(ScreenNavigationManager navManager, IScreenFactory screenFactory, Player player, Inventory inventory,
												 IControllerRepo repo, ShopController shopController, IShopDialogueController shopDialogueController, MasterConfiguration masterConfig, 
												 IAvatarThumbResourceManager thumbResourceManager)
        {
            _navManager = navManager;
            _screenFactory = screenFactory;
            _player = player;
            _inventory = inventory;
            _repo = repo;
            _shopController = shopController;
            _shopDialogueController = shopDialogueController;
            _masterConfig = masterConfig;
			_thumbResourceManager = thumbResourceManager;
        }

		public AvatarShopScreenController Create(ScreenClothingCategory activeCategory = ScreenClothingCategory.All)
        {
			IClosetSorter closetSorter = new ClothingDefaultSorter();

            return new AvatarShopScreenController(_navManager, _screenFactory, _player, _inventory, _repo, 
				_shopController, _shopDialogueController, _masterConfig, closetSorter, _thumbResourceManager, activeCategory);
        }
    }
}

