using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Screens
{
	public class ChapterRewardDialog : AbstractDialog
	{
		[HideInInspector]
		public iGUIButton btn_ok,btn_popup_close;

		[HideInInspector]
		public iGUIImage avatar_parts_MA;

		[HideInInspector]
		public iGUILabel item_name;

		public Item ReceivedItem { get; protected set; }

		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;
		IGUIHandler _buttonHandler;

		public void SetItem(Item receivedItem)
		{
			//TODO Assign Item to display here
			ReceivedItem = receivedItem;
		}

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected void Start()
		{
			btn_ok.clickDownCallback += ClickInit;
			btn_popup_close.clickDownCallback += ClickInit;

			_buttonArtMap = new Dictionary<iGUIButton, iGUIElement> ()
			{
				{btn_ok,btn_ok.getTargetContainer()},
				{btn_popup_close,btn_popup_close}
			};

			UpdateLabel();
			UpdateIcon();
		}

		void UpdateLabel ()
		{
			if((ReceivedItem as Clothing) != null)
			{
				item_name.label.text = (ReceivedItem as Clothing).Name;
			}
		}

		void UpdateIcon ()
		{
			if((ReceivedItem as Clothing) != null)
			{
				avatar_parts_MA.image = Resources.Load<Texture2D>((ReceivedItem as Clothing).IconFilePath);
				avatar_parts_MA.scaleMode = ScaleMode.ScaleToFit;
			}
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				_buttonArtMap[button].colorTo(Color.grey,0f);
			}
		}

		void HandleMovedBack(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.grey,0f);
		}

		void HandleMovedAway(iGUIButton button)
		{
			_buttonArtMap[button].colorTo(Color.white,0.3f);
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == btn_ok)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
				else if(button == btn_popup_close)
				{
					SubmitResponse((int)DialogResponse.Cancel);
				}
			}

            if (this != null)
            {
    			_buttonArtMap[button].colorTo(Color.white,0.3f);
            }
		}

        // FIXME -- Move this into Abstract Dialog when we are comfortable incorporating this change (and the null check it necessitates)
        // into all iGUI dialogs
        protected override void SubmitResponse(int response, bool autoClose=true)
        {
            base.SubmitResponse(response, false);

            if (autoClose)
            {
                (screenFrame.container as iGUIContainer).removeElement(screenFrame);
            }
        }
	}
}