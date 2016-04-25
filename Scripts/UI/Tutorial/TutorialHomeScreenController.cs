using System;
using System.Collections.Generic;
using System.Collections;


namespace Voltage.Witches.Tutorial
{
	using Voltage.Witches.Controllers;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Shop;
    using Voltage.Witches.Login;

	using Voltage.Common.UI;
	using Voltage.Witches.Tutorial.uGUI;

	using FacingDirection = Voltage.Witches.StoryMap.CharacterNarrator.FacingDirection;

	public class TutorialHomeScreenController : HomeScreenController
	{
		private TutorialOverlayCanvas _tutorialCanvas;

		private readonly IScreenFactory _screenFactory;

		public TutorialHomeScreenController(ScreenNavigationManager navMgr, IScreenFactory screenFactory, Player player, 
            IControllerRepo repo, MasterConfiguration masterConfig, ShopDialogueController shopDialogController, 
            HomeScreenFeatureLockHandler lockHandler, BonusManager bonusManager) 
            : base(navMgr, screenFactory, player, repo, masterConfig, shopDialogController, lockHandler, bonusManager, false) 
		{
			_screenFactory = screenFactory;
		}

		protected override IScreen GetScreen()
		{
			if(_tutorialCanvas == null)
			{
				_tutorialCanvas = _screenFactory.GetDialog<TutorialOverlayCanvas> ();		// _tutorialCanvas = _screenFactory.GetOverlay<TutorialOverlayCanvas> ().OverlayController;
			}

			return base.GetScreen ();
		}

        protected override void PostShowAction()
        {
           // HACK: do nothing
        }

		public override void Dispose ()
		{
			_tutorialCanvas.Dispose ();

			base.Dispose ();
		}




		public IEnumerator InitHomeTutorial()
		{
			base.MakePassive(true);

			_tutorialCanvas.OverlayController.EnableFullScreenMask(true);

			_tutorialCanvas.OverlayController.SetNarratorImage(TutorialScreenOverlay.NarratorImage.FULL_POCKET);
			_tutorialCanvas.OverlayController.SetNarratorDirection(FacingDirection.RIGHT);

			// HACK: have to resolve narrator requiring next frame up date to set initial position
			return _tutorialCanvas.OverlayController.WaitForFrameEnd();
		}

		public IEnumerator SlideInNarrator()
		{
			return _tutorialCanvas.OverlayController.SlideInNarrator(TutorialScreenOverlay.NarratorSlideOrigin.SCREEN_LEFT);
		}

		public IEnumerator ShowDialogue (string text)
		{
			return _tutorialCanvas.OverlayController.ShowDialogueWithFade(text);
		}

		public IEnumerator WaitForClickAnywhere()
		{
			return _tutorialCanvas.OverlayController.WaitForClickAnywhere ();
		}

		public IEnumerator HighlightStoryButton()
		{
			return _tutorialCanvas.OverlayController.HighlightTarget("Polaroid", 0.15f);
		}

		public IEnumerator PointAtStoryButton()
		{
			_tutorialCanvas.OverlayController.PointAtTarget("Polaroid", 0.05f, -0.05f);
			return null;
		}

		public IEnumerator WaitForStoryButtonClicked()
		{
			ButtonProxy proxyButton = _tutorialCanvas.OverlayController.GetButtonProxy("story_buttons");
			proxyButton.MakePassive (true);

			bool clicked = false;

			Action onClick = null;
			onClick = () => 
			{ 
				proxyButton.OnClick -= onClick; 
				_tutorialCanvas.OverlayController.HidePointer ();
				clicked = true;
			};
			proxyButton.OnClick += onClick;			

			proxyButton.MakePassive(false);

			while (!clicked)
			{
				yield return null;
			}

			proxyButton.MakePassive(true);	
			proxyButton.Dispose ();
		}


//		public IEnumerator Pause(float seconds)
//		{
//			return _tutorialCanvas.OverlayController.Pause(seconds);
//		}

	}
}
