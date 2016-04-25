using System;
using System.Collections;
using System.Collections.Generic;

using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class IngredientConfiguration
	{
		public bool IsInfinite { get; set; }
		public int Bottle_BG_ID { get; set; }
		public int Premium_Price { get; set; }
		public int Coins_Price { get; set; }
		public int Display_Order { get; set; }
		public int Quality { get; set; }
		public int Currency_Flag { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public string Id { get; set; }
		public string Category_Id { get; set; }
		public string Color_HEX { get; set; }
		public ItemCategory Item_Category { get; set; }
	}
}