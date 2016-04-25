using UnityEngine;
using System.Collections;
using Voltage.Witches.Util;

namespace Voltage.Witches.Models
{
	public class StarStoneItem : Item
	{
		public int Count { get; set; }
		public string CallPath { get; set; }
		public string IconFilePath { get; set; } 

		public StarStoneItem(string id):base(id)
		{
            Name = id.Capitalize();
			Category = ItemCategory.STARSTONES;
			IconFilePath = "MailboxAssets/icon_starstone";
			//TODO Add the html path or some indicator to denote what call needs to be made to the server
			CallPath = string.Empty;
		}

	}
}