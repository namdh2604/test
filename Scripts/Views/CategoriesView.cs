using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{

	using Debug = UnityEngine.Debug;

	public class CategoriesView : MonoBehaviour 
	{
		private List<iGUIElement> _itemContainers;
		private List<iGUISmartPrefab_CategoryItem> _itemViews;
		
		public GUIEventHandler OnItemClick;
		private List<string> _pageContent;
		private GlossaryCategoryContent _currentCategory = null;
		public string CurrentCategory { get; protected set; }
		public int CurrentPageInCategory { get; protected set; }

		public void SetCategory(GlossaryCategoryContent category)
		{
			_currentCategory = category;
			CurrentCategory = _currentCategory.CategoryName;
		}

		public void GoToPreviousPage()
		{
			--CurrentPageInCategory;
			SetPageContent(_currentCategory.PagesAndItems[CurrentPageInCategory]);
		}

		public void GoToNextPage()
		{
			++CurrentPageInCategory;
			SetPageContent(_currentCategory.PagesAndItems[CurrentPageInCategory]);
		}

		public void SetPageContent(List<string> pageContent)
		{
			_pageContent = new List<string>();
			CurrentPageInCategory = _currentCategory.GetPageIndexForCurrentEntries(pageContent);
			_pageContent = pageContent;
			UpdateContents();
		}

		void UpdateContents()
		{
			if((_itemViews != null) && (_itemViews.Count > 0))
			{
				for(int i = 0; i < _itemViews.Count; ++i)
				{
					var currentElement = _itemContainers[i];
					var currentView = _itemViews[i];
					if(i < _pageContent.Count)
					{
						if(!currentElement.enabled)
						{
							currentElement.setEnabled(true);
						}
						currentView.item_name_text.label.text = _pageContent[i];
					}
					else
					{
						if(currentElement.enabled)
						{
							currentElement.setEnabled(false);
						}
						currentView.item_name_text.label.text = "ENTRY_NAME_HERE";
					}
				}
			}
		}

		protected virtual void Start()
		{
			_itemContainers = new List<iGUIElement>();
			_itemViews = new List<iGUISmartPrefab_CategoryItem>();
			
			LoadPlaceholders();
//			UpdateContents();
			ConfigureClickHandlers();
		}
		
		void LoadPlaceholders ()
		{
			var viewContainer = gameObject.GetComponent<iGUIContainer>().items;
			for(int i = 0; i < viewContainer.Length; ++i)
			{
				Debug.Log(viewContainer[i].name);
				iGUIElement element = ((Placeholder)viewContainer[i]).SwapForSmartObject();
				var view = element.GetComponent<iGUISmartPrefab_CategoryItem>();
				element.setOrder(i);
				_itemContainers.Add(element);
				_itemViews.Add(view);
			}
		}
		
		void HandleItemClick(iGUIElement sender)
		{
			int index = (int)sender.userData;
			var view = _itemViews[index];
			string key = view.item_name_text.label.text;
			
			if(OnItemClick != null)
			{
				OnItemClick(this, new GlossaryItemSelectedEventArgs(index,key));
			}
		}

		public bool IsFirstEntryInCategory(string entryName)
		{
			return (_currentCategory.GetEntryIndexFromNameKey(entryName) == 0);
		}
		
		public bool IsLastEntryInCategory(string entryName)
		{
			return (_currentCategory.GetEntryIndexFromNameKey(entryName) == (_currentCategory.Entries.Count - 1));
		}

		public bool PreviousIsNewCategory()
		{
			return (!_currentCategory.PagesAndItems.ContainsKey(CurrentPageInCategory - 1));
		}

		public bool NextIsNewCategory()
		{
			return (!_currentCategory.PagesAndItems.ContainsKey(CurrentPageInCategory + 1));
		}

		private void ConfigureClickHandlers()
		{
			var viewContainer = gameObject.GetComponent<iGUIContainer>();
			for(int i = 0; i < viewContainer.itemCount; ++i)
			{
				var button = viewContainer.items[i].GetComponent<iGUISmartPrefab_CategoryItem>().GetButton();
				button.userData = i;
				button.clickCallback = HandleItemClick;
			}
		}
	}
}