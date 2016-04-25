using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Voltage.Witches.Models;

namespace Voltage.Witches.Models
{
	public class GlossaryEntry
	{
		public string Id { get ; protected set; }
		public string EntryName { get; protected set; }
		public string EntryText { get; protected set; }
		public int DisplayOrder { get; protected set; }
		public GlossaryCategory CategoryID { get; protected set; }
		public int PageCount { get; protected set; }
		public bool HasPicture { get; protected set; }
		public string PicturePath { get; protected set; }
		public Dictionary<int,string> PagesAndWords { get; protected set; }

		private const int CHARACTER_LIMIT_NO_PIC = 308;
		private const int CHARACTER_LIMIT_PIC = 220;

		//HACK For unit testing purposes
		private static string _testEntry = "TEST ENTRY";
		private static string _testEntryPic = "TEST ENTRY PIC";
		private static string _testEntryLong = "TEST ENTRY LONG";
		private static string _testEntryLongPic = "TEST ENTRY LONG PIC";

		private static string _testName = "Test Entry Name";
		private static string _testEntryText = "The only reason people do not know much is because they do not care to know. They are incurious. Incuriousity is the oddest and most foolish failing there is.";
		private static string _testEntryTextLong = "Language is my whore, my mistress, my wife, my pen-friend, my check-out girl. Language is a complimentary moist lemon-scented cleansing square or handy freshen-up wipette. Language is the breath of God, the dew on a fresh apple, it's the soft rain of dust that falls into a shaft of morning sun when you pull from an old bookshelf a forgotten volume of erotic diaries; language is the faint scent of urine on a pair of boxer shorts, it's a half-remembered childhood birthday party, a creak on the stair, a spluttering match held to a frosted pane, the warm wet, trusting touch of a leaking nappy, the hulk of a charred Panzer, the underside of a granite boulder, the first downy growth on the upper lip of a Mediterranean girl, cobwebs long since overrun by an old Wellington boot.";
		private static string _picPath = string.Empty;
		private static string _picPathPic = "Pictures/turdburglar";

		private static Dictionary<string, string> _entryNameLookUp = new Dictionary<string, string>()
		{
			{"54e3f9146f983fbe8903bc36","[USER_FIRST] [USER_LAST]"},{"54e3f9146f983fbe8903bc37","Alix [USER_LAST]"},{"54e3f9146f983fbe8903bc38","Amelia Waite"},{"54e3f9146f983fbe8903bc39","Anastasia Petrova"},{"54e3f9146f983fbe8903bc3a","Andreas Strauss"},
			{"54e3f9156f983fbe8903bc93","Alignment"},{"54e3f9156f983fbe8903bc94","Familiars"},{"54e3f9156f983fbe8903bc95","Witch"},{"54e3f9156f983fbe8903bc96","Flatulence"},{"54e3f9156f983fbe8903bc97","Opulence"},
			{"54e3f9156f983fbe8903bc56","Avatar Shop"},{"54e3f9156f983fbe8903bc57","Closet"},{"54e3f9156f983fbe8903bc58","Coin"},{"54e3f9156f983fbe8903bc59","Customer Support"},{"54e3f9156f983fbe8903bc5a","Data Transfer"},
			{"54e3f9146f983fbe8903bbe6","Artemisia Liquorice"},{"54e3f9146f983fbe8903bbe7","Artemisia Seeds"},{"54e3f9146f983fbe8903bbe8","Artemisia Twig"},{"54e3f9146f983fbe8903bbe9","Blessed Silver"},{"54e3f9146f983fbe8903bbea","Blind Mayapple"},
			{"54e3f9156f983fbe8903bc68","Werbury"},{"54e3f9156f983fbe8903bc69","Grier Bookshop"},{"54e3f9156f983fbe8903bc6a","Jump Start Cafe"},{"54e3f9156f983fbe8903bc6b","Salem Attunement Site"},{"54e3f9156f983fbe8903bc6c","Castle Lismere"},
			{"54e3f9156f983fbe8903bc83","Witch Hunter"},{"54e3f9156f983fbe8903bc84","Coven"},{"54e3f9156f983fbe8903bc85","Witch Council"},{"54e3f9156f983fbe8903bc86","Hunter's Code"},{"54e3f9156f983fbe8903bc87","Cat Fanciers"},
			{"54e3f9156f983fbe8903bc75","Wayward Magic"},{"54e3f9156f983fbe8903bc76","Starfall"},{"54e3f9156f983fbe8903bc77","Starflood"},{"54e3f9156f983fbe8903bc78","Starstone"},{"54e3f9156f983fbe8903bc79","Magic"},
			{"54da8ad86f983f60ee01f996","Arlen's Tonic"},{"54da8ad86f983f60ee01f997","Astral Projection"},{"54da8ad86f983f60ee01f998","Attunement Spell"},{"54da8ad86f983f60ee01f999","Blindeye Shroud"},{"54da8ad86f983f60ee01f99a","Bogveil"}
		};

		private static Dictionary<string, int> _entryOrderLookUp = new Dictionary<string, int>()
		{
			{"54e3f9146f983fbe8903bc36",1},{"54e3f9146f983fbe8903bc37",2},{"54e3f9146f983fbe8903bc38",3},{"54e3f9146f983fbe8903bc39",4},{"54e3f9146f983fbe8903bc3a",5},
			{"54e3f9156f983fbe8903bc93",1},{"54e3f9156f983fbe8903bc94",2},{"54e3f9156f983fbe8903bc95",3},{"54e3f9156f983fbe8903bc96",4},{"54e3f9156f983fbe8903bc97",5},
			{"54e3f9156f983fbe8903bc56",1},{"54e3f9156f983fbe8903bc57",2},{"54e3f9156f983fbe8903bc58",3},{"54e3f9156f983fbe8903bc59",4},{"54e3f9156f983fbe8903bc5a",5},
			{"54e3f9146f983fbe8903bbe6",1},{"54e3f9146f983fbe8903bbe7",2},{"54e3f9146f983fbe8903bbe8",3},{"54e3f9146f983fbe8903bbe9",4},{"54e3f9146f983fbe8903bbea",5},
			{"54e3f9156f983fbe8903bc68",1},{"54e3f9156f983fbe8903bc69",2},{"54e3f9156f983fbe8903bc6a",3},{"54e3f9156f983fbe8903bc6b",4},{"54e3f9156f983fbe8903bc6c",5},
			{"54e3f9156f983fbe8903bc83",1},{"54e3f9156f983fbe8903bc84",2},{"54e3f9156f983fbe8903bc85",3},{"54e3f9156f983fbe8903bc86",4},{"54e3f9156f983fbe8903bc87",5},
			{"54e3f9156f983fbe8903bc75",1},{"54e3f9156f983fbe8903bc76",2},{"54e3f9156f983fbe8903bc77",3},{"54e3f9156f983fbe8903bc78",4},{"54e3f9156f983fbe8903bc79",5},
			{"54da8ad86f983f60ee01f996",1},{"54da8ad86f983f60ee01f997",2},{"54da8ad86f983f60ee01f998",3},{"54da8ad86f983f60ee01f999",4},{"54da8ad86f983f60ee01f99a",5}
		};

		public GlossaryEntry(string id, GlossaryCategory categoryID)
		{
			Id = id;
			if(!Id.Contains("TEST"))
			{
				EntryName = _entryNameLookUp[Id];
				EntryText = EntryName + " body text";
				CategoryID = categoryID;
				DisplayOrder = _entryOrderLookUp[Id];
				SetPicture(string.Empty);
				CalculatePages();
			}
			else
			{
				if(Id == _testEntry)
				{
					EntryName = _testName;
					EntryText = _testEntryText;
					CategoryID = categoryID;
					DisplayOrder = 1;
					SetPicture(_picPath);
					CalculatePages();
				}
				else if(Id == _testEntryPic)
				{
					EntryName = _testName;
					EntryText = _testEntryText;
					CategoryID = categoryID;
					DisplayOrder = 1;
					SetPicture(_picPathPic);
					CalculatePages();
				}
				else if(Id == _testEntryLong)
				{
					EntryName = _testName;
					EntryText = _testEntryTextLong;
					CategoryID = categoryID;
					DisplayOrder = 1;
					SetPicture(_picPath);
					CalculatePages();
				}
				else if(Id == _testEntryLongPic)
				{
					EntryName = _testName;
					EntryText = _testEntryTextLong;
					CategoryID = categoryID;
					DisplayOrder = 1;
					SetPicture(_picPathPic);
					CalculatePages();
				}
			}
		}

		void SetPicture(string pictureFilePath)
		{
			PicturePath = pictureFilePath;
			if(!string.IsNullOrEmpty(PicturePath))
			{
				HasPicture = true;
			}
			else
			{
				HasPicture = false;
			}
		}

		void CalculatePages ()
		{
			++PageCount;
			PagesAndWords = new Dictionary<int, string>();
			string entryCharacters = EntryText;
			if(HasPicture)
			{
				if(entryCharacters.Length > CHARACTER_LIMIT_PIC)
				{
					string firstPage = entryCharacters.Substring(0,CHARACTER_LIMIT_PIC);
					PagesAndWords[PageCount] = firstPage;
					entryCharacters = entryCharacters.Remove(0,CHARACTER_LIMIT_PIC);
					while(entryCharacters.Length > CHARACTER_LIMIT_NO_PIC)
					{
						++PageCount;
						string newPage = entryCharacters.Substring(0,CHARACTER_LIMIT_NO_PIC);
						PagesAndWords[PageCount] = newPage;
						entryCharacters = entryCharacters.Remove (0,CHARACTER_LIMIT_NO_PIC);
					}
					if(entryCharacters.Length <= CHARACTER_LIMIT_NO_PIC)
					{
						++PageCount;
						string lastPage = entryCharacters;
						PagesAndWords[PageCount] = lastPage;
					}
				}
				else
				{
					PagesAndWords[PageCount] = entryCharacters;
				}
			}
			else
			{
				if(entryCharacters.Length > CHARACTER_LIMIT_NO_PIC)
				{
					string firstPage = entryCharacters.Substring(0,CHARACTER_LIMIT_NO_PIC);
					PagesAndWords[PageCount] = firstPage;
					entryCharacters = entryCharacters.Remove(0,CHARACTER_LIMIT_NO_PIC);
					while(entryCharacters.Length > CHARACTER_LIMIT_NO_PIC)
					{
						++PageCount;
						string newPage = entryCharacters.Substring(0,CHARACTER_LIMIT_NO_PIC);
						PagesAndWords[PageCount] = newPage;
						entryCharacters = entryCharacters.Remove (0,CHARACTER_LIMIT_NO_PIC);
					}
					if(entryCharacters.Length <= CHARACTER_LIMIT_NO_PIC)
					{
						++PageCount;
						string lastPage = entryCharacters;
						PagesAndWords[PageCount] = lastPage;
					}
				}
				else
				{
					PagesAndWords[PageCount] = entryCharacters;
				}
			}

			PageCount = PagesAndWords.Count;
		}

		public int GetPageIndexForCurrentWords(List<string> entriesList)
		{
			if(PagesAndWords.Count == entriesList.Count)
			{
				for(int i = 0; i < entriesList.Count; ++i)
				{
					if((PagesAndWords.ContainsKey(i + 1)) && (entriesList[i] == PagesAndWords[i + 1]))
					{
						return (i + 1);
					}
				}
				
			}
			
			return 0;
		}
	}
}