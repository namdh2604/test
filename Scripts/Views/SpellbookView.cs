using System;
using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class SpellbookView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIContainer potionPane;

		[HideInInspector]
		public iGUIImage Spellbook_Complete_Badge;

		List<iGUISmartPrefab_PotionDisplay> _potionViews;
		List<iGUIElement> _potionContainers;

		Spellbook _book;

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Init();
		}

		public void Init()
		{
			if (_potionViews == null)
			{
				CreatePotionViews();
			}
		}

		public void SetBook(Spellbook book)
		{
			_book = book;

			if (book.IsClear())
			{
				Spellbook_Complete_Badge.setEnabled(true);
			}
			else
			{
				Spellbook_Complete_Badge.setEnabled(false);
			}

			UpdateRecipes();
		}

		public void ListenForRecipeSelections(EventHandler handler)
		{
			foreach (var view in _potionViews)
			{
				view.OnPotionSelected += handler;
			}
		}

		public void RemoveListeners(EventHandler handler)
		{
			foreach (var view in _potionViews)
			{
				view.OnPotionSelected -= handler;
			}
		}

		void UpdateRecipes()
		{
			for (int i = 0; i < _potionViews.Count; ++i)
			{
				if (i < _book.Recipes.Count)
				{
					_potionContainers[i].setEnabled(true);
					_potionViews[i].SetRecipe(_book.Recipes[i]);
				}
				else
				{
					_potionContainers[i].setEnabled(false);
				}
			}
		}

		void CreatePotionViews()
		{
			_potionContainers = new List<iGUIElement>();
			_potionViews = new List<iGUISmartPrefab_PotionDisplay>();

			var potionPlaceholders = potionPane.items;

			for (int i = 0; i < potionPlaceholders.Length; ++i)
			{
				iGUIElement element = ((Placeholder)potionPlaceholders[i]).SwapForSmartObject();
				var view = element.GetComponent<iGUISmartPrefab_PotionDisplay>();
				_potionContainers.Add(element);
				_potionViews.Add(view);
			}
		}

		void HandleOnClick(object sender, EventArgs e)
		{
			var view = sender as iGUISmartPrefab_PotionDisplay;
			Debug.Log("You clicked on: " + view.potionTitle.label.text);
		}
	}
}

