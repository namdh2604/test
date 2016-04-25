using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Voltage.Witches.Models;
using Voltage.Witches.Bundles;

namespace Voltage.Witches.Screens.Closet
{
	public enum ClosetShelfButton
	{
		Archive,
		Filter,
		ClosetSpace,
		Sort
	};

	// Handles all closet scroll view interactions
	public class ClosetView : MonoBehaviour
	{
		#region Unity
		[SerializeField]
		private Button _filterButton;
		[SerializeField]
		private Button _sortButton;
		[SerializeField]
		private Button _archiveButton;
		[SerializeField]
		private Button _closetSpaceButton;

		[SerializeField]
		private CategorySubscreen _categoryScreen;

		[SerializeField]
		private GameObject _columnPrefab;
		[SerializeField]
		private GameObject _itemPrefab;
		[SerializeField]
		private GameObject _dividerPrefab;
		[SerializeField]
		private RectTransform _content;

        [SerializeField]
        private TextMeshProUGUI _spaceText;

        [SerializeField]
        private ScrollRect _scrollRect;

		[SerializeField]
		private TextMeshProUGUI _categoryLabel;

		#endregion

        public event EventHandler OnAction;

		private Dictionary<ScreenClothingCategory, string> _categoryDisplayNames = new Dictionary<ScreenClothingCategory, string>() {
			{ ScreenClothingCategory.All, "CATEGORY" },
			{ ScreenClothingCategory.Accessories, "ACCESSORIES" },
			{ ScreenClothingCategory.Bottoms, "BOTTOMS" },
			{ ScreenClothingCategory.Dresses, "DRESSES" },
			{ ScreenClothingCategory.Hair, "HAIRSTYLES" },
			{ ScreenClothingCategory.Hats, "HATS" },
			{ ScreenClothingCategory.Intimates, "INTIMATES" },
			{ ScreenClothingCategory.Jackets, "JACKETS & COATS" },
			{ ScreenClothingCategory.Outfits, "OUTFITS" },
			{ ScreenClothingCategory.Shoes, "SHOES" },
			{ ScreenClothingCategory.Skin, "SKIN" },
			{ ScreenClothingCategory.Tops, "TOPS" }
		};

		private const int ITEMS_PER_COLUMN = 3;

		private bool _archiveButtonsVisible = false;
        private bool _isPassive = false;

		private Dictionary<ScreenClothingCategory, List<Clothing>> _items;
        private ScreenClothingCategory _activeCategory = ScreenClothingCategory.None;
		private List<ClosetItem> _activeItems;

        public void MakePassive(bool value)
        {
            _filterButton.interactable = !value;
            _archiveButton.interactable = !value;
            _sortButton.interactable = !value;
            _closetSpaceButton.interactable = !value;

            _scrollRect.enabled = !value;

            if (_activeItems != null)
            {
                foreach (var item in _activeItems)
                {
                    item.MakePassive(value);
                }
            }

            _categoryScreen.MakePassive(value);

            _isPassive = value;
        }

		private IAvatarThumbResourceManager _thumbResourceManager;

		public void Init(Dictionary<ScreenClothingCategory, List<Clothing>> items, IAvatarThumbResourceManager thumbResourceManager)
        {
            _activeCategory = ScreenClothingCategory.All;
			_thumbResourceManager = thumbResourceManager;
            SetItems(items);
        }

        public void SetItems(Dictionary<ScreenClothingCategory, List<Clothing>> items)
        {
            _items = items;
            PopulateCategory(_activeCategory, true);
        }

        public void SetClosetSpace(int numOwnedItems, int totalSpace)
        {
            _spaceText.text = String.Format("{0}/{1}", numOwnedItems, totalSpace);
        }

		private void PopulateCategory(ScreenClothingCategory category, bool forceRefresh=false)
		{
            if ((!forceRefresh) && (_activeCategory == category))
			{
				return;
			}

            _scrollRect.horizontalNormalizedPosition = 0.0f;

            // check to ensure that the user actually has any items in this category -- if not, fire off the event
            List<Clothing> items = _items[category];
            if (items.Count == 0)
            {
                HandleEmptyCategory(category);
                //return;
            }

			_activeItems = new List<ClosetItem>();

			_categoryLabel.text = _categoryDisplayNames[category];

			// unload the previous category
			foreach (Transform child in _content)
			{
				GameObject.Destroy(child.gameObject);
			}


            int numColumns = (int)(Math.Ceiling(items.Count / 3.0));
			// enforce a minimum of 3 columns -- that is how many fit on the screen
			if (numColumns < 3)
			{
				numColumns = 3;
			}
			float totalWidth = numColumns * (COLUMN_WIDTH + DIVIDER_WIDTH) + DIVIDER_WIDTH;

			_content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);

			for (int i = 0; i < numColumns; ++i)
			{
				CreateDivider();
				GameObject column = CreateColumn(i + 1);

				for (int j = 0; j < ITEMS_PER_COLUMN; ++j)
				{
					int itemIndex = i * ITEMS_PER_COLUMN + j;
					if (itemIndex < items.Count)
					{
                        Clothing clothing = items[itemIndex];
						ClosetItem closetItem = CreateItem(clothing, column.transform);
                        closetItem.DisplayDeleteButton(_archiveButtonsVisible);
                        closetItem.MakePassive(_isPassive);
                        closetItem.onClick += (() => HandleClosetItem(clothing));
                        closetItem.onArchive += (() => HandleClothingArchival(clothing));
						_activeItems.Add(closetItem);
					}
				}
			}

			CreateDivider();
			_activeCategory = category;
        }

        private void HandleClosetItem(Clothing clothing)
        {
            if (_archiveButtonsVisible)
            {
                // this is an archival request
                HandleClothingArchival(clothing);
            }
            else
            {
                // this is a request to change clothing
                if (OnAction != null)
                {
                    OnAction(this, new ClothingChangeEventArgs { item = clothing, Type = "Clothing" });
                }
            }
        }

        private void HandleClothingArchival(Clothing clothing)
        {
            if (OnAction != null)
            {
                OnAction(this, new ClothingArchivalEventArgs { item = clothing, Type = "Archival" });
            }
        }

        private void HandleClosetExpansion()
        {
            if (OnAction != null)
            {
                OnAction(this, new ClosetScreenEventArgs { Type = "ClosetExpansion" });
            }
        }

        private void HandleEmptyCategory(ScreenClothingCategory category)
        {
            if (OnAction != null)
            {
                OnAction(this, new ClosetEmptyEventArgs { Type = "EmptyCategory", Category = category });
            }
        }

		private GameObject CreateDivider()
		{
			GameObject divider = Instantiate(_dividerPrefab);
			divider.name = "Divider";
			divider.transform.SetParent(_content, false);

			return divider;
		}

		private GameObject CreateColumn(int index)
		{
			GameObject column = Instantiate(_columnPrefab);
			column.name = "Column" + index;
			column.transform.SetParent(_content, false);

			return column;
		}

		private ClosetItem CreateItem(Clothing clothing, Transform column)
		{
			GameObject item = Instantiate(_itemPrefab);
			ClosetItem closetItem = item.GetComponent<ClosetItem>();
			Sprite itemSprite = _thumbResourceManager.GetIcon(clothing);
			closetItem.SetItem(clothing, itemSprite);

			item.name = "Item";
			item.transform.SetParent(column, false);

			return closetItem;
		}

		private float COLUMN_WIDTH;
		private float DIVIDER_WIDTH;

		private void Awake()
		{
			COLUMN_WIDTH = _itemPrefab.GetComponent<LayoutElement>().minWidth;
			DIVIDER_WIDTH = _dividerPrefab.GetComponent<LayoutElement>().minWidth;
		}

		private void Start()
		{
			_categoryScreen.onCategorySelected += HandleCategory;
			_categoryScreen.onClose += (() => HandleCategory(ScreenClothingCategory.None));
			_archiveButton.onClick.AddListener(() => HandleShelfButton(ClosetShelfButton.Archive));
			_filterButton.onClick.AddListener(() => HandleShelfButton(ClosetShelfButton.Filter));
			_closetSpaceButton.onClick.AddListener(() => HandleShelfButton(ClosetShelfButton.ClosetSpace));
		}

		private void HandleCategory(ScreenClothingCategory category)
		{
			_categoryScreen.gameObject.SetActive(false);

			if (category != ScreenClothingCategory.None)
			{
				PopulateCategory(category);
			}

		}

		private void ToggleArchival()
		{
			_archiveButtonsVisible = !_archiveButtonsVisible;

            if (_activeItems == null)
            {
                return;
            }

            foreach (var item in _activeItems)
			{
				item.DisplayDeleteButton(_archiveButtonsVisible);
			}
		}

		private void HandleShelfButton(ClosetShelfButton button)
		{
			switch (button)
			{
			case ClosetShelfButton.Archive:
				ToggleArchival();
				break;
			case ClosetShelfButton.Filter:
				// toggle the category view
				_categoryScreen.gameObject.SetActive(true);
				break;
			case ClosetShelfButton.ClosetSpace:
                HandleClosetExpansion();
				break;
			case ClosetShelfButton.Sort:
				Debug.LogWarning("Sorting: not supported yet");
				break;
			}
		}
	}
}

