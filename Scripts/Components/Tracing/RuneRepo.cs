using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Voltage.Witches.Components
{
	using TextAsset = UnityEngine.TextAsset;
	using Texture2D = UnityEngine.Texture2D;
	using Resources = UnityEngine.Resources;
	using Sprite = UnityEngine.Sprite;

	using Newtonsoft.Json;

	using Voltage.Common.DebugTool.Timer;

	public class RuneRepo 
	{
		public List<RuneObject> Available_Runes { get; protected set; }
		public Dictionary<string,Sprite> Available_Rune_Art { get; protected set; }

        public List<RuneObject> Available_Trace { get; protected set; }
        public Dictionary<string,Sprite> Available_Trace_Art { get; protected set; }

		private string _runeArtPath = "Textures/Runes";

		private static Dictionary<string,string> _spriteMapping = new Dictionary<string,string>()
		{
			{"23","Cross_Box"},
			{"18","Double_Arrow"},
			{"15","Dual_Trident"},
			{"20","Evil_Woman"},
			{"21","Four_Box"},
			{"16","Iron_Sight"},
			{"22","Kull_Box"},
			{"17","Arrow_Up"},
			{"19","The_Eye"},
			{"14","Hour_Glass"},
			{"13","Triangle_Tri"},
			{"12","Wine_Glass"}
		};


		public RuneRepo()
		{
			Dictionary<string,List<Texture2D>> allLoaded = GetAllLoadedInGroup();

			BuildRuneList(allLoaded);
			BuildSpriteRepo();
		}

		public void UnloadResources()
		{
			Available_Rune_Art = null;
			Available_Runes = null;
			Available_Trace = null;
			Available_Trace_Art = null;
			Resources.UnloadUnusedAssets ();
		}

		Dictionary<string, List<Texture2D>> GetAllLoadedInGroup()
		{
			Dictionary<string, List<Texture2D>> parsedDictionary = new Dictionary<string, List<Texture2D>>();
			var runeStrokes = Resources.LoadAll<Texture2D>("Tracing");

			for(int i = 0; i < runeStrokes.Length; ++i)
			{

				var current = runeStrokes[i];
				var runeName = GetRuneNameFromFileName(current.name);
				var id = GetStrokeIDFromName(current.name);
				if(!parsedDictionary.ContainsKey(runeName))
				{
					List<Texture2D> newList = new List<Texture2D>();
					newList.Insert(id,current);
					parsedDictionary[runeName] = newList;
				}
				else
				{
					var list = parsedDictionary[runeName];
					list.Insert(id,current);
				}
			}

			return parsedDictionary;
		}

		string GetRuneNameFromFileName(string name)
		{
			var index = name.LastIndexOf(('_'));
			var toRemove = name.Substring(index);
			name = name.Replace(toRemove, "");

			return name;
		}

		int GetStrokeIDFromName(string name)
		{
			var parts = name.Split(('_'));
			var id = (int)Convert.ToSingle(parts[parts.Length - 1]);
			return id;
		}

		void BuildRuneList(Dictionary<string, List<Texture2D>> loadedImages)
		{
			Available_Runes = new List<RuneObject>();
			foreach(var pair in loadedImages)
			{
				RuneObject rune = new RuneObject(pair.Key);
				var list = pair.Value;
                rune.Stroke_Count = list.Count;

				for(int i = 0; i < list.Count; ++i)
				{
					var texture = list[i];
                    rune.AddTexture(texture);
				}

				Available_Runes.Add(rune);
			}
		}

		void BuildSpriteRepo()
		{
			var allSprites = Resources.LoadAll<Sprite>(_runeArtPath);
			Available_Rune_Art = new Dictionary<string,Sprite>();
            Available_Trace_Art = new Dictionary<string,Sprite>();
            for(int i = 0; i < allSprites.Length; ++i)
			{
				var currentSprite = allSprites[i];
				if(currentSprite.texture.name.Contains("assets02"))
				{
					var parsedName = GetParsedNameFromSpriteName(currentSprite.name);
					Available_Rune_Art[parsedName] = currentSprite;
				}
                if(currentSprite.texture.name.Contains("rune_trace"))
                {
                    Available_Trace_Art[currentSprite.name] = currentSprite;
                }
			}
		}

		string GetParsedNameFromSpriteName(string name)
		{
			var parts = name.Split(('_'));
			var key = parts[parts.Length - 1];

			return _spriteMapping[key];
		}

		public List<RuneObject> GetRandomizedRuneSetByNumber(int desiredRunes)
		{
			List<RuneObject> runeSet = new List<RuneObject>();
			var indexes = GetIndexList(desiredRunes);

			for(int i = 0; i < indexes.Count; ++i)
			{
				runeSet.Add(Available_Runes[indexes[i]]);
			}

			return runeSet;
		}

		List<int> GetIndexList(int desiredCount)
		{
			Random randomizer = new Random();
			List<int> indexes = new List<int>();

            for(int i = 0; i < desiredCount; ++i)
            {
				var index = randomizer.Next(Available_Runes.Count);
                if (i > 0)
                {
                    bool done = false;
                    while(!done)
                    {
                        bool found = false;
                        for (int j = 0; j < indexes.Count; j++)
                        {
                            if (indexes[j] == index)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            index = randomizer.Next(Available_Runes.Count);
                        }
                        else
                        {
                            indexes.Add(index);
                            done = true;
                        }
                    }
                }
                else
                {
				    indexes.Add(index);
                }
			}

			return indexes;
		}
	}
}