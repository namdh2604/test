using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;

namespace Voltage.Witches.Controllers
{
	using Debug = UnityEngine.Debug;
	using URLs = Voltage.Witches.Net.URLs;

	using Voltage.Story.Variables;

	using Voltage.Common.Logging;
	using Voltage.Common.Net;
	using Voltage.Witches.Net;
	using Voltage.Witches.Shop;

	using Voltage.Witches.Configuration;
	using Voltage.Witches.Controllers.Factories;

	using Voltage.Witches.Metrics;
	using Voltage.Common.Metrics;

	public class MailboxScreenController : ScreenController
	{
		protected IScreenFactory _factory;
		protected Player _player;
		private readonly Inventory _inventory;
		private IControllerRepo _repo;
		protected iGUISmartPrefab_MailboxScreen _screen;
		private Dictionary<int,Mail> _firstMailBox;
		private Dictionary<int, Mail> _totalMailBox;
		private List<Mail> _indexedOrderAllMail;
		private Dictionary<int,Mail> _mailBox;
		private List<Mail> _mailIndexedOrder;
		private MailFactory _mailFactory;
		protected MasterConfiguration _masterConfig;

		public ILogger Logger { get; private set; }

		private INetworkTimeoutController<WitchesRequestResponse> _networkController;

		public int MailCount { get { return _mailIndexedOrder.Count; } }

		public enum MailType
		{
			UNREAD = 0,
			READ = 1,
			CHARACTER = 2,
			SYSTEM = 3,
			ALL = 4
		}

		public ShopController ShopController;
		private ShopItemData _iapItem;
		private IDialog _completingPurchase;
		private readonly ShopDialogueController _shopDialogController;

		protected virtual MailType mailType{ get{ return MailType.UNREAD; } }

		public bool HasEnoughSpace { get { return (_player.AvailableClosetSpace > 0); } }

		public MailboxScreenController (ScreenNavigationManager controller, IScreenFactory factory, Player player, Inventory inventory, 
			IControllerRepo repo, MasterConfiguration masterConfig, ShopDialogueController shopDialogController) : base(controller)
		{
			if (masterConfig == null || repo == null) {
				throw new ArgumentNullException ("MailboxScreenController::Ctor >>> ");
			}
			
			_factory = factory;
			_player = player;
			_inventory = inventory;
			_repo = repo;

			ShopController = _repo.Get<ShopController> ();

			_masterConfig = masterConfig;
			_networkController = repo.Get<INetworkTimeoutController<WitchesRequestResponse>> ();
			Logger = repo.Get<ILogger> ();

			CreateMailBox ();
			_mailFactory = new MailFactory (_masterConfig, _repo.Get<VariableMapper> ());
			_shopDialogController = shopDialogController;
			GetMyMailBox ();			
			GetAllMail ();
			UpdateAvailableSpace ();
			InitializeView ();
		}

		public void FilterMail(MailType mailType)
		{
			GetMailSet (mailType);
			_screen.UpdateMail ();
		}

		void UpdateAvailableSpace ()
		{
			var allItems = _inventory.GetAllItemsByCategory (ItemCategory.CLOTHING);
			int itemCount = 0;
			for (int i = 0; i < allItems.Count; ++i) {
				itemCount += _inventory.GetCount (allItems [i]);
			}

			_player.AvailableClosetSpace = (_player.ClosetSpace - itemCount);
		}

		public void GetMyMailBox ()
		{
			var userID = _player.UserID;
			if (string.IsNullOrEmpty (userID)) {
				userID = PlayerPrefs.GetString ("phone_id");
			}

			Dictionary<string,string> parameters = new Dictionary<string, string> ()
			{
				{"phone_id", userID}
			};
			_networkController.Send (URLs.GET_MAIL, parameters, GetMailSuccess, GetMailFailed);	
		}

        protected virtual void GetMailSuccess (WWWNetworkPayload response)
		{
			var mail = _mailFactory.Create (response.Text);
            SortMail(mail);
			AssignMail (mail);
			GetMailSet(mailType);
			_screen.DisplayMail ();

			SendAllNewMailReceivedMetric (mail);	// messages!
		}

        protected virtual void GetMailFailed (WWWNetworkPayload response)
		{
			Logger.Log ("Failed to get mail!", LogLevel.WARNING);
		}

        private void SortMail(List<Mail> items)
        {
            items.Sort((item1, item2) => item2.Timestamp.CompareTo(item1.Timestamp));
        }

		public virtual void AssignMail (List<Mail> items)
		{
			if ((_totalMailBox == null) && (_indexedOrderAllMail == null)) {
				_totalMailBox = new Dictionary<int, Mail> ();
				_indexedOrderAllMail = new List<Mail> ();
			}

			_mailBox = new Dictionary<int, Mail> ();
			_mailIndexedOrder = new List<Mail> ();

			for (int i = 0; i < items.Count; ++i) {
				var currentMail = items [i];
				if (!MailboxHasItem (currentMail)) {
					_totalMailBox [i] = currentMail;
					_indexedOrderAllMail.Add (_totalMailBox [i]);
				}
			}
		}

		bool MailboxHasItem (Mail mail)
		{
			for (int i = 0; i < _indexedOrderAllMail.Count; ++i) {
				var current = _indexedOrderAllMail [i];
				if (mail.Id == current.Id) {
					return true;
				}
			}

			return false;
		}

		void CreateMailBox ()
		{
			_totalMailBox = new Dictionary<int, Mail> ();
			_indexedOrderAllMail = new List<Mail> ();
						
			_mailBox = new Dictionary<int, Mail> ();
			_mailIndexedOrder = new List<Mail> ();
		}

		public void GetAllMail ()
		{
			_mailBox = _totalMailBox;
			_mailIndexedOrder = _indexedOrderAllMail;
		}

		void SetCurrentMailBox (MailType mailType)
		{
			Dictionary<int,Mail> mailSet = new Dictionary<int, Mail> ();
			List<Mail> mailIndex = new List<Mail> ();
			for (int i = 0; i < _indexedOrderAllMail.Count; ++i) {
				var mail = _indexedOrderAllMail [i];
				if ((!mail.isRead ()) && (mailType == MailType.UNREAD)) {
					mailSet [mailSet.Count] = mail;
					mailIndex.Add (mail);
				} else if ((mail.isRead ()) && (mailType == MailType.READ)) {
					mailSet [mailSet.Count] = mail;
					mailIndex.Add (mail);
				} else if ((mail.isCharacterMail ()) && (mailType == MailType.CHARACTER)) {
					mailSet [mailSet.Count] = mail;
					mailIndex.Add (mail);
				} else if ((mail.isSystemMail ()) && (mailType == MailType.SYSTEM)) {
					mailSet [mailSet.Count] = mail;
					mailIndex.Add (mail);
				} else if (mailType == MailType.ALL) {
					mailSet [mailSet.Count] = mail;
					mailIndex.Add (mail);
				}
			}
			_mailBox = mailSet;
			_mailIndexedOrder = mailIndex;
		}

        public override void Dispose()
        {
            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
        }

		protected override IScreen GetScreen ()
		{
			if (_screen != null) {
				return _screen;
			} else {
				_screen = _factory.GetScreen<iGUISmartPrefab_MailboxScreen> ();
				_screen.Init (_player, this, _masterConfig);
				return _screen;
			}
		}

		public override void MakePassive (bool value)
		{			
			_screen.ToggleScreenElements(!value);
		}
		
		protected virtual void InitializeView ()
		{
			_screen = _factory.GetScreen<iGUISmartPrefab_MailboxScreen> ();
			_screen.Init (_player, this, _masterConfig);
		}

		public Dictionary<int,Mail> GetMailbox ()
		{
			return _mailBox;
		}

		public void PickUpGiftsFromMail (string mailID, string itemID)
		{
			AmbientLogger.Current.Log ("Pickup Gifts API is DEPRECATED, remove call and code", LogLevel.WARNING);		// temporary, all call/code should be removed

//			string userID = _player.UserID;
//			if (string.IsNullOrEmpty (userID)) {
//				userID = PlayerPrefs.GetString ("phone_id");
//			}
//
//			Dictionary<string,string> parameters = new Dictionary<string,string> ()
//			{
//				{"phone_id",userID},
//				{"mail_id",mailID},
//				{"gifts",itemID}
//			};
//			
//			_networkController.Send (URLs.GET_ITEM_FROM_MAIL, parameters, PickUpGiftSuccess, PickUpGiftFailed);
		}

		public void PickUpNonItemFromMail (string mailID, string parameterKey)
		{
			AmbientLogger.Current.Log ("Pickup Gifts API is DEPRECATED, remove call and code", LogLevel.WARNING);		// temporary, all call/code should be removed

//			string userID = _player.UserID;
//			if (string.IsNullOrEmpty (userID)) {
//				userID = PlayerPrefs.GetString ("phone_id");
//			}
//			
//			Dictionary<string,string> parameters = new Dictionary<string,string> ()
//			{
//				{"phone_id",userID},
//				{"mail_id",mailID},
//				{parameterKey,"received"}
//			};
//			
//			_networkController.Send (URLs.GET_ITEM_FROM_MAIL, parameters, PickUpGiftSuccess, PickUpGiftFailed);
		}

		private void PickUpGiftSuccess (WWWNetworkPayload response)
		{
			UnityEngine.Debug.LogWarning ("Successfully received the item");
		}

		private void PickUpGiftFailed (WWWNetworkPayload response)
		{
			UnityEngine.Debug.LogWarning ("Picking up item was unsuccessful");
		}

		private void OpenMailItem (string mailID)
		{
			string userID = _player.UserID;
			if (string.IsNullOrEmpty (userID)) {
				userID = PlayerPrefs.GetString ("phone_id");
			}

			Dictionary<string,string> parameters = new Dictionary<string,string> ()
			{
				{"phone_id",userID},
				{"mail_id",mailID}
			};
			
			_networkController.Send (URLs.OPEN_MAIL, parameters, OpenMailSuccess, OpenMailFailed);	// TODO: Do these callbacks do anything in the context of use??? 
		}

		private void OpenMailSuccess (WWWNetworkPayload response)
		{
			Debug.LogWarning ("Mail opened successfully");
		}

		private void OpenMailFailed (WWWNetworkPayload response)
		{
			Debug.LogWarning ("Mail was NOT opened");
		}

		protected void GetMailSet (MailType mailType)
		{
			switch ((MailType)mailType) {
			case MailType.UNREAD:
				Debug.Log ("Get un-read");
				SetCurrentMailBox ((MailType)mailType);
				break;
			case MailType.READ:
				Debug.Log ("Get All Read mail");
				SetCurrentMailBox ((MailType)mailType);
				break;
			case MailType.CHARACTER:
				Debug.Log ("Get all character mails");
				SetCurrentMailBox ((MailType)mailType);
				break;
			case MailType.SYSTEM:
				Debug.Log ("Get All System Mail");
				SetCurrentMailBox ((MailType)mailType);
				break;
			case MailType.ALL:
				Debug.Log ("Get EVERYTHING");
				SetCurrentMailBox ((MailType)mailType);
				break;
			}
		}

		public Mail GetMailFromIndex (int index)
		{
			return _mailBox [index];
		}

		public IDialog OpenMail (Mail selectedMail)
		{
            // TODO: Add in out of space avatar handling
            // for every attachment that hasn't been received, attempt to receive that item
            for (int i = 0; i < selectedMail.Attachments.Count; ++i)
            {
                var item = selectedMail.Attachments[i];

                // TODO: This is temp fix until we serialize the Received attachment, or goto a true server/cleint model.  This code is to prevent 
                // user from exploiting the system should the server is not responding.  We now check to see if the mail is read, if it has been read
                // we don't add items.  Checking to see if the mail attachment has been received or not require a server sync.
                // We will need to fix this when we deal with clothing item that can't be recieved due to no space.
                selectedMail.ReceivedAttachements[i] = true;
                bool alreadyReceived = selectedMail.isRead();
                if (!alreadyReceived)
                {
                    int? itemCount = null;
                    if ((item.Category == ItemCategory.POTION) && ((item as Potion).PotionCategory == PotionCategory.STAMINA))
                    {
                        itemCount = selectedMail.Stamina_Count;
                    }

                    AddItem(item, itemCount);

                }
            }
			SendMailMetric (MetricEvent.MAIL_OPENED, selectedMail);	// or put in OpenMailItem()???

			var dialog = _factory.GetDialog<iGUISmartPrefab_OpenMailDialog> ();
            List<MailAttachment> attachments = ConstructAttachments(selectedMail);
            dialog.AssignMail(selectedMail, attachments);
			OpenMailItem(selectedMail.Id);
			return dialog;
		}

		public void ShowCurrencyPurchaseDialog()
		{
			_shopDialogController.Show (ShopDisplayType.STARSTONES, OnFinishTransaction);
		}

		public void OnFinishTransaction()
		{
			_screen.UpdateInterfaceBar();
			MakePassive(false);
		}

		public IDialog GetPrePurchaseLoadingDialog ()
		{
			return _factory.GetDialog<iGUISmartPrefab_ConfirmPurchaseLoadDialog> ();
		}
		
		public IDialog GetPostPurchaseLoadingDialog ()
		{
			return _factory.GetDialog<iGUISmartPrefab_ConfirmCompleteLoadDialog> ();
		}
		
		public IDialog GetSystemDialog (string message)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_SystemPopupDialog> ();
			dialog.SetText (message);
			return dialog;
		}

		void HandleSystemDialogResponse (int answer)
		{
			UnityEngine.Debug.Log ("Close system dialog");
		}

		public void ExecuteNotEnoughSpace ()
		{
			var dialog = GetInsufficientSpaceDialog ();
			dialog.Display (HandleNotEnoughSpaceResponse);
			_screen.ToggleScreenElements (false);
		}

		public IDialog GetInsufficientSpaceDialog ()
		{
			return _factory.GetDialog<iGUISmartPrefab_InsufficientSpaceDialog> ();
		}

		void HandleNotEnoughSpaceResponse (int answer)
		{
			switch ((DialogResponse)answer) {
			case DialogResponse.Cancel:
				_screen.ToggleScreenElements (true);
				break;
			case DialogResponse.OK:
				var dialog = GetExpandClosetDialog ();
				dialog.Display (HandleExpandClosetResponse);
				break;
			}
		}

		public void PurchaseClosetSpace ()
		{
			ShopController.SpendPremium (MasterConfiguration.ADDITIONAL_CLOSET_SLOTS_ITEM_ID, CompleteClosetSpacePurchase);
		}

		void CompleteClosetSpacePurchase (bool isSuccessful)
		{
			if (isSuccessful) {
				_player.AddClosetSpace ();
				_player.UpdatePremiumCurrency (-1);
				_screen.UpdateInterfaceBar ();
				
				IDialog dialog = GetExpansionFinishedDialog ();
				dialog.Display (_screen.HandleErrorMessageClosed);
				
				SendExpandSpaceMetric ();
			} else {
				HandlePurchaseFailure ();
			}
		}

		void HandlePurchaseFailure ()
		{
			var dialog = GetSystemDialog ("Sorry, there was an error in your purchase");
			dialog.Display (HandleSystemDialogResponse);
		}

		void HandleExpandClosetResponse (int answer)
		{
			switch ((ExpandClosetResponse)answer) {
			case ExpandClosetResponse.Buy:
				PurchaseClosetSpace ();
				break;
			case ExpandClosetResponse.Not_Enough:
				HandleGetMoreStarStonesDialog ();
				break;
			case ExpandClosetResponse.Cancel:
				_screen.ToggleScreenElements (true);
				break;
			}
		}
		
		public void HandleGetMoreStarStonesDialog ()
		{
			var dialog = GetNotEnoughStarstonesDialog ();
			dialog.Display (_screen.HandleNotEnoughStarStonesResponse);
		}

		public IDialog GetExpansionFinishedDialog ()
		{
			return _factory.GetDialog<iGUISmartPrefab_ExpandFinishedDialog> ();
		}
		
		public IDialog GetNotEnoughStarstonesDialog ()
		{
			return _factory.GetDialog<iGUISmartPrefab_NotEnoughStonesDialog> ();
		}
		
		public IDialog GetExpandClosetDialog ()
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_ExpandClosetDialog> ();
			dialog.HasCurrency ((_player.CurrencyPremium > 0));
			SendDisplayDialogueExpandSpaceMetric ();
			return dialog;
		}

		public IDialog GetItemReceivedDialog (Item item)
		{
			var dialog = _factory.GetDialog<iGUISmartPrefab_ItemReceivedDialog> ();
			dialog.SetItem (item);
			return dialog;
		}

		public void GoToGlossary ()
		{
			Debug.LogWarning ("Glossary Screen Feature is not enabled right now");
//			IScreenController nextScreen = new GlossaryScreenController(Manager, _factory, _player);
//			Manager.Add (nextScreen);
		}

		public void GoToCloset ()
		{
			IScreenController nextScreen = _repo.Get<NewClosetScreenController> ();
			Manager.Add (nextScreen);
		}

		public void GoToInventory (int inventoryLayout)
		{
			IInventoryScreenControllerFactory factory = _repo.Get<IInventoryScreenControllerFactory> ();
			IScreenController nextScreen = factory.Create (inventoryLayout);
			Manager.Add (nextScreen);
		}

		public virtual void GoHome ()
		{
			Manager.GoToExistingScreen ("/Home");
		}
		
		public void SetEnabled (bool value)
		{
			_screen.SetEnabled (value);
		}
		
		public void Unload ()
		{
			_screen.SetEnabled (false);
		}

        private List<MailAttachment> ConstructAttachments(Mail mail)
        {
            List<MailAttachment> attachments = new List<MailAttachment>();

            for (int i = 0; i < mail.Attachments.Count; ++i)
            {
                int count = 1;
                var item = mail.Attachments[i];
                switch (item.Category)
                {
                    case ItemCategory.COINS:
                        count = (item as CoinItem).Count;
                        break;
                    case ItemCategory.STARSTONES:
                        count = (item as StarStoneItem).Count;
                        break;
                    default:
                        if (IsNotStaminaPotion(item))
                        {
                            count = 1;
                        }
                        else 
                        {
                            count = mail.Stamina_Count.Value;
                        }
                        break;
                }

                attachments.Add(new MailAttachment(item, count, mail.ReceivedAttachements[i]));
            }

            return attachments;
        }

		public void AddItem (Item item, int? count)
		{
			if (!IsInventoryItem (item.Category)) {
				if (item.Category == ItemCategory.COINS) {
					var coins = (item as CoinItem);
					_player.UpdateCurrency (coins.Count);
					_screen.UpdateInterfaceBar ();
				} else if (item.Category == ItemCategory.STARSTONES) {
					var stones = (item as StarStoneItem);
					_player.UpdatePremiumCurrency (stones.Count);
					_screen.UpdateInterfaceBar ();
				} else {
					//TODO EI shit here
				}
			} else {
				if (IsNotStaminaPotion (item)) {
					_inventory.Add (item, 1);
					if (item.Category == ItemCategory.CLOTHING) {
						UpdateAvailableSpace ();
					}
				} else {
					_player.UpdateStaminaPotion (count.Value);
				}
			}
		}

		bool IsNotStaminaPotion (Item item)
		{
			if ((item as Potion) == null) {
				return true;
			} else if (((item as Potion) != null) && ((item as Potion).PotionCategory != PotionCategory.STAMINA)) {
				return true;
			} else {
				return false;
			}
		}

		bool IsInventoryItem (ItemCategory category)
		{
			return ((category == ItemCategory.CLOTHING) || (category == ItemCategory.INGREDIENT) || (category == ItemCategory.OUTFIT) || (category == ItemCategory.POTION));
		}

		private void SendDisplayDialogueExpandSpaceMetric ()	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"expand_cost", 1}	// value looks hardcoded, make sure it stays in sync
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.CLOSET_DISPLAY_DIALOGUE_EXPAND_SPACE, data);
		}

		private void SendExpandSpaceMetric ()	// TODO: eventually move out to its own class
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"starstones_paid", 1}	// value looks hardcoded, make sure it stays in sync
			};
			
			AmbientMetricManager.Current.LogEvent (MetricEvent.CLOSET_EXPAND_SPACE, data);
		}

		private void SendAllNewMailReceivedMetric (IEnumerable<Mail> messages)
		{
			foreach (Mail message in messages) {
				if (!message.isRead ()) {
					SendMailMetric (MetricEvent.MAIL_RECIEVED_NEW, message);
				}
			}
		}

		private void SendMailMetric (string eventName, Mail mail) 	// bool opened=true)
		{
			IDictionary<string,object> data = new Dictionary<string,object> 
			{
				{"mail_id", mail.Id},
				{"mail_type", mail.Type},
			};
			
			AmbientMetricManager.Current.LogEvent (eventName, data);
		}
	}

    public struct MailAttachment
    {
        public Item item;
        public int count;
        public bool isClaimed;

        public MailAttachment(Item item, int count, bool isClaimed)
        {
            this.item = item;
            this.count = count;
            this.isClaimed = isClaimed;
        }
    }
}
 