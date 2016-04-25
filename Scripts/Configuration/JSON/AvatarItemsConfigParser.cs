using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Models;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public interface IAvatarItemsConfigParser
	{
		AvatarItemsConfiguration Construct(string json);
		AvatarItemsConfiguration Construct (List<AvatarItemData> avatarItemData);
	}

	public class AvatarItemsConfigParser : IAvatarItemsConfigParser
	{
		public AvatarItemsConfigParser()
		{
		}


		public AvatarItemsConfiguration Construct(List<AvatarItemData> avatarItemData)
		{
			AvatarItemsConfiguration avatarItemsConfig = new AvatarItemsConfiguration();
			AddParsedListToConfigDictionary<AvatarItemData> (avatarItemData, avatarItemsConfig.Avatar_Items);

			return avatarItemsConfig;
		}

		public AvatarItemsConfiguration Construct(string json)
		{
			AvatarItemsConfiguration avatarItemsConfig = new AvatarItemsConfiguration();
			JObject jsonObject = null;
			try
			{
				jsonObject = JObject.Parse(json);
			}
			catch(Exception)
			{
				System.Console.WriteLine("There was an error parsing the avatar items json");
			}
			AvatarItemsDataModel avatarItemsData = JsonConvert.DeserializeObject<AvatarItemsDataModel>(jsonObject.ToString());
//			AddParsedListToConfigDictionary<ItemCategoryLayerData>(avatarItemsData.item_categories, avatarItemsConfig.Item_Categories);
			AddParsedListToConfigDictionary<AvatarItemData> (avatarItemsData.avatar_items, avatarItemsConfig.Avatar_Items);

			return avatarItemsConfig;
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