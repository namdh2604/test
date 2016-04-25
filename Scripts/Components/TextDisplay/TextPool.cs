using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Voltage.Witches.Components.TextDisplay
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Random = System.Random;

	public interface ITextPool
	{
		List<string> GetRandomizedStringSet(int? seed);
	}

	public class TextPool : ITextPool
	{
		private string _fileName;
		private readonly List<string> _availableHints;
		private Random _randomizer;

		public TextPool(string fileName)
		{
			if(string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("_fileName",new Exception("File Name was Null"));
			}
			_fileName = System.IO.Path.Combine("JSON",fileName);
			var jsonObject = UnityEngine.Resources.Load<UnityEngine.Object>(_fileName);
			_availableHints = BuildHintList(jsonObject);
		}

		List<string> BuildHintList(UnityEngine.Object textAsset)
		{
			try
			{
				string json = textAsset.ToString();
				JObject job = JObject.Parse(json);

				List<HintObject> hints = JsonConvert.DeserializeObject<List<HintObject>>(job["hints"].ToString());
				List<string> returnList = new List<string>();

				for(int i = 0; i < hints.Count; ++i)
				{
					var current = hints[i];
					returnList.Add(current.hinttext);
				}

				return returnList;
			}
			catch(Exception)
			{
				throw new Exception(_fileName + " JSON was malformed");
			}
		}

		public List<string> GetRandomizedStringSet(int? seed)
		{
			var hintSet = Shuffle(_availableHints, seed);
			return hintSet;
		}

		private List<string> Shuffle(List<string> collection, int? seed)
		{
			string[] elements = collection.ToArray();
			if(!seed.HasValue)
			{
				_randomizer = new Random();
			}
			else
			{
				_randomizer = new Random(seed.Value);
			}
			for(int i = elements.Length - 1; i >= 0; --i)
			{
				int swapIndex = _randomizer.Next(i + 1);
				elements[swapIndex] = elements[i];
			}

			return elements.ToList();
		}

		private class HintObject
		{
			public string hinttext = string.Empty;
		}
	}
}