using System.Collections.Generic;
using UnityEngine;
using iGUI;

using Voltage.Witches.Models;
using Voltage.Witches.Util;
using Voltage.Witches.Events;

using IngredientEntry = System.Collections.Generic.KeyValuePair<Voltage.Witches.Models.Ingredient, int>;

namespace Voltage.Witches.Views
{
	public class IngredientShelfView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIImage insufficient_ingredient_popup;

		[HideInInspector]
		public iGUILabel categoryLabel;

		[HideInInspector]
		public iGUIContainer ingredients;

		public event GUIEventHandler SelectionChanged;

		public float AlertFadeTime = 2.0f;
		public iTweeniGUI.EaseType AlertEaseType = iTweeniGUI.EaseType.easeInBack;

		private CircularArray<KeyValuePair<Ingredient, int>> _ingredients;
		private List<iGUIElement> _ingredientContainers;
		private List<IngredientView> _ingredientViews;

		protected virtual void Awake()
		{
			_ingredients = new CircularArray<KeyValuePair<Ingredient, int>>();
		}

		protected virtual void Start()
		{
			Init();
			insufficient_ingredient_popup.setOpacity(0.0f);
			insufficient_ingredient_popup.consumeClicks = false;
			insufficient_ingredient_popup.ignoreClicks = true;
			insufficient_ingredient_popup.setEnabled(true);

//			UpdateView();
//			HandleSelectionChange();
		}

		public void Init()
		{
			if (_ingredientViews != null && _ingredientContainers != null)
			{
				return;
			}

			_ingredientContainers = new List<iGUIElement>();
			_ingredientViews = new List<IngredientView>();
			LoadPlaceholders();
			UpdateView();
//			HandleSelectionChange();
		}

		public void AddPurchaseRequestHandler(GUIEventHandler handler)
		{
			foreach (var view in _ingredientViews)
			{
				view.PurchaseRequest += handler;
			}
		}

		public void RemovePurchaseRequestHandler(GUIEventHandler handler)
		{
			foreach (var view in _ingredientViews)
			{
				view.PurchaseRequest -= handler;
			}
		}

		public void SetIngredients(List<KeyValuePair<Ingredient, int>> ingredients)
		{
			_ingredients = new CircularArray<KeyValuePair<Ingredient, int>>();
			foreach (var entry in ingredients)
			{
				_ingredients.Add(entry);
			}
		}

		public void shelf1_arrow_left_Click(iGUIButton sender)
		{
			_ingredients.RotateLeft(1);
			UpdateView();
			HandleSelectionChange();
		}

		public void shelf1_arrow_right_Click(iGUIButton sender)
		{
			_ingredients.RotateRight(1);
			UpdateView();
			HandleSelectionChange();
		}

		private void LoadPlaceholders()
		{
			for (int i = 0; i < ingredients.items.Length; ++i)
			{
				Placeholder ph = ingredients.items[i] as Placeholder;
				iGUIElement element = ph.SwapForSmartObject();
				var ingredientView = element.GetComponent<iGUISmartPrefab_IngredientView>();
				_ingredientContainers.Add(element);
				_ingredientViews.Add(ingredientView);
			}
		}

		public void DisplayIngredientAlert()
		{
			insufficient_ingredient_popup.setOpacity(1.0f);
			insufficient_ingredient_popup.fadeTo(0.0f, AlertFadeTime, AlertEaseType);
		}

		public KeyValuePair<Ingredient, int> GetSelectedIngredient()
		{
			int selectedIngredient = _ingredientViews.Count / 2;
			return new KeyValuePair<Ingredient, int>(_ingredientViews[selectedIngredient].Ingredient, _ingredientViews[selectedIngredient].Count);
		}

		private void UpdateView()
		{
			for (int i = 0; i < _ingredientViews.Count; ++i)
			{
				var view = _ingredientViews[i];
				var container = _ingredientContainers[i];

				if (i < _ingredients.Count)
				{
					view.SetIngredient(_ingredients[i].Key, _ingredients[i].Value);

					container.setEnabled(true);
				}
				else
				{
					container.setEnabled(false);
				}
			}
		}

		public void SetCategory(IngredientCategory category)
		{
			categoryLabel.label.text = category.Name;
		}

		private void HandleSelectionChange()
		{
			if (SelectionChanged != null)
			{
				SelectionChanged(this, new GUIEventArgs());
			}

		}
	}
}

