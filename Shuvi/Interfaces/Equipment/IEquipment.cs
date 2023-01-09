using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Enums;

namespace Shuvi.Interfaces.Equipment
{
    public interface IEquipment
    {
        public ObjectId? Weapon { get; }
        public ObjectId? Helmet { get; }
        public ObjectId? Armor { get; }
        public ObjectId? Leggings { get; }
        public ObjectId? Boots { get; }

        public void SetEquipment(ItemType type, ObjectId? id);
        public ObjectId? GetEquipment(ItemType type);
        public EquipmentItem? GetEquipmentItem(ItemType type);
        public void RemoveAllEquipment();
    }
}
