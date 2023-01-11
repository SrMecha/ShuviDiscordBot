using Discord;
using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Enums;
using Shuvi.Interfaces.Equipment;

namespace Shuvi.Classes.Equipment
{
    public class UserEquipment : IEquipment
    {
        public ObjectId? Weapon { get; private set; }
        public ObjectId? Helmet { get; private set; }
        public ObjectId? Armor { get; private set; }
        public ObjectId? Leggings { get; private set; }
        public ObjectId? Boots { get; private set; }

        public UserEquipment(ObjectId? weapon, ObjectId? head, ObjectId? body, ObjectId? legs, ObjectId? foots)
        {
            Weapon = weapon;
            Helmet = head;
            Armor = body;
            Leggings = legs;
            Boots = foots;
        }
        public IEnumerator<ObjectId?> GetEnumerator()
        {
            yield return Weapon;
            yield return Helmet;
            yield return Armor;
            yield return Leggings;
            yield return Boots;
        }
        public void SetEquipment(ItemType type, ObjectId? id)
        {
            switch (type)
            {
                case ItemType.Weapon:
                    Weapon = id;
                    break;
                case ItemType.Helmet:
                    Helmet = id;
                    break;
                case ItemType.Armor:
                    Armor = id;
                    break;
                case ItemType.Leggings:
                    Leggings = id;
                    break;
                case ItemType.Boots:
                    Boots = id;
                    break;
                default:
                    break;
            };
        }
        public ObjectId? GetEquipment(ItemType type)
        {
            return type switch
            {
                ItemType.Weapon => Weapon,
                ItemType.Helmet => Helmet,
                ItemType.Armor => Armor,
                ItemType.Leggings => Leggings,
                ItemType.Boots => Boots,
                _ => null
            };
        }
        public EquipmentItem? GetEquipmentItem(ItemType type)
        {
            return type switch
            {
                ItemType.Weapon => Weapon == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)Weapon, 0),
                ItemType.Helmet => Helmet == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)Helmet, 0),
                ItemType.Armor => Armor == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)Armor, 0),
                ItemType.Leggings => Leggings == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)Leggings, 0),
                ItemType.Boots => Boots == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)Boots, 0),
                _ => null
            };
        }
        public void RemoveAllEquipment()
        {
            Weapon = null;
            Helmet = null;
            Armor = null;
            Leggings = null;
            Boots = null;
        }
    }
}
