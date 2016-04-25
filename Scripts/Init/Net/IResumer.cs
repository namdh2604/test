using System;
using System.Collections;
using System.Collections.Generic;
using VoltUtil;

namespace Voltage.Witches.DI
{
	using Voltage.Witches.Models;
	using Voltage.Witches.Controllers;
	using Voltage.Witches.Data.Tutorial;
	using Voltage.Common.Unity;
	
    using Voltage.Witches.Controllers.Factories;
	
	public interface IResumer
	{
		void Resume (string progress);
	}
	
	public class GameResumer : IResumer
	{
		private IControllerRepo _repo = null;
		
		private readonly ScreenNavigationManager _navManager;

		
		public GameResumer (IControllerRepo controllerRepo, ScreenNavigationManager navManager)
		{
			_repo = controllerRepo;
			
			_navManager = navManager;
		}

		
		public void Resume(string progress)
		{			
			UnitySingleton.Instance.StartCoroutine(StartGame());
		}

		
		private IEnumerator StartGame()
		{
			IEnumerator tutorialRoutine = ExecuteMainTutorial();
			if (tutorialRoutine != null)
			{
				yield return UnitySingleton.Instance.StartCoroutine(tutorialRoutine);
			}

			#region Ambient tutorial hack
			AmbientTutorialResumer ambientResumer = _repo.Get<AmbientTutorialResumer>();
			if (ambientResumer.ShouldResumeAvatarTutorial ()) {
				ambientResumer.HandleAmbientTutorial ();
			}
			#endregion
			else 
			{
				ResumeGame ();
			}
		}
		
		private IEnumerator ExecuteMainTutorial()
		{
			MainTutorial mainTutorial = _repo.Get<MainTutorial>();
			Player player = _repo.Get<Player>();
			
			if (mainTutorial.IsDone(player))
			{
				return null;
			}
			
			return mainTutorial.Execute(player);
		}
		
		

		private void ResumeGame()
		{
            IScreenController homeScreen = _repo.Get<HomeScreenControllerFactory>().Create(true);       // true, always show login bonus when resuming game (even after completing main tutorial)

			_navManager.OpenScreenAtPath (homeScreen, "/");		// ensures /Home is at the root path, some screens try to reduce to it explicitly...does not rely on MainTutorial::Cleanup as that will not when all tutorials are completed
		}
		
		
		

	}
	
	
}