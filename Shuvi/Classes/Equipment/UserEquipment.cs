using MongoDB.Bson;
using Shuvi.Interfaces.Equipment;

namespace Shuvi.Classes.Equipment
{
    public class UserEquipment : IEquipment
    {
        public ObjectId? Weapon { get; private set; }
        public ObjectId? Head { get; private set; }
        public ObjectId? Body { get; private set; }
        public ObjectId? Legs { get; private set; }
        public ObjectId? Foots { get; private set; }

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
        public void RemoveAllEquipment()
        {
            Weapon = null;
            Head = null;
            Body = null;
            Legs = null;
            Foots = null;
        }
    }
}
