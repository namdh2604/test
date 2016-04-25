using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Models
{
	public class Glossary
	{
		public Dictionary<string,GlossaryCategoryContent> Categories { get; protected set; }
		public List<GlossaryEntry> AllEntries { get; protected set; }
		public int PageCount { get; protected set; }
		public int TotalPagesInGlossary { get; protected set; }
		public Dictionary<int,List<string>> PagesAndCategories { get; protected set; }
		public Dictionary<int,List<string>> TotalPagesAndContents { get; protected set; }

		private Dictionary<int,string> _itemNumberRef;
		private const int CATEGORIES_PER_PAGE = 12;
		private List<int> _transitionPageIndex;
		private enum GlossaryLayer
		{
			CONTENTS = 0,
			CATEGORIES = 1,
			ENTRIES = 2
		}

		public int ContentsLayerMaxPage()
		{
			return _transitionPageIndex[(int)GlossaryLayer.CONTENTS];
		}

		public int CategoriesLayerMaxPage()
		{
			return _transitionPageIndex[(int)GlossaryLayer.CATEGORIES];
		}

		public int EntriesLayerMaxPage()
		{
			return _transitionPageIndex[(int)GlossaryLayer.ENTRIES];
		}

		private static List<string> _categoryNames = new List<string>()
		{
			"Culture","Locations","Characters","Organizations","Phenomena","Spells","Ingredients","Gameplay"
		};

		private static List<string> _categoryIDLookUp = new List<string>()
		{
			"54da8ad76f983f60ee01f851","54da8ad76f983f60ee01f852","54da8ad76f983f60ee01f853","54da8ad76f983f60ee01f854","54da8ad76f983f60ee01f855",
			"54da8ad76f983f60ee01f856","54da8ad76f983f60ee01f857","54da8ad76f983f60ee01f858"
		};

		private static List<string> _testCategoryNames = new List<string>()
		{
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE",
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE"
		};

		private static List<string> _multiPageCategoryNames = new List<string>()
		{
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE",
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE",
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE",
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE",
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE",
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE",
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE",
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE"
		};

		//TODO Change this to accept a list of string IDs
		public Glossary()
		{
			Categories = new Dictionary<string,GlossaryCategoryContent>();
			PagesAndCategories = new Dictionary<int, List<string>>();
			_itemNumberRef = new Dictionary<int, string>();
			_transitionPageIndex = new List<int>();

			for(int i = 0; i < _categoryIDLookUp.Count; ++i)
			{
				GlossaryCategoryContent newContent = new GlossaryCategoryContent(_categoryIDLookUp[i]);
				Categories[newContent.CategoryName] = newContent;
				_itemNumberRef[i] = newContent.CategoryName;

			}
			AllEntries = new List<GlossaryEntry>();
			for(int l = 0; l < _categoryNames.Count; ++l)
			{
				var currentKey = _categoryNames[l];
				var currentDictionary = Categories[currentKey].Entries;
				foreach(KeyValuePair<string,GlossaryEntry> pair in currentDictionary)
				{
					if(!AllEntries.Contains(pair.Value))
					{
						AllEntries.Add(pair.Value);
					}
				}
			}

			CalculatePageCount();
			CalculateCumulativePages();
			BuildTotalPagesAndReferences();
		}

		public void MakeTestGlossary(bool isSingle)
		{
			Categories.Clear();
			AllEntries.Clear();
			_itemNumberRef.Clear();
			_transitionPageIndex = new List<int>();
			PageCount = 0;
			TotalPagesInGlossary = 0;

			if(isSingle)
			{
				for(int i = 0; i < _testCategoryNames.Count; ++i)
				{
					GlossaryCategoryContent newContent = new GlossaryCategoryContent(_testCategoryNames[i]);
					Categories[_testCategoryNames[i] + "_" + i.ToString()] = newContent;
					_itemNumberRef[i] = _testCategoryNames[i] + "_" + i.ToString();
				}
			}
			else
			{
				for(int i = 0; i < _multiPageCategoryNames.Count; ++i)
				{
					GlossaryCategoryContent newContent = new GlossaryCategoryContent(_multiPageCategoryNames[i]);
					Categories[_multiPageCategoryNames[i] + "_" + i.ToString()] = newContent;
					_itemNumberRef[i] = _multiPageCategoryNames[i] + "_" + i.ToString();
				}
			}
			CalculatePageCount();
			CalculateCumulativePages();
			BuildTotalPagesAndReferences();
		}

		void CalculatePageCount ()
		{
			++PageCount;
			PagesAndCategories = new Dictionary<int, List<string>>();
			if(Categories.Count < CATEGORIES_PER_PAGE)
			{
				List<string> firstEntry = new List<string>();
				for(int i = 0; i < _itemNumberRef.Count; ++i)
				{
					firstEntry.Add(_itemNumberRef[i]);
				}
				
				PagesAndCategories[PageCount] = firstEntry;
			}
			else
			{
				int lastIndex = 0;

				PageCount = UnityEngine.Mathf.CeilToInt((float)Categories.Count / (float)CATEGORIES_PER_PAGE);
				for(int i = 0; i < PageCount; ++i)
				{
					List<string> entries = new List<string>();
					if(lastIndex < _itemNumberRef.Count)
					{
						for(int l = lastIndex; l < _itemNumberRef.Count; ++l)
						{
							entries.Add(_itemNumberRef[l]);
							if((entries.Count == CATEGORIES_PER_PAGE) && (l < (_itemNumberRef.Count - 1)))
							{
								lastIndex = l + 1;
								PagesAndCategories[i + 1] = entries;
								break;
							}
							else if(l == (_itemNumberRef.Count - 1))
							{
								lastIndex = _itemNumberRef.Count;
								PagesAndCategories[i + 1] = entries;
								break;
							}
						}
					}
					else
					{
						break;
					}
				}
			}
		}

		void CalculateCumulativePages ()
		{
			foreach(KeyValuePair<string,GlossaryCategoryContent> pair in Categories)
			{
				TotalPagesInGlossary += pair.Value.CumulativePages;
			}
			
			TotalPagesInGlossary += PageCount;
		}

		void BuildTotalPagesAndReferences()
		{
			TotalPagesAndContents = new Dictionary<int, List<string>>();
			bool hasAddedPagesAndContentsFromCategories = false;
			bool hasAddedPagesAndWordsFromEntries = false;
			bool hasAddedTransitionToCategories = false;
			bool hasAddedTransitionToEntries = false;
			for(int i = 0; i < TotalPagesInGlossary; ++i)
			{
				if(TotalPagesAndContents.ContainsKey(i))
				{
					continue;
				}

				if(PagesAndCategories.ContainsKey(i + 1))
				{
					TotalPagesAndContents[i] = PagesAndCategories[i + 1];
				}
				else if(!hasAddedPagesAndContentsFromCategories)
				{
					if(!hasAddedTransitionToCategories)
					{
						_transitionPageIndex.Add(TotalPagesAndContents.Count);
						hasAddedTransitionToCategories = true;
					}
					for(int l = 0; l < _itemNumberRef.Count; ++l)
					{
						var currentCategory = Categories[_itemNumberRef[l]];
						var currentEntryCount = TotalPagesAndContents.Count - 1;
						for(int m = 0; m < currentCategory.PagesAndItems.Count; ++m)
						{
							++currentEntryCount;
							if(currentCategory.PagesAndItems.ContainsKey(m + 1))
							{
								TotalPagesAndContents[currentEntryCount] = currentCategory.PagesAndItems[m + 1];
							}
						}
					}
					hasAddedPagesAndContentsFromCategories = true;
				}
				else if(!hasAddedPagesAndWordsFromEntries)
				{
					if(!hasAddedTransitionToEntries)
					{
						_transitionPageIndex.Add(TotalPagesAndContents.Count);
						hasAddedTransitionToEntries = true;
					}
					for(int n = 0; n < _itemNumberRef.Count; ++n)
					{
						var currentCat = Categories[_itemNumberRef[n]];
						var currentEntryCount = TotalPagesAndContents.Count - 1;
						List<List<string>> runningList = new List<List<string>>();
						for(int o = 0; o < currentCat.Entries.Count; ++o)
						{
							var currentEntryPage = currentCat.GetEntryPageAndWordsFromIndex(o);

							for(int p = 0; p < currentEntryPage.Count; ++p)
							{
								if(currentEntryPage.ContainsKey(p + 1))
								{
									List<string> newList = new List<string>();
									string entryString = currentEntryPage[p + 1];
									newList.Add(entryString);
									runningList.Add(newList);
								}
							}
						}
						for(int q = 0; q < runningList.Count; ++q)
						{
							++currentEntryCount;
							if(!TotalPagesAndContents.ContainsKey(currentEntryCount))
							{
								TotalPagesAndContents[currentEntryCount] = runningList[q];
							}
						}
					}
					if(TotalPagesAndContents.Count == TotalPagesInGlossary)
					{
						hasAddedPagesAndWordsFromEntries = true;
					}
				}
				else
				{
					if(!TotalPagesAndContents.ContainsKey(i))
					{
						TotalPagesAndContents[i] = new List<string>();
					}
				}
			}
			_transitionPageIndex.Add(TotalPagesAndContents.Count);
		}

		public int GetPageIndexForCurrentEntries(List<string> entriesList)
		{
			foreach(var pair in PagesAndCategories)
			{
				if(pair.Value.Count == entriesList.Count)
				{
					var currentPageList = pair.Value;
					bool allItemsEqual = true;
					for(int i = 0; i < currentPageList.Count; ++i)
					{
						if(currentPageList[i] != entriesList[i])
						{
							allItemsEqual = false;
							break;
						}
					}
					if(allItemsEqual)
					{
						return pair.Key;
					}
				}
			}
			
			return 0;
		}
	}
}