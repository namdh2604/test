using System;
using System.Collections.Generic;
using System.Collections;


namespace Voltage.Witches.Tutorial.Controllers
{
	using Voltage.Witches.Controllers;

	using Voltage.Witches.Models;
	using Voltage.Witches;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Shop;

	using Voltage.Witches.Tutorial.uGUI;
	using Voltage.Witches.Screens.Closet;
	using Voltage.Witches.Screens.Dialogues;
	using Voltage.Common.UI;

	using Voltage.Witches.Bundles;

	// more of a Facade
	public class TutorialShopScreenController : AvatarShopScreenController
	{
		private TutorialOverlayCanvas _tutorialCanvas;

		private readonly IScreenFactory _screenFactory;
		private readonly IAvatarThumbResourceManager _thumbResourceManager;

		public TutorialShopScreenController(ScreenNavigationManager navManager, IScreenFactory screenFactory, Player player, Inventory inventory, IControllerRepo repo, 
											ShopController shopController, IShopDialogueController shopDialogueController, MasterConfiguration masterConfig, IClosetSorter closetSorter,
											IAvatarThumbResourceManager thumbResourceManager) 
			: base(navManager, screenFactory, player, inventory, repo, shopController, shopDialogueController, masterConfig, closetSorter, thumbResourceManager) 
		{
			_screenFactory = screenFactory;
			_thumbResourceManager = thumbResourceManager;
		}


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


		public IEnumerator InitShopTutorial(string itemID)
		{
            // wait for the display to be ready
            while (!IsLoaded)
            {
                yield return null;
            }

			base.MakePassive (true);

			_tutorialCanvas.OverlayController.InitShop (itemID);
			_tutorialCanvas.OverlayController.EnableFullScreenMask (true);
			_tutorialCanvas.OverlayController.SetNarratorImage (TutorialScreenOverlay.NarratorImage.FULL_POCKET);

            var frameEnd = _tutorialCanvas.OverlayController.WaitForFrameEnd();

            while (frameEnd.MoveNext())
            {
                yield return frameEnd.Current;
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
			return _tutorialCanvas.OverlayController.ShowShopItemSpotlight ();
		}

		public IEnumerator PointAtPajamas()
		{
			_tutorialCanvas.OverlayController.PointAtShopItem ();
			return null;
		}

		public IEnumerator WaitForPajamasClicked()
		{
			bool clicked = false;

			Action onClick = () => 
			{
				// remove custom listener?
				_tutorialCanvas.OverlayController.MakeShopItemPassive (true);
				_tutorialCanvas.OverlayController.HidePointer();
				clicked = true;
			};

			_tutorialCanvas.OverlayController.HijackShopItemOnClick(onClick);
			_tutorialCanvas.OverlayController.MakeShopItemPassive(false);

			while (!clicked) 
			{
				yield return null;
			}
		}

		public IEnumerator HighlightToFullScreenMask()
		{
			return _tutorialCanvas.OverlayController.SpotlightToFullScreenMask ();
		}

		public IEnumerator HideNarratorAndDialogue()
		{
			_tutorialCanvas.OverlayController.HideDialogue ();
			_tutorialCanvas.OverlayController.HideNarrator ();
			return null;
		}

		public IEnumerator GivePlayerPajamas(Clothing clothingReq, Player player)
		{
			AddClothingToInventory (clothingReq);
//			player.UpdatePremiumCurrency(-clothingReq.PremiumPrice);		// in the tutorial player can only purchase with premium currency

			return null;
		}


		private AvatarClothingBuyDialogue _buyDialogue;

		public IEnumerator ShowBuyDialogue(IClothing clothingReq, Player player)
		{
			_buyDialogue = _screenFactory.GetDialog<AvatarClothingBuyDialogue>();
			_buyDialogue.transform.SetParent(_tutorialCanvas.OverlayController.transform);	// HACK: maybe should move this to TutorialOverlay
			_buyDialogue.Init(clothingReq, player.CurrencyPremium, player.Currency, true, _thumbResourceManager);
			_buyDialogue.MakePassive(true);

			// FIXME! relies on dialogue being displayable by default

			return null;
		}

		public IEnumerator PointAtBuyButton()
		{
			_tutorialCanvas.OverlayController.PointAtTarget("btn_buy_starstone", 0.05f, -0.05f);
			return null;
		}


		public IEnumerator WaitForBuyButtonClicked()
		{
			if (_buyDialogue == null) 
			{
				throw new NullReferenceException();
			}

			bool clicked = false;

			Action<int> onClick = (i) => 
			{
				_buyDialogue.MakeButtonPassive (AvatarClothingBuyDialogue.DialogueButton.STARSTONE, true);
				_tutorialCanvas.OverlayController.HidePointer();
				_buyDialogue.Dispose();
				clicked = true;
			};
			_buyDialogue.Display (onClick);
			_buyDialogue.MakeButtonPassive (AvatarClothingBuyDialogue.DialogueButton.STARSTONE, false);

			while (!clicked) 
			{
				yield return null;
			}
		}

		public IEnumerator HighlightClosetButton()
		{
			float radius = 0.04f;
			return _tutorialCanvas.OverlayController.HighlightTarget ("closet_button", radius);	// use actual object from screen?
		}

		public IEnumerator PointAtClosetButton()
		{
			_tutorialCanvas.OverlayController.PointAtTarget ("closet_button", 0.025f, -0.05f);
			return null;
		}

		public IEnumerator WaitForClosetButtonClicked()
		{
			ButtonProxy proxyButton = _tutorialCanvas.OverlayController.GetButtonProxy ("closet_button");
			proxyButton.MakePassive (true);

			bool clicked = false;
			Action onClick = null;
			onClick = () => 
			{
				proxyButton.MakePassive(true);
				proxyButton.OnClick -= onClick;
				_tutorialCanvas.OverlayController.HidePointer();
				clicked = true;
			};
			proxyButton.OnClick += onClick;

			proxyButton.MakePassive(false);

			while (!clicked) 
			{
				yield return null;
			}

			proxyButton.Dispose();
		}


		public IEnumerator Pause(float seconds)
		{
			return _tutorialCanvas.OverlayController.Pause(seconds);
		}
			


	}

}