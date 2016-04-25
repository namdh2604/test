using System;
using Voltage.Witches.Models;

namespace Voltage.Witches.Events
{
	public class PremiumPurchaseRequestEventArgs : GUIEventArgs
	{
		public ShopItemData Shop_Item { get; protected set; }

		public PremiumPurchaseRequestEventArgs(ShopItemData data)
		{
			Shop_Item = data;
		}
	}
}