using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Models
{
	public interface IAvatarData
	{
		List<ItemCategoryLayerData> Item_categories { get; }
		List<AvatarItemData> Avatar_items { get; }
	}

	public class AvatarItemsData: IAvatarData
	{
		public List<ItemCategoryLayerData> item_categories;
		public List<AvatarItemData> avatar_items;

		public List<ItemCategoryLayerData> Item_categories { get; protected set; }
		public List<AvatarItemData> Avatar_items { get; protected set; }

		public void SetUp()
		{
			Item_categories = item_categories;
			Avatar_items = avatar_items;
		}

//		public AvatarItemsData()
//		{
//			Item_categories = item_categories;
//			Avatar_items = avatar_items;
//		}
	}
}
