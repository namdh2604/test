using System;
using System.Collections.Generic;
using System.Collections;


namespace Voltage.Witches.Controllers.Factories
{
	using Voltage.Witches.Models;

//	using Voltage.Witches;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Configuration;
//	using Voltage.Witches.Controllers;
	using Voltage.Witches.Shop;
    using Voltage.Witches.Login;

	public class HomeScreenControllerFactory 
	{
		private readonly ScreenNavigationManager _navManager;
		private readonly IScreenFactory _screenFactory;
		private readonly Player _player;
		private readonly ShopDialogueController _shopDialogueController;
		private readonly MasterConfiguration _masterConfig;
		private readonly IControllerRepo _repo;
		private readonly HomeScreenFeatureLockHandler _unlockHandler;
        private readonly BonusManager _bonusManager;

		public HomeScreenControllerFactory (ScreenNavigationManager navManager, IScreenFactory screenFactory, Player player, 
                                            IControllerRepo repo, MasterConfiguration masterConfig, 
                                            ShopDialogueController shopDialogueController,
											HomeScreenFeatureLockHandler featureLockHandler, BonusManager bonusManager)
		{
			_navManager = navManager;
			_screenFactory = screenFactory;
			_player = player;
			_repo = repo;
			_masterConfig = masterConfig;
			_shopDialogueController = shopDialogueController;
			_unlockHandler = featureLockHandler;
            _bonusManager = bonusManager;
		}

		public HomeScreenController Create(bool enableLoginBonus)
		{
			return new HomeScreenController(_navManager, _screenFactory, _player, _repo, _masterConfig, _shopDialogueController, 
											_unlockHandler, _bonusManager, enableLoginBonus);
		}
	}
}