using System;
using System.Collections.Generic;
using System.Collections;

namespace Voltage.Witches.Login
{
    using Voltage.Common.Logging;

	using Voltage.Witches.Models;
	using Voltage.Witches.Configuration;
	using Voltage.Witches.Configuration.JSON;

	// This class manages the ingame interaction with the bonus item data
	// It can further return a modelview list of the data for use by a dialog, BUT this feature may be refactored out in the future
	public class BonusManager 
	{
		private readonly Player _player;
		private readonly Inventory _inventory;
		private readonly MasterConfiguration _masterConfig;
		private readonly IItemRawParser _itemParser;
		private readonly BonusItemViewModelFactory _modelViewFactory;


        public const int BONUS_INDEX = 1;                   // 0-based
        private const int EXPECTED_BONUS_LIST_SIZE = 4;     // 0: past, 1: bonus, 2: next, 3: future


		public BonusManager(Player player, Inventory inventory, MasterConfiguration masterConfig, IItemRawParser itemParser, BonusItemViewModelFactory modelViewFactory)
		{
            // TODO: guard clauses

            _player = player;
            _inventory = inventory;
            _masterConfig = masterConfig;
			_itemParser = itemParser;
			_modelViewFactory = modelViewFactory;
		}


		
		public void GiveBonusItem()
		{
            if (HasBonusItem())
            {
                BonusItem bonus = _player.BonusItems[BONUS_INDEX];     

                GiveItem(bonus.ID, bonus.Quantity);    
                _player.AwardedBonusItem();          // FIXME: giving and clearing should be atomic
            }
            else
            {
                AmbientLogger.Current.Log("No Bonus Item to Give", LogLevel.WARNING);
            }
		}



        private void GiveItem(string id, int quantity)
        {
			// FIXME: switching on ID instead of ItemCategory as coin/starstone/stamina potions are not in the master item list
            switch (id)						
            {
                case MasterConfiguration.STARSTONE_ID:
                    _player.UpdatePremiumCurrency(quantity);
                    break;
        
                case MasterConfiguration.COIN_ID:
					_player.UpdateCurrency(quantity);
                    break;
        
				// SyncResources handles updating Stamina Potions!
                case MasterConfiguration.STAMINA_POTION_ID:     // does not apply to other potions! so can't use ItemCategory
                  	_player.UpdateStaminaPotion(quantity);
                    break;
        
                default:
                    Item item = GetItem(id);
                    _inventory.Add(item, quantity);                                 // item can be null
                    break;
            }
        }

		private Item GetItem(string id)
		{
			BaseData data = _masterConfig.Items_Master[id].Item as BaseData;        // can throw an exception or be null
			ItemCategory category = (ItemCategory)data.item_cat;               		// can throw an exception

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


		// maybe rename to BonusAvailable()?
        public bool HasBonusItem()
        {
			// BonusItems are cleared after shown to prevent repeated showings
			return _player.BonusItems != null && _player.BonusItems.Count == EXPECTED_BONUS_LIST_SIZE;   // exactly match bonus design
        }




        // FIXME: possibly move this feature outside of this manager
        public List<BonusItemViewModel> GetViewModelList()
        {
            List<BonusItemViewModel> viewModelList = new List<BonusItemViewModel>();

            foreach (BonusItem item in _player.BonusItems)
            {
				viewModelList.Add(_modelViewFactory.Create(item));
            }

            return viewModelList;
        }


	}





}




