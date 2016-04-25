using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class BookshelfView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIContainer Spellbook_Cover;

		public Texture2D _bookAvailableTexture = Resources.Load<Texture2D>("BookLayout/Spellbook_Available");
		public Texture2D _bookLockedTexture = Resources.Load<Texture2D>("BookLayout/Spellbook_Locked");
		public Texture2D _bookNoninteractiveTexture = Resources.Load<Texture2D>("BookLayout/Spellbook_NonInteractive");
		public Texture2D _bookSelectedTexture = Resources.Load<Texture2D>("BookLayout/Spellbook_Selected");

		public event EventHandler OnBookClick;

		const int NO_SELECTION = -1;
		const int MAX_BOOKS_DISPLAYED = 7;

		int _selectedIndex = NO_SELECTION;
		private List<bool?> _bookAvailability;

		private Rect _normalSize = new Rect(0.0f, 0.5142857f, 0.1057692f, 0.8813559f);
		private Rect _selectedSize = new Rect(0.0f, 0.5f, 0.1201923f, 1.0f);

		void Awake()
		{
			_bookAvailability = new List<bool?>();
		}

		void Start()
		{
			ConfigureClickHandlers();
			UpdateDisplay();
		}

		public void SetBooks(List<Spellbook> books)
		{
			_bookAvailability = new List<bool?>();

			for (int i = 0; i < books.Count; ++i)
			{
				_bookAvailability.Add(books[i].IsAccessible);
			}
			_bookAvailability.Add(null);
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			for (int i = 0; i < MAX_BOOKS_DISPLAYED; ++i)
			{
				var bookDisplay = Spellbook_Cover.items[i] as iGUIButton;

				if (i < _bookAvailability.Count)
				{
					Texture2D bookTexture = GetBookTexture(i);
					bookDisplay.style.normal.background = bookTexture;
					bookDisplay.setEnabled(true);
				}
				else
				{
					bookDisplay.setEnabled(false);
				}
			}
		}

		private Texture2D GetBookTexture(int index)
		{
			Texture2D texture;

			if (_bookAvailability[index].HasValue)
			{
				texture = (_bookAvailability[index].Value) ? _bookAvailableTexture : _bookLockedTexture;
			}
			else
			{
				texture = _bookNoninteractiveTexture;
			}

			return texture;
		}

		private void ConfigureClickHandlers()
		{
			for (int i = 0; i < Spellbook_Cover.items.Length; ++i)
			{
				var button = Spellbook_Cover.items[i] as iGUIButton;
				button.userData = i;
				button.clickCallback = HandleBookClick;
			}
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

		public void ChangeBookSelection(int index)
		{
			if (_selectedIndex != NO_SELECTION)
			{
				ToggleBookSize(index);
			}

			_selectedIndex = index;
			ToggleBookSize(_selectedIndex);
		}

		const float ZOOM_SCALE = 1.2f;

		private void ToggleBookSize(int index)
		{
			if (_selectedIndex != NO_SELECTION)
			{
				// Reset selected book
				var oldBook = Spellbook_Cover.items[_selectedIndex] as iGUIButton;
				_normalSize.x = oldBook.positionAndSize.x;
				oldBook.setPositionAndSize(_normalSize);
				oldBook.style.normal.background = GetBookTexture(_selectedIndex);
			}

			var newBook = Spellbook_Cover.items[index] as iGUIButton;
			_selectedSize.x = newBook.positionAndSize.x;
			newBook.setPositionAndSize(_selectedSize);
			newBook.style.normal.background = _bookSelectedTexture;
		}
	}
}

