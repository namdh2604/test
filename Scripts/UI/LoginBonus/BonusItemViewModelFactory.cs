using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Login
{
	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Configuration.JSON;

	// This class creates a viewmodel representation of an item based off bonus item
	// TODO: maybe generalize to item not just bonus item, but maybe that factory already exists?
	public class BonusItemViewModelFactory 
	{

		private readonly MasterConfiguration _masterConfig;
		private readonly IItemRawParser _itemParser;

		public BonusItemViewModelFactory(MasterConfiguration masterConfig, IItemRawParser itemParser)
		{
			// TODO: guard clauses

			_masterConfig = masterConfig;
			_itemParser = itemParser;
		}


		public BonusItemViewModel Create(BonusItem bonusItem)
        {
			string itemName = string.Empty;
            string iconPath = string.Empty;

			// FIXME: switching on ID instead of ItemCategory as coin/starstone/stamina potions are not in the master item list
			switch (bonusItem.ID)                                         
            {
                case MasterConfiguration.STARSTONE_ID:
                    itemName = "Starstone";
					iconPath = "Icons/received_item_starstone";
                    break;

                case MasterConfiguration.COIN_ID:
                    itemName = "Coin";
					iconPath = "Icons/received_item_coin";
                    break;

				case MasterConfiguration.STAMINA_POTION_ID:     
					itemName = "Stamina Potion";
					iconPath = "Icons/stamina_bottle";		// "Icons/icon_stamina_potion_small"
                    break;

                default:
					Item item = GetItem(bonusItem.ID);
                    itemName = item.Name;
                    iconPath = GetPathForItem(item);
                    break;
            }
           
            return new BonusItemViewModel() 
            {
                Name = itemName,
				Quantity = bonusItem.Quantity,
                IconPath = iconPath
            };
        }

		private string GetPathForItem(Item item)
		{
			switch (item.Category)
			{
			case ItemCategory.POTION:
//				return (item as Potion).Icon_Path;		// no assets!
				return "Icons/potion_bottle";			// "Icons/bottle_front";

			case ItemCategory.INGREDIENT:
				return (item as Ingredient).IconFilePath;

			case ItemCategory.BUNDLE:
			default:
				throw new ArgumentException("Category Not Supported");
			}
		}

      private Item GetItem(string id)
      {
          BaseData data = _masterConfig.Items_Master[id].Item as BaseData;        // can throw an exception or be null
          ItemCategory category = (ItemCategory)data.item_cat;               	 // can throw an exception

          switch (category) 
          {
              case ItemCategory.POTION:
                  return _itemParser.CreatePotion(data as PotionData);            // can throw an exception

              case ItemCategory.INGREDIENT:
                  return _itemParser.CreateIngredient(data as IngredientData);    // can throw an exception

                case ItemCategory.BUNDLE:
                default:
                    throw new ArgumentException("Category Not Supported");
          }
      }


	}
}









//public BonusItemViewModel Create(string id)
//{
//	Item item = GetItem(id);
//
//	string itemName = item.Name;
//	string iconPath = string.Empty;
//
//	switch (item.Category)
//	{
//	case ItemCategory.COINS:
//		iconPath = "Icons/coin_icon";
//		break;
//
//	case ItemCategory.STARSTONES:
//		iconPath = "Icons/icon_starstone";
//		break;
//
//	case ItemCategory.POTION:
//		//                    iconPath = "Icons/icon_stamina_potion_small";
//
//	default:
//		if (item.Id == MasterConfiguration.STAMINA_POTION_ID)
//		{
//			iconPath = "Icons/icon_stamina_potion_small";
//			break;
//		}
//
//		iconPath = GetPathForItem(item);
//		break;
//	}
//
//	return new BonusItemViewModel() {
//		Name = itemName,
//		IconPath = iconPath
//	};
//}
//
//
//private Item GetItem(string id)
//{
//	BaseData data = _masterConfig.Items_Master[id].Item as BaseData;        // can throw an exception or be null
//	ItemCategory category = (ItemCategory)data.item_cat;                // can throw an exception
//
//	switch (category)
//	{
//	case ItemCategory.INGREDIENT:
//		return _itemParser.CreateIngredient(data as IngredientData);
//
//	case ItemCategory.POTION:
//		return _itemParser.CreatePotion(data as PotionData);
//
//	case ItemCategory.STARSTONES:
//	case ItemCategory.COINS:
//		return new Item(data.id) 
//		{
//			Name = data.name,
//			Category = category
//		};
//
//	case ItemCategory.BUNDLE:
//	default:
//		throw new ArgumentException("Category Not Supported");
//	}
//}