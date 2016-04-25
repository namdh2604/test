using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voltage.Witches.UI
{
	using UnityEngine.UI;
	using Voltage.Witches.Screens;


    // FIXME: this probably shouldn't be a view, but a common event handler that all views subscribe to (along with their own controllers)
    // FIXME: and then this class would reside in the screen controller
	public class StoryPlayerUIScreen : BaseUGUIScreen
    {
		
		[SerializeField]
		private Button _infoButton;
		public event Action OnInfoButton;

		[SerializeField]
		private Image _mask;


		public event Action OnUISelected;
       
		private UIRibbonView _ribbon;
		public UIRibbonView RibbonView { get { return _ribbon; } }

		private IList<Button> _ribbonButtons;


        public void Init(UIRibbonView ribbonView)  // or pass in IScreenFactory instead for other potential views??
		{
            if (ribbonView == null)
            {
                throw new ArgumentNullException();
            }

			SetupRibbonView(ribbonView);

            SubscribeButtons();
            // TODO: guard clause that buttons are initialized

            EnableMask (false);
		}

        private void SetupRibbonView(UIRibbonView ribbonView)
		{
            _ribbon = ribbonView;
			_ribbon.transform.SetParent(this.transform);
            _ribbon.transform.SetAsFirstSibling();

            Button[] buttons = _ribbon.GetComponentsInChildren<Button>(true);
            _ribbonButtons = new List<Button>(buttons);
		}
            


		private void SubscribeButtons()		// or explicitly set persistent listener on button!
		{
			Action<Action> onClick = ((action) => 
			{
				if(action != null)
				{
					action();
				}
			});

			_infoButton.onClick.AddListener(() => onClick (OnInfoButton));
            _infoButton.onClick.AddListener(() => onClick (OnUISelected));

            foreach(Button button in _ribbonButtons)
            {
                button.onClick.AddListener(() => onClick (OnUISelected));
            }
		}

        public override void Dispose()
        {
            _infoButton.onClick.RemoveAllListeners();

            if (_ribbon != null)
            {
                foreach (Button button in _ribbonButtons)
                {
                    button.onClick.RemoveAllListeners();
                }

//                _ribbon.Dispose();    // rely on external RibbonController to dispose?
            }

            base.Dispose();
        }



		public void EnableMask(bool value)
		{
			_mask.gameObject.SetActive (value);
		}


		public void MakePassive(bool value)
		{
            if (_ribbon != null)
            {
                _ribbon.MakePassive(value);				// HACK: need to resolve the interface for enabling/disabling ribbon buttons w/ support for passive and disabling while animating
                _ribbon.EnableToggleButtonInput(!value);
                _ribbon.EnableShopButtonInput(!value);
            }
            else
            {
                Voltage.Common.Logging.AmbientLogger.Current.Log("StoryPlayerUIScreen::MakePassive >>> No Ribbon!", Voltage.Common.Logging.LogLevel.WARNING);
            }
               

			MakeInfoButtonPassive (value);
		}


		public void MakeInfoButtonPassive(bool value)
		{
			_infoButton.enabled = !value;
		}





		
		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
            return null;
		}


    }

}


