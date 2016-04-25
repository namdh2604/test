using Voltage.Witches.Lib;

namespace Voltage.Witches.Components
{
	public class DifficultyMap
	{
		float[] _difficultyRanges;

		// Expects an array of difficulty threshholds, 
		// where values directly below the given threshhold, but greater than the previous threshold
		// are identified by that difficulty
		public DifficultyMap(float[] difficultyRanges)
		{
			_difficultyRanges = difficultyRanges;
		}

		private static readonly MiniGameDifficulty[] _difficulties = new MiniGameDifficulty[] {
			MiniGameDifficulty.Trouble,
			MiniGameDifficulty.Hard,
			MiniGameDifficulty.Tricky,
			MiniGameDifficulty.Normal,
			MiniGameDifficulty.Easy
		};

		public override bool Equals(System.Object obj)
		{
			if(obj == null)
			{
				return false;
			}

			DifficultyMap candidateMap = obj as DifficultyMap;
			if((System.Object)candidateMap == null)
			{
				return false;
			}

			if(_difficultyRanges.Length != candidateMap._difficultyRanges.Length)
			{
				return false;
			}

			for(int i = 0; i < _difficultyRanges.Length; ++i)
			{
				var myValue = _difficultyRanges[i];
				var comparisonValue = candidateMap._difficultyRanges[i];
				if(myValue != comparisonValue)
				{
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			return this.GetHashCode();
		}

		public MiniGameDifficulty GetDifficulty(float rawRating)
		{
			for(int i = 0; i < _difficultyRanges.Length - 1; ++i)
			{
				if(rawRating < _difficultyRanges[i])
				{
					return _difficulties[i];
				}
			}

			return _difficulties[_difficulties.Length - 1];
		}
	}
}

