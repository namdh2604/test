using Newtonsoft.Json;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{	
	public class PlayerStateData
	{
		//TODO Update regarding recipe stages of completion
		public int currency;
		public int mail_badge;
		public int stamina_potion;
		public bool tutorial_flag;
		public string scene_id;
		public List<BooksData> books;
		public List<InventoryData> inventory;
		public List<PlayerAvatarItemData> avatar_items;
		public int stamina;
		public int premium_currency;
		public int focus;
		public int closet_space;
		public int affinity;
		public List<UserCharacterData> user_character;
		public string first_name;
		public string last_name;
	}
}