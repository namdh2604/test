using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class IngredientViewNew : MonoBehaviour 
	{	
		[HideInInspector]
		public iGUILabel quality_tag_label,ingredient_count_label;
		
		[HideInInspector]
		public iGUIButton buy_ingredient_btn;

		[HideInInspector]
		public iGUIImage BUY_text,buy_item_cell,ingredients_icons_MA_50,gold_badges,bronze_badges,silver_badges,artemisia_01,artemisia_02,artemisia_03,
						 moonstone_01,moonstone_02,moonstone_03,silver_01,silver_02,silver_03,juniper_01,juniper_02,juniper_03,mayapple_01,mayapple_02,
						 mayapple_03,rowan_01,rowan_02,rowan_03,water_01,water_02,water_03,rosemary_01,rosemary_02,rosemary_03;
		
		public event GUIEventHandler PurchaseRequest;
		
		public Ingredient Ingredient { get; protected set; }
		public int Count { get; protected set; }
		
		protected virtual void Start()
		{
		}
		
		public void SetIngredient(Ingredient ingredient, int count)
		{
			Ingredient = ingredient;
			if(Ingredient.IsInfinite)
			{
				count = 1;
			}
			Count = count;

			quality_tag_label.label.text = ingredient.Value.ToString();
			ingredient_count_label.label.text = GetQuantityString();
			buy_ingredient_btn.setEnabled(NeedsIngredients());
			BUY_text.setEnabled(NeedsIngredients());
			buy_item_cell.setEnabled(NeedsIngredients());

			var isBronze = (Ingredient.QualityBadge == QualityBadge.BRONZE);
			var isSilver = (Ingredient.QualityBadge == QualityBadge.SILVER);
			var isGold = (Ingredient.QualityBadge == QualityBadge.GOLD);

			bronze_badges.setEnabled(isBronze);
			silver_badges.setEnabled(isSilver);
			gold_badges.setEnabled(isGold);

			UpdateIcon();
		}

		void UpdateIcon()
		{
			ingredients_icons_MA_50.setEnabled(true);
			ingredients_icons_MA_50.image = Resources.Load<Texture2D>(Ingredient.IconFilePath);
			ingredients_icons_MA_50.scaleMode = ScaleMode.ScaleToFit;
		}

		public string GetQuantityString()
		{
			if(Ingredient.IsInfinite)
			{
				return "∞";
			}
			else if((Ingredient.IsInfinite) && (Count > 0))
			{
				return "∞";
			}
			
			return Count.ToString();
		}
		
		public bool NeedsIngredients()
		{
			if((Ingredient.IsInfinite) && (Count > 0))
			{
				return false;
			}
			
			return (Count <= 0);
		}

		public void Button_Click()
		{
			Debug.Log("Clicked!");
			if(PurchaseRequest != null)
			{
				PurchaseRequest(this, new IngredientPurchaseRequestEventArgs(Ingredient));
			}
		}
	}
}