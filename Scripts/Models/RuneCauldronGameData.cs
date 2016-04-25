using System.Collections.Generic;
using System.Linq;
using System;
using Voltage.Witches.Components;
using Voltage.Witches.Configuration;
using Voltage.Witches.Controllers;
using Voltage.Witches.Util;

namespace Voltage.Witches.Models
{
	public interface IRuneCauldronGameData
	{
		Dictionary<string,float> SafeZoneDistances { get; }
		Dictionary<string,float> DifficultySpeeds { get; }
		Dictionary<string,int> FeedbackScores { get; }
	}

	public class RuneCauldronGameData :  MiniGameData, IRuneCauldronGameData
	{
		public Dictionary<string,float> SafeZoneDistances { get; protected set; }
		public Dictionary<string,float> DifficultySpeeds { get; protected set; }
		public Dictionary<string,int> FeedbackScores { get; protected set; }

		public Player Player { get; set; }
		public MasterConfiguration MasterData { get; set; }
		public IControllerRepo Repo { get; set; }

		private static Dictionary<int,string> _speedAndZoneKeys = new Dictionary<int,string>()
		{
			{0,"minigame_safe_zone_trouble"},
			{1,"minigame_safe_zone_hard"},
			{2,"minigame_safe_zone_tricky"},
			{3,"minigame_safe_zone_normal"},
			{4,"minigame_safe_zone_easy"},
			{5,"minigame_speed_trouble"},
			{6,"minigame_speed_hard"},
			{7,"minigame_speed_tricky"},
			{8,"minigame_speed_normal"},
			{9,"minigame_speed_easy"}
		};

		private static List<string> _labelStrings = new List<string>()
		{
			"Perfect",
			"perfect",
			"Magical",
			"magical",
			"Not Bad",
			"Nice",
			"Not Quite"
		};

		public RuneCauldronGameData(string name, Dictionary<string,Dictionary<string,float>> difficultyMapping, Dictionary<string,float> speedsAndZones, Dictionary<string,int> scoring)
		:base(name,difficultyMapping)
		{
			SafeZoneDistances = new Dictionary<string,float>();
			DifficultySpeeds = new Dictionary<string,float>();
			FeedbackScores = new Dictionary<string,int>();

			SetSafeZonesAndSpeeds(speedsAndZones);
//			UnityEngine.Debug.Log("Speeds and Zones built");

			SetFeedbackScores(scoring);
//			UnityEngine.Debug.Log("Scoring built");
//
//			UnityEngine.Debug.Log("Data built");
		}

		void SetSafeZonesAndSpeeds(Dictionary<string, float> speedsAndZones)
		{
			if(speedsAndZones == null)
			{
				return;
			}
			for(int i = 0; i < speedsAndZones.Count; ++i)
			{
				if(!_speedAndZoneKeys.ContainsKey(i))
				{
					continue;
				}
				var lookupKey = _speedAndZoneKeys[i];
				if(!speedsAndZones.ContainsKey(lookupKey))
				{
					continue;
				}
				var valueToAdd = speedsAndZones[lookupKey];
				if(lookupKey.Contains("zone"))
				{
					SafeZoneDistances[lookupKey] = valueToAdd;
				}
				else
				{
					DifficultySpeeds[lookupKey] = valueToAdd;
				}
			}
		}

		void SetFeedbackScores(Dictionary<string, int> scoring)
		{
			if(scoring == null)
			{
				return;
			}
			for(int i = 0; i < scoring.Count; ++i)
			{
				var lookupKey = _labelStrings[i];
//				UnityEngine.Debug.Log(lookupKey);
				//HACK Because the data key was input wrong on the server
//				if(lookupKey == "Not Bad")
//				{
//					lookupKey = lookupKey.Replace("Bad","bad");
//				}

				if((scoring.ContainsKey(lookupKey)) && (!FeedbackScores.ContainsKey(lookupKey)))
				{
//					UnityEngine.Debug.Log("LookupKey exists in the incoming dicitonary");
					var valuetoAdd = scoring[lookupKey];
//					UnityEngine.Debug.Log(valuetoAdd.ToString() + " is the score value in the dictionary");
					if(!FeedbackScores.ContainsValue(valuetoAdd))
					{
//						UnityEngine.Debug.Log("Value is not in the Dictionary, adding");
//						if(lookupKey == "Not bad")
//						{
//							lookupKey = lookupKey.Replace("bad","Bad");
//						}
						FeedbackScores[lookupKey] = valuetoAdd;
//						UnityEngine.Debug.Log("Added value to FeedbackScores");
					}
				}
			}
		}
	}
}