using System;
using System.Collections.Generic;
using System.Collections;


namespace Voltage.Witches.Tutorial.Controllers.Factories
{
	using Voltage.Witches.Tutorial.Controllers;

	using Voltage.Witches.Models;
	using Voltage.Witches.Controllers;
	using Voltage.Witches;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Shop;
	using Voltage.Witches.Screens.Closet;

	using Voltage.Witches.Data.Tutorial;
	using Voltage.Witches.Bundles;

	public class TutorialShopScreenFactory 
	{
		private readonly Player _player;
		private readonly ScreenNavigationManager _navManager;
		private readonly IScreenFactory _screenFactory;
		private readonly IControllerRepo _repo;
		private readonly Inventory _inventory;
		private readonly ShopController _shopController;
		private readonly IShopDialogueController _shopDialogueController;
		private readonly MasterConfiguration _masterConfig;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;

		public TutorialShopScreenFactory(ScreenNavigationManager navManager, IScreenFactory screenFactory, Player player, Inventory inventory, IControllerRepo repo, 
										 ShopController shopController, IShopDialogueController shopDialogueController, MasterConfiguration masterConfig,
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


		public TutorialShopScreenController Create()
		{
			IClosetSorter closetSorter = new ClothingTutorialSorter (AvatarTutorial.CLOTHING_ID, AvatarTutorial.CLOTHING_POSITION_IN_CLOSET);

			return new TutorialShopScreenController (_navManager, _screenFactory, _player, _inventory, _repo, _shopController, _shopDialogueController, _masterConfig, closetSorter, _thumbResourceManager);
		}

	}

}
