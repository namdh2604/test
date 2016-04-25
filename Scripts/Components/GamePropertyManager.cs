using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Models;

namespace Voltage.Witches.Components
{
	public interface IGameProperties
	{
		int DefaultFreeCurrency { get; }
		string DefaultUserBook { get; }
		float DefaultTicketRefreshRate { get; }
		int DefaultTicket { get; }
		int DefaultPremiumCurrency { get ; }
		int DefaultFocus { get; }
		int DefaultCloset { get; }
		int DefaultPreppy { get; }
		int DefaultFunky { get; }
		int DefaultRebel { get; }
		float DefaultFocusRefreshRate { get; }
		int CumulativeMax { get; }
		int WitchesVersion { get; }
		Dictionary<string,float> MiniGameSpeedAndZones { get; }
//		Dictionary<string,float> MiniGameSafeZone { get; }
		Dictionary<string,Dictionary<string,float>> MiniGameDifficulty { get; }
		Dictionary<string,int> MiniGameScoring { get; }
	}

	public class GamePropertyManager: IGameProperties
	{
		private Dictionary<string,int> _idKeyAndNameValue;
		private List<GamePropertiesData> _gameProps;

		public int DefaultFreeCurrency { get; protected set; }
		public string DefaultUserBook { get; protected set; }
		public float DefaultTicketRefreshRate { get; protected set; }
		public int DefaultTicket { get; protected set; }
		public int DefaultPremiumCurrency { get ; protected set; }
		public int DefaultFocus { get; protected set; }
		public int DefaultCloset { get; protected set; }
		public int DefaultPreppy { get; protected set; }
		public int DefaultFunky { get; protected set; }
		public int DefaultRebel { get; protected set; }
		public float DefaultFocusRefreshRate { get; protected set; }
		public int CumulativeMax { get; protected set; }
		public int WitchesVersion { get; protected set; }
		public Dictionary<string,float> MiniGameSpeedAndZones { get; protected set; }
//		public Dictionary<string,float> MiniGameSafeZone { get; protected set; }
		public Dictionary<string,Dictionary<string,float>> MiniGameDifficulty { get; protected set; }
		public Dictionary<string,int> MiniGameScoring { get; protected set; }

		public GamePropertyManager(List<GamePropertiesData> gameProps)
		{
			_gameProps = gameProps;
			MiniGameSpeedAndZones = new Dictionary<string,float>();
//			MiniGameSafeZone = new Dictionary<string,float>();
			MiniGameDifficulty = new Dictionary<string,Dictionary<string,float>>();
			MiniGameScoring = new Dictionary<string,int>();
			BuildPropertiesKey();
			BuildFromValues();
		}

		void BuildPropertiesKey()
		{
			_idKeyAndNameValue = new Dictionary<string,int> ();

			for(int i = 0; i < _gameProps.Count; ++i)
			{
				var prop = _gameProps[i];
				if((!_idKeyAndNameValue.ContainsKey(prop.name)) && (!_idKeyAndNameValue.ContainsValue(i)))
				{
					_idKeyAndNameValue[prop.id] = i;
				}
			}
		}

		void BuildFromValues()
		{
			foreach(KeyValuePair<string,int> pair in _idKeyAndNameValue)
			{
				int current = pair.Value;
				var currentData = _gameProps[current];
				switch((KeyValueIndex)current)
				{
				case KeyValueIndex.CUMULATIVE_MAX:
					CumulativeMax = Convert.ToInt32(currentData.value);
					break;
				case KeyValueIndex.DEFAULT_BOOK:
					DefaultUserBook = currentData.value;
					break;
				case KeyValueIndex.DEFAULT_CLOSET:
					DefaultCloset = Convert.ToInt32(currentData.value);
					break;
				case  KeyValueIndex.DEFAULT_FOCUS:
					DefaultCloset = Convert.ToInt32(currentData.value);
					break;
				case KeyValueIndex.DEFAULT_FOCUS_REFRESH:
					DefaultFocusRefreshRate = float.Parse(currentData.value,System.Globalization.NumberStyles.AllowDecimalPoint);
					break;
				case KeyValueIndex.DEFAULT_FREE:
					DefaultFreeCurrency = Convert.ToInt32(currentData.value);
					break;
				case KeyValueIndex.DEFAULT_FUNKY:
					DefaultFunky = Convert.ToInt32(currentData.value);
					break;
				case KeyValueIndex.DEFAULT_PREMIUM:
					DefaultPremiumCurrency = Convert.ToInt32(currentData.value);
					break;
				case KeyValueIndex.DEFAULT_PREPPY:
					DefaultPreppy = Convert.ToInt32(currentData.value);
					break;
				case KeyValueIndex.DEFAULT_REBEL:
					DefaultRebel = Convert.ToInt32(currentData.value);
					break;
				case KeyValueIndex.DEFAULT_TICKET:
					DefaultTicket = Convert.ToInt32(currentData.value);
					break;
				case KeyValueIndex.DEFAULT_TICKET_REFRESH:
					DefaultTicketRefreshRate = float.Parse(currentData.value,System.Globalization.NumberStyles.AllowDecimalPoint);
					break;
				case KeyValueIndex.DIFFICULTY_EASY:
					CreateNewJObAndAndToDicitonary(MiniGameDifficulty,currentData);
					break;
				case KeyValueIndex.DIFFICULTY_HARD:
					CreateNewJObAndAndToDicitonary(MiniGameDifficulty,currentData);
					break;
				case KeyValueIndex.DIFFICULTY_NORMAL:
					CreateNewJObAndAndToDicitonary(MiniGameDifficulty,currentData);
					break;
				case KeyValueIndex.DIFFICULTY_TRICKY:
					CreateNewJObAndAndToDicitonary(MiniGameDifficulty,currentData);
					break;
				case KeyValueIndex.DIFFICULTY_TROUBLE:
					CreateNewJObAndAndToDicitonary(MiniGameDifficulty,currentData);
					break;
				case KeyValueIndex.FEEDBACK_SCORES:
					CreateFeedbackScoreDictionary(MiniGameScoring,currentData);
					break;
				case KeyValueIndex.SAFE_EASY:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.SAFE_HARD:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.SAFE_NORMAL:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.SAFE_TRICKY:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.SAFE_TROUBLE:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.SPEED_EASY:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);	
					break;
				case KeyValueIndex.SPEED_HARD:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.SPEED_NORMAL:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.SPEED_TRICKY:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.SPEED_TROUBLE:
					AddToFloatDictionary(MiniGameSpeedAndZones,currentData);
					break;
				case KeyValueIndex.VERSION:
					WitchesVersion = Convert.ToInt32(currentData.value);
					break;
				}
			}
		}

		void CreateFeedbackScoreDictionary(Dictionary<string,int> miniGameScoring, GamePropertiesData data)
		{
			string inputstring = "{\"scoring\":" + data.value + "}";
			inputstring = inputstring.Replace("'","\"");
//			System.IO.File.WriteAllLines ((Application.streamingAssetsPath + "/scoring_json.json"), new string[1]{inputstring});
			JObject jsonob;
			try
			{
				jsonob = JObject.Parse(inputstring);
			}
			catch(Exception e)
			{
				UnityEngine.Debug.Log(e.ToString());
				inputstring = inputstring.Replace("900}","900},");
				jsonob = JObject.Parse(inputstring);
			}
			ScoringList scoring = JsonConvert.DeserializeObject<ScoringList>(jsonob.ToString());
			foreach(var score in scoring.scoring)
			{
				miniGameScoring[score.label] = score.score;
			}
//			List<KeyValuePair<string,JToken>> jtokenkeys = jsonob.Convert<,List<KeyValuePair<string,JToken>>>();
//
//			foreach(KeyValuePair<string,JToken> pair in jtokenkeys)
//			{
//				var obvalue = jsonob[pair.Key].ToString();
//				miniGameScoring[pair.Key] = (Convert.ToInt32(obvalue));
//			}
		}

		void CreateNewJObAndAndToDicitonary(Dictionary<string,Dictionary<string,float>> miniGameDifficulty, GamePropertiesData data)
		{
			Dictionary<string,float> dictionary = new Dictionary<string,float>();
			JObject jsonob = JObject.Parse(data.value);
			BaseMiniGameDifficulty difficulty = JsonConvert.DeserializeObject<BaseMiniGameDifficulty>(jsonob.ToString());

			string key = "low";
			string secondkey = "high";
			dictionary [key] = difficulty.low;
			dictionary [secondkey] = difficulty.high;

//			List<string> keys = new List<string>();
//			List<KeyValuePair<string,JToken>> jtokenKeys = jsonob.Convert<JObject,List<KeyValuePair<string,JToken>>>();
//			foreach(KeyValuePair<string,JToken> pair in jtokenKeys)
//			{
//				keys.Add(pair.Key);
//			}
//			for(int i = 0; i < keys.Count; ++i)
//			{
//				dictionary[keys[i]] = float.Parse(jsonob[keys[i]].ToString(),System.Globalization.NumberStyles.AllowDecimalPoint);
//			}
			miniGameDifficulty[data.name] = dictionary;
		}

		void AddToFloatDictionary(Dictionary<string,float> dictionary, GamePropertiesData data)
		{
			string key = data.name;
			float var = float.Parse(data.value,System.Globalization.NumberStyles.AllowDecimalPoint);
			dictionary[key] = var;
		}

		private enum KeyValueIndex
		{
			DEFAULT_FREE = 0,
			DEFAULT_BOOK = 1,
			DEFAULT_TICKET_REFRESH = 2,
			DEFAULT_TICKET = 3,
			DEFAULT_PREMIUM = 4,
			VERSION = 5,
			DIFFICULTY_TROUBLE = 6,
			DIFFICULTY_HARD = 7,
			DIFFICULTY_TRICKY = 8,
			DIFFICULTY_NORMAL = 9,
			DIFFICULTY_EASY = 10,
			SAFE_TROUBLE = 11,
			SAFE_HARD = 12,
			SAFE_TRICKY = 13,
			SAFE_NORMAL = 14,
			SAFE_EASY = 15,
			SPEED_TROUBLE = 16,
			SPEED_HARD = 17,
			SPEED_TRICKY = 18,
			SPEED_NORMAL =19,
			SPEED_EASY = 20,
			FEEDBACK_SCORES = 21,
			DEFAULT_FOCUS = 22,
			DEFAULT_CLOSET = 23,
			DEFAULT_PREPPY = 24,
			DEFAULT_FUNKY = 25,
			DEFAULT_REBEL = 26,
			DEFAULT_FOCUS_REFRESH = 27,
			CUMULATIVE_MAX = 28
		}
	}
}