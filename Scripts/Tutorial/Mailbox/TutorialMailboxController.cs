using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iGUI;
using Voltage.Witches.Events;


namespace Voltage.Witches.Tutorial
{
    using Voltage.Witches.Controllers;
    using Voltage.Witches.Screens;
    using Voltage.Witches.Models;
    using Voltage.Witches.Configuration;
    using Voltage.Witches.Exceptions;
    using Voltage.Common.Net;
    using Voltage.Story.Variables;
	using Voltage.Witches.Shop;

    public class TutorialMailboxController : MailboxScreenController
    {
        private TutorialHomeScreen _tutorialScreen;
        private bool _isButtonPressed;
//        private MailboxScreen.ButtonType _targetButton;
//        private MailFactory _mailFactory;

        private Dictionary<iGUIButton, iGUIElement> _buttonArtMap;
        private IGUIHandler _buttonHandler;

        public static string HOME_BUTTON = "btn_home";
		protected override MailType mailType{ get{ return MailType.ALL; } }

		private readonly ShopDialogueController _shopDialogController;

//		private bool _mailDisplayed = false;

        public TutorialMailboxController(ScreenNavigationManager navMgr, IScreenFactory screenFactory, Player player, Inventory inventory, 
			IControllerRepo repo, MasterConfiguration masterConfig, ShopDialogueController shopDialogController)
			: base(navMgr, screenFactory, player, inventory, repo, masterConfig, shopDialogController)
        {
            _isButtonPressed = false;
//            _mailFactory = new MailFactory(_masterConfig, repo.Get<VariableMapper>());
        }

        protected override void InitializeView()
        {
            _screen = _factory.GetScreen<iGUISmartPrefab_MailboxScreen>();
            _screen.Init(_player, this, _masterConfig);

            _tutorialScreen = _factory.GetOverlay<iGUISmartPrefab_TutorialHomeScreenOverlay>();
            _tutorialScreen.OnClickAnywhere += HandleClickAnywhereEvent;
//            _tutorialScreen.HidePointer();
        }

		public IEnumerator DisableAllInput()
		{
			_screen.DisableInputs (true);
			return null;
		}



        public IEnumerator DisplayMail()
        {
			while (!_screen.mailDisplayed)
            {
                yield return null;
            }
        }
			

		public IEnumerator SlideInNarrator()
		{
			return _tutorialScreen.SlideInNarrator ();
		}

        public IEnumerator ShowPointer(MailboxScreen.ButtonType button)
        {
			if (button == MailboxScreen.ButtonType.HOME_BUTTON) 
			{
				_tutorialScreen.ShowPointer (new UnityEngine.Vector2 (0.03f, 1.25f), true);
			} else if (button == MailboxScreen.ButtonType.CLOSE_MAIL) 
			{
				Rect position = GetPositionForCloseBtn();
				Rect baseRect = _tutorialScreen.screenFrame.baseRect;
				Vector2 pos = new Vector2(position.center.x / baseRect.size.x, position.center.y / baseRect.size.y);
				Vector2 offset = new Vector2 (0.08f, 0.05f);
				Debug.LogWarning ("NEW POS: " + pos);
				_tutorialScreen.ShowPointer (pos + offset, true);

			}

            return null;
        }

		private Rect GetPositionForCloseBtn()
		{
			Rect closeBtn = _screen.GetMailCloseBtnBound();
			return closeBtn;
		}

        public IEnumerator WaitForButton(MailboxScreen.ButtonType button)
        {
//            _targetButton = button;
			_inputEnabled = true;
            _isButtonPressed = false;

            while (!_isButtonPressed)
            {
                yield return null;
            }
        }

		public IEnumerator DisableOpenedMailInputs(bool value)
		{
			_screen.DisableOpenedMailInputs (value);
			return null;
		}

        public IEnumerator ShowText(string text, bool waitForInput=true)
        {
            _tutorialScreen.ShowDialogue(text);
            if (waitForInput)
            {
                return WaitForClickAnywhere();
            }
            else
            {
                return null;
            }
        }

        public IEnumerator HideNarrator()
        {
            _tutorialScreen.ShowCharacter(false);
            _tutorialScreen.HideDialogue();
            yield return null;
        }

        private bool _clickedAnywhere = false;
        private void HandleClickAnywhereEvent(object sender, EventArgs eventArgs)
        {
            _clickedAnywhere = true;
        }

        public IEnumerator ShowHighlight(MailboxScreen.ButtonType button)
        {
            yield return new WaitForEndOfFrame();
            if (button == MailboxScreen.ButtonType.HOME_BUTTON) 
			{
				UnityEngine.GameObject homeButton = UnityEngine.MonoBehaviour.Instantiate (_screen._interface.home_button.gameObject) as GameObject;
				homeButton.transform.SetParent (_tutorialScreen.transform);
				_buttonHandler = homeButton.AddComponent<IGUIHandler> ();
				_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
				_isButtonPressed = false;
				iGUIElement homeButtonElement = homeButton.GetComponent<iGUIElement> ();
				iGUIContainer container = homeButton.GetComponent<iGUIContainer> ();
				container.layer = 40;
				iGUIRoot.instance.refresh ();

				MakeClonedButtonInteractive (HOME_BUTTON, homeButtonElement);
			} else if (button == MailboxScreen.ButtonType.CLOSE_MAIL) 
			{
				_tutorialScreen.screenOverlay.setEnabled(false);
				_screen.MailCloseRequest += HandleMailCloseRequest;
			}

//            return null;
        }

        private void MakeClonedButtonInteractive(string buttonName, iGUIElement buttonElement)
        {
            iGUIButton clonedButton = (iGUIButton)_tutorialScreen.GetClonedButton(buttonName);
            if (clonedButton != null)
            {
                _buttonArtMap = new Dictionary<iGUIButton, iGUIElement>() {
                    { clonedButton, buttonElement }
                };

                clonedButton.clickDownCallback += HandleButtonPressEvent;
            }
            else
            {
                throw new WitchesException("failed to clone " + buttonName);
            }
        }

		private void HandleMailCloseRequest(object sender, GUIEventArgs e)
		{
			_tutorialScreen.screenOverlay.setEnabled(true);
			_isButtonPressed = true;
		}


		// HACK: added additional flag to support MakePassive for elements of the tutorial (e.g., duplicate home button)
		private bool _inputEnabled = false;
		public override void MakePassive (bool value)
		{
			base.MakePassive (value);

			_inputEnabled = !value;

		}


        private void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
        {
			if (_inputEnabled) 
			{
				_buttonArtMap [button].colorTo (Color.white, 0.3f);
				_isButtonPressed = true;
			}
        }

        private void HandleButtonPressEvent(iGUIElement element)
        {
            if (_inputEnabled && (_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
            {
                var button = (iGUIButton)element;
                _buttonHandler.SelectButton(button);
                _buttonArtMap[button].colorTo(Color.grey, 0f);
            }
        }



        // TODO: Bake this into show text itself, at least as a flag. This as a standalone is awkward
        private IEnumerator WaitForClickAnywhere()
        {
            _tutorialScreen.EnableTrigger(TutorialHomeScreen.TriggerType.ANYWHERE);
            _clickedAnywhere = false;

            while (!_clickedAnywhere)
            {
                yield return null;
            }

            _tutorialScreen.DisableTrigger(TutorialHomeScreen.TriggerType.ANYWHERE);
        }

        public override void GoHome()
        {
            _isButtonPressed = true;      
        }

		public override void Dispose()
		{
			base.Dispose ();
			_tutorialScreen.Dispose ();
		}
    }
}
