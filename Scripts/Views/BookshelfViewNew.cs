using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class BookshelfViewNew : MonoBehaviour 
	{
		[HideInInspector]
		public iGUIContainer spellbookCovers;

		[HideInInspector]
		public Placeholder Spellbook_Cover_0,Spellbook_Cover_1,Spellbook_Cover_2,Spellbook_Cover_3,Spellbook_Cover_4;
		
		public event EventHandler OnBookClick;
		public delegate void ButtonsReadyCallback();
		public event ButtonsReadyCallback OnComplete;

		List<iGUISmartPrefab_SpellbookDisplay> _spellBookDisplays;
		List<iGUIElement> _coversContainers;

		const int NO_SELECTION = -1;
		const int MAX_BOOKS_DISPLAYED = 5;//Will be 7

		int _lastSelectedIndex = NO_SELECTION;
		int _selectedIndex = NO_SELECTION;
		private List<bool?> _bookAvailability;

		private Rect _normalSize = new Rect(0.0f, 0.0f, 0.1224055f, 0.5569948f);
		private Rect _selectedSize = new Rect(0.0f, 0.0f, 0.1569984f, 0.7046632f);

		public List<iGUIButton> Book_Buttons { get; protected set; }

		protected void Awake()
		{
			_bookAvailability = new List<bool?>();
		}

		protected void Start()
		{
			LoadPlaceholders();
			ConfigureClickHandlers();
			UpdateDisplay();
		}

		void LoadPlaceholders()
		{
			_coversContainers = new List<iGUIElement>();
			_spellBookDisplays = new List<iGUISmartPrefab_SpellbookDisplay>();

			var spellPlaceholders = spellbookCovers.items;
			Book_Buttons = new List<iGUIButton>();
			for(int i = 0; i < spellPlaceholders.Length; ++i)
			{
				iGUIElement element;
				if(spellPlaceholders[i].GetType() == typeof(Placeholder))
				{
					element = ((Placeholder)spellPlaceholders[i]).SwapForSmartObject();
				}
				else
				{
					element = spellPlaceholders[i];
				}
				var view = element.GetComponent<iGUISmartPrefab_SpellbookDisplay>();
				_coversContainers.Add(element);
				_spellBookDisplays.Add(view);
				Book_Buttons.Add(view.Spellbook_Button);
			}

			if(OnComplete != null)
			{
				OnComplete();
			}
		}
		
		public void SetBooks(List<Spellbook> books)
		{
			_bookAvailability = new List<bool?>();
			
			for (int i = 0; i < books.Count; ++i)
			{
				_bookAvailability.Add(books[i].IsAccessible);

			}
		}
		
		private void UpdateDisplay()
		{
			for (int i = 0; i < MAX_BOOKS_DISPLAYED; ++i)
			{
				var bookDisplay = spellbookCovers.items[i] as iGUIElement;
				var spellbook = bookDisplay.GetComponent<iGUISmartPrefab_SpellbookDisplay>();

				if (i < _bookAvailability.Count)
				{
					spellbook.DisplayBook(BOOK_STATE.AVAILABLE,(i + 1));
				}
				else
				{
					spellbook.DisplayBook(BOOK_STATE.LOCKED,(i + 1));
				}
			}
		}
		
		private void ConfigureClickHandlers()
		{
			for (int i = 0; i < spellbookCovers.items.Length; ++i)
			{
				var button = spellbookCovers.items[i].GetComponent<iGUISmartPrefab_SpellbookDisplay>().GetButton();
				button.userData = i;
			}
		}

		public void ExecuteButtonClick(int index)
		{
			HandleBookClick(Book_Buttons[index]);
		}

		private void HandleBookClick(iGUIElement sender)
		{
			int index = (int)sender.userData;
			if (_selectedIndex == index)
			{
				return;
			}

			ChangeBookSelection(index);
			
			if (OnBookClick != null)
			{
				OnBookClick(this, new BookChangedEventArgs(index));
			}
		}

		void ResizeUnselectedBooks ()
		{
			for(int i = 0; i < spellbookCovers.itemCount; ++i)
			{
				if(i != _selectedIndex)
				{
					ResetBookSize(i);
					var display = spellbookCovers.items[i].GetComponent<iGUISmartPrefab_SpellbookDisplay>();
					if(display != null)
					{
						display.spellbook_glow.setEnabled(false);
					}
//					_spellBookDisplays[i].spellbook_glow.setEnabled(false);
				}
			}
		}
		
		public void ChangeBookSelection(int index)
		{
			if(_selectedIndex != NO_SELECTION)
			{
				ToggleBookSize(index);
			}
			_lastSelectedIndex = _selectedIndex;
			_selectedIndex = index;
			ToggleBookSize(_selectedIndex);
			ResizeUnselectedBooks();
		}

		private void ToggleGlow(int index)
		{
			if(_selectedIndex != NO_SELECTION)
			{
				// Reset selected book
				var oldBook = _spellBookDisplays[_selectedIndex];
				oldBook.spellbook_glow.setEnabled(true);
			}
			
			if(_selectedIndex <= (_bookAvailability.Count - 1))
			{
				var newBook = _spellBookDisplays[_selectedIndex];
				newBook.spellbook_glow.setEnabled(true);
			}
			else
			{
				_selectedIndex = _lastSelectedIndex;
				var newBook = _spellBookDisplays[_selectedIndex];
				newBook.spellbook_glow.setEnabled(true);
			}
		}
		
		const float ZOOM_SCALE = 1.2f;

		private void ResetBookSize(int index)
		{
			var oldBook = spellbookCovers.items[index] as iGUIElement;
			if((index != _selectedIndex) && (oldBook.positionAndSize.height != _normalSize.height))
			{
				_normalSize.x = oldBook.positionAndSize.x;
				_normalSize.y = oldBook.positionAndSize.y;
				oldBook.setPositionAndSize(_normalSize);
			}
		}

		private void ToggleBookSize(int index)
		{
			if (_selectedIndex != NO_SELECTION)
			{
				// Reset selected book
				var oldBook = spellbookCovers.items[_selectedIndex] as iGUIElement;
				var display = spellbookCovers.items[_selectedIndex].GetComponent<iGUISmartPrefab_SpellbookDisplay>();
				_normalSize.x = oldBook.positionAndSize.x;
				_normalSize.y = oldBook.positionAndSize.y;
				oldBook.setPositionAndSize(_normalSize);
				if(display != null)
				{
					display.spellbook_glow.setEnabled(true);
				}
			}

			if(_selectedIndex <= (_bookAvailability.Count - 1))
			{
				var newBook = spellbookCovers.items[_selectedIndex] as iGUIElement;
				var display = spellbookCovers.items[_selectedIndex].GetComponent<iGUISmartPrefab_SpellbookDisplay>();
				_selectedSize.x = newBook.positionAndSize.x;
				_selectedSize.y = newBook.positionAndSize.y;
				newBook.setPositionAndSize(_selectedSize);
				if(display != null)
				{
					display.spellbook_glow.setEnabled(true);
				}
			}
			else
			{
				_selectedIndex = _lastSelectedIndex;
				var newBook = spellbookCovers.items[_selectedIndex] as iGUIElement;
				var display = spellbookCovers.items[_selectedIndex].GetComponent<iGUISmartPrefab_SpellbookDisplay>();
				_selectedSize.x = newBook.positionAndSize.x;
				_selectedSize.y = newBook.positionAndSize.y;
				newBook.setPositionAndSize(_selectedSize);
				if(display != null)
				{
					display.spellbook_glow.setEnabled(true);
				}
			}
		}
	}
}