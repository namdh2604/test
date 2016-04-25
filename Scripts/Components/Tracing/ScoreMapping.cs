namespace Voltage.Witches.Components
{
	public enum StrokeScoreMapping
	{
		Not_Quite,
		Nice,
		Not_Bad,
		magical,
		Magical,
		perfect,
		Perfect
	}

	public class ScoreMapping 
	{
		float[] _scoreRanges;

		public ScoreMapping(float[] accuracyRanges)
		{
			_scoreRanges = accuracyRanges;
		}

		private static readonly StrokeScoreMapping[] _scores = new StrokeScoreMapping[]
		{
			StrokeScoreMapping.Not_Quite,
			StrokeScoreMapping.Nice,
			StrokeScoreMapping.Not_Bad,
			StrokeScoreMapping.magical,
			StrokeScoreMapping.Magical,
			StrokeScoreMapping.perfect,
			StrokeScoreMapping.Perfect
		};

		public override bool Equals(System.Object obj)
		{
			if(obj == null)
			{
				return false;
			}

			ScoreMapping candidateMap = obj as ScoreMapping;
			if((System.Object)candidateMap == null)
			{
				return false;
			}
			
			if(_scoreRanges.Length != candidateMap._scoreRanges.Length)
			{
				return false;
			}
			
			for(int i = 0; i < _scoreRanges.Length; ++i)
			{
				var myValue = _scoreRanges[i];
				var comparisonValue = candidateMap._scoreRanges[i];
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

		public StrokeScoreMapping GetScore(float rawRating)
		{
			for (int i = 0; i < _scoreRanges.Length - 1; ++i)
			{
				if (rawRating < _scoreRanges[i])
				{
					return _scores[i];
				}
			}
			
			return _scores[_scores.Length - 1];
		}
	}
}