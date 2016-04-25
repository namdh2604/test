using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Events;
using Voltage.Witches.Controllers;
using Voltage.Witches.Models;
using Voltage.Witches.Views;
using Voltage.Witches.Util;

namespace Voltage.Witches.Screens
{
	using Voltage.Witches.Configuration;
	using Debug = UnityEngine.Debug;

	public class MailboxScreen : BaseScreen
	{
		Player _player;

		[HideInInspector]
		public Placeholder interfaceBarPlaceholder,settingsPlaceholder,envelopePlaceholder_0,envelopePlaceholder_1,envelopePlaceholder_2,envelopePlaceholder_3,envelopePlaceholder_4;

		[HideInInspector]
		public iGUIContainer lettersContainer,mailbox;

		[HideInInspector]
		public iGUIButton drawer_up_button,drawer_down_button;

		[HideInInspector]
		public iGUIImage drawer_front;

		private List<iGUIButton> _pressableButtons;
		public iGUISmartPrefab_InterfaceShell _interface;
		iGUISmartPrefab_SettingsShell _settings;
		List<iGUISmartPrefab_EnvelopeView> _envelopeViews;
		List<iGUIElement> _envelopes;
		
//		string _openEnvelopeButton = "open_envelope_button";

		Dictionary<iGUIButton,iGUIElement> _buttonArtMap;

		MailboxScreenController _controller;
		private CircularArray<KeyValuePair<Mail,int>> _mail;
		iGUISmartPrefab_OpenMailDialog _mailDialog;
		IGUIHandler _buttonHandler;

        private bool _initialMailDisplayed = false;

		public event GUIEventHandler MailCloseRequest;

		private enum ItemType
		{
			INGREDIENT = 0,
			POTION = 1,
			AVATAR = 2,
			CURRENCY = 3,
			EI = 4,
			NONE = 5
		}

		private ItemType _receivedItemType = ItemType.NONE;

		public Rect GetMailCloseBtnBound()
		{
			return _mailDialog.btn_close.getAbsoluteRect();
		}
	
		protected virtual void Awake()
		{
			_envelopeViews = new List<iGUISmartPrefab_EnvelopeView> ();
			_mail = new CircularArray<KeyValuePair<Mail, int>>();

			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;			
		}

		public void Init(Player player, MailboxScreenController controller, MasterConfiguration masterConfig)
		{
			_player = player;
			_controller = controller;
		}

		void Start()
		{
			LoadInterfaceBar();
			LoadSettingsBar();
			LoadEnvelopes();
			SetButtonArtMap();
			AssignButtonCallbacks();
			StartCoroutine(WaitToRefresh());

            // set the flag so that we only pop up a message once
            _initialMailDisplayed = false;
		}

		public void DisableInputs(bool value)
		{
			MakePassive (value);
			drawer_up_button.passive = value;
			drawer_down_button.passive = value;
		}

		public void DisableOpenedMailInputs(bool value)
		{
			_mailDialog.btn_close.passive = value;
			_mailDialog.scrollView.passive = value;
		}


		//HACK There was a problem returning from MiniGame that made the text disappear
		IEnumerator WaitToRefresh()
		{
			iGUIRoot.instance.setEnabled(false);
			yield return new WaitForEndOfFrame();
			iGUIRoot.instance.setEnabled(true);
		}

		void LoadEnvelopes ()
		{
			_envelopeViews = new List<iGUISmartPrefab_EnvelopeView>();
			_envelopes = new List<iGUIElement>();
			_mail = new CircularArray<KeyValuePair<Mail, int>>();

			var envelopes = lettersContainer.items;
			_pressableButtons = new List<iGUIButton>();
			for(int i = 0; i < envelopes.Length; ++i)
			{
				if((_controller != null) && (i < _controller.MailCount))
				{
					if((envelopes[i] as Placeholder) != null)
					{
						iGUIElement element = ((Placeholder)envelopes[i]).SwapForSmartObject();
						var view = element.GetComponent<iGUISmartPrefab_EnvelopeView>();
						view.OpenMailRequest += HandleOnMailClick;
						_envelopeViews.Add(view);
						_envelopes.Add(element);
						if(!_pressableButtons.Contains(view.open_envelope_button))
						{
							_pressableButtons.Add(view.open_envelope_button);
						}
						element.setLayer(element.layer - i);
						Mail indexMail = _controller.GetMailFromIndex(i);
						if(i == 0)
						{
							view.SetEnvelope(indexMail,i,true);
						}
						else
						{
							view.SetEnvelope(indexMail,i,false);
						}
					}
				}
				else
				{
					if((envelopes[i] as Placeholder) != null)
					{
						iGUIElement element = ((Placeholder)envelopes[i]).SwapForSmartObject();
						var view = element.GetComponent<iGUISmartPrefab_EnvelopeView>();
						view.OpenMailRequest += HandleOnMailClick;
						_envelopeViews.Add(view);
						_envelopes.Add(element);
						if(!_pressableButtons.Contains(view.open_envelope_button))
						{
							_pressableButtons.Add(view.open_envelope_button);
						}
						element.setLayer(element.layer - i);
						element.setEnabled(false);
					}
				}
			}

			if(_controller != null)
			{
				var mailBox = _controller.GetMailbox();
				for(int i = 0; i < mailBox.Count; ++i)
				{
					KeyValuePair<Mail, int> currentPair = new KeyValuePair<Mail, int>(mailBox[i],i);
					_mail.Add(currentPair);
				}
			}

		}
		
		void LoadInterfaceBar()
		{
			_interface = LoadPlaceholder(true).GetComponent<iGUISmartPrefab_InterfaceShell>();
			_interface.SetLayout(InterfaceLayout.Standard);
			if(_player != null)
			{
				_interface.SetCounts(_player);
				_interface.BeginCountDown(CountDownType.FOCUS, _player.FocusNextUpdate);
				_interface.BeginCountDown(CountDownType.STAMINA, _player.StaminaNextUpdate);
			}
		}

		iGUIElement LoadPlaceholder(bool isInterface)
		{
			iGUIElement element = null;
			if(isInterface)
			{
				element = interfaceBarPlaceholder.SwapForSmartObject() as iGUIContainer;
			}
			else
			{
				element = settingsPlaceholder.SwapForSmartObject() as iGUIContainer;
			}
			element.setLayer(15);
			return element;
		}

		void LoadSettingsBar()
		{
			_settings = LoadPlaceholder(false).GetComponent<iGUISmartPrefab_SettingsShell>();
			_settings.SetLayout(SettingsShellLayout.MAIL);
		}

		void SetButtonArtMap()
		{
			_buttonArtMap = new Dictionary<iGUIButton,iGUIElement>()
			{
				{drawer_up_button,drawer_up_button},
				{drawer_down_button,drawer_down_button},
				{_interface.arrow_down_hitbox,_interface.arrow_down},
				{_interface.arrow_up_hitbox,_interface.arrow_up},
				{_interface.rib_add_starstones_hitbox,_interface.rib_add_starstones_hitbox.getTargetContainer()},
				{_interface.btn_home,_interface.home},
				{_settings.btn_all,_settings.btn_all},
				{_settings.btn_chara,_settings.btn_chara},
				{_settings.btn_read,_settings.btn_read},
				{_settings.btn_system,_settings.btn_system},
				{_settings.btn_unread,_settings.btn_unread}
			};
		}

		public void UpdateInterfaceBar()
		{
			_interface.PremiumCount = _player.CurrencyPremium;
			_interface.CurrencyCount = _player.Currency;
		}

		void AssignButtonCallbacks()
		{
			var pressableButtons = new List<iGUIButton> ()
			{
				_settings.btn_all,_settings.btn_chara,_settings.btn_system,_settings.btn_unread,_settings.btn_read,drawer_down_button,drawer_up_button,_interface.arrow_down_hitbox,_interface.arrow_up_hitbox,
				_interface.rib_add_starstones_hitbox,_interface.btn_home
			};

			pressableButtons.AddRange(_pressableButtons);

			for(int i = 0; i < pressableButtons.Count; ++i)
			{
				var current = pressableButtons[i];
				current.clickDownCallback += ClickInit;
			}
		}

		public void DisplayMail()
		{

			UpdateMail();

            if (!_initialMailDisplayed)
            {
                _initialMailDisplayed = true;
                OpenMostRecentMail();
            }
		}

		protected override IScreenController GetController()
		{
			return _controller;
		}

        private void OpenMostRecentMail()
        {
            if (_controller.MailCount <= 0)
            {
                return;
            }

            // use the front envelope
            OpenMail(_envelopeViews[0]);
        }

        void HandleOnMailClick(object sender, GUIEventArgs e)
        {
            OpenMail(sender as EnvelopeView);
        }

		public bool mailDisplayed;
        private void OpenMail(EnvelopeView envelopeView)
        {
            if (!envelopeView.IsOpened)
            {
                UpdateEnvelopeState(envelopeView.Mail, envelopeView);
            }

            IDialog dialog = _controller.OpenMail(envelopeView.Mail);
            _mailDialog = dialog as iGUISmartPrefab_OpenMailDialog;
            _mailDialog.OnItemReceived += HandleItemReceived;
            _buttonHandler.Deactivate();
            dialog.Display(HandleMailClosedResponse);
			mailDisplayed = true;
        }


		void HandleMailClosedResponse(int answer)
		{
			_mailDialog.OpenedMail.hasBeenRead();
			var itemsList = _mailDialog.OpenedMail.Attachments;
			var receivedList = _mailDialog.OpenedMail.ReceivedAttachements;

			for(int i = 0; i < receivedList.Count; ++i)
			{
				var currentState = receivedList[i];
				var currentItem = itemsList[i];

				var isPremium = ((currentItem as StarStoneItem) != null);
				var isFreeCurrency = ((currentItem as CoinItem) != null);
				var isStamina = (((currentItem as Potion) != null) && ((currentItem as Potion).PotionCategory == PotionCategory.STAMINA));

				if(currentState)
				{
					var mailID = _mailDialog.OpenedMail.Id;
					var itemID = currentItem.Id;

					if((isPremium) || (isFreeCurrency) || (isStamina))
					{
						if(isPremium)
						{
							_controller.PickUpNonItemFromMail(mailID,"premium");
						}
						else if(isFreeCurrency)
						{
							_controller.PickUpNonItemFromMail(mailID,"free");
						}
						else if(isStamina)
						{
							_controller.PickUpNonItemFromMail(mailID,"stamina_potion");
						}
					}
					else
					{
						_controller.PickUpGiftsFromMail(mailID, itemID);
					}
				}
			}
			_buttonHandler.Activate();
			if(MailCloseRequest != null)
			{
				MailCloseRequest(this, new GUIEventArgs());
			}
		}

		void HandleItemReceived(object sender, GUIEventArgs e)
		{
			//TODO Add check to confirm if the player has space in their closet for closet items
			var mail = (sender as iGUISmartPrefab_OpenMailDialog).OpenedMail;
			ItemReceivedArgs ItemArgs = e as ItemReceivedArgs;
			var indexItem = mail.Attachments.IndexOf(ItemArgs.ReceivedItem);
			SetReceivedType(ItemArgs.ReceivedItem.Category);
			int? itemCount = null;
			if((_receivedItemType == ItemType.POTION) && ((ItemArgs.ReceivedItem as Potion).PotionCategory == PotionCategory.STAMINA))
			{
				itemCount = mail.Stamina_Count;
			}
			if((_receivedItemType != ItemType.AVATAR) || ((_receivedItemType == ItemType.AVATAR) && (_controller.HasEnoughSpace)))
			{
				_controller.AddItem(ItemArgs.ReceivedItem,itemCount);
				mail.ReceivedAttachements[indexItem] = true;
				Debug.Log(ItemArgs.ReceivedItem.Name + "  was received!");
				_mailDialog.ToggleHandling(false);
				var dialog = _controller.GetItemReceivedDialog(ItemArgs.ReceivedItem);
				dialog.Display(HandleItemReceivedResponse);
			}
			else
			{
				_controller.ExecuteNotEnoughSpace();
			}
		}

		void SetReceivedType(ItemCategory category)
		{
			switch(category)
			{
			case ItemCategory.CLOTHING:
				_receivedItemType = ItemType.AVATAR;
				break;
			case ItemCategory.COINS:
				_receivedItemType = ItemType.CURRENCY;
				break;
			case ItemCategory.ILLUSTRATION:
				_receivedItemType = ItemType.EI;
				break;
			case ItemCategory.INGREDIENT:
				_receivedItemType = ItemType.INGREDIENT;
				break;
			case ItemCategory.OUTFIT:
				_receivedItemType = ItemType.AVATAR;
				break;
			case ItemCategory.POTION:
				_receivedItemType = ItemType.POTION;
				break;
			case ItemCategory.STARSTONES:
				_receivedItemType = ItemType.CURRENCY;
				break;
			}
		}

		void HandleItemReceivedResponse(int answer)
		{
			_mailDialog.UpdateItems();

			switch((ItemReceivedResponse)answer)
			{
				case ItemReceivedResponse.CLOSE:
					break;
				case ItemReceivedResponse.GO_TO_CLOSET:
					_mailDialog.CloseMail();
					_controller.GoToCloset();
					break;
				case ItemReceivedResponse.GO_TO_GLOSSARY:
					_mailDialog.CloseMail();
					_controller.GoToGlossary();
					break;
				case ItemReceivedResponse.GO_TO_INVENTORY:
					_mailDialog.CloseMail();
					_controller.GoToInventory((int)_receivedItemType);
					break;
			}
			_mailDialog.ToggleHandling(true);
			_receivedItemType = ItemType.NONE;
		}

		void UpdateEnvelopeState(Mail requestedMail, object sender)
		{
			var envelope = sender as iGUISmartPrefab_EnvelopeView;
//			requestedMail.hasBeenRead();
			envelope.envelope_sealed.setEnabled(false);
		}
			
		public void HandleErrorMessageClosed(int answer)
		{
			ToggleScreenElements(true);
		}

		public void HandleNotEnoughStarStonesResponse(int answer)
		{
			switch((DialogResponse)answer)
			{
			case DialogResponse.Cancel:
				ToggleScreenElements(true);
				break;
			case DialogResponse.OK:
				_controller.ShowCurrencyPurchaseDialog();
				break;
			}
		}

		public void ToggleScreenElements(bool value)
		{
			if(value)
			{
				if(_mailDialog == null)
				{
					_buttonHandler.Activate();
				}
				else
				{
					_mailDialog.ToggleHandling(value);
				}
			}
			else
			{
				if(_mailDialog == null)
				{
					_buttonHandler.Deactivate();
				}
				else
				{
					_mailDialog.ToggleHandling(value);
				}
			}
		}

		void HandleCloseIAPDialog(int answer)
		{
			ToggleScreenElements(true);
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{				
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);

				if(_pressableButtons.Contains(button))
				{
					button.getTargetContainer().colorTo(Color.grey,0f);
				}
				else
				{
					_buttonArtMap[button].colorTo(Color.grey,0f);
				}
			}
		}

		void HandleMovedAway(iGUIButton button)
		{
			if(_pressableButtons.Contains(button))
			{
				button.getTargetContainer().colorTo(Color.white,0.3f);
			}
			else
			{
				_buttonArtMap[button].colorTo(Color.white,0.3f);
			}
		}

		void HandleMovedBack(iGUIButton button)
		{
			if(_pressableButtons.Contains(button))
			{
				button.getTargetContainer().colorTo(Color.grey,0f);
			}
			else
			{
				_buttonArtMap[button].colorTo(Color.grey,0f);
			}
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{			
			if(isOverButton)
			{
				if(_pressableButtons.Contains(button))
				{
					var index = _pressableButtons.IndexOf(button);
					_envelopeViews[index].Open_Envelope();
				}
				else
				{
					if(button == _interface.btn_home)
					{
						_controller.GoHome();
					}
					else if(button == _interface.arrow_up_hitbox)
					{
						StartCoroutine(_interface.UpdateRibbonPosition (Voltage.Witches.Views.InterfaceShellView.RibbonState.CLOSED));
					}
					else if(button == _interface.arrow_down_hitbox)
					{
						StartCoroutine (_interface.UpdateRibbonPosition(Voltage.Witches.Views.InterfaceShellView.RibbonState.OPEN));
					}
					else if(button == _interface.rib_add_starstones_hitbox)
					{
						_buttonHandler.Deactivate();
						_controller.ShowCurrencyPurchaseDialog();
					}
					else if(button == _settings.btn_all)
					{
						Debug.Log("Show all mail regardless of type");
						_controller.FilterMail(MailboxScreenController.MailType.ALL);
					}
					else if(button == _settings.btn_chara)
					{
						Debug.Log("Organize mail by Character");
						_controller.FilterMail(MailboxScreenController.MailType.CHARACTER);
					}
					else if(button == _settings.btn_read)
					{
						Debug.Log("Show only read mail");
						_controller.FilterMail(MailboxScreenController.MailType.READ);
					}
					else if(button == _settings.btn_system)
					{
						Debug.Log("Organize mail by System");
						_controller.FilterMail(MailboxScreenController.MailType.SYSTEM);
					}
					else if(button == _settings.btn_unread)
					{
						Debug.Log("Show only unread mail");
						_controller.FilterMail(MailboxScreenController.MailType.UNREAD);
					}
					else if(button == drawer_down_button)
					{
						Debug.Log("Change index of mail going down");
						_mail.RotateLeft(1);
						UpdateEnvelopes();
					}
					else if(button == drawer_up_button)
					{
						Debug.Log("Change Index of mail going up");
						_mail.RotateRight(1);
						UpdateEnvelopes();
					}
				}
			}

			if(_pressableButtons.Contains(button))
			{
				button.getTargetContainer().colorTo(Color.white,0.3f);
			}
			else
			{
				_buttonArtMap[button].colorTo(Color.white,0.3f);
			}
		}

		public void UpdateMail()
		{
			_mail = new CircularArray<KeyValuePair<Mail, int>>();
			var mailBox = _controller.GetMailbox();
			for(int i = 0; i < mailBox.Count; ++i)
			{
				KeyValuePair<Mail, int> currentPair = new KeyValuePair<Mail, int>(mailBox[i],i);
				_mail.Add(currentPair);
			}
			UpdateEnvelopes();
		}

		void UpdateEnvelopes()
		{
			for (int i = 0; i < _envelopeViews.Count; ++i)
			{
				var view = _envelopeViews[i];
				if(view == null)
				{
					LoadEnvelopes();
					return;
				}

				if ((i < _mail.Count) && (i == 0))
				{
					if(!view.GetComponent<iGUIElement>().enabled)
					{
						view.GetComponent<iGUIElement>().setEnabled(true);
					}

					view.SetEnvelope(_mail[i].Key, _mail[i].Value,true);
					view.RefreshView();
				}
				else if((i < _mail.Count) && (i > 0))
				{
					if(!view.GetComponent<iGUIElement>().enabled)
					{
						view.GetComponent<iGUIElement>().setEnabled(true);
					}

					view.SetEnvelope(_mail[i].Key, _mail[i].Value,false);
					view.RefreshView();
				}
				else
				{
					view.GetComponent<iGUIElement>().setEnabled(false);
				}
			}
		}

		public void SetEnabled(bool value)
		{
			screenFrame.setEnabled(value);
			gameObject.SetActive(value);
		}

        public enum ButtonType
        {
            HOME_BUTTON,
			CLOSE_MAIL
		}

	}
}
