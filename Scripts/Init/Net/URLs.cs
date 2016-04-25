
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Net
{

	public class URLs			
	{

		// Initialization
		public const string RESTORE_PLAYER = "witches/start_restore/1";
		public const string RESTORE_EMAIL_DETAILS = "";

		public const string POST_BUILDVERSION = "witches/get_environment/1";
		public const string POST_CREATE_USER = "witches/create_user/1";
		public const string GET_MASTER = "witches/master/get_all_master/1";
		public const string REQUEST_PASSWORD = "witches/get_password/1";

		// KPI
		public const string KPI_LOGIN = "witches/login/1";

		// Mailbox
		public const string CHECK_MAIL = "witches/get_mail_badge/1";
		public const string GET_MAIL = "witches/get_all_mails/1";
		public const string OPEN_MAIL = "witches/open_mail/1";
//		public const string GET_ITEM_FROM_MAIL = "witches/pick_up_gifts/1";

		// Shop
		public const string BUY_WITH_COINS = "witches/buy_with_coins/1";
		public const string BUY_WITH_STONES = "witches/buy_with_stones/1";
		public const string BUY_PREMIUM = "witches/buy_inapp/1";					
	
//		public const string HOWTOS_PROGRESS = "witches/howtos_progress/1";				
		public const string INPUT_NAMES = "witches/input_names/1";					

		// Closet
		public const string REMOVE_AVATAR_ITEM = "witches/remove_avatar_item/1";	// TODO Server
        public const string UPDATE_OUTFIT = "witches/update_outfit/1";
		public const string SAVE_OUTFIT = "witches/save_coordination/1"; // Not Being Used

		// Minigame
		public const string SAVE_RECIPE = "witches/save_potion_result/1";		
		public const string USE_POTION = "witches/use_potion/1";				
		public const string USE_FOCUS = "witches/use_focus/1";
		public const string USE_INGREDIENT = "witches/use_ingredient/1";
		public const string BUY_FOCUS = "witches/buy_tickets/1";

		// Story
        public const string START_SCENE = "witches/start_scene/1";
		public const string UPDATE_PLAYERSTORY_STATE = "witches/update_playerstorystate/1";
		public const string UPDATE_COMPLETED_SCENE = "witches/complete_scene/1";
//		public const string USE_STAMINA = "witches/use_stamina/1";
		public const string PLAYER_RESET = "witches/story_reset/1";
		public const string MAIL_ON_SCENE_COMPLETE = "witches/check_scene_mail/1";

        // Resources
        public const string SYNC_RESOURCES = "witches/sync_resources/1";
        public const string UPDATE_STAMINA = "witches/update_stamina/1";
        public const string UPDATE_FOCUS = "witches/update_focus/1";
		public const string REFILL_STAMINA = "witches/refill_stamina/1";

		// Misc
		public const string PING = "witches/ping/1";

		// Tutorial
		public const string FINISH_TUTORIAL = "witches/finish_tutorial/1";
		public const string TUTORIAL_PROGRESS = "witches/tutorial_progress/1";


		// Error
		public const string ERROR_REPORT = "witches/error_report/1";
	}

	
}




