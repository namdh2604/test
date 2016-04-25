using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public class ShopItemData: BaseData
	{
		public string product_id;
		public float price;
		public int premium_qty;
		public int item_index;
		public BundleItems bundle_items;
	}

	public class BundleItems
	{
		public int Stamina;
		public int Starstone;
		public List<string> Avatar;
	}
}