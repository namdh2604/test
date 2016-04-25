using System;
namespace Voltage.Witches.Components
{
	public class IngredientDifficultyCalculator
	{
		private int _recipeThreshold;

		public IngredientDifficultyCalculator(int recipeThreshold)
		{
			_recipeThreshold = recipeThreshold;
		}

		public float Compute(int contribution1, int contribution2, int contribution3)
		{
			int totalSum = contribution1 + contribution2 + contribution3;
			return totalSum / (float)_recipeThreshold;
		}
	}

}

