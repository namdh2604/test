using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Models;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public interface IIngredientsMasterConfigParser
	{
		IngredientsMasterConfiguration Construct(List<IngredientData> ingredientDataList);
	}

	public class IngredientsMasterConfigParser : IIngredientsMasterConfigParser
	{
		IIngredientConfigurationParser _ingredientParser;

		public IngredientsMasterConfigParser(IIngredientConfigurationParser ingredientParser)
		{
			_ingredientParser = ingredientParser;
		}

		public IngredientsMasterConfiguration Construct(List<IngredientData> ingredientDataList)
		{
			IngredientsMasterConfiguration ingredientMaster = new IngredientsMasterConfiguration();
			for(int i = 0; i < ingredientDataList.Count; ++i)
			{
				var ingredientData = ingredientDataList[i];
				var ingredientConfig = _ingredientParser.Construct(ingredientData);
				ingredientMaster.Ingredients[ingredientConfig.Id] = ingredientConfig;
			}

			return ingredientMaster;
		}
	}
}