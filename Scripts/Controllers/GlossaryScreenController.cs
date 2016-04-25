using System.Collections.Generic;
using UnityEngine;
using Voltage.Witches.Screens;
using Voltage.Witches;
using Voltage.Witches.Models;

namespace Voltage.Witches.Controllers
{
	using Debug = UnityEngine.Debug;

	public class GlossaryScreenController : ScreenController
	{
		private IScreenFactory _factory;
		private Player _player;

		private iGUISmartPrefab_GlossaryScreen _screen;

		private Glossary _glossary;
		private List<List<string>> _pageContents;
		private int _currentPageIndex;
//		private GlossaryCategoryContent _currentCategory = null;
//		private GlossaryEntry _currentEntry = null;
		public int CurrentPage { get; protected set; }
		public int TotalPages { get; protected set; }

		public GlossaryScreenController(ScreenNavigationManager controller, IScreenFactory factory, Player player):base(controller)
		{
			_factory = factory;
			_player = player;

			Debug.Log(_player.FullName);

			_glossary = new Glossary();
			_pageContents = new List<List<string>>();
			_currentPageIndex = 0;
			CurrentPage = 1;
			TotalPages = _glossary.TotalPagesInGlossary;
			SetUpListOfContents();
			InitializeView();
		}

        public override void Dispose()
        {
            // TODO: Not calling the base because the base is calling GetScreen, which will force us to generate another screen then delete and then set to null.
            // We need to find out if we can safely turn all GetScreen to return the screen and not make one if one doesn't exist.
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
        }

		protected override IScreen GetScreen ()
		{
			if(_screen != null)
			{
				return _screen;
			}
			else
			{
				_screen = _factory.GetScreen<iGUISmartPrefab_GlossaryScreen>();
				_screen.Init(_player, this);
				return _screen;
			}
		}

		void InitializeView()
		{
			_screen = _factory.GetScreen<iGUISmartPrefab_GlossaryScreen>();
			_screen.Init(_player, this);
		}

		public override void MakePassive (bool value)
		{
			_screen.SetEnabled (!value);
		}

		void SetUpListOfContents()
		{
			var allPagesAndContents = _glossary.TotalPagesAndContents;
			for(int i = 0; i < allPagesAndContents.Count; ++i)
			{
				_pageContents.Add(allPagesAndContents[i]);
			}
		}

		public Glossary GetGlossary()
		{
			return _glossary;
		}

		private int GetPageFromContents(string nameKey)
		{
			int maxPage = _glossary.ContentsLayerMaxPage();
			for(int i = 0; i < maxPage; ++i)
			{
				var currentList = _pageContents[i];
				if(currentList.Contains(nameKey))
				{
					return ((currentList.IndexOf(nameKey) + (i + 1)) + maxPage);
				}
			}

			return 0;
		}

//		int CalculateOffset()

		private int GetPageFromCategories(string nameKey)
		{
			int startPoint = _glossary.ContentsLayerMaxPage();
			int maxPage = _glossary.CategoriesLayerMaxPage();
			int offset = maxPage;
			for(int i = startPoint; i < maxPage; ++i)
			{
				var currentList = _pageContents[i];

				if(currentList.Contains(nameKey))
				{
//					return ((currentList.IndexOf(nameKey) + ((i - startPoint) + 1)) + offset);
					return ((currentList.IndexOf(nameKey)) + (offset + 1));
				}

				offset += currentList.Count;
			}

			return startPoint;
		}

		public int GetPageNumberForItem(string nameKey)
		{
			var contentsMax = _glossary.ContentsLayerMaxPage();
			var categoriesMax = _glossary.CategoriesLayerMaxPage();

			if(_currentPageIndex < contentsMax)
			{
				return GetPageFromContents(nameKey);
			}
			else if((_currentPageIndex >= contentsMax) && (_currentPageIndex < categoriesMax))
			{
				return GetPageFromCategories(nameKey);
			}
			else
			{
				return GetPageFromContents(nameKey);
			}

//			return 0;
		}

		public void GoToPageNumber(int pageNumber)
		{
			CurrentPage = pageNumber;
			_currentPageIndex = CurrentPage - 1;
		}

		public bool FirstPageOfEntries()
		{
			bool value = ((_currentPageIndex - 1) < _glossary.CategoriesLayerMaxPage());
			return value;
		}

		public bool FirstPageOfCategories()
		{
			bool value = ((_currentPageIndex - 1) < _glossary.ContentsLayerMaxPage());
			return value;
		}

		public bool LastPageOfCategories()
		{
			bool value = ((_currentPageIndex + 1) >= _glossary.CategoriesLayerMaxPage());
			return value;
		}

		public bool LastPageOfContents()
		{
			bool value = ((_currentPageIndex + 1) >= _glossary.ContentsLayerMaxPage());
			return value;
		}

		public string GetFirstCategoryKey()
		{
			var listOfStrings = _pageContents[0];
			return listOfStrings[0];
		}

		public string GetNextCategoryKey(string currentCategoryKey)
		{
			string nextKey = string.Empty;
			for(int i = 0; i < _glossary.ContentsLayerMaxPage(); ++i)
			{
				var currentList = _pageContents[i];
				if(currentList.Contains(currentCategoryKey))
				{
					nextKey = currentList[(currentList.IndexOf(currentCategoryKey) + 1)];
					break;
				}
			}
			return nextKey;
		}

		public string GetPreviousCategoryKey(string currentCategoryKey)
		{
			string previousKey = string.Empty;
			for(int i = 0; i < _glossary.ContentsLayerMaxPage(); ++i)
			{
				var currentList = _pageContents[i];
				if(currentList.Contains(currentCategoryKey))
				{
					previousKey = currentList[(currentList.IndexOf(currentCategoryKey) - 1)];
					break;
				}
			}
			return previousKey;
		}

		public GlossaryEntry GetPreviousEntry(string currentCategoryKey,string currentEntryKey)
		{
			var category = GetCategoryContent(currentCategoryKey);
			var index = category.GetEntryIndexFromNameKey(currentEntryKey);
			if(index > 0)
			{
				return category.GetEntryFromIndex(index - 1);
			}

			return category.Entries[currentEntryKey];
		}

		public GlossaryEntry GetNextEntry(string currentCategoryKey,string currentEntryKey)
		{
			var category = GetCategoryContent(currentCategoryKey);
			var index = category.GetEntryIndexFromNameKey(currentEntryKey);
			if(index < (category.Entries.Count - 1))
			{
				return category.GetEntryFromIndex(index + 1);
			}
			
			return category.Entries[currentEntryKey];
		}

		public string GetPreviousCategoryKeyFromEntry(GlossaryEntry entry)
		{
			var categoryKey = GetCategoryKeyFromEntry(entry);
			return GetPreviousCategoryKey(categoryKey);
		}

		public string GetNextCategoryKeyFromEntry(GlossaryEntry entry)
		{
			var categoryKey = GetCategoryKeyFromEntry(entry);
			return GetNextCategoryKey(categoryKey);
		}

		public string GetCategoryKeyFromEntry(GlossaryEntry entry)
		{
			switch(entry.CategoryID)
			{
			case GlossaryCategory.CHARACTERS:
				return "Characters";
			case GlossaryCategory.CULTURE:
				return "Culture";
			case GlossaryCategory.GAMEPLAY:
				return "Gameplay";
			case GlossaryCategory.INGREDIENTS:
				return "Ingredients";
			case GlossaryCategory.LOCATIONS:
				return "Locations";
			case GlossaryCategory.ORGANIZATIONS:
				return "Organizations";
			case GlossaryCategory.PHENOMENA:
				return "Phenomena";
			case GlossaryCategory.SPELLS:
				return "Spells";
			}

			return string.Empty;
		}

		public string GetFirstEntryKey()
		{
			var listOfStrings = _pageContents[_glossary.ContentsLayerMaxPage()];
			var returnKey = listOfStrings[0];
			return returnKey;
		}

		public string GetLastCategoryKey()
		{
			var listOfStrings = _pageContents[(_glossary.ContentsLayerMaxPage() - 1)];
			return listOfStrings[(listOfStrings.Count - 1)];
		}

		public int GetIndexOfLastPageOfLastCategory()
		{
			return _glossary.ContentsLayerMaxPage();
		}

		public List<string> GetCurrentPageContents()
		{
			return _pageContents[_currentPageIndex];
		}
	
		public GlossaryCategoryContent GetCategoryContent(string nameKey)
		{
			return _glossary.Categories[nameKey];
		}

		public GlossaryEntry GetGlossaryEntry(string categoryKey,string nameKey)
		{

			return _glossary.Categories[categoryKey].Entries[nameKey];
		}

		public bool isFirstPage
		{
			get { return (_currentPageIndex == 0); }
		}

		public bool isLastPage
		{
			get { return (_currentPageIndex == (_glossary.TotalPagesInGlossary - 1)); }
		}

		public void PreviousPage()
		{
			if(_currentPageIndex > 0)
			{
				--_currentPageIndex;
				--CurrentPage;
			}
		}

		public void NextPage()
		{
			if(_currentPageIndex < _pageContents.Count)
			{
				++_currentPageIndex;
				++CurrentPage; 
			}
		}

		public void GoHome()
		{
			Manager.GoToExistingScreen("/Home");
		}
		
		public void SetEnabled(bool value)
		{
			_screen.SetEnabled(value);
		}
		
		public void Unload()
		{
			_screen.SetEnabled(false);
		}
	}
}