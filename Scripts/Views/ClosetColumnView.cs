using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class ClosetColumnView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIContainer closetColumn,divider_left,divider_right;

		[HideInInspector]
		public Placeholder clothIconPlaceholder0, clothIconPlaceholder1, clothIconPlaceholder2;

		private List<iGUISmartPrefab_ClosetItemView> _closetItemViews;
		private List<iGUIElement> _itemContainers;

		public List<iGUIButton> Item_Buttons { get; protected set; }
		public List<iGUIButton> Archive_Buttons { get; protected set; }

		public event EventHandler OnItemClick;
		public event EventHandler OnItemArchive;
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
			divider_left.setEnabled(false);
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

		public List<iGUISmartPrefab_ClosetItemView> GetClosetItemViews() 
		{
			return _closetItemViews;
		}

		void LoadPlaceholders ()
		{
			_itemContainers = new List<iGUIElement>();
			_closetItemViews = new List<iGUISmartPrefab_ClosetItemView>();
			Item_Buttons = new List<iGUIButton>();
			Archive_Buttons = new List<iGUIButton>();
			var closetColumnView = new List<iGUIElement>(){ clothIconPlaceholder0,clothIconPlaceholder1,clothIconPlaceholder2 };

			for(int i = 0; i < closetColumnView.Count; ++i)
			{
				iGUIElement element = ((Placeholder)closetColumnView[i]).SwapForSmartObject();
				var view = element.GetComponent<iGUISmartPrefab_ClosetItemView>();
				_itemContainers.Add(element);
				_closetItemViews.Add(view);
				Item_Buttons.Add(view.cloth_item_button);
				Archive_Buttons.Add(view.btn_archive);
			}

			if((_closetItemViews.Count >= 3) && (ItemsAllLoaded != null))
			{
				ItemsAllLoaded();
			}
		}

		public void HandleItemClick (iGUIElement sender)
		{
			int index = (int)sender.userData;
			var clothingItem = _closetItemViews[index].ClosetItem;

			if((OnItemClick != null) && (clothingItem != null))
			{
				OnItemClick(this, new ClosetItemChangedEventArgs(index,clothingItem));
			}
		}

		public void HandleArchiveItemClick(iGUIElement sender)
		{
			int index = (int)sender.userData;
			var clothingItem = _closetItemViews[index].ClosetItem;

			if((OnItemArchive != null) && (clothingItem != null))
			{
				OnItemArchive(this, new ClosetItemChangedEventArgs(index,clothingItem));
			}
		}

		private void ConfigureClickHandlers()
		{
			for(int i = 0; i < _closetItemViews.Count; ++i)
			{
				var button = _closetItemViews[i].cloth_item_button;
				button.userData = i;
//				button.clickCallback = HandleItemClick;

				var archiveBtn = _closetItemViews[i].btn_archive;
				archiveBtn.userData = i;
//				archiveBtn.clickCallback = HandleArchiveItemClick;
			}
		}
	}
}