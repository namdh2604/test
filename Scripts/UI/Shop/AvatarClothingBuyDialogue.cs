using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Screens.Dialogues
{
	using UnityEngine;
	using UnityEngine.UI;
	using Voltage.Witches.Models;
	using TMPro;

	using Voltage.Witches.Bundles;

	public class AvatarClothingBuyDialogue : BaseUGUIScreen, IDialog	// : AbstractDialogUGUI 
	{
		[SerializeField]
		private Button _closeButton;


		// FIXME: retaining general design from art/AvatarPartBuyDialo
		[SerializeField]
		private TextMeshProUGUI _starstoneCounterCenter;
		[SerializeField]
		private TextMeshProUGUI _starstoneCounter;
		[SerializeField]
		private TextMeshProUGUI _coinCounter;

		[SerializeField]
		private Button _starstoneCenterButton;
		[SerializeField]
		private Button _starstoneButton;
		[SerializeField]
		private Button _coinButton;

        [SerializeField] private GameObject _view;


		[SerializeField]
		private Image _avatarImage;

		[SerializeField]
		private TextMeshProUGUI _itemLabel;



		public void MakePassive(bool value)
		{
			_closeButton.interactable = !value;
			_starstoneButton.interactable = !value;
			_starstoneCenterButton.interactable = !value;
			_coinButton.interactable = !value;
		}

		public override void Dispose()
		{
			if(_clothingSprite != null) 
			{
				Destroy (_clothingSprite);
				_clothingSprite = null;
			}

			base.Dispose ();
		}

		private IClothing _clothingItem;
		private int _playerPremiumAmount;
		private int _playerCoinAmount;
		private IAvatarThumbResourceManager _thumbResourceManager;
		private const string BUNDLE = "basic";
		private Sprite _clothingSprite;

        private IEnumerator _loadRoutine = null;
        private bool _isLoaded = false;

		// FIXME: retaining general design from art/AvatarPartBuyDialo
		public void Init(IClothing clothingItem, int playerPremiumAmount, int playerCoinAmount, bool canBuy, IAvatarThumbResourceManager thumbResourceManager)
		{
			_clothingItem = clothingItem;
			_playerPremiumAmount = playerPremiumAmount;
			_playerCoinAmount = playerCoinAmount;
			_thumbResourceManager = thumbResourceManager;

			SetLabel(clothingItem.Name);
			SetupLayout(clothingItem.CurrencyType);
			SetupButtons(playerPremiumAmount, playerCoinAmount, clothingItem, canBuy);

            _isLoaded = false;
            _loadRoutine = null;
		}

		private void SetLabel(string name)
		{
			_itemLabel.text = name;
		}

		private IEnumerator SetUpImage(IClothing clothingItem)
		{
			yield return _thumbResourceManager.LoadBundle(BUNDLE);

			_clothingSprite = Instantiate<Sprite>(_thumbResourceManager.GetIcon(clothingItem));
			_avatarImage.sprite = _clothingSprite;

            _isLoaded = true;
            _loadRoutine = null;
		}

		private void SetupLayout(PURCHASE_TYPE type)
		{
			switch (type) 
			{
				case PURCHASE_TYPE.NONE:
				case PURCHASE_TYPE.BOTH:
					ToggleButtonLayout (true);
					break;
				default:
					ToggleButtonLayout (false);
					break;
			}
		}

		private void ToggleButtonLayout(bool showTwoButtons)
		{
			_starstoneButton.gameObject.SetActive (showTwoButtons);
			_coinButton.gameObject.SetActive (showTwoButtons);

			_starstoneCenterButton.gameObject.SetActive (!showTwoButtons);
		}

		private void SetupButtons(int playerPremium, int playerCoin, IClothing clothingItem, bool canBuy)
		{
			_starstoneCounter.text = clothingItem.PremiumPrice.ToString();
			_starstoneButton.interactable = canBuy;

			_starstoneCounterCenter.text = clothingItem.PremiumPrice.ToString();
			_starstoneCenterButton.interactable = canBuy;

			_coinCounter.text = clothingItem.CoinPrice.ToString();
			_coinButton.interactable = canBuy;
		}


		public void Display(Action<int> responseHandler)
		{
            StartCoroutine(SetUpView(responseHandler));
		}

        private IEnumerator SetUpView(Action<int> responseHandler)
        {
            if (!_isLoaded)
            {
                _loadRoutine = SetUpImage(_clothingItem);
            }

            if (_loadRoutine != null)
            {
                yield return StartCoroutine(_loadRoutine);
            }

            SubscribeButtons(responseHandler);
            Show();
        }

		private void SubscribeButtons(Action<int> responseHandler)		
		{
			Action<PurchaseResponse> onClick = (choice) => 
			{
				if(responseHandler != null) 
				{
					switch(choice)
					{
						case PurchaseResponse.PremiumPurchase:
							if(_playerPremiumAmount < _clothingItem.PremiumPrice)
							{
								choice = PurchaseResponse.Not_Enough;
							}
							break;
						case PurchaseResponse.NormalPurchase:
							if(_playerCoinAmount < _clothingItem.CoinPrice)
							{
								choice = PurchaseResponse.Not_Enough;
							}
							break;
					}

					responseHandler((int)choice);
				}
				Dispose ();
			};

			_closeButton.onClick.AddListener(() => onClick(PurchaseResponse.Close));
			_starstoneButton.onClick.AddListener(() => onClick(PurchaseResponse.PremiumPurchase));
			_starstoneCenterButton.onClick.AddListener(() => onClick(PurchaseResponse.PremiumPurchase));
			_coinButton.onClick.AddListener(() => onClick(PurchaseResponse.NormalPurchase));
		}



		// HACK: to disable individual buttons
		public void MakeButtonPassive(DialogueButton type, bool value)
		{
			switch (type) 
			{
			case DialogueButton.COIN:
				_coinButton.interactable = !value;
				break;
			case DialogueButton.STARSTONE:
				_starstoneButton.interactable = !value;
				_starstoneCenterButton.interactable = !value;
				break;
			default:
				throw new NotImplementedException ();
			}
		}
		public enum DialogueButton
		{
			CLOSE,
			STARSTONE,
			COIN
		}

		private Voltage.Witches.Controllers.IScreenController _controller;

		public void Init(Voltage.Witches.Controllers.IScreenController controller)
		{
			_controller = controller;
		}

		protected override Voltage.Witches.Controllers.IScreenController GetController()
		{
			return _controller;
		}

        public override void Show()
        {
            _view.SetActive(true);
        }

        public override void Hide()
        {
            _view.SetActive(false);
        }

        #region Unity

        private void OnDestroy()
        {
            Dispose();
        }

        #endregion
	}
}
