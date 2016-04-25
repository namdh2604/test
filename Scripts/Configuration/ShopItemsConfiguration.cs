using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class ShopItemsConfiguration 
	{
		public Dictionary<string,ShopItemData> Shop_Items_Master { get; set; }
		public Dictionary<int,ShopItemData> Shop_Items_Indexed { get; set; }

        // design specifies only one StarterPack ever!
        public ShopItemData StarterPack { get; set; }

		public ShopItemsConfiguration()
		{
			Shop_Items_Master = new Dictionary<string,ShopItemData>();
			Shop_Items_Indexed = new Dictionary<int,ShopItemData>();
		}
	}
}