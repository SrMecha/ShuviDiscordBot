using MongoDB.Bson;
using Shuvi.Interfaces.Equipment;

namespace Shuvi.Classes.Equipment
{
    public class UserEquipment : IEquipment
    {
        public ObjectId? Weapon { get; init; }
        public ObjectId? Head { get; init; }
        public ObjectId? Body { get; init; }
        public ObjectId? Legs { get; init; }
        public ObjectId? Foots { get; init; }

        public UserEquipment(ObjectId? weapon, ObjectId? head, ObjectId? body, ObjectId? legs, ObjectId? foots)
        {
            Weapon = weapon;
            Head = head;
            Body = body;
            Legs = legs;
            Foots = foots;
        }
        public IEnumerator<ObjectId?> GetEnumerator()
        {
            yield return Weapon;
            yield return Head;
            yield return Body;
            yield return Legs;
            yield return Foots;
        }
    }
}
