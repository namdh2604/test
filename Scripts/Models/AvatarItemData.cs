using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Models
{
	public class AvatarItemData: BaseData 
	{
		public string description;
		public string category_id;
		public int display_order;
		public string layer_name;
		public int slots_layer;
		public int coins_price;
		public int premium_price;
		public int currency_flag;

		public AvatarItemData()
		{
			item_cat = 2;
		}
	}
}
