using MongoDB.Bson;

namespace Shuvi.Interfaces.Equipment
{
    public interface IEquipment
    {
        public ObjectId? Weapon { get; }
        public ObjectId? Head { get; }
        public ObjectId? Body { get; }
        public ObjectId? Legs { get; }
        public ObjectId? Foots { get; }

        public void RemoveAllEquipment();
    }
}
