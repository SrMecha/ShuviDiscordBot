using MongoDB.Bson;

namespace Shuvi.Interfaces.Equipment
{
    public interface IEquipment
    {
        public ObjectId? Weapon { get; init; }
        public ObjectId? Head { get; init; }
        public ObjectId? Body { get; init; }
        public ObjectId? Legs { get; init; }
        public ObjectId? Foots { get; init; }
    }
}
