using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Models
{
	public class IngredientData: BaseData
	{
		public string description;
		public string category_id;
		public int display_order;
		public int quality;
		public bool isInfinite;
		public int bottle_bg;
		public string color;
		public float coins_price;
		public float premium_price;
		public int currency_flag;

		public IngredientData()
		{
			item_cat = 1;
		}
	}
}