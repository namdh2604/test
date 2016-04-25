using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iGUI;
using Voltage.Witches.Events;
using Voltage.Witches.Controllers;
using Voltage.Witches.Models;
using Voltage.Witches.Views;

namespace Voltage.Witches.Screens
{
	using Debug = UnityEngine.Debug;

	public class GlossaryScreen : BaseScreen
	{
		[HideInInspector]
		public Placeholder contentsPlaceholder,categoriesPlaceholder,entriesPlaceholder;

		[HideInInspector]
		public iGUIButton shortcut_category_button,btn_contents,btn_previous,btn_next;

		[HideInInspector]
		public iGUILabel table_header_text,page_text,content_header_text,page_subtext;

		[HideInInspector]
		public iGUIContainer shortcut_grp,page_group,page_subgroup,content_header_grp;

		private List<iGUIContainer> _viewContainers;
		private iGUISmartPrefab_Contents _contentsView;
		private iGUISmartPrefab_Categories _categoriesView;
		private iGUISmartPrefab_Entry _entriesView;
		//TODO Add the different views for the individual views

		Player _player;
		GlossaryScreenController _controller;

		private static string _contentsHeader = "TABLE OF CONTENTS";
		private int _totalPages = 0;
		enum GLOSSARY_VIEW
		{
			TABLE_OF_CONTENTS = 0,
			CATEGORIES = 1,
			ENTRIES = 2
		}

		[SerializeField]
		private GLOSSARY_VIEW _activeView = GLOSSARY_VIEW.TABLE_OF_CONTENTS;

		public void Init(Player player, GlossaryScreenController controller)
		{
			_player = player;
			_controller = controller;

			UnityEngine.Debug.Log(_player.FullName);
		}

		void Start()
		{
			_viewContainers = new List<iGUIContainer>();
			_totalPages = _controller.TotalPages;
			LoadPlaceholders();
			ViewContents();
		}

		protected override IScreenController GetController()
		{
			return _controller;
		}

		void LoadPlaceholders()
		{
			LoadContentsView();
			LoadCategoriesView();
			LoadEntriesView();
		}

		void LoadEntriesView()
		{
			var container = entriesPlaceholder.SwapForSmartObject() as iGUIContainer;
			_entriesView = container.GetComponent<iGUISmartPrefab_Entry>();
			_viewContainers.Add(container);
		}

		void LoadCategoriesView ()
		{
			var container = categoriesPlaceholder.SwapForSmartObject() as iGUIContainer;
			_categoriesView = container.GetComponent<iGUISmartPrefab_Categories>();
			_categoriesView.OnItemClick += HandleItemClick;
			_viewContainers.Add(container);
		}

		void LoadContentsView()
		{
			var container = contentsPlaceholder.SwapForSmartObject() as iGUIContainer;
			_contentsView = container.GetComponent<iGUISmartPrefab_Contents>();
			_contentsView.SetGlossary(_controller.GetGlossary());
			_contentsView.SetPageContent(_controller.GetCurrentPageContents());
			_contentsView.OnItemClick += HandleItemClick;
			_viewContainers.Add(container);
		}

		void HandleItemClick(object sender, GUIEventArgs e)
		{
			GlossaryItemSelectedEventArgs itemArgs = e as GlossaryItemSelectedEventArgs;
			Debug.Log(sender.ToString() + " " + itemArgs.Index.ToString() + " " + itemArgs.ItemKey.ToString());
			switch(_activeView)
			{
				case GLOSSARY_VIEW.TABLE_OF_CONTENTS:
					ViewCategory(itemArgs.ItemKey);
					break;
				case GLOSSARY_VIEW.CATEGORIES:
					ViewEntry(itemArgs.ItemKey);
					break;
				case GLOSSARY_VIEW.ENTRIES:
					//TODO What will this shit do??
					break;
			}
		}

		void UpdatePageNumber(int currentPage)
		{
			//TODO Get total pages from glossary and make string with the current page
			string current = "(" + currentPage.ToString() + "/" + _totalPages.ToString() + ")";
			page_text.label.text = current;
			page_subtext.label.text = current;
		}

		void ViewContents()
		{
			_activeView = GLOSSARY_VIEW.TABLE_OF_CONTENTS;
			UpdateView();
			table_header_text.label.text = _contentsHeader;
			UpdatePageNumber(_controller.CurrentPage);
			_contentsView.SetPageContent(_controller.GetCurrentPageContents());
			if(shortcut_category_button.enabled)
			{
				shortcut_category_button.setEnabled(false);
			}
			if(btn_contents.enabled)
			{
				btn_contents.setEnabled(false);
			}
			if(!content_header_grp.enabled)
			{
				content_header_grp.setEnabled(true);
			}
			if(!page_group.enabled)
			{
				page_group.setEnabled(true);
			}
			if(page_subgroup.enabled)
			{
				page_subgroup.setEnabled(false);
			}

		}

		void ViewConsecutiveContents()
		{
			UpdateView();
			UpdatePageNumber(_controller.CurrentPage);
			_contentsView.SetPageContent(_controller.GetCurrentPageContents());
		}

		void ViewCategory(string itemKey)
		{
			_activeView = GLOSSARY_VIEW.CATEGORIES;
			var page = _controller.GetPageNumberForItem(itemKey);
			_controller.GoToPageNumber(page);
			UpdateView();
			table_header_text.label.text = itemKey;
			_categoriesView.SetCategory(_controller.GetCategoryContent(itemKey));
			UpdatePageNumber(_controller.CurrentPage);
			_categoriesView.SetPageContent(_controller.GetCurrentPageContents());
			if(!btn_contents.enabled)
			{
				btn_contents.setEnabled(true);
			}
			if(shortcut_category_button.enabled)
			{
				shortcut_category_button.setEnabled(false);
			}
			if(content_header_grp.enabled)
			{
				content_header_grp.setEnabled(false);
			}
			if(page_group.enabled)
			{
				page_group.setEnabled(false);
			}
			if(!page_subgroup.enabled)
			{
				page_subgroup.setEnabled(true);
			}
			//TODO Get Item from Glossary

		}

		void ViewEntry(string itemKey)
		{
			_activeView = GLOSSARY_VIEW.ENTRIES;
			var page = _controller.GetPageNumberForItem(itemKey);
			_controller.GoToPageNumber(page);
			UpdateView();
			table_header_text.label.text = itemKey;
			UpdatePageNumber(_controller.CurrentPage);
			var entry = _controller.GetGlossaryEntry(_categoriesView.CurrentCategory,itemKey);
			_entriesView.SetUpEntry(entry);
			_entriesView.SetPageContent(_controller.GetCurrentPageContents());

			if(!btn_contents.enabled)
			{
				btn_contents.setEnabled(true);
			}
			if(!shortcut_category_button.enabled)
			{
				shortcut_category_button.setEnabled(true);
			}
			if(content_header_grp.enabled)
			{
				content_header_grp.setEnabled(false);
			}
			if(page_group.enabled)
			{
				page_group.setEnabled(false);
			}
			if(!page_subgroup.enabled)
			{
				page_subgroup.setEnabled(true);
			}
			//TODO Get Item from Glossary
		}

		void UpdateView()
		{
			switch(_activeView)
			{
				case GLOSSARY_VIEW.TABLE_OF_CONTENTS:
					_viewContainers[(int)GLOSSARY_VIEW.TABLE_OF_CONTENTS].setEnabled(true);
					_viewContainers[(int)GLOSSARY_VIEW.CATEGORIES].setEnabled(false);
					_viewContainers[(int)GLOSSARY_VIEW.ENTRIES].setEnabled(false);
					break;
				case GLOSSARY_VIEW.CATEGORIES:
					_viewContainers[(int)GLOSSARY_VIEW.TABLE_OF_CONTENTS].setEnabled(false);
					_viewContainers[(int)GLOSSARY_VIEW.CATEGORIES].setEnabled(true);
					_viewContainers[(int)GLOSSARY_VIEW.ENTRIES].setEnabled(false);
					break;
				case GLOSSARY_VIEW.ENTRIES:
					_viewContainers[(int)GLOSSARY_VIEW.TABLE_OF_CONTENTS].setEnabled(false);
					_viewContainers[(int)GLOSSARY_VIEW.CATEGORIES].setEnabled(false);
					_viewContainers[(int)GLOSSARY_VIEW.ENTRIES].setEnabled(true);
					break;
			}

			UpdatePreviousAndNextButtons();
//			btn_previous.setEnabled(isPreviousButtonEnabled());
//			btn_next.setEnabled(isNextButtonEnabled());
		}

		void UpdatePreviousAndNextButtons()
		{
			btn_previous.setEnabled(isPreviousButtonEnabled());
			btn_next.setEnabled(isNextButtonEnabled());
		}

		bool isPreviousButtonEnabled()
		{
			return (!_controller.isFirstPage);
		}

		bool isNextButtonEnabled()
		{
//			return ((!_controller.isLastPage) || (_controller.CurrentPage == _totalPages));
			return (!(_controller.CurrentPage == _totalPages));
		}

		public void btn_a_Click(iGUIButton sender)
		{
			Debug.Log("Navigate to start from A");
		}

		public void btn_g_Click(iGUIButton sender)
		{
			Debug.Log("Navigate to start from G");
		}

		public void btn_l_Click(iGUIButton sender)
		{
			Debug.Log("Navigate to start from L");
		}

		public void btn_q_Click(iGUIButton sender)
		{
			Debug.Log("Navigate to start from Q");
		}

		public void btn_v_Click(iGUIButton sender)
		{
			Debug.Log("Navigate to start from V");
		}

		public void shortcut_category_button_Click(iGUIButton sender)
		{
			Debug.Log("Return to categories layer view");
			ViewCategory(_categoriesView.CurrentCategory);
		}

		public void btn_contents_Click(iGUIButton sender)
		{
			Debug.Log("Return to contents layer view (the top)");
			_controller.GoToPageNumber(1);
			ViewContents();
		}

		public void btn_previous_Click(iGUIButton sender)
		{
			Debug.Log("Go to previous page");

			switch(_activeView)
			{
			case GLOSSARY_VIEW.TABLE_OF_CONTENTS:
				_controller.PreviousPage();
				ViewContents();
				break;
			case GLOSSARY_VIEW.CATEGORIES:
				if((!_categoriesView.PreviousIsNewCategory()) && (!_controller.FirstPageOfCategories()))
				{
					_controller.PreviousPage();
					UpdatePageNumber(_controller.CurrentPage);
					_categoriesView.GoToPreviousPage();
				}
				else if((_categoriesView.PreviousIsNewCategory()) && (!_controller.FirstPageOfCategories()))
				{
					//TODO Get the previous Category and pipe that shit in
					_controller.PreviousPage();
					UpdatePageNumber(_controller.CurrentPage);
					GlossaryCategoryContent newCategory = _controller.GetCategoryContent(_controller.GetPreviousCategoryKey(_categoriesView.CurrentCategory));
					table_header_text.label.text = newCategory.CategoryName;
					_categoriesView.SetCategory(newCategory);
					_categoriesView.SetPageContent(_controller.GetCurrentPageContents());
				}
				else
				{
					//TODO Need to get the Contents view and last page and pipe that shit in
					_controller.PreviousPage();
					ViewContents();
				}
				break;
			case GLOSSARY_VIEW.ENTRIES:
				//TODO Get the previous page in the current entry or pipe in the previous fuckin entry or jump to last page of last category
				if(_controller.FirstPageOfEntries())
				{
//					_categoriesView.SetCategory(_controller.GetCategoryContent(_controller.GetLastCategoryKey()));
					ViewCategory(_controller.GetLastCategoryKey());
				}
				else if((!_controller.FirstPageOfEntries()) && (_entriesView.PreviousIsNewEntry()) && (!_categoriesView.IsFirstEntryInCategory(_entriesView.EntryName)))
				{
					_controller.PreviousPage();
					UpdatePageNumber(_controller.CurrentPage);
					GlossaryEntry newEntry = _controller.GetPreviousEntry(_categoriesView.CurrentCategory,_entriesView.EntryName);
					table_header_text.label.text = newEntry.EntryName;
					_entriesView.SetUpEntry(newEntry);
					_entriesView.SetPageContent(_controller.GetCurrentPageContents());
				}
				else if((!_controller.FirstPageOfEntries()) && (_entriesView.PreviousIsNewEntry()) && (_categoriesView.IsFirstEntryInCategory(_entriesView.EntryName)))
				{
					Debug.Log("Need to get the last category and update that and get its last entry");
					_controller.PreviousPage();
					UpdatePageNumber(_controller.CurrentPage);
//					var firstEntry = _categoriesView.IsFirstEntryInCategory(_entriesView.EntryName);
					var nextCategoryKey = _controller.GetPreviousCategoryKeyFromEntry(_entriesView.Entry);
					var nextCat = _controller.GetCategoryContent(nextCategoryKey);
					_categoriesView.SetCategory(nextCat);
					var nextEntry = _controller.GetGlossaryEntry(nextCategoryKey,nextCat.GetEntryFromIndex((nextCat.Entries.Count - 1)).EntryName);
					table_header_text.label.text = nextEntry.EntryName;
					_entriesView.SetUpEntry(nextEntry);
					_entriesView.SetPageContent(_controller.GetCurrentPageContents());
				}
				else
				{
					_controller.PreviousPage();
					UpdatePageNumber(_controller.CurrentPage);
					_entriesView.GoToPreviousPage();
				}
				break;
			}

			UpdatePreviousAndNextButtons();
		}

		public void btn_next_Click(iGUIButton sender)
		{
			Debug.Log("Go to next page");

			switch(_activeView)
			{
			case GLOSSARY_VIEW.TABLE_OF_CONTENTS:
				if(!_controller.LastPageOfContents())
				{
					_controller.NextPage();
					ViewContents();
				}
				else
				{
					ViewCategory(_controller.GetFirstCategoryKey());
				}
				break;
			case GLOSSARY_VIEW.CATEGORIES:
				if((!_categoriesView.NextIsNewCategory()) && (!_controller.LastPageOfCategories()))
				{
					_controller.NextPage();
					UpdatePageNumber(_controller.CurrentPage);
					_categoriesView.GoToNextPage();
				}
				else if((_categoriesView.NextIsNewCategory()) && (!_controller.LastPageOfCategories()))
				{
					//TODO Get the next Category and pipe that shit in
					_controller.NextPage();
					UpdatePageNumber(_controller.CurrentPage);
					GlossaryCategoryContent newCategory = _controller.GetCategoryContent(_controller.GetNextCategoryKey(_categoriesView.CurrentCategory));
					table_header_text.label.text = newCategory.CategoryName;
					_categoriesView.SetCategory(newCategory);
					_categoriesView.SetPageContent(_controller.GetCurrentPageContents());
				}
				else
				{
					_categoriesView.SetCategory(_controller.GetCategoryContent(_controller.GetFirstCategoryKey()));
					ViewEntry(_controller.GetFirstEntryKey());
				}
				break;
			case GLOSSARY_VIEW.ENTRIES:
				if(!_entriesView.NextIsNewEntry())
				{
					_controller.NextPage();
					UpdatePageNumber(_controller.CurrentPage);
//					var current = _controller.GetCurrentPageContents();
					_entriesView.GoToNextPage();
				}
				else if((_entriesView.NextIsNewEntry()) && (!_controller.isLastPage) && (!_categoriesView.IsLastEntryInCategory(_entriesView.EntryName)))
				{
					_controller.NextPage();
					UpdatePageNumber(_controller.CurrentPage);
//					var current = _controller.GetCurrentPageContents();
					GlossaryEntry newEntry = _controller.GetNextEntry(_categoriesView.CurrentCategory,_entriesView.EntryName);
					table_header_text.label.text = newEntry.EntryName;
					_entriesView.SetUpEntry(newEntry);
					_entriesView.SetPageContent(_controller.GetCurrentPageContents());
				}
				else if((_entriesView.NextIsNewEntry()) && (!_controller.isLastPage) && (_categoriesView.IsLastEntryInCategory(_entriesView.EntryName)))
				{
					Debug.Log("Need to get the next category and update that and get its first entry");
					_controller.NextPage();
					UpdatePageNumber(_controller.CurrentPage);
//					var lastEntry = _categoriesView.IsLastEntryInCategory(_entriesView.EntryName);
					var nextCategoryKey = _controller.GetNextCategoryKeyFromEntry(_entriesView.Entry);
					var nextCat = _controller.GetCategoryContent(nextCategoryKey);
					_categoriesView.SetCategory(nextCat);
					var nextEntry = _controller.GetGlossaryEntry(nextCategoryKey,nextCat.GetEntryFromIndex(0).EntryName);
					table_header_text.label.text = nextEntry.EntryName;
					_entriesView.SetUpEntry(nextEntry);
					_entriesView.SetPageContent(_controller.GetCurrentPageContents());
				}
				else
				{
					var current = _controller.GetCurrentPageContents();
					var isNewEntry = _entriesView.NextIsNewEntry();
					var isLastPage = _controller.isLastPage;
					var entry = _entriesView.Entry;
					Debug.Log(current.ToString());
					Debug.Log(isNewEntry.ToString() + " " + isLastPage.ToString() + " " + entry.ToString());
				}
				break;
			}

			UpdatePreviousAndNextButtons();
		}

		public void home_button_Click(iGUIButton sender)
		{
			_controller.GoHome();
		}

		public void SetEnabled(bool value)
		{
			screenFrame.setEnabled(value);
			gameObject.SetActive(value);
		}
	}
}