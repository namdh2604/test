using iGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Voltage.Witches.Models;

namespace Voltage.Witches.Screens
{
	public class IngredientDetailsDialog : AbstractDialog, IDialog
	{
		[HideInInspector]
		public Placeholder ingredientPlaceholder;

		[HideInInspector]
		public iGUIImage bronze_large,silver_large,gold_large;

		[HideInInspector]
		public iGUILabel corner_label_label,quality_tag_label,main_header_ingredient_name,detail_description_label;

		[HideInInspector]
		public iGUIButton close_btn;

		iGUISmartPrefab_InventoryIngredient _ingredientView;

		Ingredient _selectedIngredient;
		int _count;

		IGUIHandler _buttonHandler;

		protected void Awake()
		{
			_buttonHandler = gameObject.AddComponent<IGUIHandler>();
			_buttonHandler.MovedAway += HandleMovedAway;
			_buttonHandler.MovedBack += HandleMovedBack;
			_buttonHandler.ReleasedButtonEvent += HandleReleasedButtonEvent;
			_buttonHandler.InputCancelledEvent += HandleReleasedButtonEvent;
		}

		protected virtual void Start()
		{
			close_btn.clickDownCallback += ClickInit;
			LoadPlaceholder();
			SetName();
			SetQuality();
			SetQuantity();
			SetDescription();
		}

		public void SetIngredient(Ingredient ingredient, int count)
		{
			_count = count;
			_selectedIngredient = ingredient;
		}

		void LoadPlaceholder()
		{
			var element = ingredientPlaceholder.SwapForSmartObject() as iGUIContainer;
			_ingredientView = element.GetComponent<iGUISmartPrefab_InventoryIngredient>();
			_ingredientView.SetIngredient(_selectedIngredient, _count);

			_ingredientView.quality_tag.setEnabled(false);
			_ingredientView.ingredients_counter_grp.setEnabled(false);
		}

		void SetName()
		{
			main_header_ingredient_name.label.text = _selectedIngredient.Name;
		}

		void SetQuality ()
		{
			quality_tag_label.label.text = _selectedIngredient.Value.ToString();

			var isBronze = (_selectedIngredient.QualityBadge == QualityBadge.BRONZE);
			var isSilver = (_selectedIngredient.QualityBadge == QualityBadge.SILVER);
			var isGold = (_selectedIngredient.QualityBadge == QualityBadge.GOLD);

			bronze_large.setEnabled(isBronze);
			silver_large.setEnabled(isSilver);
			gold_large.setEnabled(isGold);
		}

		void SetQuantity()
		{
			corner_label_label.label.text = GetQuantityString();
		}

		void SetDescription()
		{
			//TODO check against limitations of characters in string
			detail_description_label.label.text = _selectedIngredient.Description;
		}

		string GetQuantityString()
		{
			if((_selectedIngredient.IsInfinite))
			{
				return "∞";
			}
			
			return _count.ToString();
		}

		void ClickInit(iGUIElement element)
		{
			if((_buttonHandler.IsActive) && (_buttonHandler.PressedButton == null))
			{
				var button = (iGUIButton)element;
				_buttonHandler.SelectButton(button);
				button.colorTo(Color.grey,0f);
			}
		}

		void HandleMovedAway(iGUIButton button)
		{
			button.colorTo(Color.white,0.3f);
		}

		void HandleMovedBack(iGUIButton button)
		{
			button.colorTo(Color.grey,0f);
		}

		void HandleReleasedButtonEvent(iGUIButton button, bool isOverButton)
		{
			if(isOverButton)
			{
				if(button == close_btn)
				{
					SubmitResponse((int)DialogResponse.OK);
				}
			}
			button.colorTo(Color.white,0.3f);
		}
	}
}