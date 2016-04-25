using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Voltage.Witches.Models
{
	public class RecipeData: BaseData
	{
		public string hint;
		public int display_order;
		public bool replay_flag;
		public List<IngredientListData> ingredient_list;
		public int focus_cost;
		public Dictionary<string,float> score_list;
		public Dictionary<string,string> prize_list;
		public Dictionary<string,string> potion_list;
		public int game_duration;
		public int continue_duration;
	}
}
