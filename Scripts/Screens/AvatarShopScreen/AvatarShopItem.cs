using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Voltage.Witches.Exceptions;
using Voltage.Witches.Bundles;
using Voltage.Witches.Models;

using TMPro;

using Voltage.Witches.Controllers;

namespace Voltage.Witches.Screens.AvatarShop
{

	// Represents a closet item button in the closet and shop screen
	public class AvatarShopItem : MonoBehaviour
	{
		private const string IMAGE_ROOT = "Avatar/Icons/";

		public string ID { get { return _clothing.Clothing.Id; } }
        private IAvatarThumbResourceManager _bundleManager;

        public void MakePassive(bool value)
        {
            _itemButton.interactable = !value;
        }

		public void SetItem(AvatarShopItemViewModel clothing, Sprite sprite)
		{
			_clothing = clothing;

			if (_texture != null)
			{
				_image.sprite = null;
			}

//            _texture = EditorAvatarThumbResourceManager.GetIcon(clothing.Clothing);
			_texture = Instantiate<Sprite>(sprite);

			_image.sprite = _texture;

            _premiumPrice.text = clothing.Clothing.PremiumPrice.ToString();

            if (clothing.Clothing.CurrencyType == PURCHASE_TYPE.BOTH || clothing.Clothing.CurrencyType == PURCHASE_TYPE.COIN)
            {
                _coinContainer.SetActive(true);
            }
            else
            {
                _coinContainer.SetActive(false);
            }

            RefreshOwnership();
		}

        public void RefreshOwnership()
        {
            // Only can buy items that aren't already owned
            // NOTE -- this distinction can be too subtle because it only dims the actual image.
            // It may be better to also "dim" the background
            if (_clothing.Owned)
            {
                _image.color = Color.grey;
                _itemButton.enabled = false;
            }
            else
            {
                _image.color = Color.white;
                _itemButton.enabled = true;
            }
        }

        private string GetIconPath(Clothing clothing)
        {
            return IMAGE_ROOT + clothing.Layer_Name;
        }


		public event UnityAction onClick;

		private Sprite _texture;
        private AvatarShopItemViewModel _clothing;

		#region Unity
        [SerializeField]
        private TextMeshProUGUI _premiumPrice;
        [SerializeField]
        private GameObject _coinContainer;

		[SerializeField]
		private Button _itemButton;

		[SerializeField]
		private Image _image;

		private void Awake()
		{
            if ((_itemButton == null) || (_image == null))
			{
				throw new WitchesException("Missing Fields");
			}
		}

		private void Start()
		{

			if (onClick != null)
			{
				_itemButton.onClick.AddListener(onClick);
			}
		}

		private void OnDestroy()
		{
			if (_texture != null)
			{
                Destroy(_texture);
                _texture = null;
			}

			_itemButton.onClick.RemoveAllListeners();
		}
		#endregion
	}
}