using UnityEngine;
using iGUI;

namespace Voltage.Witches.Screens
{
    public class ErrorDialog : AbstractDialog
    {
        [HideInInspector]
        public iGUIButton btn_support;
        [HideInInspector]
        public iGUIButton btn_refresh;

        [HideInInspector]
        public iGUILabel lbl_userID;

        [HideInInspector]
        public iGUILabel lbl_errorMsg;

        public IGUIHandler _buttonHandler;

        private void Awake()
        {
            _buttonHandler = gameObject.AddComponent<IGUIHandler>();
            _buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
            _buttonHandler.MovedAway += HandleMovedAway;
            _buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
        }

        private void Start()
        {
            btn_support.clickDownCallback += ClickInit;
            btn_refresh.clickDownCallback += ClickInit;
        }

        public void Init(string userID, string errorMsg)
        {
            // Hide the user ID field for new users/users without user ids
            if (string.IsNullOrEmpty(userID))
            {
                lbl_userID.setEnabled(false);
            }
            else
            {
                lbl_userID.label.text = "USER ID: " + userID;
                lbl_userID.setEnabled(true);
            }

            lbl_errorMsg.label.text = errorMsg;
        }

        private void ClickInit(iGUIElement element)
        {
            if ((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
            {
                var button = (iGUIButton)element;
                _buttonHandler.SelectButton(button);
                button.colorTo(Color.grey, 0f);
            }
        }

        private void HandleMovedBack(iGUIButton pressedButton)
        {
            pressedButton.colorTo(Color.grey, 0f);
        }

        private void HandleMovedAway(iGUIButton pressedButton)
        {
            pressedButton.colorTo(Color.white, 0.3f);
        }

        private void HandleReleasedButtonEvent(iGUIButton pressedButton, bool isOverButton)
        {
            if (isOverButton)
            {
                if (pressedButton == btn_support)
                {
                    SubmitResponse((int)ErrorDialogResponse.Support, false);
                }
                else if (pressedButton == btn_refresh)
                {
                    SubmitResponse((int)ErrorDialogResponse.Reset, false);
                }
            }

            pressedButton.colorTo(Color.white, 0.3f);
        }
    }

    public enum ErrorDialogResponse
    {
        Support = 1,
        Reset = 2
    }
}

