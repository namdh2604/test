using System;
using System.Collections;
using System.Collections.Generic;

public class PotionObject 
{
	private string _potionName = string.Empty;
	private PotionObject _instance = null;

	private enum POTION_QUALITY
	{
		LOW,
		FAIR,
		AVERAGE,
		GOOD,
		HIGH,
		FAIL,
		UNASSIGNED
	}

	private POTION_QUALITY _quality = POTION_QUALITY.UNASSIGNED;

	private string ConvertQualityToString()
	{
		string qualityID = string.Empty;
		switch(_quality)
		{
			case POTION_QUALITY.LOW:
				qualityID = "Low";
				break;
			case POTION_QUALITY.FAIR:
				qualityID = "Fair";
				break;
			case POTION_QUALITY.AVERAGE:
				qualityID = "Average";
				break;
			case POTION_QUALITY.GOOD:
				qualityID = "Good";
				break;
			case POTION_QUALITY.HIGH:
				qualityID = "High";
				break;
			case POTION_QUALITY.FAIL:
				qualityID = "Junk";
				break;
			case POTION_QUALITY.UNASSIGNED:
				qualityID = "Unassigned";
				break;
		}

		return qualityID;
	}

	private void SetPotionName(string potionName)
	{
		_potionName = potionName;
	}

	private void SetQuality(float qualityScalar)
	{
		if(qualityScalar <= 0.0f)
		{
			_quality = POTION_QUALITY.FAIL;
		}
		else if((qualityScalar < 0.2f) && (qualityScalar > 0.0f))
		{
			_quality = POTION_QUALITY.LOW;
		}
		else if((qualityScalar >= 0.2f) && (qualityScalar < 0.4f))
		{
			_quality = POTION_QUALITY.FAIR;
		}
		else if((qualityScalar >= 0.4f) && (qualityScalar < 0.6f))
		{
			_quality = POTION_QUALITY.AVERAGE;
		}
		else if((qualityScalar >= 0.6f) && (qualityScalar < 0.8f))
		{
			_quality = POTION_QUALITY.GOOD;
		}
		else
		{
			_quality = POTION_QUALITY.HIGH;
		}
	}

	public string GetPotionName()
	{
		return _potionName;
	}

	public string GetPotionQuality()
	{
		string quality = ConvertQualityToString();
		return quality;
	}

	public PotionObject CreatePotion(string potionName, float qualityScalar)
	{
		_instance = this;
		_instance.SetPotionName(potionName);
		_instance.SetQuality(qualityScalar);
		return _instance;
	}
}
