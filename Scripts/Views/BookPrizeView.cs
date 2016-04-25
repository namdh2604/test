using System.Collections.Generic;
using UnityEngine;
using iGUI;
using Voltage.Witches.Models;
using Voltage.Witches.Events;

namespace Voltage.Witches.Views
{
	using Debug = UnityEngine.Debug;

	public class BookPrizeView : MonoBehaviour 
	{
		[HideInInspector]
		public iGUIImage level_prize_lacey_boots,level_prize_floral_lace_underwear,level_prize_moonstone_dreamglass,level_prize_stamina_potion,level_prize_starstones;

		public Item Prize_Type { get; protected set; }

		public void SetPrize(Item prize)
		{
			Prize_Type = prize;

			DisplayPrize();
		}

		void DisplayPrize()
		{
			var itemCategory = Prize_Type.Category;

			var isStamina = (itemCategory == ItemCategory.POTION);
			var isStarstones = (itemCategory == ItemCategory.STARSTONES);
			var isIngredient = (itemCategory == ItemCategory.INGREDIENT);
			var isAvatar = (itemCategory == ItemCategory.CLOTHING);

			level_prize_moonstone_dreamglass.setEnabled(isIngredient);
			level_prize_stamina_potion.setEnabled(isStamina);
			level_prize_starstones.setEnabled(isStarstones);

			if(!isAvatar)
			{
				level_prize_lacey_boots.setEnabled(isAvatar);
				level_prize_floral_lace_underwear.setEnabled(isAvatar);
			}
			else
			{
				var clothingCategory = (Prize_Type as Clothing).ClothingCategory;
				var isLingerie = (clothingCategory == ClothingCategory.INTIMATES);
//				Debug.Log("Is Lingerie? :: " + isLingerie.ToString());

				level_prize_lacey_boots.setEnabled((!isLingerie));
				level_prize_floral_lace_underwear.setEnabled(isLingerie);
			}
		}
	}
}