using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Models
{
	public class GlossaryCategoryContent
	{
		public string CategoryName { get; protected set; }
		public GlossaryCategory Category { get; protected set; }
		public Dictionary<string, GlossaryEntry> Entries { get; protected set; }
		public int PageCount { get; protected set; }
		public Dictionary<int, List<string>> PagesAndItems { get; protected set; }
		public int CumulativePages { get; protected set; }

		private Dictionary<int, string> _itemNumberRef;
		private const int ENTRIES_PER_PAGE = 12;

		private static List<string> _testCategoryLookUp = new List<string>(){
			"TEST CATEGORY SINGLE PAGE",
			"TEST CATEGORY MULTI PAGE"
		};
		private static List<int> _testEntryCount = new List<int>()
		{
			5,
			26
		};

		private static Dictionary<string, string> _categoryLookUp = new Dictionary<string, string>()
		{
			{"54da8ad76f983f60ee01f851","Culture"}, {"54da8ad76f983f60ee01f852","Locations"}, {"54da8ad76f983f60ee01f853","Characters"},
			{"54da8ad76f983f60ee01f854","Organizations"}, {"54da8ad76f983f60ee01f855","Phenomena"}, {"54da8ad76f983f60ee01f856","Spells"},
			{"54da8ad76f983f60ee01f857","Ingredients"}, {"54da8ad76f983f60ee01f858","Gameplay"}
		};

		private static List<List<string>> _entryLookUp = new List<List<string>>()
		{
			{new List<string>(){"54e3f9146f983fbe8903bc36",
								"54e3f9146f983fbe8903bc37",
								"54e3f9146f983fbe8903bc38",
								"54e3f9146f983fbe8903bc39",
								"54e3f9146f983fbe8903bc3a"}},
			{new List<string>(){"54e3f9156f983fbe8903bc93",
								"54e3f9156f983fbe8903bc94",
								"54e3f9156f983fbe8903bc95",
								"54e3f9156f983fbe8903bc96",
								"54e3f9156f983fbe8903bc97"}},
			{new List<string>(){"54e3f9156f983fbe8903bc56",
								"54e3f9156f983fbe8903bc57",
								"54e3f9156f983fbe8903bc58",
								"54e3f9156f983fbe8903bc59",
								"54e3f9156f983fbe8903bc5a"}},
			{new List<string>(){"54e3f9146f983fbe8903bbe6",
								"54e3f9146f983fbe8903bbe7",
								"54e3f9146f983fbe8903bbe8",
								"54e3f9146f983fbe8903bbe9",
								"54e3f9146f983fbe8903bbea"}},
			{new List<string>(){"54e3f9156f983fbe8903bc68",
								"54e3f9156f983fbe8903bc69",
								"54e3f9156f983fbe8903bc6a",
								"54e3f9156f983fbe8903bc6b",
								"54e3f9156f983fbe8903bc6c"}},
			{new List<string>(){"54e3f9156f983fbe8903bc83",
								"54e3f9156f983fbe8903bc84",
								"54e3f9156f983fbe8903bc85",
								"54e3f9156f983fbe8903bc86",
								"54e3f9156f983fbe8903bc87"}},
			{new List<string>(){"54e3f9156f983fbe8903bc75",
								"54e3f9156f983fbe8903bc76",
								"54e3f9156f983fbe8903bc77",
								"54e3f9156f983fbe8903bc78",
								"54e3f9156f983fbe8903bc79"}},
			{new List<string>(){"54da8ad86f983f60ee01f996",
								"54da8ad86f983f60ee01f997",
								"54da8ad86f983f60ee01f998",
								"54da8ad86f983f60ee01f999",
								"54da8ad86f983f60ee01f99a"}}
		};




		public GlossaryCategoryContent(string categoryID)
		{
			_itemNumberRef = new Dictionary<int, string>();
			if(_categoryLookUp.ContainsKey(categoryID))
			{
				Entries = new Dictionary<string, GlossaryEntry>();
				CategoryName = _categoryLookUp[categoryID];
				AssignCategory();
				GetCategoryEntries();
				CalculatePageCount();
				GetCumulativeCount();
			}
			else
			{
				if(_testCategoryLookUp.Contains(categoryID))
				{
//					Console.WriteLine("Valid Test");
					Entries = new Dictionary<string, GlossaryEntry>();
					CategoryName = categoryID;
					Category = GlossaryCategory.GAMEPLAY;
					GetTestCategoryEntries();
					CalculatePageCount();
					GetCumulativeCount();
				}
				else
				{
//					Console.WriteLine("Invalid Entry");
				}
			}
		}

		void AssignCategory ()
		{
			switch(CategoryName)
			{
				case "Culture":
					Category = GlossaryCategory.CULTURE;
					break;
				case "Locations":
					Category = GlossaryCategory.LOCATIONS;
					break;
				case "Characters":
					Category = GlossaryCategory.CHARACTERS;
					break;
				case "Organizations":
					Category = GlossaryCategory.ORGANIZATIONS;
					break;
				case "Phenomena":
					Category = GlossaryCategory.PHENOMENA;
					break;
				case "Spells":
					Category = GlossaryCategory.SPELLS;
					break;
				case "Ingredients":
					Category = GlossaryCategory.INGREDIENTS;
					break;
				case "Gameplay":
					Category = GlossaryCategory.GAMEPLAY;
					break;
			}
		}

		void GetTestCategoryEntries()
		{
			int indexOfEntries = _testCategoryLookUp.IndexOf(CategoryName);

			for(int i = 0; i < _testEntryCount[indexOfEntries]; ++i)
			{
				if(CategoryName.Contains("SINGLE"))
				{
					GlossaryEntry newEntry = new GlossaryEntry("TEST ENTRY",Category);
					Entries[newEntry.EntryName + "_" + i.ToString()] = newEntry;
					_itemNumberRef[i] = newEntry.EntryName + "_" + i.ToString();
				}
				else
				{
					GlossaryEntry newEntry = new GlossaryEntry("TEST ENTRY LONG",Category);
					Entries[newEntry.EntryName + "_" + i.ToString()] = newEntry;
					_itemNumberRef[i] = newEntry.EntryName + "_" + i.ToString();
				}
			}
		}

		void GetCategoryEntries ()
		{
			List<string> entries = _entryLookUp[(int)Category];
			for(int i = 0; i < entries.Count; ++i)
			{
				GlossaryEntry newEntry = new GlossaryEntry(entries[i],Category);
				Entries[newEntry.EntryName] = newEntry;
				_itemNumberRef[i] = newEntry.EntryName;
			}
		}

		void CalculatePageCount ()
		{
			++PageCount;
			PagesAndItems = new Dictionary<int, List<string>>();
			if(Entries.Count < ENTRIES_PER_PAGE)
			{
				List<string> firstEntry = new List<string>();
				for(int i = 0; i < _itemNumberRef.Count; ++i)
				{
					firstEntry.Add(_itemNumberRef[i]);
				}

				PagesAndItems[PageCount] = firstEntry;
			}
			else
			{
				int lastIndex = 0;
				List<string> entries = new List<string>();
				PageCount = UnityEngine.Mathf.CeilToInt((float)Entries.Count / (float)ENTRIES_PER_PAGE);
				for(int i = 0; i < PageCount; ++i)
				{
					entries.Clear();
					if(lastIndex < _itemNumberRef.Count)
					{
						for(int l = lastIndex; l < _itemNumberRef.Count; ++l)
						{
							entries.Add(_itemNumberRef[l]);
							if((entries.Count == ENTRIES_PER_PAGE) && (l < (_itemNumberRef.Count - 1)))
							{
								lastIndex = l;
								PagesAndItems[i + 1] = entries;
								break;
							}
							else if(l == (_itemNumberRef.Count - 1))
							{
								lastIndex = _itemNumberRef.Count;
								PagesAndItems[i + 1] = entries;
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

		void GetCumulativeCount ()
		{
			foreach(KeyValuePair<string,GlossaryEntry> pair in Entries)
			{
				CumulativePages += pair.Value.PageCount;
			}

			CumulativePages += PageCount;
		}

		public int GetEntryIndexFromNameKey(string nameKey)
		{
			int indexOfEntry = 0;
			if(!_itemNumberRef.ContainsValue(nameKey))
			{
				UnityEngine.Debug.LogWarning("Name Key doesn't exist");
				return indexOfEntry;
			}

			foreach(KeyValuePair<int,string> pair in _itemNumberRef)
			{
				if(pair.Value == nameKey)
				{
					indexOfEntry = pair.Key;
				}
			}

			return indexOfEntry;
		}

		public GlossaryEntry GetEntryFromIndex(int index)
		{
			string key = _itemNumberRef[index];
			return Entries[key];
		}

		public Dictionary<int,string> GetEntryPageAndWordsFromIndex(int index)
		{
			string key = _itemNumberRef[index];
			return Entries[key].PagesAndWords;
		}

		public int GetPageIndexForCurrentEntries(List<string> entriesList)
		{
			foreach(var pair in PagesAndItems)
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