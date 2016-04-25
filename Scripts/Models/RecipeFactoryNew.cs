using System;
using Voltage.Witches.Models;
using Voltage.Witches.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Voltage.Witches.Models
{
	public interface IRecipeFactoryNew
	{
		Recipe Create(PlayerRecipeConfig playerRecipeConfig, RecipeReference recipeConfig);
		void SetConfigs(MasterConfiguration gameConfig, PotionsConfiguration potionsConfig);
	}
	
	public class RecipeFactoryNew : IRecipeFactoryNew
	{
		MasterConfiguration _gameConfig;
		PotionsConfiguration _potionsConfig;
		
		public RecipeFactoryNew(MasterConfiguration gameConfig)
		{
			_gameConfig = gameConfig;
			_potionsConfig = _gameConfig.Potions_Configuration;
		}
		
		public Recipe Create(PlayerRecipeConfig playerRecipeConfig, RecipeReference recipeConfig)
		{
			Recipe recipe = new Recipe(recipeConfig.Name);
			recipe.Id = recipeConfig.Id;
			if(playerRecipeConfig != null)
			{
				recipe.IsAccessible = true;
				recipe.HighScore = playerRecipeConfig.HighScore;
				//HACK this needs to be confirmed when server side is available
				recipe.SetStage(playerRecipeConfig.CompletionStage);
			}

			recipe.Hint = recipeConfig.Hint;
			recipe.StaminaReq = recipeConfig.FocusRequirement;
			//HACK temporary solution needs to be resolved
			recipe.ClearScore = Convert.ToInt32(recipeConfig.ScoreRanges["low"] * 10000);
			recipe.ScoreScalars = ConvertDictionaryToFloatList(recipeConfig.ScoreRanges);


			foreach (var requirement in recipeConfig.IngredientRequirements)
			{
				CategoryData data = _gameConfig.Categories[requirement.Key];
				IngredientCategory category = new IngredientCategory(data.id,data.name);
				recipe.IngredientRequirements.Add(new IngredientRequirement(category,requirement.Value));
			}
			
			foreach(var itemId in recipeConfig.PotionInfo)
			{
				PotionData data = _potionsConfig.Potions_Dictionary[itemId.Value];
				Dictionary<string,int> effects = new Dictionary<string,int>();
				foreach(var element in data.effect_list)
				{
					var keys = element.Keys.ToArray();
					var values = element.Values.ToArray();
					for(int i = 0; i < keys.Length; ++i)
					{
						effects[keys[i]] = values[i];
					}
				}

				Potion potion = new Potion(data.id,data.name,data.description,data.color,effects);
				recipe.Products.Add(potion);
			}
			return recipe;
		}

		List<float> ConvertDictionaryToFloatList(Dictionary<string,float> scoreRange)
		{
			List<string> keys = new List<string>{ "low","mid","high" };
			List<float> scalars = new List<float>();
			for(int i = 0 ; i < keys.Count; ++i)
			{
				var key = keys[i];
				var scalar = scoreRange[key];

				scalars.Add(scalar);
			}

			return scalars;
		}

		public void SetConfigs(MasterConfiguration gameConfig,PotionsConfiguration potionsConfig)
		{
			if(_gameConfig == null)
			{
				_gameConfig = gameConfig;
			}
			if(_potionsConfig == null)
			{
				_potionsConfig = potionsConfig;
			}
		}
	}
}
