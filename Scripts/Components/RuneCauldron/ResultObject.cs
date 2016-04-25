using System;
using System.Collections;
using System.Collections.Generic;

public class ResultObject
{
	private Dictionary<int,float> _gestureNumberAndPercentage = new Dictionary<int,float>();
	private Dictionary<int,int> _gestureNumberAndPoints = new Dictionary<int,int>();
//	private Dictionary<int,SCORE_TYPE> _gestureNumberAndScoreType = new Dictionary<int, SCORE_TYPE>();
	private int _currentRound = 0;
	private int _minimumTargetPoints = 0;
	private int _defaultTargetPoints = 2500;

	private List<int> _scoresFromData;

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

	private static List<int> _staticScores = new List<int>()
	{
		0,
		200,
		400,
		600,
		800,
		900,
		1000
	};

	private enum SCORE_TYPE
	{
		NOT_QUITE = 0,
		NICE = 1,
		NOT_BAD = 2,
		MAGICAL = 3,
		PERFECT = 4
	}

	private SCORE_TYPE _currentScoreType = SCORE_TYPE.NOT_QUITE;

	public int GetScoreType()
	{
		return (int)_currentScoreType;
	}

	public int CurrentRound
	{
		get { return _currentRound; }
	}

	public void CreateResults(int currentRound, int targetPoints,Dictionary<string,int> scoring)
	{
		_currentRound = currentRound;
		_minimumTargetPoints = targetPoints;
		_currentScoreType = SCORE_TYPE.NOT_QUITE;
		if(scoring != null)
		{
			BuildScoresList(scoring);
		}
		if(_minimumTargetPoints <= 0)
		{
			_minimumTargetPoints = _defaultTargetPoints;
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

	private void PopulatePoints(int gestureNumber, bool isTargetGesture)
	{
		if(isTargetGesture)
		{
			_gestureNumberAndPoints[gestureNumber] = GetPointsFromGestureIndex(gestureNumber);
		}
		else
		{
			_currentScoreType = SCORE_TYPE.NOT_QUITE;
			_gestureNumberAndPoints[gestureNumber] = 0;
		}
	}

	private int GetScoreFromIndex(int index)
	{
		if((_scoresFromData != null) && (_scoresFromData[index] > -1))
		{
			return _scoresFromData[index];
		}

		return _staticScores[index];
	}

	private int GetPointsFromGestureIndex(int gestureNum)
	{
		float accuracy = _gestureNumberAndPercentage [gestureNum];
		int points = 0;
		if(accuracy < 0.5f)
		{
			_currentScoreType = SCORE_TYPE.NOT_QUITE;
			return points;
		}
		else if((accuracy >= 0.5f) && (accuracy < 0.6f))
		{
			_currentScoreType = SCORE_TYPE.NICE;
//			points = Convert.ToInt32((_minimumTargetPoints * 0.05f) / 3);
			points = GetScoreFromIndex(1);
			return points;
		}
		else if((accuracy >= 0.6f) && (accuracy < 0.7f))
		{
			_currentScoreType = SCORE_TYPE.NOT_BAD;
//			points = Convert.ToInt32((_minimumTargetPoints * 0.25f) / 3);
			points = GetScoreFromIndex(2);
			return points;
		}
		else if((accuracy >= 0.7f) && (accuracy < 0.8f))
		{
			_currentScoreType = SCORE_TYPE.MAGICAL;
//			points = Convert.ToInt32((_minimumTargetPoints * 0.35f) / 3);
			points = GetScoreFromIndex(3);
			return points;
		}
		else if((accuracy >= 0.8f) && (accuracy < 0.9f))
		{
			_currentScoreType = SCORE_TYPE.MAGICAL;
//			points = Convert.ToInt32((_minimumTargetPoints * 0.65f) / 3);
			points = GetScoreFromIndex(4);
//			points = _staticScores[4];
			return points;
		}
		else if((accuracy >= 0.9f) && (accuracy < 0.95f))
		{
			_currentScoreType = SCORE_TYPE.PERFECT;
//			points = Convert.ToInt32((_minimumTargetPoints * 0.75f) / 3);
			points = GetScoreFromIndex(5);
//			points = _staticScores[5];
			return points;
		}
		else
		{
			_currentScoreType = SCORE_TYPE.PERFECT;
//			points = Convert.ToInt32((_minimumTargetPoints * 0.95f) / 3);
			points = GetScoreFromIndex(6);
//			points = _staticScores[6];
			return points;
		}
	}

	public void AddResults(int gestureNumber, float percentAccuracy, bool isTargetGesture)
	{
		_gestureNumberAndPercentage[gestureNumber] = percentAccuracy;
		PopulatePoints(gestureNumber,isTargetGesture);
	}

	public float GetAverageAccuracy()
	{
		float averageAccuracy = 0.0f;
		float sumAccuracy = 0.0f;
		for(int i = 0; i < _gestureNumberAndPercentage.Count; ++i)
		{
			sumAccuracy += _gestureNumberAndPercentage[i];
		}
		averageAccuracy = (sumAccuracy / (float)_gestureNumberAndPercentage.Count);

		return averageAccuracy;
	}

	public int GetTotalPointsFromRound()
	{
		int totalPoints = 0;
		for(int i = 0; i < _gestureNumberAndPoints.Count; ++i)
		{
			totalPoints += _gestureNumberAndPoints[i];
		}

		return totalPoints;
	}

	public int GetPointsFromGesture(int gestureNumber)
	{
		return _gestureNumberAndPoints[gestureNumber];
	}

	public float GetAccuracyFromGesture(int gestureNumber)
	{
		return _gestureNumberAndPercentage[gestureNumber];
	}
}
