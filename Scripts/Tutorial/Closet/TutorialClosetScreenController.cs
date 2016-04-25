using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Tutorial.Controllers
{
	using Voltage.Witches;
	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Controllers;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Shop;

	using Voltage.Common.UI;
	using Voltage.Witches.Tutorial.uGUI;
	using Voltage.Witches.Screens.Closet;
    using Voltage.Witches.Models.Avatar;
	using Voltage.Witches.Bundles;

	// more of a Facade
	public class TutorialClosetScreenController : NewClosetScreenController
	{
		
		private TutorialOverlayCanvas _tutorialCanvas;

		public TutorialClosetScreenController(	ScreenNavigationManager navManager, IScreenFactory screenFactory, Player player, Inventory inventory, 
												ShopController shopController, IShopDialogueController shopDialogueController, MasterConfiguration masterConfig, 
												AvatarShopScreenControllerFactory avatarShopScreenFactory, IClosetSorter closetSorter, AvatarManifest avatarManifest,
												IAvatarThumbResourceManager thumbResourceManager)
		: base(navManager, screenFactory, player, inventory, shopController, shopDialogueController, masterConfig, avatarShopScreenFactory, closetSorter, avatarManifest, thumbResourceManager) {}


		protected override IScreen GetScreen()
		{
			if(_tutorialCanvas == null)
			{
				_tutorialCanvas = _screenFactory.GetDialog<TutorialOverlayCanvas> ();		// _tutorialCanvas = _screenFactory.GetOverlay<TutorialOverlayCanvas> ().OverlayController;
			}

			return base.GetScreen ();
		}

		public override void Dispose ()
		{
			_tutorialCanvas.Dispose ();

			base.Dispose ();
		}


		private bool _ignoreMakePassive = false;
		public IEnumerator InitClosetTutorial(string itemID)
		{
            // wait for screen to be ready
            while (!IsLoaded)
            {
                yield return null;
            }

			base.MakePassive (true);
			_ignoreMakePassive = true; //HACK to prevent generating Avatar from making buttons active

			_tutorialCanvas.OverlayController.InitCloset(itemID);
			_tutorialCanvas.OverlayController.EnableFullScreenMask (true);
			_tutorialCanvas.OverlayController.SetNarratorImage (TutorialScreenOverlay.NarratorImage.FULL_POCKET);

            var waitForFrameEnd = _tutorialCanvas.OverlayController.WaitForFrameEnd();

            while (waitForFrameEnd.MoveNext())
            {
                yield return waitForFrameEnd.Current;
            }
		}
			


		public IEnumerator SlideInNarrator()
		{
			return _tutorialCanvas.OverlayController.SlideInNarrator();
		}

		public IEnumerator ShowDialogue(string text)		
		{
			return _tutorialCanvas.OverlayController.ShowDialogueWithFade (text);
		}

		public IEnumerator WaitForClickAnywhere()
		{
			return _tutorialCanvas.OverlayController.WaitForClickAnywhere ();
		}




		public IEnumerator HighlightPajamas()
		{
			return _tutorialCanvas.OverlayController.ShowClosetItemSpotlight ();
		}

		public IEnumerator PointAtPajamas()
		{
			_tutorialCanvas.OverlayController.PointAtClosetItem ();
			return null;
		}

		public IEnumerator WaitForPajamasClicked()
		{
			bool clicked = false;

			// HACK need to investigate why hooking into screen.OnAction isn't working
			_screen.ClosetView.OnAction += (sender, arg) => 
			{
				_tutorialCanvas.OverlayController.MakeClosetItemPassive (true);
				_tutorialCanvas.OverlayController.HidePointer();
				clicked = true;
			};

			_tutorialCanvas.OverlayController.MakeClosetItemPassive (false);

			while (!clicked) 
			{
				yield return null;
			}
		}

		public IEnumerator HideNarratorAndDialogue()
		{
			_tutorialCanvas.OverlayController.HideDialogue ();
			_tutorialCanvas.OverlayController.HideNarrator ();
			return null;
		}


		public IEnumerator RevealAvatar()
		{
			return _tutorialCanvas.OverlayController.SpotlightReveal ();
		}


		public IEnumerator HighlightHomeButton()
		{
			float radius = 0.075f;
			return _tutorialCanvas.OverlayController.HighlightTarget ("btn_home", radius);
		}


		public IEnumerator PointAtHomeButton()
		{
			_tutorialCanvas.OverlayController.PointAtTarget ("btn_home", 0.01f, -0.02f);
			return null;
		}

		public IEnumerator WaitForHomeButtonClicked()
		{
			ButtonProxy proxyButton = _tutorialCanvas.OverlayController.GetButtonProxy("btn_home");
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
			_ignoreMakePassive = false;
		}

		public IEnumerator Pause(float seconds)
		{
			return _tutorialCanvas.OverlayController.Pause(seconds);
		}

		public override void MakePassive(bool value)
		{
			if (!_ignoreMakePassive) //HACK to prevent generating Avatar from making buttons active
			{
				base.MakePassive (value);
			}
		}
	}

}
