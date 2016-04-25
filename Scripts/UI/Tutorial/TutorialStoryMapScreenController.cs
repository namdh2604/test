
using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Tutorial.uGUI
{
	using Voltage.Witches.Controllers;
	using Voltage.Witches;
	using Voltage.Witches.Screens;
	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;
	using Voltage.Story.Configurations;
	using Voltage.Story.StoryDivisions;
	using Voltage.Witches.Shop;
	using Voltage.Witches.Story;
	using Voltage.Witches.User;

	using Voltage.Witches.Controllers.DressCode;
	using Voltage.Witches.Screens.Dialogues;
	using Voltage.Witches.Bundles;


	public class TutorialStoryMapScreenController : StoryMapUGUIScreenController
    {
		private TutorialOverlayCanvas _tutorialCanvas;

		private readonly IAvatarThumbResourceManager _thumbResourceManager;

		public TutorialStoryMapScreenController(ScreenNavigationManager navigationMgr, IScreenFactory screenFactory, IShopDialogueController shopDialogController, INoStaminaController noStaminaController, IControllerRepo repo,
		                                    	IStoryLoaderFacade storyLoader, ISceneHeaderFactory headerFactory, ISceneViewModelFactory sceneFactory, Player player, MasterConfiguration masterConfig, MasterStoryData storyData,
												IAvatarThumbResourceManager thumbResourceManager, FavorabilityMilestoneController favorabilityController)		// FIXME: this should be passed into the view, NOT the controller
		: base(navigationMgr, screenFactory, shopDialogController, noStaminaController, repo, storyLoader, headerFactory, sceneFactory, player, masterConfig, storyData, null, thumbResourceManager, favorabilityController) 
		{
			_thumbResourceManager = thumbResourceManager;
		}


		protected override void InitializeView()
		{
			base.InitializeView ();

			_tutorialCanvas.Init (this);
		}
		
		protected override IScreen GetScreen()		// FIXME: appears to be called multiple times???
		{
			if(_tutorialCanvas == null)
			{
				_tutorialCanvas = _screenFactory.GetDialog<TutorialOverlayCanvas> ();		// _tutorialCanvas = _screenFactory.GetOverlay<TutorialOverlayCanvas> ();
			}

			return base.GetScreen ();
		}

		public override void Dispose ()
		{
			_tutorialCanvas.Dispose ();

			if(_sceneCard != null) 
			{
				_sceneCard.Dispose();
			}

			if(_dressCodeLockDialogue != null) 
			{
				_dressCodeLockDialogue.Dispose();
			}

			base.Dispose ();
		}


		public override void MakePassive (bool value)
		{
			base.MakePassive (value);

			_tutorialCanvas.OverlayController.MakePassive (value);
		}


		public IEnumerator ShowStaminaTimer(bool value)
		{
			_ribbonController.ShowStaminaTimer (value);
			return null;
		}
		
		// maybe change to MakeBasePassive?
		public IEnumerator DisableAllInput()
		{
			base.MakePassive (true);
			return null;
		}




		public IEnumerator PointAtRibbon()
		{
			_tutorialCanvas.OverlayController.PointAtRibbon ();

			return null;
		}



		public IEnumerator HidePointer()
		{
			_tutorialCanvas.OverlayController.HidePointer ();

			return null;
		}


		public IEnumerator WaitForRibbonSelected()
		{
			bool ribbonClicked = false;

			Action onClick = null;
			onClick = () => { 
				_ribbonController.OnOpenEvent -= onClick; 
				_tutorialCanvas.OverlayController.HidePointer ();
				ribbonClicked = true;
			};
			_ribbonController.OnOpenEvent += onClick;		

			_ribbonController.MakePassive (false);			// HACK: need to resolve the interface for enabling/disabling ribbon buttons w/ support for passive and disabling while animating
			_ribbonController.EnableToggleButton (true);
			_ribbonController.EnableShopButton (true);

			while (!ribbonClicked)
			{
				yield return null;
			}

			_ribbonController.MakePassive (true);			// HACK: need to resolve the interface for enabling/disabling ribbon buttons w/ support for passive and disabling while animating
			_ribbonController.EnableToggleButton (false);
			_ribbonController.EnableShopButton (false);
			yield return new UnityEngine.WaitForSeconds (1f);		// HACK...to wait for ribbon transition to open
		}



		public IEnumerator HighlightStamina()
		{
			return _tutorialCanvas.OverlayController.ShowStaminaSpotlight ();
		}



		public IEnumerator HideHighlightStamina()
		{
			_tutorialCanvas.OverlayController.DisableSpotlight ();

			return null;
		}





		public IEnumerator ShowNarrator()
		{
			_tutorialCanvas.OverlayController.ShowNarrator ();

			return null;
		}

		public IEnumerator HideNarrator()
		{
			_tutorialCanvas.OverlayController.HideNarrator ();
			
			return null;
		}

		public IEnumerator FadeInNarrator()
		{
			return _tutorialCanvas.OverlayController.FadeInNarrator ();
		}

		public IEnumerator FadeOutNarrator()
		{
			return _tutorialCanvas.OverlayController.FadeOutNarrator();
		}


		public IEnumerator SlideInNarrator()
		{
			return _tutorialCanvas.OverlayController.SlideInNarrator();
		}


		public IEnumerator SlideOutNarrator()
		{
			return _tutorialCanvas.OverlayController.SlideOutNarrator();
		}



		public IEnumerator ShowDialogue(string text)		
		{
			return _tutorialCanvas.OverlayController.ShowDialogueWithFade (text);
		}

		public IEnumerator HideDialogue()
		{
			return _tutorialCanvas.OverlayController.FadeOutDialogue();
		}



		public IEnumerator InitLockTutorial()
		{
			DisableAllInput ();
			_tutorialCanvas.OverlayController.EnableFullScreenMask (true);
			_tutorialCanvas.OverlayController.SetNarratorImage (TutorialScreenOverlay.NarratorImage.FULL_POCKET);

			yield return new UnityEngine.WaitForEndOfFrame ();	// HACK: FIXME: bug with narrator initial placement
		}




		public IEnumerator WaitForClickAnywhere()
		{
			return _tutorialCanvas.OverlayController.WaitForClickAnywhere ();
		}



		private Voltage.Witches.StoryMap.SceneCardView _sceneCard;

        // FIXME: scene card requires special scaling, otherwise could generalize
        private void PointAtSceneCard(float offsetX, float offsetY)
		{
			Voltage.Witches.StoryMap.SceneCardView prototypeCard = _screen.GetSceneCardAt (0);	
			_sceneCard = _tutorialCanvas.OverlayController.DuplicateSceneCard (prototypeCard);	
			_sceneCard.enabled = false;
			
            _tutorialCanvas.OverlayController.PointAtReadButton(_sceneCard, offsetX, offsetY);
		}

		// FIXME: may not be happy with using alignment to set position
		public IEnumerator PointAtSceneCardReadButton(Alignment alignment)
		{
			switch (alignment) 
			{
				case Alignment.LEFT:
                    PointAtSceneCard(-0.1f, -0.05f);
					break;

				case Alignment.RIGHT:
                    PointAtSceneCard(0.1f, -0.05f);
					break;

				case Alignment.CENTER:
				default:
                    PointAtSceneCard(0f, -0.05f);
					break;
			}

			return null;
		}



		public IEnumerator WaitForReadStorySelected()
		{
			if (_sceneCard == null) 
			{
				throw new NullReferenceException ("Requires Scene Card!");
			}

			bool clicked = false;
			
			EventHandler onClick = null;
			onClick = (sender, eventArgs) => { 
				_sceneCard.EnableButtonInput(false); 
				_sceneCard.OnButtonSelected -= onClick;
				clicked = true;
				_sceneCard.Dispose();	// _sceneCard = null;
			};
			_sceneCard.OnButtonSelected += onClick;
			_sceneCard.EnableButtonInput (true);
            _sceneCard.enabled = true;

			while(!clicked)
			{
				yield return null;
			}
		}



        public IEnumerator TransitionToReadCard()
        {
            var fadeOutNarrator = _tutorialCanvas.OverlayController.FadeOutNarratorAndDialogue ();
            while (fadeOutNarrator.MoveNext())
            {
                yield return fadeOutNarrator.Current;
            }

            yield return PointAtSceneCardReadButton(Alignment.RIGHT);
        }














		public IEnumerator RefillStamina()
		{
			// update player state
			_player.RefillStamina ();
            yield break;
		}


		public IEnumerator SpotlightToFullScreenMask()
		{
			return _tutorialCanvas.OverlayController.SpotlightToFullScreenMask ();
		}


		



		public IEnumerator OpenRibbon()
		{
			_ribbonController.HideStaminaTimer ();

			yield return new UnityEngine.WaitForSeconds (1f);		// HACK...need to look into why ribbon isn't animating out

			_ribbonController.OpenRibbon ();
			yield return new UnityEngine.WaitForSeconds (1f);		// HACK...to wait for ribbon transition to open
		}


		public IEnumerator Pause(float seconds)
		{
			return _tutorialCanvas.OverlayController.Pause(seconds);
		}



		private DressCodeDialogue _dressCodeLockDialogue;

		public IEnumerator ShowLockDialogue(string id, string name, string category, string path)
		{
			_tutorialCanvas.OverlayController.HideNarrator ();
			_tutorialCanvas.OverlayController.HideDialogue ();

			Clothing clothingReq = new Clothing(id, name, string.Empty , string.Empty, category);
			clothingReq.IconFilePath = path;

			_dressCodeLockDialogue = _screenFactory.GetDialog<DressCodeDialogue>();
            _dressCodeLockDialogue.Hide();
			_dressCodeLockDialogue.transform.SetParent (_tutorialCanvas.OverlayController.transform);
			_dressCodeLockDialogue.Init (new TutorialDressCodeMissionDialogViewModel(clothingReq), _thumbResourceManager);
			_dressCodeLockDialogue.EnableCloseButton(false);
			_dressCodeLockDialogue.MakePassive (true);
            _dressCodeLockDialogue.Display(null);

            while (!_dressCodeLockDialogue.IsLoaded())
            {
                yield return null;
            }
		}

		public IEnumerator WaitForLockSelected()
		{
			if (_dressCodeLockDialogue == null) 
			{
				throw new NullReferenceException ("dialogue is null!");
			}

			PointAtLockDialogue ();

			bool clicked = false;
			Action<int> responseHandler = (input) => 
			{
				if(input == (int)DressCodeResponse.BUY)
				{
					clicked = true;
					HidePointer();
					_dressCodeLockDialogue.Dispose();	// _dressCodeLockDialogue = null;
				}
			};

			_dressCodeLockDialogue.Display(responseHandler);
			_dressCodeLockDialogue.MakePassive(false);

			while(!clicked)
			{
				yield return null;
			}
		}

		private void PointAtLockDialogue()
		{
			if (_dressCodeLockDialogue == null) 
			{
				throw new NullReferenceException ("dialogue is null!");
			}

			_tutorialCanvas.OverlayController.PointAt (0.55f, 0.15f);		// FIXME: may need to change to target object when deploying to ipad
		}



		public enum Alignment
		{
			CENTER,
			LEFT,
			RIGHT
		}


		private sealed class TutorialDressCodeMissionDialogViewModel : IDressCodeMissionViewModel
		{
			public IClothing DressReq { get; private set; }
			public bool HasItem { get { return false; } }
			public bool IsWearingItem { get { return false; } }

			public DressCodeDialogType Type { get { return DressCodeDialogType.BUY; } }

			public TutorialDressCodeMissionDialogViewModel (IClothing clothingReq)
			{
				DressReq = clothingReq;
			}
		}

    }
    
}




//public IEnumerator ShowDialogue<T>() where T: UnityEngine.Component
//{
//	T dialogue = _screenFactory.GetDialog<T> ();
//
//	UnityEngine.Debug.LogWarning ("dialogue created: " + dialogue.name);
//
//	return null;
//}

