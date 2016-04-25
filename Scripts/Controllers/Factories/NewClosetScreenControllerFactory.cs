using System;
using Voltage.Witches.Screens;
using Voltage.Witches.Shop;
using Voltage.Witches.Models;
using Voltage.Witches.Models.Avatar;

using Voltage.Witches.Configuration;
using Voltage.Witches.Bundles;

namespace Voltage.Witches.Controllers.Factories
{
	using Voltage.Witches.Screens.Closet;

	public class NewClosetScreenControllerFactory
	{
		private readonly ScreenNavigationManager _navManager;
		private readonly IScreenFactory _screenFactory;
		private readonly ShopController _shopController;
        private readonly Inventory _inventory;
        private readonly MasterConfiguration _masterConfig;
        private readonly IShopDialogueController _shopDialogueController;
        private readonly AvatarShopScreenControllerFactory _avatarShopScreenFactory;
        private readonly AvatarManifest _avatarManifest;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;

		public NewClosetScreenControllerFactory(ScreenNavigationManager navManager, IScreenFactory screenFactory, 
            ShopController shopController, IShopDialogueController shopDialogueController, Inventory inventory, 
            MasterConfiguration masterConfig, AvatarShopScreenControllerFactory avatarShopScreenFactory,
			AvatarManifest avatarManifest, IAvatarThumbResourceManager thumbResourceManager)
		{
			_navManager = navManager;
			_screenFactory = screenFactory;
			_shopController = shopController;
            _inventory = inventory;
            _masterConfig = masterConfig;
            _shopDialogueController = shopDialogueController;
            _avatarShopScreenFactory = avatarShopScreenFactory;
            _avatarManifest = avatarManifest;
			_thumbResourceManager = thumbResourceManager;
		}

        public NewClosetScreenController Create(Player player)
		{
			IClosetSorter closetSorter = new ClothingDefaultSorter();

            return new NewClosetScreenController(_navManager, _screenFactory, player, _inventory, _shopController,
				_shopDialogueController, _masterConfig, _avatarShopScreenFactory, closetSorter, _avatarManifest, _thumbResourceManager);
		}
	}
}

