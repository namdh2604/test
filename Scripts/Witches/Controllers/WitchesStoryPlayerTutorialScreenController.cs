using System;
using System.Collections;

namespace Voltage.Witches.Controllers
{
    using Voltage.Witches.Story;
    using Voltage.Witches.Models;
    using Voltage.Common.Logging;

	using Voltage.Witches.Tutorial.uGUI;
	using Voltage.Witches.Screens;

	using Voltage.Common.UI;
	using Voltage.Witches.Screens.Dialogues;

    public class WitchesStoryPlayerTutorialScreenController : WitchesStoryPlayerScreenController
    {
		private readonly IScreenFactory _screenFactory;

        private bool _sceneEnded;
        public bool ShowCompletedScreen { get; set; }

        public WitchesStoryPlayerTutorialScreenController(StoryPlayerScreenControllerData data, bool showInterface,
            IStoryPlayerDialogController dialogController, StoryMusicPlayer musicPlayer) : base(data, dialogController, musicPlayer, showInterface)
        {
			_sceneEnded = false;
            ShowCompletedScreen = true;

			_screenFactory = data.ScreenFactory;
        }
	

		// HACK: override to prevent tutorial from calling base.QueryServerForSceneSettings
		protected override void SetupStoryPlayer(Player player, Voltage.Story.StoryPlayer.StoryPlayerSettings settings, Voltage.Witches.UI.StoryPlayerUIScreen screen, Voltage.Story.Views.ILayoutDisplay layoutDisplay)
		{
			CreateAndLaunchScene(player, settings, screen, layoutDisplay);
		}







        public IEnumerator WaitForSceneEnd(bool resetMusic)		// override exit to home/storymap to set _sceneEnded
        {
            _sceneEnded = false;
            while (!_sceneEnded)
            {
                yield return null;
            }

            if (resetMusic)
            {
                RestartBGM();
            }
        }



		private void ResetInputState()
		{
			_passiveCounter = 0;
		}

        public IEnumerator EnableInput(bool value)
        {
			if (value) 
			{
				ResetInputState ();
			}

			MakePassive (!value);

            return null;
        }

		public IEnumerator MoveToNextNode()
		{
			_activeStoryPlayer.Next();

			return null;
		}


        protected override void ProcessSceneCompleteChoice(int choice)
        {
            _sceneEnded = true;
        }

        protected override void DisplaySceneCompleteDialog()
        {
            // Check to see if the tutorial want to display a scene complete screen dialog.  If it doesn't
            // don't display the dialog and set the _sceneEnded to true to end the Coroutine WaitForSceneEnd()
            if (ShowCompletedScreen)
            {
				OnProcessCompleteScene += () => _sceneEnded = true;
                base.DisplaySceneCompleteDialog();
            }
			else
			{
                _player.CompleteLocalScene();
            	_sceneEnded = true;
			}
        }





		// HACK: quick copy/paste, maybe instead return StoryPlayerInfoDialogue from the base OnInfoButton and disable that
		// TEMPORARY disable info dialogue button to prevent return to storymap
		protected override void OnInfoButton ()
		{
			PlayerPreferences prefs = PlayerPreferences.GetInstance ();
			
			StoryPlayerInfoDialogue infoDialogue = _screenFactory.GetDialog<StoryPlayerInfoDialogue> ();
			infoDialogue.RefreshInfo (_player, _storySettings, prefs.SoundEnabled, prefs.SFXEnabled);
			infoDialogue.OnCloseButton += () => 
			{
				infoDialogue.Dispose();
				_storyPlayerUIScreen.EnableMask(false);
				
				// the passive and ui selected conditions must be set here since MakePassive will enable the screens input
				OnUISelected();
				MakePassive(false);
			};

			infoDialogue.EnableDialogueButton (false);

			infoDialogue.OnRefillStamina += () =>
			{	
				HandleRefillStamina();
				infoDialogue.RefreshInfo (_player, _storySettings, prefs.SoundEnabled, prefs.SFXEnabled);
			};

			_storyPlayerUIScreen.EnableMask (true);
			MakePassive (true);
		}








		private TutorialOverlayCanvas _tutorialCanvas;
		private TutorialScreenOverlay _tutorialOverlay;




		// public override IScreen GetScreen()?
		public override void Show ()
		{
			if(_tutorialCanvas == null)
			{
				_tutorialCanvas = _screenFactory.GetDialog<TutorialOverlayCanvas> ();		// FIXME: rename TutorialStoryMapCanvas to generic TutorialCanvas
				_tutorialOverlay = _tutorialCanvas.OverlayController;
			}

			_tutorialCanvas.Show ();
			base.Show ();
		}

		public override void Hide ()
		{
			_tutorialCanvas.Hide ();

			base.Hide ();
		}


		public override void Dispose ()
		{
			_tutorialCanvas.Dispose ();
			base.Dispose ();
		}




		public IEnumerator DisableAllInput()
		{
			MakePassive (true);
			return null;
		}








		private ButtonProxy _proxyInfoBookButton;

		public IEnumerator HighlightInfoBook()
		{
			return _tutorialOverlay.ShowInfoBookSpotlight();
		}


		public IEnumerator PointAtInfoBook()
		{
			_proxyInfoBookButton = _tutorialOverlay.GetInfoButtonProxy ();

//			_storyPlayerScreen.EnableMask (true);
			_tutorialOverlay.PointAtInfoButton ();
			_proxyInfoBookButton.MakePassive (true);

			return null;
		}

		public IEnumerator WaitForInfoBookSelected()			// PointAtInfoBook() needs to be called first to assign _proxyInfoBookButton
		{
			bool clicked = false;
			
			Action onClick = null;
			onClick = () => 
			{ 
				_proxyInfoBookButton.OnClick -= onClick; 
				_tutorialOverlay.HidePointer ();
				clicked = true;
			};
			_proxyInfoBookButton.OnClick += onClick;			
			
			_proxyInfoBookButton.MakePassive(false);
			
			while (!clicked)
			{
				yield return null;
			}
			
			_proxyInfoBookButton.MakePassive(true);	// maybe destroy
			_proxyInfoBookButton.Dispose ();

//			yield return _tutorialOverlay.SpotlightToFullScreenMask ();
//			_tutorialOverlay.DisableSpotlight ();
		}


		public IEnumerator SpotlightToFullScreenMask()
		{
			return _tutorialOverlay.SpotlightToFullScreenMask ();
		}



		private StoryPlayerInfoDialogue _infoDialogue;

		public IEnumerator ShowInformationDialogue()
		{
			_tutorialOverlay.DisableSpotlight ();
			_storyPlayerUIScreen.EnableMask (true);
			MakePassive (true);

            PlayerPreferences prefs = PlayerPreferences.GetInstance ();
			_infoDialogue = _screenFactory.GetDialog<StoryPlayerInfoDialogue> ();
            _infoDialogue.RefreshInfo(_player, _storySettings, prefs.SoundEnabled, prefs.SFXEnabled);
//			_infoDialogue.OnCloseButton += () => 
//			{
//				_infoDialogue.Dispose();
//				_storyPlayerScreen.EnableMask(false);
//				_storyPlayerScreen.MakePassive(false);
//			};
//			infoDialogue.OnDialogueButton += ;
//			infoDialogue.OnRefillStamina += ;

			_infoDialogue.MakePassive (true);

			return null;
		}





		public IEnumerator ShowNarrator()
		{
			return _tutorialOverlay.FadeInNarratorB ();
		}

		public IEnumerator ShowDialogue(string text)
		{
			return _tutorialOverlay.ShowNarratorBDialogueWithFade (text);
		}


		public IEnumerator HighlightInfoStaminaGroup()
		{
			return _tutorialOverlay.ShowInfoStaminaSpotlight ();
		}


		public IEnumerator WaitForClickAnywhere()
		{
			return _tutorialCanvas.OverlayController.WaitForClickAnywhere ();
		}



		public IEnumerator CloseStaminaGrpHighlight()
		{
			return _tutorialOverlay.SpotlightReveal ();
		}


		public IEnumerator HideNarratorAndDialogue()
		{
			return _tutorialOverlay.FadeOutNarratorBAndDialogue ();
		}



		public IEnumerator PointAtInfoCloseButton()
		{
			_tutorialOverlay.PointAtInfoDialogueCloseButton ();
			return null;
		}


		public IEnumerator WaitForCloseInfoSelected()
		{
			bool clicked = false;

			_infoDialogue.OnCloseButton += () => 
			{
				_infoDialogue.Dispose();					// infoDialogue must have been created and assigned by calling ShowInformationDialogue()
				_tutorialOverlay.HidePointer();
				_storyPlayerUIScreen.EnableMask(false);
//				_storyPlayerScreen.MakePassive(false);
				clicked = true;
			};

			_infoDialogue.MakeCloseButtonPassive (false);

			while(!clicked)
			{
				yield return null;
			}
				
		}





		public IEnumerator Pause(float durationInSec)
		{
			UnityEngine.Debug.Log ("paused: " + durationInSec);

			yield return new UnityEngine.WaitForSeconds(durationInSec);
		}




		public IEnumerator ShowTapToContinuePrompt()
		{
			TapToContinueDialogue.StartTimer (1f);
			return null;
		}

		public IEnumerator HideTapToContinuePrompt()
		{
			TapToContinueDialogue.StopTimer ();
			return null;
		}


		public IEnumerator PromptOnInactivity(bool value)
		{
			EnableInactivityPrompt (value);
			return null;
		}










    }
}

