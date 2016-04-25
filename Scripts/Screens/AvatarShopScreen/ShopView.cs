using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Voltage.Witches.Models;

using Voltage.Witches.Screens.Closet;
using Voltage.Witches.Controllers;
using Voltage.Witches.Bundles;


namespace Voltage.Witches.Screens.AvatarShop
{
	public enum ShopButton
	{
		Filter
	};

	// Handles all closet scroll view interactions
	public class ShopView : MonoBehaviour
	{
		#region Unity
        [SerializeField]
        private ShopCategorySubScreen _categoryScreen;
        [SerializeField]
        private Button _filterButton;

		[SerializeField]
		private GameObject _columnPrefab;
		[SerializeField]
		private GameObject _itemPrefab;
		[SerializeField]
		private GameObject _dividerPrefab;
		[SerializeField]
		private RectTransform _content;

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

        private Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> _items;
        private ScreenClothingCategory _activeCategory = ScreenClothingCategory.None;
		private List<AvatarShopItem> _activeItems;
		private IAvatarThumbResourceManager _thumbResourceManager;

        public void MakePassive(bool value)
        {
            _scrollRect.enabled = !value;

            foreach (var item in _activeItems)
            {
                item.MakePassive(value);
            }

            _categoryScreen.MakePassive(value);
            _filterButton.interactable = !value;
        }

		public void Init(Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> items, IAvatarThumbResourceManager thumbResourceManager, ScreenClothingCategory activeCategory)
        {
			if(thumbResourceManager == null) 
			{
				throw new ArgumentException();
			}

			_activeCategory = activeCategory;
			_thumbResourceManager = thumbResourceManager;

            SetItems(items);
        }

        public void SetItems(Dictionary<ScreenClothingCategory, List<AvatarShopItemViewModel>> items)
        {
            _items = items;
            PopulateCategory(_activeCategory, true);
        }

        public void RefreshItemOwnership()
        {
            foreach (var itemDisplay in _activeItems)
            {
                itemDisplay.RefreshOwnership();
            }
        }

		private void PopulateCategory(ScreenClothingCategory category, bool forceRefresh=false)
		{
            if ((!forceRefresh) && (_activeCategory == category))
			{
				return;
			}

            _scrollRect.horizontalNormalizedPosition = 0.0f;

            // check to ensure that the user actually has any items in this category -- if not, fire off the event
            List<AvatarShopItemViewModel> items = _items[category];

			_activeItems = new List<AvatarShopItem>();

			_categoryLabel.text = _categoryDisplayNames[category];

			// unload the previous category
			foreach (Transform child in _content)
			{
				GameObject.Destroy(child.gameObject);
			}


			int numColumns = items.Count / 3 + 1;
			// enforce a minimum of 3 columns -- that is how many fit on the screen
			if (numColumns < 3)
			{
				numColumns = 3;
			}
			float totalWidth = numColumns * COLUMN_WIDTH;

			_content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);

			for (int i = 0; i < numColumns; ++i)
			{
				GameObject column = CreateColumn(i + 1);

				for (int j = 0; j < ITEMS_PER_COLUMN; ++j)
				{
					int itemIndex = i * ITEMS_PER_COLUMN + j;
					if (itemIndex < items.Count)
					{
                        AvatarShopItemViewModel clothing = items[itemIndex];
						AvatarShopItem closetItem = CreateItem(clothing, column.transform);
                        closetItem.onClick += (() => HandleClosetItem(clothing.Clothing));
						_activeItems.Add(closetItem);
					}
				}
			}

			_activeCategory = category;
		}

        private void HandleClosetItem(Clothing clothing)
        {
            // this is a request to change clothing
            if (OnAction != null)
            {
                OnAction(this, new ClothingPurchaseRequestArgs { Item = clothing, Type = "Purchase" });
            }
        }

		private GameObject CreateColumn(int index)
		{
			GameObject column = Instantiate(_columnPrefab);
			column.name = "Column" + index;
			column.transform.SetParent(_content, false);

			return column;
		}

        private AvatarShopItem CreateItem(AvatarShopItemViewModel clothing, Transform column)
		{
			GameObject item = Instantiate(_itemPrefab);
            AvatarShopItem closetItem = item.GetComponent<AvatarShopItem>();
			Sprite itemSprite = _thumbResourceManager.GetIcon(clothing.Clothing);
			closetItem.SetItem(clothing, itemSprite);

			item.name = "Item";
			item.transform.SetParent(column, false);

			return closetItem;
		}

		private float COLUMN_WIDTH;

		private void Awake()
		{
            if (_itemPrefab == null)
            {
                Debug.LogError("Item Prefab not set!");
            }
            else
            {
                COLUMN_WIDTH = _itemPrefab.GetComponent<LayoutElement>().minWidth;
            }
		}

		private void Start()
		{
			_categoryScreen.onCategorySelected += HandleCategory;
			_categoryScreen.onClose += (() => HandleCategory(ScreenClothingCategory.None));
            _filterButton.onClick.AddListener(() => HandleShelfButton(ShopButton.Filter));
		}

		private void HandleCategory(ScreenClothingCategory category)
		{
			_categoryScreen.gameObject.SetActive(false);

			if (category != ScreenClothingCategory.None)
			{
				PopulateCategory(category);
			}
		}

		private void HandleShelfButton(ShopButton button)
		{
			switch (button)
			{
			case ShopButton.Filter:
				// toggle the category view
				_categoryScreen.gameObject.SetActive(true);
				break;
			}
		}
	}
}

