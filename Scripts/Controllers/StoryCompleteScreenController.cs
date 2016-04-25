using System.Collections.Generic;
using UnityEngine;
using Voltage.Witches.Configuration.JSON;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;

namespace Voltage.Witches.Controllers
{
	using Debug = UnityEngine.Debug;

	public class StoryCompleteScreenController : ScreenController
	{
		private IScreenFactory _factory;
		private Player _player;
		
		public iGUISmartPrefab_StoryCompleteScreen _screen;

		public readonly ScreenNavigationManager _navManager;
		
		public StoryCompleteScreenController(ScreenNavigationManager navManager,IScreenFactory factory,Player player):base(navManager)
		{
			_factory = factory;
			_player = player;

			_navManager = navManager;
			
			InitializeView();
		}

        public override void Dispose()
        {
            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
        }

		protected override IScreen GetScreen ()
		{
			if(_screen != null)
			{
				return _screen;
			}
			else
			{
				_screen = _factory.GetScreen<iGUISmartPrefab_StoryCompleteScreen>();
				_screen.Init(_player, this);
				return _screen;
			}
		}
		
		void InitializeView()
		{
			_screen = _factory.GetScreen<iGUISmartPrefab_StoryCompleteScreen>();
			_screen.Init(_player, this);
		}

		public override void MakePassive(bool value)
		{
			_screen.SetEnabled(!value);
		}
			
		
		public void Unload()
		{
			_screen.SetEnabled(false);
		}

		public void GoBackHome()
		{
			_navManager.GoToExistingScreen("/Home");
		}

	}
}