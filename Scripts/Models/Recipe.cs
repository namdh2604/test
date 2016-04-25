using System.Collections.Generic;
using System.Linq;
using System;

namespace Voltage.Witches.Models
{

	public struct IngredientRequirement
	{
		public IngredientCategory Category;
		public int MaxQuality;

		public IngredientRequirement(IngredientCategory category, int quality)
		{
			Category = category;
			MaxQuality = quality;
		}

	}

	public enum CompletionStage
	{
		NONE = 0,
		FIRST = 1,
		SECOND = 2,
		THIRD = 3
	}

	public interface IRecipe
	{
		string Id { get; }
		string Name { get; }
		string Hint { get; }
		int StaminaReq { get; }
		int HighScore { get; }
		CompletionStage CurrentStage { get; }
		void SetStage(int stage);
		BrewGameType BrewGameCategory { get; }
		List<Item> Products { get; }
		List<IngredientRequirement> IngredientRequirements { get; }
		bool IsClear();
		bool HasIngredientRequirements { get; }
		bool ContainsRecipeForPotion(Potion potion);
		float GetDifficulty (Ingredient ingredient1);
		float GetDifficulty (Ingredient ingredient1, Ingredient ingredient2);
		float GetDifficulty (Ingredient ingredient1, Ingredient ingredient2, Ingredient ingredient3);
		Item GetProductForScore(int score); //FIXME Might not be relevant anymore
		Item GetProductForCompleteStage(int stage);
		List<float> ScoreScalars { get; }
	}

	public class Recipe : IRecipe
	{
		public string Id { get; set; }
		public bool IsAccessible { get; set; }
		public int HighScore { get; set; }
		public List<Item> Products { get; protected set; }
		public CompletionStage CurrentStage { get; protected set; }
		public string Name { get; protected set; }
		public string Hint { get; set; }
		public int StaminaReq { get; set; }
		public int ClearScore { get; set; }
		public BrewGameType BrewGameCategory { get; set; }
		public List<IngredientRequirement> IngredientRequirements { get; protected set; }
		public List<float> ScoreScalars { get; set; }

		const int SCORE_RANGE = 100;//FIXME Might not be relevant anymore

		public Recipe(string name)
		{
			Name = name;
			Products = new List<Item>();
			IsAccessible = false;
			IngredientRequirements = new List<IngredientRequirement>();
			ScoreScalars = new List<float>();

			CurrentStage = CompletionStage.NONE;
		}

		public bool HasIngredientRequirements
		{
			get { return (IngredientRequirements.Count > 0); }
		}

		public bool IsClear()
		{
			return (HighScore >= ClearScore);
		}

		public bool ContainsRecipeForPotion(Potion potion)
		{
			for(int i = 0; i < Products.Count; ++i)
			{
				var item = Products[i];
				if(item.Name == potion.Name)
				{
					return true;
				}
			}

			return false;
		}

		public float GetDifficulty(Ingredient ingredient1)
		{
			return GetDifficulty(ingredient1, null, null);
		}

		public float GetDifficulty(Ingredient ingredient1, Ingredient ingredient2)
		{
			return GetDifficulty(ingredient1, ingredient2, null);
		}

		public float GetDifficulty(Ingredient ingredient1, Ingredient ingredient2, Ingredient ingredient3)
		{
			if (IngredientRequirements.Count == 0)
			{
				throw new Exception("Difficulty cannot be calculated without requirements");
			}

			int score = 0;
			int maximumScore = 0;

			IEnumerable<Ingredient> ingredients = (new Ingredient[] { ingredient1, ingredient2, ingredient3 }).Where(x => x != null);
			foreach (IngredientRequirement requirement in IngredientRequirements)
			{
				Ingredient matchingIngredient = FindMatchingIngredient(requirement, ingredients);
				if (matchingIngredient == null)
				{
					throw new Exception("No ingredient specified for category: " + requirement.Category.Name);
				}

				int ingredientContribution = Math.Min(matchingIngredient.Value, requirement.MaxQuality);

				score += ingredientContribution;
				maximumScore += requirement.MaxQuality;
			}


			return (float)score / maximumScore;
//			return (int)((1 - ((float)score / maximumScore)) * SCORE_RANGE);
		}

		Ingredient FindMatchingIngredient(IngredientRequirement requirement, IEnumerable<Ingredient> ingredients)
		{
			return ingredients.FirstOrDefault(x => x.IngredientCategory.Name == requirement.Category.Name);
		}

		public void SetStage(int stage)
		{
			CurrentStage = (CompletionStage)stage;
		}

		public Item GetProductForCompleteStage(int stage)
		{
			Item item = null;

			for(int i = 0; i < Products.Count; ++i)
			{
				var potion = Products[i] as Potion;
				if(((int)potion.PotionCategory) == (stage - 1))
				{
					item = potion;
					break;
				}
			}

			if(item == null)
			{
				item = Products[0];
			}

			return item;
		}

		public Item GetProductForScore(int score)
		{
			// split product range up equally in the score range
			float range = (float)SCORE_RANGE / Products.Count;

			int currentTier = 0;
			float scoreForTier = 0;

			for (; currentTier < Products.Count; ++currentTier)
			{
				scoreForTier += range;
				if (score <= scoreForTier)
				{
					return Products[currentTier];
				}
			}

			return Products[currentTier];
		}

		public void SetIngredientRequirements(List<IngredientRequirement> requirements)
		{
			IngredientRequirements = requirements;
		}

		public void SetProducts(List<Item> products)
		{
			Products = products;
		}
	}
}

