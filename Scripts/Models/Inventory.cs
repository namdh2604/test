using System.Collections.Generic;
using System;

namespace Voltage.Witches.Models
{
    using Voltage.Story.User;


    // An inventory class to present the user inventory data in a more friendly manner
    // Note that because this class maintains its own user-friendly dictionary of items,
    // it could become out of sync with the actual player data if someone externally modified
    // the player data after this class was constructed.
    // The trade-off is efficiency -- we don't have to dynamically construct the items on each call
	public class Inventory
	{
		// hopefully this won't become out of sync with player
		private Dictionary<string, KeyValuePair<Item, int>> _items;
        private readonly IPlayer _player;

        public Inventory(IItemFactory itemFactory, IPlayer player)
		{
            _player = player;

			_items = ConstructInitialInventory (itemFactory);
		}

        private Dictionary<string, KeyValuePair<Item, int>> ConstructInitialInventory(IItemFactory itemFactory)
        {
            var items = new Dictionary<string, KeyValuePair<Item, int>>();
            foreach (var itemEntry in _player.GetInventoryData())
            {
                var constructedItem = itemFactory.Create(itemEntry.Key);
                items[itemEntry.Key] = new KeyValuePair<Item, int>(constructedItem, itemEntry.Value);
            }

            return items;
        }

		public void Add(Item item, int count)
		{
			ModifyInventoryCount(item, count);
		}

		public void Remove(Item item, int count)
		{
			ModifyInventoryCount(item, -count);
		}

		void ModifyInventoryCount(Item item, int delta)
		{
			int existingCount = GetCount(item);
			if (-delta > existingCount)
			{
				RaiseInvalidRemovalMessage(delta, existingCount);
			}
			//TODO if this becomes zero, we should remove the item
			if((existingCount + delta) > 0)
			{
				_items[item.Id] = new KeyValuePair<Item, int>(item, existingCount + delta);
			}
			else
			{
				_items.Remove(item.Id);
			}

            _player.UpdateInventory(item.Id, delta);
		}

		void RaiseInvalidRemovalMessage(int requestedAmount, int currentAmount)
		{
			string errorFormat = "Attempted to remove {0} items when only {1} exist";
			throw new Exception(string.Format(errorFormat, -requestedAmount, currentAmount));
		}

		public int GetCount(Item item)
		{
			KeyValuePair<Item, int> itemEntry;
			bool found = _items.TryGetValue(item.Id, out itemEntry);
			if (found)
			{
				return itemEntry.Value;
			}

			return 0;
		}

		public int GetIngredientCount(Ingredient ingredient)
		{
			if(_items.ContainsKey(ingredient.Id))
			{
				return _items[ingredient.Id].Value;
			}

			return 0;
		}

		public List<IngredientCategory> GetAvailableIngredientCategories()
		{
			List<IngredientCategory> allCategories = new List<IngredientCategory>();
			foreach(KeyValuePair<string,KeyValuePair<Item,int>> pair in _items)
			{
				if(!pair.Equals(null))
				{
					Item currentItem = pair.Value.Key;
					if((currentItem.Category == ItemCategory.INGREDIENT) && (!string.IsNullOrEmpty(pair.Key)))
					{
						Ingredient currentIngredient = currentItem as Ingredient;
						if((currentIngredient != null) && (currentIngredient.IngredientCategory != null))
						{
							IngredientCategory currentCat = currentIngredient.IngredientCategory;
							if(!allCategories.Contains(currentCat))
							{
								allCategories.Add(currentIngredient.IngredientCategory);
							}
						}
					}
				}
			}

			return allCategories;
		}

		public List<Item> GetAllItemsByCategory(ItemCategory category)
		{
			List<Item> allItems = new List<Item>();
			foreach(KeyValuePair<string,KeyValuePair<Item,int>> pair in _items)
			{
				if(!pair.Equals(null))
				{
					Item currentItem = pair.Value.Key;
					int itemCount = pair.Value.Value;
					if((currentItem != null) && (currentItem.Category == category) && 
					   (!allItems.Contains(currentItem)) && (!string.IsNullOrEmpty(pair.Key)) &&
					   (itemCount > 0))
					{
						allItems.Add(currentItem);
					}
				}
			}

			allItems.Sort((ds1,ds2) => ds1.Display_Order.CompareTo(ds2.Display_Order));

			return allItems;
		}
	}
}

