using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class ContentsView : MonoBehaviour
	{
		private List<iGUIElement> _itemContainers;
		private List<iGUISmartPrefab_ContentItem> _itemViews;
		
		public GUIEventHandler OnItemClick;
		private List<string> _pageContent;
		private Glossary _glossary = null;

		public int CurrentPageOfContents { get; protected set; }

		public void SetGlossary(Glossary glossary)
		{
			_glossary = glossary;
		}

		public void SetPageContent(List<string> pageContent)
		{
			_pageContent = new List<string>();
			_pageContent = pageContent;
			CurrentPageOfContents = _glossary.GetPageIndexForCurrentEntries(pageContent);
			UpdateContents();
		}

		string GetIndexString(string nameKey)
		{
			int totalEntries = _glossary.Categories[nameKey].Entries.Count;
//			int index = _currentCategory.GetEntryIndexFromNameKey(nameKey);

			string entriesString = @"(" + totalEntries.ToString() + @")";

			return entriesString;
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
						currentView.item_counter_text.label.text = GetIndexString(_pageContent[i]);
					}
					else
					{
						if(currentElement.enabled)
						{
							currentElement.setEnabled(false);
						}
						currentView.item_name_text.label.text = "CATEGORY_GOES_HERE";
						currentView.item_counter_text.label.text = @"(0000)";
					}
				}
			}
		}

		protected virtual void Start()
		{
			_itemContainers = new List<iGUIElement>();
			_itemViews = new List<iGUISmartPrefab_ContentItem>();

			LoadPlaceholders();
			ConfigureClickHandlers();
			UpdateContents();
		}

		void LoadPlaceholders ()
		{
			var viewContainer = gameObject.GetComponent<iGUIContainer>().items;
			for(int i = 0; i < viewContainer.Length; ++i)
			{
				Debug.Log(viewContainer[i].name);
				iGUIElement element = ((Placeholder)viewContainer[i]).SwapForSmartObject();
				var view = element.GetComponent<iGUISmartPrefab_ContentItem>();
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

		private void ConfigureClickHandlers()
		{
			var viewContainer = gameObject.GetComponent<iGUIContainer>();
			for(int i = 0; i < viewContainer.itemCount; ++i)
			{
				var button = viewContainer.items[i].GetComponent<iGUISmartPrefab_ContentItem>().GetButton();
				button.userData = i;
				button.clickCallback = HandleItemClick;
			}
		}
	}
}