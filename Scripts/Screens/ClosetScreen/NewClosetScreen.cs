using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Voltage.Witches.Models;
using Voltage.Witches.Controllers;

// TODO: Get rid of this. Unrelated dependency which should eventually be moved into the closet controller as a factory or a persistent dependency(?)
using Voltage.Witches.Bundles;
using Voltage.Witches.Models.Avatar;

namespace Voltage.Witches.Screens.Closet
{
    public class ClosetScreenEventArgs : EventArgs
    {
        public string Type { get; set; }
    }

    public class ClothingChangeEventArgs : ClosetScreenEventArgs
    {
        public Clothing item { get; set; }
    }

    public class ClothingArchivalEventArgs : ClosetScreenEventArgs
    {
        public Clothing item { get; set; }
    }

    public class ClosetEmptyEventArgs : ClosetScreenEventArgs
    {
        public ScreenClothingCategory Category { get; set; }
    }

	public class NewClosetScreen : BaseUGUIScreen
	{
		// HACK need to investigate why hooking into screen.OnAction isn't working on ClosetView.OnAction
		public ClosetView ClosetView { get { return _closetView; } }

        #region UNITY_FIELDS
        [SerializeField]
        private ClosetView _closetView;

        [SerializeField]
        private AvatarView _avatarView;

        [SerializeField]
        private Button _btnZoomIn;
        [SerializeField]
        private Button _btnZoomOut;
        [SerializeField]
        private Button _btnUndress;
        [SerializeField]
        private Button _btnSaveOutfit;
        [SerializeField]
        private Button _btnCloset;
        [SerializeField]
        private Button _btnHome;


        #endregion

        public EventHandler OnAction;

		private IAvatarResourceManager _avatarResourceManager;
		private AvatarGenerator _generator;

        private NewClosetScreenController _controller;
        private bool _isZoomed = false;

		private void Start()
		{
            _closetView.OnAction += OnAction;
            _isZoomed = false;
            UpdateZoomButtons();
		}

		private void OnDestroy()
		{
            _closetView.OnAction -= OnAction;

			if (_generator != null)
			{
				Destroy(_generator.gameObject);
			}

//			if (_bundleManager != null) 
//			{
//				_bundleManager.UnloadBundle("basic");
//			}
		}

		public void Init(NewClosetScreenController controller, Dictionary<ScreenClothingCategory, List<Clothing>> items, Action onLoad, IAvatarThumbResourceManager bundleManager)
        {
			if(bundleManager == null) 
			{
				throw new ArgumentNullException();
			}

            _controller = controller;
			_bundleManager = bundleManager;

            #if UNITY_EDITOR
            _avatarResourceManager = new EditorAvatarResourceManager();
            #else
            AssetBundleManager assetManager = UnityEngine.Object.FindObjectOfType<AssetBundleManager>();
            _avatarResourceManager = new AvatarResourceManager(assetManager);
            #endif

            GameObject go = Instantiate(Resources.Load("AvatarGenerator")) as GameObject;
            _generator = go.GetComponent<AvatarGenerator>();
            _generator.Init(_avatarResourceManager);

            StartCoroutine(InitializeClosetView(items, onLoad));
        }

        private IAvatarThumbResourceManager _bundleManager;

        private IEnumerator InitializeClosetView(Dictionary<ScreenClothingCategory, List<Clothing>> items, Action onLoad)
        {
			yield return _bundleManager.LoadBundle("basic");

			_closetView.Init(items, _bundleManager);

            if (onLoad != null)
            {
                onLoad();
            }
        }

		// TODO: Remove this -- temporary to give the controller access to the generator
		public AvatarGenerator GetAvatarGenerator()
		{
			return _generator;
		}

        public void SetItems(Dictionary<ScreenClothingCategory, List<Clothing>> items)
		{
            _closetView.SetItems(items);
		}

		public void SetClosetSpace(int currentItems, int totalSpace)
		{
            _closetView.SetClosetSpace(currentItems, totalSpace);
		}

		public void MakePassive(bool value)
		{
            _closetView.MakePassive(value);
            _btnZoomIn.interactable = !value;
            _btnZoomOut.interactable = !value;
            _btnUndress.interactable = !value;
            _btnSaveOutfit.interactable = !value;
            _btnCloset.interactable = !value;
            _btnHome.interactable = !value;
		}

		public void RefreshAvatar(Action onComplete)
		{
            _avatarView.UpdateTexture();

            // Need to scroll out before we're done -- if shoes were changed, for example, the user wouldn't realize
            if (!_isZoomed)
            {
                onComplete();
            }
            else
            {
                StartCoroutine(RefreshAvatarRoutine(onComplete));
            }
		}

        private IEnumerator RefreshAvatarRoutine(Action onComplete)
        {
            yield return StartCoroutine(ZoomRoutine());
            onComplete();
        }

		public void DisplayAvatarLoadingIndicator(bool value)
		{
            _avatarView.ShowLoadingIndicator(value);
		}

        protected override IScreenController GetController()
        {
            return _controller;
        }


        public void HandleHome()
        {
			if (_bundleManager != null) 
			{
				_bundleManager.UnloadBundle("basic");
			}

            TriggerEvent("Home");
        }

        public void HandleUndress()
        {
            TriggerEvent("Undress");
        }

        public void HandleShop()
        {
            TriggerEvent("Shop");
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

        private void TriggerEvent(string eventname)
        {
            if (OnAction != null)
            {
                OnAction(this, new ClosetScreenEventArgs { Type = eventname });
            }
        }
	}
}

