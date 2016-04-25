using System;
using System.Collections;
using System.Collections.Generic;
using Voltage.Witches.Configuration;

namespace Voltage.Witches.Configuration.JSON
{
	using Components;
	using Models;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public interface IMasterDataParser
	{
		MasterConfiguration Construct(string mainJson);
//		MasterConfiguration Construct(string mainJson,string avatarJson,string potionsJson,string recipesJson);
//		MasterConfiguration Construct(string mainJson,string avatarJson);
		bool HasGamePropertiesConfigParser { get; }
	}

	public class MasterDataConfigParser: IMasterDataParser
	{
		IGamePropertiesConfigParser _gamePropertiesConfigParser;
		IAffinityConfigParser _affinityConfigParser;
		IBooksConfigParser _booksConfigParser;
		IAvatarItemsConfigParser _avatarItemsConfigParser;
		IPotionsConfigParser _potionsConfigParser;
		IRecipesConfigParser _recipesConfigParser;

		public bool HasGamePropertiesConfigParser
		{
			get { return (_gamePropertiesConfigParser != null); }
		}

		public MasterDataConfigParser(IGamePropertiesConfigParser gamePropertiesConfigParser,IBooksConfigParser booksConfigParser,
		                              IAffinityConfigParser affinityConfigParser,
		                              IAvatarItemsConfigParser avatarItemsConfigParser,IPotionsConfigParser potionsConfigParser,
		                              IRecipesConfigParser recipesConfigParser)
		{
			_gamePropertiesConfigParser = gamePropertiesConfigParser;
			_booksConfigParser = booksConfigParser;
			_affinityConfigParser = affinityConfigParser;
			_avatarItemsConfigParser = avatarItemsConfigParser;
			_potionsConfigParser = potionsConfigParser;
			_recipesConfigParser = recipesConfigParser;

		}

		public MasterConfiguration Construct(string mainJson) 
		{
			MasterConfiguration masterConfig = new MasterConfiguration();

			JObject jsonObject = null;
			try
			{
				jsonObject = JObject.Parse(mainJson);
			}
			catch(Exception)
			{
				throw new Exception("There was a problem parsing the json");
			}
			MasterGameData master = null;
			try
			{
				master = JsonConvert.DeserializeObject<MasterGameData>(jsonObject.ToString());
			}
			catch(Exception)
			{
				throw new Exception("Master Data JSON was malformed");
			}

			masterConfig.Version = master.version;

			AddParsedListToConfigDictionary<IngredientData>(master.ingredient_mst,masterConfig.Ingredients);
			AddParsedListToConfigDictionary<CategoryData>(master.category_mst,masterConfig.Categories);
			AddParsedListToConfigDictionary<GamePropertiesData>(master.game_mst,masterConfig.Game_Properties);
			masterConfig.Focus_Refresh_Rate = master.focus_refresh_rate;
			masterConfig.Ticket_Refresh_Rate = master.ticketRefreshRate;
			AddParsedListToConfigDictionary<ShopItemData>(master.shop_items_mst,masterConfig.Shop_Items.Shop_Items_Master);
			AddParsedListToConfigDictionary<CharacterData>(master.characters_mst,masterConfig.Character_Info);
			if(master.book_prizes != null)
			{
				AddParsedListToConfigDictionary<BookPrizeData>(master.book_prizes,masterConfig.Book_Prizes);
			}

            // TODO: Fix this once the server properly returns these values
            masterConfig.Max_Focus = (master.max_focus == 0) ? 5 : master.max_focus;
            masterConfig.Max_Tickets = (master.max_tickets == 0) ? 5 : master.max_tickets;

			try
			{
				var shopItems = masterConfig.Shop_Items.Shop_Items_Indexed;
				for(int l = 0; l < master.shop_items_mst.Count; ++l)
				{
					var currentItem = master.shop_items_mst[l];
					if(currentItem.name.Contains("Starstone"))
					{
						currentItem.item_cat = (int)ItemCategory.STARSTONES;
					}
					else if(currentItem.name.Contains("Stamina"))
					{
						currentItem.item_cat = (int)ItemCategory.POTION;
					}
                    else if(currentItem.name.Contains("Starter Pack"))
                    {
                        // design specified there will ever only be one starter pack
                        currentItem.item_cat = (int)ItemCategory.BUNDLE;
                        masterConfig.Shop_Items.StarterPack = currentItem;   
                    }
                    shopItems[currentItem.item_index] = currentItem;        // includes starter pack
				}
			}
			catch(Exception)
			{
				throw new Exception("There was a problem building the indexed shop item list");
			}

			try
			{
				masterConfig.Books_Configuration = _booksConfigParser.Construct(master.book_mst);
				masterConfig.Books_Configuration.BookUnlockMap = master.book_unlock;
			}
			catch(Exception)
			{
				throw new Exception("Book data was malformed");
			}

			if(master.potions_mst != null)
			{
				try
				{
					masterConfig.Potions_Configuration = _potionsConfigParser.Construct(master.potions_mst);
				}
				catch(Exception)
				{
					throw new Exception("Potion data was malformed");
				}
			}
			else
			{
				System.Console.WriteLine("Potion Data NOT included in master");
			}




//			masterConfig.Avatar_Items_Configuration = _avatarItemsConfigParser.Construct(master.avatar_items_mst);
//			string avatarJson = "{\"avatar_items\":" + jsonObject["avatar_items_mst"].ToString() + "}";
//			masterConfig.Avatar_Items_Configuration = _avatarItemsConfigParser.Construct(avatarJson);		// TODO: construct like other parsers
			try
			{
				masterConfig.Avatar_Items_Configuration = _avatarItemsConfigParser.Construct(master.avatar_items_mst);
			}
			catch(Exception)
			{
				throw;
			}



			try
			{
				masterConfig.Game_Properties_Config = _gamePropertiesConfigParser.Construct(master.game_mst);
			}
			catch(Exception)
			{
				throw new Exception("Game Data was malformed");
			}

			if(master.recipes_mst != null)
			{
				try
				{
					masterConfig.Recipes_Configuration = _recipesConfigParser.Construct(master.recipes_mst);
				}
				catch(Exception)
				{
					throw new Exception("Recipes Data was malformed");
				}
			}
			else
			{
				try
				{
					System.Console.WriteLine("Recipes Data NOT included in master");
				}
				catch(Exception)
				{
					throw new Exception("Recipes Data was malformed");
				}
			}

			List<Affinity> affinities = new List<Affinity>();

			try
			{
				for(int j = 0; j < master.affinity_mst.Count; ++j)
				{
					var affinityData = master.affinity_mst[j];
					var affinityConfig = CreateAffinityConfigFromData(affinityData);
					masterConfig.Affinity_Configs[affinityConfig.Id] = affinityConfig;
					affinities.Add(new Affinity(affinityConfig.Name,affinityConfig.Grade,affinityConfig.TotalAffinity));
				}
			}
			catch(Exception)
			{
				throw new Exception("There was a problem creating the affinity config from the data");
//				System.Console.WriteLine("There was a problem creating the affinity config from the data");
			}

			Affinity[] affinityArray = null;
			try
			{
				affinityArray = affinities.ToArray();
			}
			catch(Exception)
			{
				throw new Exception("There was a problem converting the affinities into arrays");
			}

			try
			{
				masterConfig.Affinity_Map = new AffinityMap(affinityArray);
			}
			catch(Exception)
			{
				throw new Exception("There was a problem creating the affinity maps");
//				System.Console.WriteLine("There was a problem creating the affinity maps");
			}

			var itemsMaster = masterConfig.Items_Master;	
			var itemConfigs = new List<ItemConfiguration>();

			try
			{
				var ingredientMaster = masterConfig.Ingredients;
				var avatarMaster = masterConfig.Avatar_Items_Configuration.Avatar_Items;
				var potionMaster = masterConfig.Potions_Configuration.Potions_Dictionary;

				
				itemConfigs.AddRange(GetItemConfigsFromDictionary<IngredientData>(ingredientMaster));
				itemConfigs.AddRange(GetItemConfigsFromDictionary<AvatarItemData>(avatarMaster));
				itemConfigs.AddRange(GetItemConfigsFromDictionary<PotionData>(potionMaster));
			}
			catch(Exception)
			{
				throw new Exception("There was a problem adding the item configs");
			}

			try
			{
				for(int k = 0; k < itemConfigs.Count; ++k)
				{
                    var current = itemConfigs[k];
					itemsMaster[current.Id] = current;
				}
			}
			catch(Exception)
			{
				throw new Exception("There was a problem configuring the item configs");
			}
//			}
//			catch(Exception e)
//			{
//				Console.WriteLine(e.StackTrace);
//				Console.WriteLine(e.Message);
//				Console.WriteLine(e.Source);
////				UnityEngine.Debug.LogError(e.ToString());
//				throw e;
//			}
			return masterConfig;
		}

		private List<ItemConfiguration> GetItemConfigsFromDictionary<T>(Dictionary<string, T> dataDictionary) where T : BaseData
		{
			List<ItemConfiguration> itemConfigs = new List<ItemConfiguration>();
			foreach(var pairing in dataDictionary)
			{
				T data = pairing.Value;
				ItemConfiguration itemConfig = new ItemConfiguration(pairing.Key);
				itemConfig.Item = data;
				itemConfig.ItemCategory = (ItemCategory)data.item_cat;
				itemConfigs.Add(itemConfig);
			}

			return itemConfigs;
		}

		private AffinityConfiguration CreateAffinityConfigFromData(AffinityData data)
		{
			return _affinityConfigParser.Create(data);
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








// HACK: coins/starstones/stamina potion(?)/bundles do not have an ID
//private void AddCustomItems(Dictionary<string,ItemConfiguration> masterItems)
//{
//	BaseData starstoneData = new BaseData() 
//	{
//		id = MasterConfiguration.STARSTONE_ID,
//		name = "Starstones",
//		item_cat = (int)ItemCategory.STARSTONES
//	};
//	ItemConfiguration starstoneConfig = new ItemConfiguration(starstoneData.id) 
//	{
//		Item = starstoneData,
//		ItemCategory = (ItemCategory)starstoneData.item_cat,
//		CategoryID = string.Empty
//	};
//	masterItems.Add(starstoneConfig.Id, starstoneConfig);
//
//
//	// Coins
//	BaseData coinData = new BaseData() 
//	{
//		id = MasterConfiguration.COIN_ID,
//		name = "Coins",
//		item_cat = (int)ItemCategory.COINS,
//	};
//	ItemConfiguration coinConfig = new ItemConfiguration(coinData.id)
//	{
//		Item = coinData,
//		ItemCategory = (ItemCategory)coinData.item_cat,
//		CategoryID = string.Empty
//	};
//	masterItems.Add(coinConfig.Id, coinConfig);
//
//
//	// Stamina Potions
//	// TODO: verify stamina potion doesn't already exist in master item list
//	BaseData staminaPotionData = new PotionData() 
//	{
//		id = MasterConfiguration.STAMINA_POTION_ID,
//		name = "Stamina Potion",
//		item_cat = (int)ItemCategory.POTION,
//		effect_list = new List<Dictionary<string,int>>(),
//	};
//	ItemConfiguration staminaPotionConfig = new ItemConfiguration(staminaPotionData.id)
//	{
//		Item = staminaPotionData,
//		ItemCategory = (ItemCategory)staminaPotionData.item_cat,
//		CategoryID = string.Empty
//	};
//	masterItems.Add(staminaPotionConfig.Id, staminaPotionConfig);
//
//
//	// TODO: bundle config
//
//}


