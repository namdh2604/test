
using System;
using System.Collections.Generic;

namespace Voltage.Witches.Factory
{
	using Voltage.Story.General;

	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;

	public class DefaultRecipeFactory : IFactory<string,IRecipe>
    {
		private readonly RecipesConfiguration _recipeConfig;
//		private readonly PotionsConfiguration _potionConfig;
//		private readonly IDictionary<string,CategoryData> _ingredientCategories;
//
//		public DefaultRecipeFactory(RecipesConfiguration recipeConfig, PotionsConfiguration potionsConfig, IDictionary<string,CategoryData> ingredientCategories)
//		{
//			if(recipeConfig == null || potionsConfig == null || ingredientCategories == null)
//			{
//				throw new ArgumentNullException();
//			}
//
//			_recipeConfig = recipeConfig;
//			_potionConfig = potionsConfig;
//			_ingredientCategories = ingredientCategories;
//		}


		private readonly IRecipeFactoryNew _recipeFactory;

		public DefaultRecipeFactory(RecipesConfiguration recipeConfig, IRecipeFactoryNew recipeFactory)
		{
			if(recipeConfig == null || recipeFactory == null)
			{
				throw new ArgumentNullException();
			}

			_recipeConfig = recipeConfig;
			_recipeFactory = recipeFactory;
		}

		public IRecipe Create(string recipeID)
		{
			if (!string.IsNullOrEmpty(recipeID))
			{
				RecipeReference recipeRef = _recipeConfig.Recipes [recipeID];
				Recipe recipe = _recipeFactory.Create(null, recipeRef);

				return recipe;
			}
			else
			{
				throw new ArgumentNullException("DefaultRecipeFactory::Create >>> recipeID is null/empty");
			}
		}

    }
    
}




