using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Models;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public interface IPotionsConfigParser
	{
		PotionsConfiguration Construct(string json);
		PotionsConfiguration Construct(List<PotionData> potionsData);
	}

	public class PotionsConfigParser : IPotionsConfigParser
	{
		public PotionsConfigParser()
		{
		}

		public PotionsConfiguration Construct(List<PotionData> potionsData)
		{
			PotionsConfiguration potionsConfig = new PotionsConfiguration();
			AddParsedListToConfigDictionary<PotionData>(potionsData, potionsConfig.Potions_Dictionary);

			return potionsConfig;
		}

		public PotionsConfiguration Construct(string json)
		{
			PotionsConfiguration potionsConfig = new PotionsConfiguration();
			JObject jsonObject = JObject.Parse(json);
			PotionsAllData potionsData = JsonConvert.DeserializeObject<PotionsAllData>(jsonObject.ToString());
			AddParsedListToConfigDictionary<PotionData>(potionsData.potions,potionsConfig.Potions_Dictionary);

			return potionsConfig;
		}

		private void AddParsedListToConfigDictionary<T>(List<T> parsedList, Dictionary<string,T> configDictionary) where T : BaseData
		{
			for(int i = 0; i < parsedList.Count; ++i)
			{
				T data = parsedList[i];
				configDictionary[data.id] = data;
			}
		}
	}
}