using System;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Configuration.JSON
{
	using SimpleJSON;

	public interface IRecipeRefParser
	{
		RecipeRef Parse(string json);
	}

	public class RecipeRefParser : IRecipeRefParser
	{
		const int MAX_REQS = 3;

		public RecipeRef Parse(string json)
		{
			JSONNode node = JSON.Parse(json);
			string id = (string)node["id"];


			RecipeRef recipe = new RecipeRef(id);

			string name = node["name"].Value;
			recipe.Name = name;

			string hint = node["hint"].Value;
			recipe.Hint = hint;

			int staminaReq = node["staminaRequirement"].AsInt;
			recipe.StaminaReq = staminaReq;

			int clearScore = node["clearScore"].AsInt;
			recipe.ClearScore = clearScore;

			int rawGameType = node["brewGameType"].AsInt;
			recipe.BrewGameCategory = (BrewGameType)rawGameType;

			foreach (JSONNode productNode in node["products"].AsArray)
			{
				recipe.Products.Add((string)productNode);
			}

			AddRequirements(node, recipe);

			return recipe;
		}

		void AddRequirements(JSONNode node, RecipeRef recipe)
		{
			var reqs = node["ingredients"].AsArray;

			foreach (JSONNode reqNode in reqs)
			{
				string category = (string)reqNode["category"];
				int contribution = reqNode["max"].AsInt;
				recipe.AddRequirement(category, contribution);
			}
		}
	}
}

