using System;
using System.Collections;
using System.Collections.Generic;

using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class AvatarItemsConfiguration
	{
		public Dictionary<string,ItemCategoryLayerData> Item_Categories { get; set; }
		public Dictionary<string,AvatarItemData> Avatar_Items { get; set; }

		public AvatarItemsConfiguration()
		{
			Item_Categories = new Dictionary<string,ItemCategoryLayerData>();
			Avatar_Items = new Dictionary<string,AvatarItemData>();
		}
	}
}