using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Shuvi.Enums;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Classes.Characteristics;

namespace Shuvi.Classes.Items
{
    public sealed class ItemData
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Хуйня без имени";
        public string Description { get; set; } = "Долбаеб блять добавь описание предмету!!!";
        public ItemType Type { get; set; } = ItemType.Simple;
        public Rank Rank { get; set; } = Rank.E;
        public bool CanTrade { get; set; } = false;
        public int Max { get; set; } = -1;
        public CharacteristicBonuses Bonuses { get; set; } = new();
        public Dictionary<ItemNeeds, int> Needs { get; set; } = new();
    }
}
