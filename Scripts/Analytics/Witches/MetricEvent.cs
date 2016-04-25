
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Metrics
{

	// TODO: Breakout to configuration file 

    public class MetricEvent
    {

		// Prologue
		public const string TUTORIAL_STARTED = "TUTORIALSTARTED";
		public const string TUTORIAL_COMPLETED = "TUTORIALCOMPLETED";

		// Story
		public const string STORY_SCENE_READ = "SCENEREAD";
		public const string STORY_SCENE_COMPLETED = "SCENECOMPLETED";
		public const string STORY_ROUTE_COMPLETED = "ROUTECOMPLETED";
		public const string STORY_USE_STAMINA_POTION = "STORYUSESTAMINA";
		public const string STORY_INSUFFICIENT_STAMINA_POTION = "STORYINSUFFICIENTSTAMINAPOTIONS";
//		public const string STORY_INSUFFICIENT_STAMINA = "STORYINSUFFICIENTSTAMINA"; // TODO -- Remove this? is this being relocated?
		public const string STORY_UNLOCKED_ARC = "MAPUNLOCKARC";

		// IAP Shop (Starstones)
		public const string SHOP_STARSTONE_CONFIRM_BUTTON = "STARSTONESSHOPCONFIRM";
		public const string SHOP_STARSTONE_BOUGHT = "STARSTONESSHOPBUY";
        public const string PURCHASE_AFFINITY = "PURCHASE_AFFINITY";

		// IAP Shop (Stamina Potion)
		public const string SHOP_STAMINA_POTION_CONFIRM_BUTTON = "STAMINAPOTIONSSHOPCONFIRM";
		public const string SHOP_STAMINA_POTION_BOUGHT = "STAMINAPOTIONSSHOPBUY";


		// Story Map
		public const string STORYMAP_DISPLAY_DRESSCODE_RESUME = "MAPDRESSRESUME";
		public const string STORYMAP_DISPLAY_DRESSCODE_SHOP = "MAPDRESSSHOP";
		public const string STORYMAP_DISPLAY_DRESSCODE_GETCHANGED = "MAPDRESSGET";

		// Minigame (Page)
		public const string MINIGAMEPAGE_SELECTED_RECIPE = "GAMERECIPESELECTED";
		public const string MINIGAMEPAGE_SELECTED_INGREDIENT = "GAMEINGREDIENTSSELECTED";
		public const string MINIGAMEPAGE_DISPLAY_DIALOGUE_BUY_INGREDIENT = "GAMEBUYINGREDIENTSCONFIRM";
		public const string MINIGAMEPAGE_BOUGHT_INGREDIENT = "GAMEBUYINGREDIENTSDONE";

		// Minigame (Cauldron)
		public const string MINIGAME_DISPLAY_DIALOGUE_CONTINUE = "CAULDRONCONTINUECONFIRM";
		public const string MINIGAME_CONTINUE = "CAULDRONCONTINUEDONE";
		public const string MINIGAME_COMPLETED = "CAULDRONFINISH";

		// Mail
		public const string MAIL_RECIEVED_NEW = "MAILRECIEVEDONE";
		public const string MAIL_OPENED = "MAILOPENDONE";

		// Closet
		public const string CLOSET_PLAYER_WEARS_ITEM = "CLOSETWEAR";
		public const string CLOSET_DISPLAY_DIALOGUE_EXPAND_SPACE = "CLOSETEXPANDCONFIRM";
		public const string CLOSET_EXPAND_SPACE = "CLOSETEXPANDDONE";
		public const string CLOSET_DISPLAY_DIALOGUE_ARCHIVE_ITEM = "CLOSETARCHIVECONFIRM";
		public const string CLOSET_PLAYER_ARCHIVED_ITEM = "CLOSETARCHIVEDONE";

		public const string CLOSET_SAVE_OUTFIT = "CLOSETSAVE";

		// Avatar Shop
		public const string AVATARSHOP_DISPLAY_DIALOGUE_BUY_ITEM = "AVATARSHOPBUYCONFIRM";
		public const string AVATARSHOP_BOUGHT_ITEM = "AVATARSHOPBUYDONE";

		// Inventory
		public const string INVENTORY_USED_POTION = "POTIONUSEDONE";

    }
    

//	public class AdjustTokens
//	{
//		// Revenue
//		public const string STARSTONE_BOUGHT = "yzapr6";
//		public const string STAMINA_POTION_BOUGHT = "un633m";
//
//		// Non-Revenue
//		public const string TUTORIAL_STARTED = "";
//		public const string TUTORIAL_COMPLETED = "";
//	}
}




