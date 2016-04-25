using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class AvatarColumnView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIContainer item_column;
		
		[HideInInspector]
		public Placeholder item_placeholder_0, item_placeholder_1, item_placeholder_2;
		
		private List<iGUISmartPrefab_AvatarItem> _avatarItemViews;
		private List<iGUIElement> _itemContainers;

		public List<iGUIButton> Item_Buttons { get; protected set; }

		public event EventHandler OnItemClick;
		public delegate void OnItemsLoaded();
		public event OnItemsLoaded ItemsAllLoaded;

		public bool IsActiveColumn
		{
			get { return (AnyItemsEnabled()); }
		}

		void Start()
		{
			LoadPlaceholders();
			ConfigureClickHandlers();
		}
		
		public List<iGUISmartPrefab_AvatarItem> GetAvatarItems() 
		{
			return _avatarItemViews;
		}

		bool AnyItemsEnabled()
		{
			int itemsActive = 0;

			for(int i = 0; i < _itemContainers.Count; ++i)
			{
				if(_itemContainers[i].enabled)
				{
					++itemsActive;
				}
			}

			return (itemsActive > 0);
		}

		void LoadPlaceholders()
		{
			_itemContainers = new List<iGUIElement>();
			_avatarItemViews = new List<iGUISmartPrefab_AvatarItem>();
			
			var itemColumnView = item_column.items;
			Item_Buttons = new List<iGUIButton>();
			for(int i = 0; i < itemColumnView.Length; ++i)
			{
				iGUIElement element = ((Placeholder)itemColumnView[i]).SwapForSmartObject();
				var view = element.GetComponent<iGUISmartPrefab_AvatarItem>();
				_itemContainers.Add(element);
				_avatarItemViews.Add(view);
				Item_Buttons.Add(view.item_button);
			}
			
			if((_avatarItemViews.Count >= 3) && (ItemsAllLoaded != null))
			{
				ItemsAllLoaded();
			}
		}

		public void HandleItemClick(iGUIElement sender)
		{
			int index = (int)sender.userData;
			var clothingItem = _avatarItemViews[index].AvatarItem;

			if((OnItemClick != null) && (clothingItem != null))
			{
				OnItemClick(_avatarItemViews[index], new ClosetItemChangedEventArgs(index,clothingItem));
			}
		}
		
		private void ConfigureClickHandlers()
		{
			for(int i = 0; i < item_column.itemCount; ++i)
			{
				var button = item_column.items[i].GetComponent<iGUISmartPrefab_AvatarItem>().GetButton();
				button.userData = i;
			}
		}
	}
}