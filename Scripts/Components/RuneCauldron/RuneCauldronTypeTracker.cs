using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

public class RuneCauldronTypeTracker : MonoBehaviour 
{
	private static RuneCauldronTypeTracker _instance = null;
	public IRecipe Recipe { get; protected set; }
	public List<Ingredient> WageredIngredients { get; protected set; }
	private List<ResultObject> _resultsObjectList = new List<ResultObject>();
	private Dictionary<int,Potion> _possiblePotions;
	public Player Player { get; protected set; }

	public static RuneCauldronTypeTracker GetTracker()
	{
		return _instance;
	}

	enum RUNE_CAULDRON_CONFIG
	{
		PIP,
		OVERLAY,
		OVERLAY_REVERSE
	}

	private RUNE_CAULDRON_CONFIG _currentConfig = RUNE_CAULDRON_CONFIG.OVERLAY_REVERSE;
	private float _difficultyScalar = 1.0f;

	private static int _defaultRounds = 1;
	private int _roundLimit = 0;
	private int _currentRound = 1;

	private int _minimumTargetScore = 0;
	private int _medianTargetScore = 0;
	private int _highTargetScore = 0;
	private int _maximumTargetScore = 0;
	private static int _defaultMaximumTargetScore = 10000;

	private int _currentScore = 0;
	private float _timerLastPosition = 0.0f;

	private List<float> _targetPercentageList = new List<float>();

	private static List<float> _defaultPercentageList = new List<float>(3)
	{
		0.25f,
		0.5f,
		0.75f
	};

	void Awake()
	{
		if(_instance != null)
		{
			KillTracker();
		}

		_instance = this;
		DontDestroyOnLoad(gameObject);
		if(_roundLimit <= 0)
		{
			_roundLimit = _defaultRounds;
		}

		_targetPercentageList = _defaultPercentageList;

		SetUpPointsLimits();
	}

	public void SetWageredIngredients(List<Ingredient> ingredients)
	{
		WageredIngredients = ingredients;
	}

	public void SetPlayer(Player player)
	{
		Player = player;
	}

	public void SetRecipe(IRecipe recipe)
	{
		Recipe = recipe;
		SetUpPotionList();
	}

	private void SetUpPotionList()
	{
		_possiblePotions = new Dictionary<int, Potion>();
		var products = Recipe.Products;
		for(int i = 0; i < products.Count; ++i)
		{
			Potion current = products[i] as Potion;
			_possiblePotions[(int)current.PotionCategory] = current;
		}
	}

	public Potion GetSuccesfulPotion()
	{
		if(hasThreeStars())
		{
			return _possiblePotions[(int)PotionCategory.MASTER];
		}
		else if((hasTwoStars()) && (!hasThreeStars()))
		{
			return _possiblePotions[(int)PotionCategory.SUPERIOR];
		}
		else if((hasOneStar()) && (!hasTwoStars()))
		{
			return _possiblePotions[(int)PotionCategory.BASIC]; 
		}
		else
		{
			return null;
		}
	}

	public Potion GetBasicPotion()
	{
		return _possiblePotions[(int)PotionCategory.BASIC];
	}

	public string GetCurrentPotionName()
	{
		return _possiblePotions[(int)PotionCategory.BASIC].Name;
	}

	public string GetSuccessTypeAsString()
	{
		string success = string.Empty;
		if(hasThreeStars())
		{
			success = "PERFECT";
		}
		else if((hasTwoStars()) && (!hasThreeStars()))
		{
			success = "GOOD";
		}
		else if((hasOneStar()) && (!hasTwoStars()))
		{
			success = "OKAY";
		}
		else
		{
			success = "FAIL";
		}

		return success;
	}

	public float GetLastPosition()
	{
		return _timerLastPosition;
	}

	public void SetLastTimerPosition(float position)
	{
		_timerLastPosition = position;
	}

	public int GetMinimumTarget()
	{
		return _minimumTargetScore;
	}

	public int GetMedianTarget()
	{
		return _medianTargetScore;
	}

	public int GetHighTarget()
	{
		return _highTargetScore;
	}

	public int GetMaximumTarget()
	{
		return _maximumTargetScore;
	}

	public int ReturnCurrentStars()
	{
		if(!hasOneStar())
		{
			return 0;
		}
		else
		{
			if((hasOneStar()) && (!hasTwoStars()))
			{
				return 1;
			}
			else if((hasTwoStars()) && (!hasThreeStars()))
			{
				return 2;
			}
			else if(hasThreeStars())
			{
				return 3;
			}
			else
			{
				return 0;
			}
		}
	}

	bool hasOneStar()
	{
		if(_currentScore >= _minimumTargetScore)
		{
			return true;
		}

		return false;
	}

	bool hasTwoStars()
	{
		if(_currentScore >= _medianTargetScore)
		{
			return true;
		}

		return false;
	}

	bool hasThreeStars()
	{
		if(_currentScore >= _highTargetScore)
		{
			return true;
		}

		return false;
	}

	void SetUpPointsLimits()
	{
		//TODO Update this to calculate the min, median, and high score levels
//		if(_minimumTargetScore <= 0)
//		{
//			_minimumTargetScore = _defaultMinimumTargetScore;
//		}

		if(_maximumTargetScore <= 0)
		{
			_maximumTargetScore = _defaultMaximumTargetScore;
		}

		_minimumTargetScore = Mathf.CeilToInt(_maximumTargetScore * _targetPercentageList[0]);
		_medianTargetScore = Mathf.CeilToInt(_maximumTargetScore * _targetPercentageList[1]);
		_highTargetScore = Mathf.CeilToInt(_maximumTargetScore * _targetPercentageList[2]);

//		_medianTargetScore = Mathf.CeilToInt(_minimumTargetScore * 1.25f);
//		_highTargetScore = Mathf.CeilToInt(_minimumTargetScore * 1.5f);
	}

	public void SetCurrentScore(int score)
	{
		_currentScore = score;
	}

	public void SetMinimumTargetPoints(int pointTarget)
	{
		_minimumTargetScore = pointTarget;
		SetUpPointsLimits();
	}

	public void SetMaximumTargetPoints(int pointTarget, float? minPercent, float? medPercent, float? highPercent)
	{
		_maximumTargetScore = pointTarget;

		if(minPercent.HasValue)
		{
			_targetPercentageList.Add(minPercent.Value);
		}
		else
		{
			_targetPercentageList.Add(_defaultPercentageList[0]);
		}

		if(medPercent.HasValue)
		{
			_targetPercentageList.Add(medPercent.Value);
		}
		else
		{
			_targetPercentageList.Add(_defaultPercentageList[1]);
		}

		if(highPercent.HasValue)
		{
			_targetPercentageList.Add(highPercent.Value);
		}
		else
		{
			_targetPercentageList.Add(_defaultPercentageList[2]);
		}

		SetUpPointsLimits();
	}

	public int GetCurrentScore()
	{
		return _currentScore;
	}

	public int GetRoundsLimit()
	{
		return _roundLimit;
	}

	public int GetCurrentRound()
	{
		return _currentRound;
	}

	public int GetPointsToNextStar()
	{
		int morePointsTilNextStar = 0;
		int stars = ReturnCurrentStars();
		switch(stars)
		{
			case 0:
				morePointsTilNextStar = _minimumTargetScore - _currentScore;
				break;
			case 1:
			morePointsTilNextStar = _medianTargetScore - _currentScore;
				break;
			case 2:
				morePointsTilNextStar = _highTargetScore - _currentScore;
				break;
			case 3:
				//
				break;
		}

		return morePointsTilNextStar;
	}

	public void AcceptResultObject(ResultObject myResults)
	{
		_resultsObjectList.Add(myResults);
	}

	public float GetCurrentPointStatusPercent()
	{
		//TODO Update this to make use of the precalculated limits
		float percent = 0.0f;
//		if(_currentScore <= _minimumTargetScore)
//		{
//			percent = ((float)_currentScore / (float)_minimumTargetScore) * 0.25f;
//		}
//		else if((_currentScore > _minimumTargetScore) && (_currentScore <= (Mathf.CeilToInt(_minimumTargetScore * 1.25f))))
//		{
//			percent = ((float)_currentScore / (Mathf.CeilToInt(_minimumTargetScore * 1.25f))) * 0.5f;
//		}
//		else if((_currentScore > (Mathf.CeilToInt(_minimumTargetScore * 1.25f))) && (_currentScore <= (Mathf.CeilToInt(_minimumTargetScore * 1.5f))))
//		{
//			percent = ((float)_currentScore / (Mathf.CeilToInt(_minimumTargetScore * 1.5f))) * 0.75f;
//		}
//		else
//		{
//			percent = ((float)_currentScore / (Mathf.CeilToInt(_minimumTargetScore * 1.75f)));
//		}

		percent = (float)_currentScore / (float)_maximumTargetScore;
		return percent;
	}

	public float GetDifficultyScalar()
	{
		return _difficultyScalar;
	}

	public void SetDifficultyScalar(float inValue)
	{
		_difficultyScalar = inValue;
	}

	public void SetRuneCauldronConfiguration(int index)
	{
		switch(index)
		{
			case 0:
				_currentConfig = RUNE_CAULDRON_CONFIG.PIP;
				break;
			case 1:
				_currentConfig = RUNE_CAULDRON_CONFIG.OVERLAY;
				break;
			case 2:
				_currentConfig = RUNE_CAULDRON_CONFIG.OVERLAY_REVERSE;
				break;
		}
	}

	public int GetRuneCauldronTypeIndex()
	{
		int index = (int)_currentConfig;
		return index;
	}

	public void KillTracker()
	{
		Destroy(gameObject);
		_instance = null;
	}
}
