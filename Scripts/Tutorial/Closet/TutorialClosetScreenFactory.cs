using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Tutorial.Controllers.Factories
{
	using Voltage.Witches.Models;
	using Voltage.Witches.Tutorial.Controllers;

	using Voltage.Witches;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Controllers;
	using Voltage.Witches.Shop;

	using Voltage.Witches.Screens.Closet;
	using Voltage.Witches.Data.Tutorial;
    using Voltage.Witches.Models.Avatar;
	using Voltage.Witches.Bundles;

	public class TutorialClosetScreenFactory 
	{

		private readonly ScreenNavigationManager _navManager;
		private readonly IScreenFactory _screenFactory;
		private readonly Inventory _inventory;
		private readonly ShopController _shopController;
		private readonly IShopDialogueController _shopDialogueController;
		private readonly MasterConfiguration _masterConfig;
		private readonly AvatarShopScreenControllerFactory _avatarShopScreenFactory;
        private readonly AvatarManifest _avatarManifest;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;


		public TutorialClosetScreenFactory(	ScreenNavigationManager navManager, IScreenFactory screenFactory, Inventory inventory, 
											ShopController shopController, IShopDialogueController shopDialogueController, MasterConfiguration masterConfig, 
											AvatarShopScreenControllerFactory avatarShopScreenFactory, AvatarManifest avatarManifest, IAvatarThumbResourceManager thumbResourceManager)
		{
			// TODO: guard clauses

			_navManager = navManager;
			_screenFactory = screenFactory;
			_inventory = inventory;
			_shopController = shopController;
			_shopDialogueController = shopDialogueController;
			_masterConfig = masterConfig;
			_avatarShopScreenFactory = avatarShopScreenFactory;
            _avatarManifest = avatarManifest;
			_thumbResourceManager = thumbResourceManager;
		}

		public TutorialClosetScreenController Create(Player player)
		{
			IClosetSorter closetSorter = new ClothingTutorialSorter(AvatarTutorial.CLOTHING_ID, AvatarTutorial.CLOTHING_POSITION_IN_CLOSET);
			
			return new TutorialClosetScreenController (_navManager, _screenFactory, player, _inventory, _shopController, _shopDialogueController,
				_masterConfig, _avatarShopScreenFactory, closetSorter, _avatarManifest, _thumbResourceManager);
		}

	}



}
