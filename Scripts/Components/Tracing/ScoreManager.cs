using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Components
{
	public class ScoreManager 
	{
		public int Current_Score { get; protected set; }
		public int Target_Score { get; protected set; }
		public int Median_Score { get; protected set; }
		public int High_Score { get; protected set; }
		public int Max_Score { get; protected set; }
		public int Tiers_Reached { get { return _tiers; } }

		private List<int> _scoresFromData;
//		private int _defaultTargetPoints = 2500;
		private int _defaultMaximumTargetScore = 12000;
		private int _tiers = 0;

		ScoreMapping _scoreMapper;

		private static List<string> _scoreTypes = new List<string>()
		{
			"Not Quite",
			"Nice",
			"Not Bad",
			"magical",
			"Magical",
			"perfect",
			"Perfect"
		};

		private static List<float> _defaultPercentages = new List<float>()
		{
			0.25f,
			0.5f,
			0.75f
		};

		public ScoreManager(int? maxPoints,List<float> percentages,Dictionary<string,int> scoring)
		{
			try
			{
				_scoreMapper = new ScoreMapping(new float[]{0.49f,0.59f,0.69f,0.79f,0.89f,0.95f,1f});
			}
			catch(Exception)
			{
				throw new Exception("problem making the score mapping");
			}

			try
			{
				SetMaximum(maxPoints);
			}
			catch(Exception)
			{
				throw new Exception("problem dealing with max points");
			}

			try
			{
				SetUpScoreValues(percentages);
			}
			catch(Exception)
			{
				throw new Exception("Problem setting up the Score Tiers");
			}

			try
			{
				BuildScoresList(scoring);
			}
			catch(Exception)
			{
				throw new Exception("problem building the score list");
			}
		}

		void SetMaximum(int? maxPoints)
		{
			if(maxPoints.HasValue)
			{
				Max_Score = maxPoints.Value;
			}
			else
			{
				Max_Score = _defaultMaximumTargetScore;
			}
		}

		public void UpdateScoresList(List<int> scores)
		{
			_scoresFromData.Clear();
			_scoresFromData = scores;
		}

		public void UpdateScoreMapping(List<float> scoreMapping)
		{
			var mapping = scoreMapping.ToArray();
			try
			{
				_scoreMapper = new ScoreMapping(mapping);
			}
			catch(Exception)
			{
				throw new Exception("problem making the score mapping");
			}
		}

		void BuildScoresList(Dictionary<string, int> scoring)
		{
			_scoresFromData = new List<int>();
			for(int i = 0; i < scoring.Count; ++i)
			{
				var lookUpKey = _scoreTypes[i];
				int scoreValue = 0;
				if(scoring.TryGetValue(lookUpKey, out scoreValue))
				{
					_scoresFromData.Add(scoreValue);
				}
			}
		}

		void SetUpScoreValues(List<float> percentages)
		{
			List<float> maxScalingPercentages = new List<float>();
			if((percentages == null) || (percentages.Count < 1))
			{
				maxScalingPercentages = _defaultPercentages;
			}
			else
			{
				maxScalingPercentages = percentages;
			}

			Target_Score = UnityEngine.Mathf.CeilToInt(Max_Score * maxScalingPercentages[0]);
			Median_Score = UnityEngine.Mathf.CeilToInt(Max_Score * maxScalingPercentages[1]);
			High_Score = UnityEngine.Mathf.CeilToInt(Max_Score * maxScalingPercentages[2]);
		}

		int GetMyCurrentTier()
		{
			if(Current_Score >= High_Score)
			{
				return 3;
			}
			else if((Current_Score >= Median_Score) && (Current_Score < High_Score))
			{
				return 2;
			}
			else if((Current_Score >= Target_Score) && (Current_Score < Median_Score))
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		public KeyValuePair<string,int> EvaluatePoints(float rawAccuracy)
		{
			var score = _scoreMapper.GetScore(rawAccuracy);
			var index = (int)score;

			Current_Score += _scoresFromData[index];
			_tiers = GetMyCurrentTier();
			KeyValuePair<string,int> scoreLabel = new KeyValuePair<string, int>(_scoreTypes[index],_scoresFromData[index]);

			return scoreLabel;
		}

		public void ResetScore()
		{
			Current_Score = 0;
		}
	}
}