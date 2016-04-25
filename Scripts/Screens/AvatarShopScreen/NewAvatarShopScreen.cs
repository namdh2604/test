using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Voltage.Witches.Controllers;
using Voltage.Witches.Screens.Closet;
using Voltage.Witches.Models;

using Voltage.Witches.Bundles;


namespace Voltage.Witches.Screens.AvatarShop
{
    public class ShopScreenEventArgs : EventArgs
    {
        public string Type { get; set; }
    }

    public class ClothingPurchaseRequestArgs : ShopScreenEventArgs
    {
        public Clothing Item { get; set; }
    }

    public class NewAvatarShopScreen : BaseUGUIScreen
    {
        #region UNITY_FIELDS
        [SerializeField]
        private ShopView _shopView;

        [SerializeField]
        private AvatarView _avatarView;

        [SerializeField]
        private Button _closetButton;
        [SerializeField]
        private Button _homeButton;
        [SerializeField]
        private Button _btnZoomIn;
        [SerializeField]
        private Button _btnZoomOut;
        #endregion

        public EventHandler OnAction;

        private bool _isZoomed = false;

        private AvatarShopScreenController _controller;

        private void Start()
        {
            _shopView.OnAction += OnAction;
            _isZoomed = false;
            UpdateZoomButtons();
        }

        private void OnDestroy()
        {
            _shopView.OnAction -= OnAction;

//			if (_bundleManager != null) 
//			{
//				_bundleManager.UnloadBundle("basic");
//			}
        }

		private IAvatarThumbResourceManager _bundleManager;

		public void Init(AvatarShopScreenController controller, Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> items, ScreenClothingCategory activeCategory, Action onLoad, IAvatarThumbResourceManager bundleManager)
        {
            _controller = controller;
			_bundleManager = bundleManager;

			StartCoroutine(InitializeShopView(items, activeCategory, onLoad));
        }

		private IEnumerator InitializeShopView(Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> items, ScreenClothingCategory activeCategory, Action onLoad)
        {
            // Load all the asset bundle
			yield return _bundleManager.LoadBundle("basic");

            // initialize the shop view
			_shopView.Init(items, _bundleManager, activeCategory);

            if (onLoad != null)
            {
                onLoad();
            }
        }

        public void MakePassive(bool value)
        {
            _shopView.MakePassive(value);
            _btnZoomIn.interactable = !value;
            _btnZoomOut.interactable = !value;
            _closetButton.interactable = !value;
            _homeButton.interactable = !value;
        }

        protected override IScreenController GetController()
        {
            return _controller;
        }

        private void TriggerEvent(string eventname)
        {
            if (OnAction != null)
            {
                OnAction(this, new ShopScreenEventArgs { Type = eventname });
            }
        }

        public void RefreshItemOwnership()
        {
            _shopView.RefreshItemOwnership();
        }

        public void HandleHome()
        {
			if (_bundleManager != null) 
			{
				_bundleManager.UnloadBundle("basic");
			}

            TriggerEvent("Home");
        }

        public void HandleCloset()
        {
            TriggerEvent("Closet");
        }

        public void HandleZoom()
        {
            StartCoroutine(ZoomRoutine());
        }

        private IEnumerator ZoomRoutine()
        {
            MakePassive(true);

            Coroutine avatarRoutine = _avatarView.ToggleZoom();
            if (avatarRoutine != null)
            {
                yield return avatarRoutine;
            }

            _isZoomed = !_isZoomed;
            UpdateZoomButtons();

            MakePassive(false);
        }

        private void UpdateZoomButtons()
        {
            if (_isZoomed)
            {
                _btnZoomIn.gameObject.SetActive(false);
                _btnZoomOut.gameObject.SetActive(true);
            }
            else
            {
                _btnZoomIn.gameObject.SetActive(true);
                _btnZoomOut.gameObject.SetActive(false);
            }
        }
    }
}

