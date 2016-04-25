using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Models
{
	public interface IMasterData
	{
		int Version { get; }
		List<IngredientData> Ingredient_mst { get; }
		List<CategoryData> Category_mst { get; }
		List<GamePropertiesData> Game_mst { get; }
		float Focus_refresh_rate { get; }
		List<ItemCategoryLayerData> Item_category_mst { get; }
		float TicketRefreshRate { get; }
	}

	public class MasterData: IMasterData
	{
//		public string version = string.Empty;
//		public string ingredient_mst = string.Empty;
//		public string category_mst = string.Empty;
//		public string game_mst = string.Empty;
//		public string focus_refresh_rate = string.Empty;
//		public string item_category_mst = string.Empty;
//		public string ticketRefreshRate = string.Empty;

//		public int version;
//		public List<IngredientData> ingredient_mst;
//		public List<CategoryData> category_mst;
//		public List<GamePropertiesData> game_mst;
//		public float focus_refresh_rate;
//		public List<ItemCategoryLayerData> item_category_mst;
//		public float ticketRefreshRate; 

		public int Version { get; protected set; }
		public List<IngredientData> Ingredient_mst { get; protected set; }
		public List<CategoryData> Category_mst { get; protected set; }
		public List<GamePropertiesData> Game_mst { get; protected set; }
		public float Focus_refresh_rate { get; protected set; }
		public List<ItemCategoryLayerData> Item_category_mst { get; protected set; }
		public float TicketRefreshRate { get; protected set; } 
//		public GamePropMaster GameProps { get; protected set; }

		public MasterData(int vers,List<IngredientData> ingrees,List<CategoryData> cats,List<GamePropertiesData> props, float focus,List<ItemCategoryLayerData> items,float tickets)
		{
			Version = vers;
			Ingredient_mst = ingrees;
			Category_mst = cats;
			Game_mst = props;
//			GameProps = Game_mst;
			Focus_refresh_rate = focus;
			Item_category_mst = items;
			TicketRefreshRate = tickets;
		}
	}
}