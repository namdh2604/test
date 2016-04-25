using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class InventoryIngredientShelfView : MonoBehaviour 
	{
		[HideInInspector]
		public iGUIScrollView scrollAble;
		
		[HideInInspector]
		public iGUIContainer Ingredients_Container;
		
		[HideInInspector]
		public Placeholder ingredientPlaceHolder_0;
		
		private List<iGUISmartPrefab_InventoryIngredient> _ingredientViews;
		private List<iGUIContainer> _ingredientContainers;

		public GUIEventHandler OnIngredientSelection;
		public GUIEventHandler IngredientViewsSetUp;

		public List<iGUIButton> Ingredient_Buttons;

		public void SetUpView(int ingredientCount)
		{
			if(ingredientCount > 0)
			{
				Rect newPlaceholderRect = ingredientPlaceHolder_0.positionAndSize;
				newPlaceholderRect.x = 0.001f;
				for(int i = 1; i < ingredientCount; ++i)
				{
					CloneAndAddIngredientPlaceholder(i,newPlaceholderRect);
				}
			}
		}

		public void SetIngredient(int index, Ingredient ingredient,int count)
		{
			_ingredientViews[index].SetIngredient(ingredient,count);
		}

		void CloneAndAddIngredientPlaceholder(int index, Rect positionAndSize)
		{
			string newObjectName = "ingredient_" + index.ToString();
			GameObject clonedPlaceholder = Instantiate(ingredientPlaceHolder_0.gameObject,Vector3.zero,Quaternion.identity) as GameObject;
			clonedPlaceholder.name = newObjectName;
			clonedPlaceholder.gameObject.transform.parent = Ingredients_Container.gameObject.transform;
			iGUIElement element = clonedPlaceholder.GetComponent<iGUIElement>();
			element.setPositionAndSize(positionAndSize);
			element.setVariableName(newObjectName);
			element.order = index;
		}
		
		protected virtual void Start()
		{
			ResetPositions();
			scrollAble.hideScrollBars = true;
		}

		float GetScaleFactor(float groupsOfTen)
		{
			var scaleFactor = 2.5f;

			if(groupsOfTen <= 10)
			{
				scaleFactor = 2.5f + (Mathf.FloorToInt(groupsOfTen) * 0.02f);
			}
			else
			{
//				0.0125f
//				var length = Mathf.FloorToInt(groupsOfTen).ToString("D");
//				char[] digits = length.ToCharArray();
//				Debug.LogWarning(digits.Length.ToString() + " is the number of digits in the group");
				scaleFactor = 2.5f + (Mathf.FloorToInt(groupsOfTen) * ((groupsOfTen / 10) * 0.0125f));
			}

			return scaleFactor;
		}

		void ResetPositions ()
		{
			//TODO adjust some of these values to best fit with the ingredient views
			var totalItems = Ingredients_Container.itemCount;
//			var numberOfPlaces = totalItems.ToString("D").Length - 1;
//			Debug.LogWarning(numberOfPlaces.ToString());
			var groupsOfTen = (float)totalItems / 10.0f;
//			Debug.LogWarning(groupsOfTen.ToString());
			
			if(groupsOfTen > 0.4f)
			{
				var scaleFactor = GetScaleFactor(groupsOfTen);
//				Debug.LogWarning(scaleFactor.ToString());
				scrollAble.setAreaWidth(groupsOfTen * scaleFactor);
				var scrollAbleWidth = scrollAble.areaWidth;
//				var numberOfDecimals = Mathf.FloorToInt(groupsOfTen).ToString("D").Length;

//				var power = Mathf.Pow(10.0f,numberOfDecimals);

//				Debug.LogWarning(power.ToString());
				scrollAble.setAreaWidth(scrollAbleWidth - ((groupsOfTen - 1) * 0.5f));
				ingredientPlaceHolder_0.setX(0.05f / groupsOfTen);
			}
			else if((groupsOfTen > 0.1f) && (groupsOfTen <= 0.4f))
			{
				scrollAble.setAreaWidth(0.8f);
				var scrollAbleWidth = scrollAble.areaWidth;
				scrollAble.setAreaWidth(scrollAbleWidth - ((groupsOfTen - 1) * 0.5f));
				ingredientPlaceHolder_0.setX(0.05f / groupsOfTen);
			}
			else
			{
				scrollAble.setAreaWidth(0.7f);
				var scrollAbleWidth = scrollAble.areaWidth;
				scrollAble.setAreaWidth(scrollAbleWidth - ((groupsOfTen - 1) * 0.5f));
				ingredientPlaceHolder_0.setX(0.05f / groupsOfTen);
			}

			LoadPlaceholders();
		}
		
		void LoadPlaceholders ()
		{
			_ingredientViews = new List<iGUISmartPrefab_InventoryIngredient>();
			_ingredientContainers = new List<iGUIContainer>();
			
			var totalPlaceholders = Ingredients_Container.items;

			Ingredient_Buttons = new List<iGUIButton>();
			for(int i = 0; i < totalPlaceholders.Length; ++i)
			{
				var indexOrder = totalPlaceholders[i].order;
				iGUIElement element = ((Placeholder)totalPlaceholders[i]).SwapForSmartObject();
				element.setOrder(indexOrder);
				var view = element.GetComponent<iGUISmartPrefab_InventoryIngredient>();
				view.OnIngredientClick += HandleIngredientClick;
				_ingredientContainers.Add((iGUIContainer)element);
				_ingredientViews.Add(view);
				Ingredient_Buttons.Add(view.ingredient_button);
			}
			
			Ingredients_Container.refreshRect();
			if(IngredientViewsSetUp != null)
			{
				IngredientViewsSetUp(this, new GUIEventArgs());
			}
		}

		public void ExecuteIngredientClick(iGUIButton button)
		{
			var index = Ingredient_Buttons.IndexOf(button);
			var view = _ingredientViews[index];
			view.Ingredient_Click(button);
		}

		void HandleIngredientClick(object sender, GUIEventArgs e)
		{
//			var IngredientArgs = e as IngredientPurchaseRequestEventArgs;

			if(OnIngredientSelection != null)
			{
				OnIngredientSelection(sender,e);
			}
		}
	}
}