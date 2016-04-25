using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;

namespace Voltage.Witches.Controllers
{
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public class StoryTestController : ScreenController
	{
		public StoryTestController(ScreenNavigationManager controller):base(controller)
		{
			SceneManager.LoadScene("StoryTest", LoadSceneMode.Additive);
		}

		protected override IScreen GetScreen()
		{
//			BaseScreen newScreen = new BaseScreen();

			return default(BaseScreen);
		}

		public override void MakePassive (bool value)
		{
			throw new NotImplementedException ();
		}
	}
}