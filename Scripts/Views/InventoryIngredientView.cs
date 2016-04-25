using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	public class InventoryIngredientView : MonoBehaviour
	{
		[HideInInspector]
		public iGUIImage ingredients_infinite_counter,ingredients_icons_MA,dried_inside_01,dried_inside_02,liquid_inside,
						 gold_large,silver_large,bronze_large,sphere_inside_01;

		[HideInInspector]
		public iGUILabel ingredients_counter, ingredient_name_text, quality_tag_counter;

		[HideInInspector]
		public iGUIContainer quality_tag, ingredients_counter_grp;

		[HideInInspector]
		public iGUIButton ingredient_button;

		public string MyIngredient { get; protected set; }

		public GUIEventHandler OnIngredientClick;
		private Ingredient _myIngredient;
		int _count;

		public void SetIngredient(Ingredient ingredient, int count)
		{
			_myIngredient = ingredient;
			_count = count;
			MyIngredient = _myIngredient.Name;
			ingredient_name_text.label.text = MyIngredient;

			var isBasicIngredient = ((_myIngredient.IsInfinite) && (_myIngredient.Value <= 10));
			var isPremium = (!isBasicIngredient);
			ingredients_counter.setEnabled(isPremium);

			ingredients_infinite_counter.setEnabled(isBasicIngredient);
			ingredients_counter.label.text = GetQuantityString();
			ingredients_counter.style.alignment = TextAnchor.MiddleCenter;
			quality_tag_counter.label.text = _myIngredient.Value.ToString();
			quality_tag_counter.style.alignment = TextAnchor.MiddleCenter;

			DisplayBottle();
			UpdateColor();

			UpdateIcon();
			UpdateQualityLabel();
		}

		void DisplayBottle()
		{
			if((int)_myIngredient.BottleType > 3)
			{
				string error = string.Format("INGREDIENT {0} has an invalid bottle type :: {1}",_myIngredient.Name,(int)_myIngredient.BottleType);
				throw new System.Exception(error);
			}

			var isLiquid = (_myIngredient.BottleType == BottleBGType.LIQUID);
			var isDried1 = (_myIngredient.BottleType == BottleBGType.DRIED_1);
			var isDried2 = (_myIngredient.BottleType == BottleBGType.DRIED_2);
			var isSphere = (_myIngredient.BottleType == BottleBGType.SPHERE);

			dried_inside_01.setEnabled(isDried1);
			dried_inside_02.setEnabled(isDried2);
			liquid_inside.setEnabled(isLiquid);
			sphere_inside_01.setEnabled(isSphere);
		}

		void UpdateColor()
		{
			iGUIImage[] images = new iGUIImage[]{ dried_inside_01,dried_inside_02,liquid_inside,sphere_inside_01 };
			var image = GetActiveImage(images);
			try
			{
				image.setColor(HexToColor(_myIngredient.Color));
			}
			catch(System.Exception)
			{
				string error = string.Format("INGREDIENT {0} has an invalid hex code for color :: {1}",_myIngredient.Name,_myIngredient.Color);
				throw new System.Exception(error);
			}
		}

		iGUIImage GetActiveImage(iGUIImage[] images)
		{
			iGUIImage image = null;
			for(int i = 0; i < images.Length; ++i)
			{
				if(images[i].enabled)
				{
					image = images[i];
				}
			}

			return image;
		}

		void UpdateIcon()
		{
			ingredients_icons_MA.image = Resources.Load<Texture2D>(_myIngredient.IconFilePath);
			ingredients_icons_MA.scaleMode = ScaleMode.ScaleToFit;
		}

		void UpdateQualityLabel()
		{
			var isBronze = (_myIngredient.QualityBadge == QualityBadge.BRONZE);
			var isSilver = (_myIngredient.QualityBadge == QualityBadge.SILVER);
			var isGold = (_myIngredient.QualityBadge == QualityBadge.GOLD);

			bronze_large.setEnabled(isBronze);
			silver_large.setEnabled(isSilver);
			gold_large.setEnabled(isGold);
		}

		string GetQuantityString()
		{
			if((_myIngredient.IsInfinite) && (_myIngredient.QualityBadge == QualityBadge.NONE))
			{
				return "∞";
			}
			
			return _count.ToString();
		}

		Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			return new Color32(r,g,b, 255);
		}

		public void Ingredient_Click(iGUIButton sender)
		{
			if(OnIngredientClick != null)
			{
				OnIngredientClick(this, new IngredientPurchaseRequestEventArgs(_myIngredient));
			}
		}
	}
}