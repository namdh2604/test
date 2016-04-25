using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Models;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public interface IRecipesConfigParser
	{
		RecipesConfiguration Construct(string json);
		RecipesConfiguration Construct(List<RecipeData> recipesData);
	}

	public class RecipesConfigParser :  IRecipesConfigParser
	{
		public RecipesConfigParser()
		{
		}

		public RecipesConfiguration Construct(List<RecipeData> recipesData)
		{
			RecipesConfiguration recipesConfig = new RecipesConfiguration();
			AddParsedListToConfigDictionary<RecipeData>(recipesData, recipesConfig.Recipes_Dictionary);

			foreach(RecipeData data in recipesData)
			{
				RecipeReference recipeRef = new RecipeReference(data);
				recipesConfig.Recipes[data.id] = recipeRef;
			}

			return recipesConfig;
		}

		public RecipesConfiguration Construct(string json)
		{
			RecipesConfiguration recipesConfig = new RecipesConfiguration();
			JObject jsonObject = JObject.Parse(json);
			RecipesListData recipesData = JsonConvert.DeserializeObject<RecipesListData>(jsonObject.ToString());

			AddParsedListToConfigDictionary<RecipeData>(recipesData.recipes,recipesConfig.Recipes_Dictionary);

			foreach(RecipeData data in recipesData.recipes)
			{
				RecipeReference recipeRef = new RecipeReference(data);
				recipesConfig.Recipes[data.id] = recipeRef;
			}

			return recipesConfig;
		}

		private void AddParsedListToConfigDictionary<T>(List<T> parsedList, Dictionary<string,T> configDictionary) where T : BaseData
		{
			for(int i = 0; i < parsedList.Count; ++i)
			{
				T data = parsedList[i];
				configDictionary[data.id] = data;
			}
		}
	}
}