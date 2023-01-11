using MongoDB.Bson;
using Shuvi.Enums;
using Shuvi.Interfaces.Items;

namespace Shuvi.Classes.Items
{
    public static class ItemFactory
    {
        public static IItem CreateItem(ItemData itemDocument, int amount)
        {
            return itemDocument.Type switch
            {
                ItemType.Weapon => new EquipmentItem(itemDocument, amount),
                ItemType.Helmet => new EquipmentItem(itemDocument, amount),
                ItemType.Armor => new EquipmentItem(itemDocument, amount),
                ItemType.Leggings => new EquipmentItem(itemDocument, amount),
                ItemType.Boots => new EquipmentItem(itemDocument, amount),
                ItemType.Simple => new SimpleItem(itemDocument, amount),
                ItemType.Potion => new PotionItem(itemDocument, amount),
                ItemType.Chest => new ChestItem(itemDocument, amount),
                _ => new SimpleItem(itemDocument, amount)
            };
        }
        public static IItem CreateItem(ObjectId itemId, int amount)
        {
            var itemData = AllItemsData.GetItemData(itemId);
            return itemData.Type switch
            {
                ItemType.Weapon => new EquipmentItem(itemData, amount),
                ItemType.Helmet => new EquipmentItem(itemData, amount),
                ItemType.Armor => new EquipmentItem(itemData, amount),
                ItemType.Leggings => new EquipmentItem(itemData, amount),
                ItemType.Boots => new EquipmentItem(itemData, amount),
                ItemType.Simple => new SimpleItem(itemData, amount),
                ItemType.Potion => new PotionItem(itemData, amount),
                ItemType.Chest => new ChestItem(itemData, amount),
                _ => new SimpleItem(itemData, amount)
            };
        }
    }
}
