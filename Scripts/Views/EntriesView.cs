using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class EntriesView : MonoBehaviour 
	{
		[HideInInspector]
		public iGUIContainer entry_pic_group;

		[HideInInspector]
		public iGUILabel entry_text_pic_1,entry_text_1,entry_text_2;

		[HideInInspector]
		public iGUIImage entry_placeholder_image;

		[HideInInspector]
		public iGUIButton expand_img_btn;

		private List<string> _pageContent;

		public GlossaryEntry Entry { get; protected set; }
		public string EntryName { get; protected set; }
		public int CurrentPageInEntry { get; protected set; }

		public void SetUpEntry(GlossaryEntry entry)
		{
			Entry = entry;
			EntryName = entry.EntryName;
		}

		protected virtual void Start()
		{

		}

		public void GoToPreviousPage()
		{
			CurrentPageInEntry -= 2;
			List<string> pageContent = new List<string>(){ Entry.PagesAndWords[CurrentPageInEntry], Entry.PagesAndWords[(CurrentPageInEntry + 1)] };
			SetPageContent(pageContent);
		}
		
		public void GoToNextPage()
		{
			CurrentPageInEntry += 2;
			List<string> pageContent = new List<string>(){ Entry.PagesAndWords[CurrentPageInEntry], Entry.PagesAndWords[(CurrentPageInEntry + 1)] };
			SetPageContent(pageContent);
		}

		public void SetPageContent(List<string> pageContent)
		{
			_pageContent = new List<string>();
			CurrentPageInEntry = Entry.GetPageIndexForCurrentWords(pageContent);
			_pageContent = pageContent;
			for(int i = 0; i < _pageContent.Count; ++i)
			{
				Debug.Log(_pageContent[i]);
			}
			UpdateContents();
		}

		bool isPageOddNumber()
		{
			return ((CurrentPageInEntry % 2) != 0);
		}

		bool currentPageHasSecondPage()
		{
			return (Entry.PagesAndWords.ContainsKey(CurrentPageInEntry + 1));
		}

		bool EntryHasNoPic()
		{
			return (!Entry.HasPicture);
		}

		void LoadPicture ()
		{
			entry_placeholder_image.image = Resources.Load<Texture2D>(Entry.PicturePath);
		}

		public bool PreviousIsNewEntry()
		{
			return (!Entry.PagesAndWords.ContainsKey(CurrentPageInEntry - 2));
		}

		public bool NextIsNewEntry()
		{
			return (!Entry.PagesAndWords.ContainsKey(CurrentPageInEntry + 2));
		}

		public void expand_img_btn_Click(iGUIButton sender)
		{
			Debug.Log("Expand Image Please");
		}

		void UpdateContents()
		{
			if(Entry == null)
			{
				return;
			}

			if((CurrentPageInEntry == 1))
			{
				entry_pic_group.setEnabled(Entry.HasPicture);
				entry_text_1.setEnabled(EntryHasNoPic());
				if(entry_pic_group.enabled)
				{
					LoadPicture();
					entry_text_pic_1.label.text = Entry.PagesAndWords[CurrentPageInEntry];
					entry_text_2.setEnabled(currentPageHasSecondPage());
					if(currentPageHasSecondPage())
					{
						entry_text_2.label.text = Entry.PagesAndWords[(CurrentPageInEntry + 1)];
					}
				}
				else
				{
					entry_text_1.label.text = Entry.PagesAndWords[CurrentPageInEntry];
					entry_text_2.setEnabled(currentPageHasSecondPage());
					if(currentPageHasSecondPage())
					{
						entry_text_2.label.text = Entry.PagesAndWords[(CurrentPageInEntry + 1)];
					}
				}
			}
			else
			{
				entry_pic_group.setEnabled(false);
				entry_text_1.setEnabled(true);
				if(Entry.PagesAndWords.ContainsKey(CurrentPageInEntry))
				{
					entry_text_1.label.text = Entry.PagesAndWords[CurrentPageInEntry];
				}
//				else if()
				entry_text_2.setEnabled(currentPageHasSecondPage());
				if(currentPageHasSecondPage())
				{
					entry_text_2.label.text = Entry.PagesAndWords[(CurrentPageInEntry + 1)];
				}
			}
		}
	}
}