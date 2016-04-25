using System.Collections.Generic;
using UnityEngine;
using iGUI;

using Voltage.Witches.Models;
using Voltage.Witches.Util;
using Voltage.Witches.Events;

using IngredientEntry = System.Collections.Generic.KeyValuePair<Voltage.Witches.Models.Ingredient, int>;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class IngredientShelfViewNew : MonoBehaviour
	{
		[HideInInspector]
		public iGUIImage no_ingredient_popup;

		[HideInInspector]
		public iGUIButton arrow_left, arrow_right;

		public List<iGUIButton> Ingredient_Buttons { get; protected set; }
		
		[HideInInspector]
		public iGUIContainer ingredients;
		
		public event GUIEventHandler SelectionChanged;
		
		public float AlertFadeTime = 2.0f;
		public iTweeniGUI.EaseType AlertEaseType = iTweeniGUI.EaseType.easeInBack;
		
		private CircularArray<KeyValuePair<Ingredient, int>> _ingredients;
		private List<iGUIElement> _ingredientContainers;
		private List<IngredientViewNew> _ingredientViews;

		public delegate void ButtonsReadyCallback ();
		public event ButtonsReadyCallback OnComplete;

		protected virtual void Awake()
		{
			_ingredients = new CircularArray<KeyValuePair<Ingredient, int>>();
		}
		
		protected virtual void Start()
		{
			Init();
			no_ingredient_popup.setOpacity(0.0f);
			no_ingredient_popup.consumeClicks = false;
			no_ingredient_popup.ignoreClicks = true;
			no_ingredient_popup.setEnabled(true);
		}
		
		public void Init()
		{
			if (_ingredientViews != null && _ingredientContainers != null)
			{
				return;
			}
			
			_ingredientContainers = new List<iGUIElement>();
			_ingredientViews = new List<IngredientViewNew>();
			LoadPlaceholders();
			UpdateView();
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
			if(_ingredients != null)
			{
				_ingredients = null;
			}
			_ingredients = new CircularArray<KeyValuePair<Ingredient, int>>();
			foreach(var entry in ingredients)
			{
				_ingredients.Add(entry);
			}
			_ingredients.RotateRight(1);
		}

		public void RotateLeft()
		{
			_ingredients.RotateLeft(1);
			UpdateView();
			HandleSelectionChange();
		}

		public void RotateRight()
		{
			_ingredients.RotateRight(1);
			UpdateView();
			HandleSelectionChange();
		}

		public void ExecuteButtonPress(iGUIButton button)
		{
			var index = Ingredient_Buttons.IndexOf(button);
			_ingredientViews[index].Button_Click();
		}
		
		private void LoadPlaceholders()
		{
			Ingredient_Buttons = new List<iGUIButton>();

			for (int i = 0; i < ingredients.items.Length; ++i)
			{
				iGUIElement element;
				if(ingredients.items[i].GetType()== typeof(Placeholder))
				{
					Placeholder ph = ingredients.items[i] as Placeholder;
					element = ph.SwapForSmartObject();
				}
				else
				{
					element = ingredients.items[i];
				}
				var ingredientView = element.GetComponent<iGUISmartPrefab_IngredientViewNew>();
				_ingredientContainers.Add(element);
				_ingredientViews.Add(ingredientView);
				Ingredient_Buttons.Add(ingredientView.buy_ingredient_btn);
			}

			if(OnComplete != null)
			{
				OnComplete();
			}
		}
		
		public void DisplayIngredientAlert()
		{
			no_ingredient_popup.setOpacity(1.0f);
			no_ingredient_popup.fadeTo(0.0f, AlertFadeTime, AlertEaseType);
		}
		
		public KeyValuePair<Ingredient, int> GetSelectedIngredient()
		{
			int selectedIngredient = _ingredientViews.Count / 2;
			return new KeyValuePair<Ingredient, int>(_ingredientViews[selectedIngredient].Ingredient, _ingredientViews[selectedIngredient].Count);
		}
		
		public void UpdateView()
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
		
//		public void SetCategory(IngredientCategory category)
//		{
//			Debug.Log(category.Name + " is the category for this shelf");
//		}
		
		private void HandleSelectionChange()
		{
			if (SelectionChanged != null)
			{
				SelectionChanged(this, new GUIEventArgs());
			}
			
		}
	}
}