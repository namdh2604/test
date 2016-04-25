using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Voltage.Witches.Exceptions;
using Voltage.Witches.Bundles;
using Voltage.Witches.Models;

namespace Voltage.Witches.Screens.Closet
{

	// Represents a closet item button in the closet and shop screen
	public class ClosetItem : MonoBehaviour
	{
		// HACK: tutorial (for now) needs to search scene for specific ClosetItem
		public string ID { get; private set; }

		private const string IMAGE_ROOT = "Avatar/Icons/";

        private IAvatarThumbResourceManager _bundleManager;

        public void MakePassive(bool value)
        {
            _itemButton.interactable = !value;
            _archiveButton.interactable = !value;
        }


		public void SetItem(Clothing clothing, Sprite sprite)
		{
			ID = clothing.Id;

			if (_texture != null)
			{
				_image.sprite = null;
			}

//            _texture = EditorAvatarThumbResourceManager.GetIcon(clothing);
			_texture = Instantiate<Sprite>(sprite);

			_image.sprite = _texture;

		}

        private string GetIconPath(Clothing clothing)
        {
            return IMAGE_ROOT + clothing.Layer_Name;
        }

		public void DisplayDeleteButton(bool value)
		{
			_archiveButton.gameObject.SetActive(value);
		}


		public event UnityAction onClick;
		public event UnityAction onArchive;

		private Sprite _texture;

		#region Unity
		[SerializeField]
		private Button _itemButton;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private Button _archiveButton;

		private void Awake()
		{
			if ((_itemButton == null) || (_image == null) || (_archiveButton == null))
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

			if (onArchive != null)
			{
				_archiveButton.onClick.AddListener(onArchive);
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
			_archiveButton.onClick.RemoveAllListeners();
		}
		#endregion
	}
}