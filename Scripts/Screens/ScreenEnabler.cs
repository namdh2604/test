using UnityEngine;
using iGUI;

namespace Voltage.Witches.Screens
{
    using Voltage.Witches.Controllers;

    // handles enabling or disabling all input across all screens
    public class ScreenEnabler
    {
        private ScreenNavigationManager _navManager;

        public ScreenEnabler(ScreenNavigationManager navManager)
        {
            _navManager = navManager;
        }

        public void EnableInput(bool value)
        {
            // HACK: to disable input for any screen
            // uGUI: Quit dialogues uGUI click-blocker prevents input
            // iGUI: is prevented by static IGUIHandler::DisableAll, the Quit dialogue is set to IGUIHandler::OverideDisableAll
            // StoryPlayer: UI input is prevented by click-blocker, storyplayer input is disabled by static DialogueNodeDisplay::InputEnabled
            // Chapter Clear: dialogue's mail button is not tied into screens IGUIHandler, so its being set with a static ChapterClearDialog::MailButtonPassive
            // uGUI Scrollviews: prevented by click-blocker
            // iGUI Scrollviews: not tied into IGUIHandler, finding all instances and setting them to passive
            IGUIHandler.DisableAll = !value;
            Voltage.Witches.Layout.DialogueNodeDisplay.InputEnabled = value;
            ChapterClearDialog.MailButtonPassive = !value;

            MakePassiveAnyActiveScrollViews(!value);

            // HACK: handle Minigame and Minigame's pause dialogue
            IScreenController controller = _navManager.CurrentController;
            if (controller != null) 
            {
                if (controller is TracingScreenController) 
                {
                    if (!((TracingScreenController)controller).ShowingPauseDialogue) 
                    {
                        controller.MakePassive(!value);
                    }
                }
            }

            // HACK: handle textfield for name tutorial
            var nameDialogue = GameObject.FindObjectOfType<NameRegistrationDialog>();
            if (nameDialogue != null) 
            {
                nameDialogue.MakePassive(!value);
            }
        }

        private void MakePassiveAnyActiveScrollViews(bool value)
        {
            var scrollviews = GameObject.FindObjectsOfType<iGUIScrollView>();
            foreach (var scrollview in scrollviews) 
            {
                scrollview.passive = value;
            }
        }

    }
}

