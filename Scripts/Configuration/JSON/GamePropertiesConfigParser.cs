using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Models;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public interface IGamePropertiesConfigParser
	{
		GamePropertiesConfiguration Construct(List<GamePropertiesData> gamePropertiesData);
	}

	public class GamePropertiesConfigParser: IGamePropertiesConfigParser
	{
		public GamePropertiesConfigParser()
		{

		}

		private enum GameType
		{
			EASY = 0,NORMAL = 1,TRICKY = 2,HARD = 3,TROUBLE = 4
		}

		private enum IntType
		{
			FREE_CURRENCY = 0,PREMIUM_CURRENCY = 1,FOCUS = 2,REBEL = 3,FUNKY = 4,PREPPY = 5,TICKET = 6,CLOSET = 7
		}

		private static List<string> _intQualifiers = new List<string>()
		{
			"free_currency","premium_currency","focus","rebel","funky","preppy","ticket","closet"
		};

		private enum FloatType
		{
			TICKET = 0,FOCUS = 1,SPEED = 2,ZONE =3
		}

		private static List<string> _floatQualifiers = new List<string>()
		{
			"ticket_Refresh","focus_Refresh","speed","zone"
		};

		private enum StringType
		{
			BOOK = 0,AFFINITY = 1,
		}

		private static List<string> _stringQualifiers = new List<string>()
		{
			"book","affinity",
		};

		public GamePropertiesConfiguration Construct(List<GamePropertiesData> gamePropertiesData)
		{
			GamePropertiesConfiguration gameConfig = new GamePropertiesConfiguration();
			Dictionary<string,float> safeZonesAndSpeedValues = new Dictionary<string,float>();
			Dictionary<string,Dictionary<string,float>> difficultyValues = new Dictionary<string,Dictionary<string,float>>();
			Dictionary<string,int> scoringValues = new Dictionary<string,int>();
			if(gamePropertiesData != null)
			{
				for(int i = 0; i < gamePropertiesData.Count; ++i)
				{
					var currentData = gamePropertiesData[i];
					var propertyName = currentData.name;

					if(isNotADefaultOrMiniGameValue(propertyName))
					{
						int value = ConvertPropertyToInt(currentData.value);
						if(isVersionValue(propertyName))
						{
							gameConfig.Witches_Version = value;
						}
                        else if (propertyName == "affinity_per_premium")
                        {
                            gameConfig.Affinity_Per_Premium = value;
                        }
						else
						{
							gameConfig.Cumulative_Max = value;
						}
					}
					else if((shouldConvertToString(propertyName)) && (isDefaultIngredients(propertyName)))
					{
						var job = JObject.Parse(currentData.value);
						try
						{
							var deserialized = JsonConvert.DeserializeObject<Dictionary<string,int>>(job.ToString());
							foreach(var pair in deserialized)
							{
								var key = pair.Key;
								var value = pair.Value;
								InventoryData data = new InventoryData();
								data.id = key;
								data.name = key;
								data.quantity = value;
								data.type = "ingredient";
								gameConfig.Default_Inventory.Add(data);
							}
						}
						catch(Exception)
						{
							throw new Exception("String not properly formatted for desrialization");
						}
					}
					else if((shouldConvertToString(propertyName)) && (!isAMiniGameValue(propertyName)))
					{
						string value = currentData.value;
						int tokenIndex = _stringQualifiers.Select((x, index) => new {x = x, index = index}).First(s => propertyName.Contains(s.x)).index;

						switch((StringType)tokenIndex)
						{
							case StringType.AFFINITY:
								gameConfig.Default_Affinity = value;
								break;
							case StringType.BOOK:
								gameConfig.Default_User_Book = value;
								break;
						}
					}
					else if((shouldConvertToInt(propertyName)) && (!isAMiniGameValue(propertyName)))
					{
						int value = ConvertPropertyToInt(currentData.value);
						int tokenIndex = _intQualifiers.Select((x, index) => new {x = x, index = index}).First(s => propertyName.Contains(s.x)).index;
						
						switch((IntType)tokenIndex)
						{
							case IntType.FREE_CURRENCY:
								gameConfig.Default_Free_Currency = value;
								break;
							case IntType.PREMIUM_CURRENCY:
								gameConfig.Default_Premium_Currency = value;
								break;
							case IntType.FOCUS:
								gameConfig.Default_Focus = value;
								break;
							case IntType.CLOSET:
								gameConfig.Default_Closet = value;
								break;
							case IntType.FUNKY:
								gameConfig.Default_Funky = value;
								break;
							case IntType.PREPPY:
								gameConfig.Default_Preppy = value;
								break;
							case IntType.REBEL:
								gameConfig.Default_Rebel = value;
								break;
							case IntType.TICKET:
								gameConfig.Default_Ticket = value;
								break;
						}
					}
					else if(shouldConvertToFloat(propertyName) && (!isAMiniGameValue(propertyName)))
					{
						float value = ConvertPropertyToFloat(currentData.value);
						int tokenIndex = _floatQualifiers.Select((x, index) => new {x = x, index = index}).First(s => propertyName.Contains(s.x)).index;

						switch((FloatType)tokenIndex)
						{
						case FloatType.FOCUS:
							gameConfig.Default_Focus_Refresh_Rate = value;
							break;
						case FloatType.TICKET:
							gameConfig.Default_Ticket_Refresh_Rate = value;
							break;
						}
					}
					else if((shouldConvertToFloat(propertyName)) && (isAMiniGameValue(propertyName)))
					{
						float value = ConvertPropertyToFloat(currentData.value);
						safeZonesAndSpeedValues[propertyName] = value;
					}
					else if((isAMiniGameValue(propertyName)) && (isADifficultyDictionary(propertyName)))
					{
						BaseMiniGameDifficulty difficulty = HandleDifficultyConversion(currentData.value);
						difficultyValues[propertyName] = new Dictionary<string,float>(){{"low",difficulty.low},{"high",difficulty.high}};
					}
					else if((isAMiniGameValue(propertyName)) && (!isADifficultyDictionary(propertyName)))
					{
						ScoringList feedbackScoringLabels = HandleFeedbackScoreLabelsList(currentData.value);
						for(int k = 0; k < feedbackScoringLabels.scoring.Count; ++k)
						{
							ScoringObject scoreObject = feedbackScoringLabels.scoring[k];
							scoringValues[scoreObject.label] = scoreObject.score;
						}
					}
					else
					{
						//TODO Catch if the value doesn't exist, maybe need some sort of update values
	//					UnityEngine.Debug.LogWarning("Invalid property");
						Console.WriteLine("Invalid Property");
					}
				}

				gameConfig.Mini_Game_Difficulty = difficultyValues;
				gameConfig.Mini_Game_Scoring = scoringValues;
				gameConfig.Mini_Game_Speed_And_Zones = safeZonesAndSpeedValues;
			}
			return gameConfig;
		}

		private bool isVersionValue(string propertyName)
		{
			return (propertyName.Contains("version"));
		}

		private bool isNotADefaultOrMiniGameValue(string propertyName)
		{
			return ((!isADefaultValue(propertyName)) && (!isAMiniGameValue(propertyName)) && (!isDefaultIngredients(propertyName)));
		}

		private bool isAMiniGameValue(string propertyName)
		{
			return (propertyName.Contains("minigame"));
		}

		private bool isADifficultyDictionary(string propertyName)
		{
			return (propertyName.Contains("difficulty"));
		}

		private bool isDefaultIngredients(string propertyName)
		{
			return (propertyName.Contains("ingredients"));
		}

		private bool isADefaultValue(string propertyName)
		{
			return (propertyName.Contains("default"));
		}

		private bool shouldConvertToFloat(string propertyName)
		{
			return (_floatQualifiers.Any(s =>propertyName.Contains(s)));
		}

		private bool shouldConvertToInt(string propertyName)
		{
			return (_intQualifiers.Any(s =>propertyName.Contains(s)));		// staying with "convention" here, tho this isn't the best way to go about this
		}

		private bool shouldConvertToString(string propertyName)
		{
			return ((_stringQualifiers.Any(s =>propertyName.Contains(s))) || (isDefaultIngredients(propertyName)));
		}

		private ScoringList HandleFeedbackScoreLabelsList(string dataValue)
		{
			JObject jsonObject;
			ScoringList feedbackLabelList;
			if(!dataValue.Contains("{\"scoring\":"))
			{
				string inputstring = "{\"scoring\":" + dataValue + "}";
				inputstring = inputstring.Replace("'","\"");
				dataValue = inputstring;
			}

			try
			{
				jsonObject = JObject.Parse(dataValue);
			}
			catch(Exception)
			{
				throw new Exception("Problem parsing the score list");
			}

			feedbackLabelList = JsonConvert.DeserializeObject<ScoringList>(jsonObject.ToString());
			return feedbackLabelList;
		}

		private BaseMiniGameDifficulty HandleDifficultyConversion(string dataValue)
		{
			JObject jsonObject = null;

			try
			{
				jsonObject = JObject.Parse(dataValue);
			}
			catch(Exception)
			{
				throw new Exception("Problem handling the difficulty value conversions");
			}
			BaseMiniGameDifficulty miniGameDifficulty = JsonConvert.DeserializeObject<BaseMiniGameDifficulty>(jsonObject.ToString());
			return miniGameDifficulty;
		}

		private Single ConvertPropertyToFloat(string dataValue)
		{
			return Convert.ToSingle(dataValue);
//			return (float.Parse(dataValue, System.Globalization.NumberStyles.AllowDecimalPoint));

		}

		private int ConvertPropertyToInt(string dataValue)
		{
			if(dataValue.Contains("."))
			{
				return (int)ConvertPropertyToFloat(dataValue);
			}

			return (Convert.ToInt32(dataValue));
		}
	}
}
