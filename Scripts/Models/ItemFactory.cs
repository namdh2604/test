using System;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
    using Voltage.Witches.Configuration;
    using Voltage.Witches.Configuration.JSON;
    using Voltage.Witches.Exceptions;

    public interface IItemFactory
    {
        Item Create(string id);
    }

    public class ItemFactory : IItemFactory
    {
        private readonly Dictionary<string, ItemConfiguration> _config;
        private readonly IItemRawParser _itemParser;

        public ItemFactory(MasterConfiguration config, IItemRawParser itemParser)
        {
            _config = config.Items_Master;
            _itemParser = itemParser;
        }

        public Item Create(string id)
        {
            if (!_config.ContainsKey(id))
            {
                throw new WitchesException("Invalid item id requested: " + id);
            }

            ItemConfiguration itemConfig = _config[id];

            Item item = null;
            switch (itemConfig.ItemCategory)
            {
                case ItemCategory.INGREDIENT:
                    IngredientData ingredient = itemConfig.Item as IngredientData;
                    item = _itemParser.CreateIngredient(ingredient);
                    item.Display_Order = ingredient.display_order;
                    break;
                case ItemCategory.POTION:
                    PotionData potion = itemConfig.Item as PotionData;
                    item = _itemParser.CreatePotion(potion);
                    break;
                case ItemCategory.CLOTHING:
                    AvatarItemData clothing = itemConfig.Item as AvatarItemData;
                    item = _itemParser.CreateAvatarItem(clothing);
                    item.Display_Order = clothing.display_order;
                    break;
                default:
                    throw new Exception("Unrecognized Item Category: " + itemConfig.ItemCategory);
            }

            return item;
        }
    }
}

