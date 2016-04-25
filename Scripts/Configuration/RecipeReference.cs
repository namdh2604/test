using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration
{
	public class RecipeReference
	{
		public string Id { get; protected set; }
		public string Name { get; protected set; }
		public string Hint { get; protected set; }
		public int FocusRequirement { get; set; }
		public int DisplayOrder { get; set; }
		public int ContinueDuration { get; set; }
		public int GameDuration { get; set; }
		public bool ReplayFlag { get; set; }
		public Dictionary<string, int> IngredientRequirements { get; protected set; }
		public Dictionary<string, float> ScoreRanges { get; protected set; }
		public Dictionary<string, string> PotionInfo { get; protected set; }
		public Dictionary<string, string> PrizeInfo { get; protected set; }

		public RecipeReference(RecipeData data)
		{
			Id = data.id;
			Name = data.name;
			Hint = data.hint;
			FocusRequirement = data.focus_cost;
			DisplayOrder = data.display_order;
			ContinueDuration = data.continue_duration;
			GameDuration = data.game_duration;
			ReplayFlag = data.replay_flag;
			IngredientRequirements = new Dictionary<string,int>();
			ScoreRanges = new Dictionary<string,float>();
			PotionInfo = new Dictionary<string,string>();
			PrizeInfo = new Dictionary<string,string>();

			ScoreRanges = data.score_list;
			PotionInfo = data.potion_list;
			PrizeInfo = data.prize_list;

			for(int i = 0; i < data.ingredient_list.Count; ++i)
			{
				AddIngredient(data.ingredient_list[i]);
			}
		}

		private void AddIngredient(IngredientListData data)
		{
			IngredientRequirements[data.category] = data.quality;
		}
	}
}