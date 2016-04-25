using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class IngredientView : MonoBehaviour
	{
		[HideInInspector]
		public iGUILabel qualityName;

		[HideInInspector]
		public iGUILabel qualityCountLabel;

		[HideInInspector]
		public iGUILabel countLabel;

		[HideInInspector]
		public iGUIButton buy_ingredient_badge;

		public event GUIEventHandler PurchaseRequest;

		public Ingredient Ingredient { get; protected set; }
		public int Count { get; protected set; }

		protected virtual void Start()
		{
		}

		public void SetIngredient(Ingredient ingredient, int count)
		{
			Ingredient = ingredient;
			Count = count;

			qualityName.label.text = ingredient.Name;
			qualityCountLabel.label.text = ingredient.Value.ToString();
			countLabel.label.text = GetQuantityString();
			buy_ingredient_badge.setEnabled(NeedsIngredients());
		}

		public string GetQuantityString()
		{
			if (Ingredient.IsInfinite)
			{
				return "∞";
			}

			return Count.ToString();
		}

		public bool NeedsIngredients()
		{
			if (Ingredient.IsInfinite)
			{
				return false;
			}

			return (Count == 0);
		}

		public void buy_ingredient_badge_Click(iGUIButton sender)
		{
			Debug.Log("Clicked!");
			if (PurchaseRequest != null)
			{
				PurchaseRequest(this, new IngredientPurchaseRequestEventArgs(Ingredient));
			}
		}
	}
}

