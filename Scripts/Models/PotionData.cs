using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public class PotionData: BaseData 
	{
		public string description;
		public string color;
		public List<Dictionary<string,int>> effect_list;
		public int type;
		public int display_order;

		public PotionData()
		{
			item_cat = 0;
		}
	}
}