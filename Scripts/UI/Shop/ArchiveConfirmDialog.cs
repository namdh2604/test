using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

namespace Voltage.Witches.Screens.Dialogs
{
    using Voltage.Witches.Models;
    using Voltage.Witches.Bundles;
    using Voltage.Witches.Exceptions;
    
    public class ArchiveConfirmDialog : BaseUGUIScreen, IDialog
    {
        [SerializeField]
        private Button _btnOk;
        [SerializeField]
        private Button _btnCancel;

        [SerializeField]
        private Image _partImage;

        [SerializeField]
        private TextMeshProUGUI _itemLabel;

        private const string BUNDLE = "basic";
        private bool _isInit = false;
        private Sprite _partSprite;
        private IAvatarThumbResourceManager _thumbResourceManager;

        private Action<int> _callback;

        public void MakePassive(bool value)
        {
            _btnOk.interactable = !value;
            _btnCancel.interactable = !value;
        }

        public override void Dispose()
        {
            if (_partSprite != null)
            {
                Destroy(_partSprite);
                _partSprite = null;
            }

            DeregisterButtons();
            base.Dispose();
        }

        public void Init(IClothing clothingItem, IAvatarThumbResourceManager thumbResourceManager)
        {
            _thumbResourceManager = thumbResourceManager;
            StartCoroutine(SetUpImage(clothingItem));
            SetLabel(clothingItem.Name);

            _isInit = true;
        }

        public void Display(Action<int> responseHandler)
        {
            if (!_isInit)
            {
                throw new WitchesException("Dialog not initialized");
            }

            _callback = responseHandler;
            gameObject.SetActive(true);
        }

        private IEnumerator SetUpImage(IClothing clothingItem)
        {
            yield return _thumbResourceManager.LoadBundle(BUNDLE);
            _partSprite = Instantiate<Sprite>(_thumbResourceManager.GetIcon(clothingItem));
            _partImage.sprite = _partSprite;
        }

        private void SetLabel(string name)
        {
            _itemLabel.text = name;
        }

        private void SetupButtons()
        {
            _btnOk.onClick.AddListener(HandleOK);
            _btnCancel.onClick.AddListener(HandleCancel);
        }

        private void DeregisterButtons()
        {
            _btnOk.onClick.RemoveListener(HandleOK);
            _btnCancel.onClick.RemoveListener(HandleCancel);
        }

        private void HandleOK()
        {
            SubmitResponse((int)DialogResponse.OK);
        }

        private void HandleCancel()
        {
            SubmitResponse((int)DialogResponse.Cancel);
        }

        protected virtual void SubmitResponse(int response)
        {
            gameObject.SetActive(false);
            if (_callback != null)
            {
                _callback(response);
            }
            Dispose();
        }

        // should not be used -- eventually remove when interface changes
        protected override Voltage.Witches.Controllers.IScreenController GetController()
        {
            return null;
        }

        #region Unity
        private void Awake()
        {
            SetupButtons();
        }

        private void OnDestroy()
        {
            Dispose();
        }
        #endregion
    }

}