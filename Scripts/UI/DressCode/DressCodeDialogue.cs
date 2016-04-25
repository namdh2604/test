using System;
using System.Collections.Generic;
using System.Collections;


namespace Voltage.Witches.Screens.Dialogues
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	using Voltage.Witches.Models;
	using Voltage.Witches.Bundles;
	using Voltage.Witches.Controllers.DressCode;

	public class DressCodeDialogue : BaseUGUIScreen	// : AbstractDialogUGUI 
	{
		[SerializeField]
		private Button _closeButton;


		// FIXME! following structure from art and iGUISmartPrefab_DressCodeLockDialog
		[SerializeField]
		private Button _resumeButton;
		[SerializeField]
		private Button _buyButton;
		[SerializeField]
		private Button _changeButton;

		[SerializeField]
		private Transform _clearGrp;
		[SerializeField]
		private Transform _failGrp;
		[SerializeField]
		private Transform _buyGrp;
		[SerializeField]
		private Transform _changeGrp;

        [SerializeField] private GameObject _view;

		[SerializeField]
		private Image _avatarImage;

		[SerializeField]
		private TextMeshProUGUI _itemLabel;

		public void MakePassive(bool value)
		{
			_closeButton.interactable = !value;
			_resumeButton.interactable = !value;
			_buyButton.interactable = !value;
			_changeButton.interactable = !value;
		}

		public void EnableCloseButton(bool value)
		{
			_closeButton.gameObject.SetActive (value);
		}

        #region Unity
		private void Awake()
		{
			// TODO: guard clauses
		}

        private void OnDestroy()
        {
            Dispose();
        }
        #endregion

		public override void Dispose()
		{
			if(_clothingSprite != null) 
			{
				Destroy (_clothingSprite);
				_clothingSprite = null;
			}
			
			base.Dispose ();
		}



		private IAvatarThumbResourceManager _thumbResourceManager;
		private const string BUNDLE = "basic";
        private IClothing _clothingItem;
		private Sprite _clothingSprite;
        private IEnumerator _loadRoutine = null;
        private bool _isLoaded = false;

		public void Init(IDressCodeMissionViewModel model, IAvatarThumbResourceManager thumbResourceManager)
		{
			if (model == null || model.DressReq == null || thumbResourceManager == null) 
			{
				throw new ArgumentNullException();
			}

			_thumbResourceManager = thumbResourceManager;

            _isLoaded = false;
            _loadRoutine = null;

            _clothingItem = model.DressReq;
			_itemLabel.text = model.DressReq.Name;

			SetLayout (model.IsWearingItem, model.HasItem);
		}

		private IEnumerator SetUpImage(IClothing clothingItem)
		{
			yield return _thumbResourceManager.LoadBundle(BUNDLE);

			_clothingSprite = Instantiate<Sprite>(_thumbResourceManager.GetIcon(clothingItem));
			_avatarImage.sprite = _clothingSprite;

            _loadRoutine = null;
            _isLoaded = true;
		}


		private void SetLayout(bool isWearingItem, bool hasItem)
		{
			// FIXME direct translation from iGUISmartPrefab_DressCodeLockDialog
			_clearGrp.gameObject.SetActive(isWearingItem);
			_failGrp.gameObject.SetActive(!isWearingItem || !hasItem);
			_changeGrp.gameObject.SetActive(hasItem);
			_buyGrp.gameObject.SetActive(!hasItem);
		}


		public void Display(Action<int> responseHandler)
		{
            StartCoroutine(SetUpView(responseHandler));
		}

        private IEnumerator SetUpView(Action<int> responseHandler)
        {
            if (!IsLoaded())
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
			Action<DressCodeResponse> onClick = (choice) => 
			{
				if(responseHandler != null) 
				{
					responseHandler((int)choice);
				}
				Dispose ();
			};

			_closeButton.onClick.AddListener(() => onClick(DressCodeResponse.CLOSE));
			_resumeButton.onClick.AddListener(() => onClick(DressCodeResponse.RESUME));
			_buyButton.onClick.AddListener(() => onClick(DressCodeResponse.BUY));
			_changeButton.onClick.AddListener(() => onClick(DressCodeResponse.CHANGE));
		}

		private void UnsubscribeButtons()
		{
			_closeButton.onClick.RemoveAllListeners();
			_resumeButton.onClick.RemoveAllListeners();
			_buyButton.onClick.RemoveAllListeners();
			_changeButton.onClick.RemoveAllListeners();
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

        public bool IsLoaded()
        {
            return _isLoaded;
        }

        public override void Show()
        {
            _view.SetActive(true);
        }

        public override void Hide()
        {
            _view.SetActive(false);
        }


	}
}
