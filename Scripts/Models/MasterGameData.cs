using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
	public class MasterGameData
	{
		public int version;
		public List<IngredientData> ingredient_mst;
		public List<CategoryData> category_mst;
		public List<GamePropertiesData> game_mst;
		public float focus_refresh_rate;
		public float ticketRefreshRate;
		public List<SpellBookData> book_mst;
		public List<AffinityData> affinity_mst;
		public List<ShopItemData> shop_items_mst;
		public List<CharacterData> characters_mst;
		public List<RecipeData> recipes_mst;
		public List<PotionData> potions_mst;
		public List<AvatarItemData> avatar_items_mst;
		public List<BookPrizeData> book_prizes;
        public int max_focus;
        public int max_tickets;

		public Dictionary<string,string> book_unlock;
	}
}