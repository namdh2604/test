using UnityEngine;
using System.Collections;
using Voltage.Witches.Util;

namespace Voltage.Witches.Models
{
	public class CoinItem : Item
	{
		public int Count { get; set; }
		public string CallPath { get; set; }
		public string IconFilePath { get; set; } 

		public CoinItem(string id):base(id)
		{
            Name = id.Capitalize();
			Category = ItemCategory.COINS;
			IconFilePath = "MailboxAssets/icon_coin";
			CallPath = string.Empty;
		}
	}
}