using System.Collections.Generic;
using System.Linq;
using System;
using Voltage.Witches.Components;
using Voltage.Witches.Util;

namespace Voltage.Witches.Models
{
	public interface IMiniGameData
	{
		string Name { get; }
		DifficultyMap DifficultyMap { get; }
	}

	public class MiniGameData : IMiniGameData
	{
		public string Name { get; protected set; }
		public DifficultyMap DifficultyMap { get; protected set; }

		public MiniGameData(string name, Dictionary<string,Dictionary<string,float>> difficultyMapping)
		{
			Name = name;
			AssignDifficultyMap(difficultyMapping);
		}

		private static Dictionary<int,string> _difficultyKeyMap = new Dictionary<int,string>()
		{
			{0, "minigame_difficulty_trouble"},
			{1, "minigame_difficulty_hard"},
			{2, "minigame_difficulty_tricky"},
			{3, "minigame_difficulty_normal"},
			{4, "minigame_difficulty_easy"}
		};

		void AssignDifficultyMap(Dictionary<string,Dictionary<string,float>> difficultyMapping)
		{
			float[] mapValues = new float[5];
			for(int i = 0; i < 5; ++i)
			{
				var lookupKey = _difficultyKeyMap[i];
				var currentDictionary = difficultyMapping[lookupKey];
				mapValues[i] = currentDictionary["high"];
			}

			DifficultyMap = new DifficultyMap(mapValues);
		}
	}
}