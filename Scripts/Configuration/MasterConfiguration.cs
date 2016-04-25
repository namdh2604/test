using System;
using System.Collections;
using System.Collections.Generic;

using Voltage.Witches.Models;
using Voltage.Witches.Components;

namespace Voltage.Witches.Configuration
{
	public class MasterConfiguration
	{
        // HACK: no unique id for: starstones, coins, stamina potion
        public const string STARSTONE_ID = "starstone";                // readonly static?
        public const string COIN_ID = "coin";
        public const string STAMINA_POTION_ID = "stamina_potion";      // TODO: verify that stamina potion is not included in Master Item list


        public const string ADDITIONAL_CLOSET_SLOTS_ITEM_ID = "AdditionalClosetSlots";

//		public string BaseURL { get; set; }			
		public string GameResources { get; set; }	// FIXME: Doesn't get set by MasterConfigParser

		public int Version { get; set; }
		public Dictionary<string,IngredientData> Ingredients { get; protected set; }
		public Dictionary<string,CategoryData> Categories { get; protected set; }
		public Dictionary<string,GamePropertiesData> Game_Properties { get; protected set; }
		public float Focus_Refresh_Rate { get; set; }
		public Dictionary<string,ItemCategoryLayerData> Item_Categories { get; protected set; }
		public float Ticket_Refresh_Rate { get; set; } 
		public GamePropertiesConfiguration Game_Properties_Config { get; set; }
		public BooksConfiguration Books_Configuration { get; set; }
		public PotionsConfiguration Potions_Configuration { get; set; }
		public AvatarItemsConfiguration Avatar_Items_Configuration { get; set; }
		public RecipesConfiguration Recipes_Configuration { get; set; }
		public Dictionary<string,AffinityConfiguration> Affinity_Configs { get; set; }
		public AffinityMap Affinity_Map { get; set; }
		public Dictionary<string, ItemConfiguration> Items_Master { get; set; }
		public ShopItemsConfiguration Shop_Items { get; set; }
		public Dictionary<string,CharacterData> Character_Info { get; set; }
		public Dictionary<string,BookPrizeData> Book_Prizes { get; set; }
        public int Max_Focus { get; set; }
        public int Max_Tickets { get; set; }

		public Dictionary<string,string> BookUnlockMap { get; set; }	// could move into BooksConfiguration

		public MasterConfiguration()
		{
			Ingredients = new Dictionary<string, IngredientData>();
			Categories = new Dictionary<string, CategoryData>();
			Game_Properties = new Dictionary<string, GamePropertiesData>();
			Item_Categories = new Dictionary<string, ItemCategoryLayerData>();
			Affinity_Configs = new Dictionary<string, AffinityConfiguration>();
			Items_Master = new Dictionary<string, ItemConfiguration>();
			Shop_Items = new ShopItemsConfiguration();
			Character_Info = new Dictionary<string, CharacterData>();
			Book_Prizes = new Dictionary<string, BookPrizeData>();

			Books_Configuration = new BooksConfiguration ();
			Avatar_Items_Configuration = new AvatarItemsConfiguration ();
		}
	}
}





