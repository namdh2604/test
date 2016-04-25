using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Controllers;
using Voltage.Witches.Screens;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Voltage.Common.Logging;
	using Voltage.Story.Variables;
	using Debug = UnityEngine.Debug;

	public class SpellbookViewNew : MonoBehaviour
	{
		[HideInInspector]
		public iGUIContainer potionPane;
		
		[HideInInspector]
		public iGUIImage Spellbook_Complete_Badge;

		[HideInInspector]
		public iGUILabel Spellbook_Title_Header;
		
		[HideInInspector]
		public iGUIContainer book;
				
		[HideInInspector]
		public Placeholder bookPrizePlaceholder;

		iGUISmartPrefab_BookPrizeView _prizeView;
		List<iGUISmartPrefab_PotionDisplayNew> _potionViews;
		List<iGUIElement> _potionContainers;

		public List<iGUIButton> Recipe_Buttons { get; protected set; }

		VariableMapper _mapper;
		Spellbook _book;
		private static string _tokenCapture = @"(\[+.+?\])";
		

		protected virtual void Awake()
		{
		}
		
		protected virtual void Start()
		{
			Init();

			SwapPlaceholder();
			SetPrize();
		}

		public void SetVariableMapper(VariableMapper mapper)
		{
			_mapper = mapper;
//			if(_mapper != null)
//			{
//				Debug.LogWarning("Mapper Exists? " + (_mapper != null).ToString());
//			}
		}

		public void Init()
		{
			if (_potionViews == null)
			{
				CreatePotionViews();
			}
		}

		void SwapPlaceholder()
		{
			var element = bookPrizePlaceholder.SwapForSmartObject() as iGUIContainer;
			_prizeView = element.GetComponent<iGUISmartPrefab_BookPrizeView>();
		}

		void SetPrize()
		{
			if(_book != null)
			{
				_prizeView.SetPrize(_book.ClearItems[0].Key);
			}
			_prizeView.GetComponent<iGUIElement>().setLayer(9);
		}

		public void SetBook(Spellbook book)
		{
			AmbientLogger.Current.Log (string.Format ("Book Selected: {0} [{1}]", book.Name, book.Id), LogLevel.INFO);

			_book = book;
			
			if(book.IsClear())
			{
				Spellbook_Complete_Badge.setEnabled(true);
			}
			else
			{
				Spellbook_Complete_Badge.setEnabled(false);
			}

//			Debug.LogWarning(_book.ClearItems.Count.ToString());
			UpdateHeader();
			UpdateRecipes();
			if(_prizeView != null)
			{
				SetPrize();
			}
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

		void UpdateHeader()
		{
			var title = _book.Name;

			var matches = Regex.Matches(title, _tokenCapture);
			foreach(Match group in matches)
			{
				foreach(Capture capture in group.Captures)
				{
					var pattern = capture.ToString();
					var match = pattern.Substring(1,capture.Length - 2);
					string replacement = string.Empty;
					if(_mapper.TryGetValue(match, out replacement))
					{
						title = title.Replace(pattern,replacement);
					}
				}
			}

			Spellbook_Title_Header.label.text = title;
		}
		
		void UpdateRecipes()
		{
			for (int i = 0; i < _potionViews.Count; ++i)
			{
				if (i < _book.Recipes.Count)
				{
					AmbientLogger.Current.Log (string.Format ("Refreshing Recipe: {0} [{1}]", _book.Recipes[i].Name, _book.Recipes[i].Id), LogLevel.INFO);

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
			_potionViews = new List<iGUISmartPrefab_PotionDisplayNew>();
			
			var potionPlaceholders = potionPane.items;
			Recipe_Buttons = new List<iGUIButton>();
			for (int i = 0; i < potionPlaceholders.Length; ++i)
			{
				iGUIElement element;
				if(potionPlaceholders[i].GetType() == typeof(Placeholder))
				{
					element = ((Placeholder)potionPlaceholders[i]).SwapForSmartObject();
				}
				else
				{
					element = potionPlaceholders[i];
				}
				var view = element.GetComponent<iGUISmartPrefab_PotionDisplayNew>();
				_potionContainers.Add(element);
				_potionViews.Add(view);
				Recipe_Buttons.Add(view.potionButton);
			}
		}

		public void ExecuteRecipeClick(int index)
		{
			_potionViews[index].SelectPotion();
		}

		void HandleOnClick(object sender, EventArgs e)
		{
			var view = sender as iGUISmartPrefab_PotionDisplayNew;
			Debug.Log("You clicked on: " + view.potionTitle.label.text);
		}

	}
}