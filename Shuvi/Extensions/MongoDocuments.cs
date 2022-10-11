using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShuviBot.Enums.ItemType;
using ShuviBot.Enums.Ranks;
using ShuviBot.Enums.UserProfessions;
using ShuviBot.Enums.UserRaces;
using ShuviBot.Enums.Characteristics;
using ShuviBot.Enums.ItemNeeds;

namespace ShuviBot.Extensions.MongoDocuments
{
    public sealed class UserDocument
    {
        [BsonElement("_id")]
        public ulong Id { get; set; } = 0;

        [BsonElement("rating")]
        public Ranks Rating { get; set; } = Ranks.E;

        [BsonElement("money")]
        public int Money { get; set; } = 0;

        [BsonElement("race")]
        public UserRaces Race { get; set; } = UserRaces.ExMachina;

        [BsonElement("profession")]
        public UserProfessions Profession { get; set; } = UserProfessions.NoProfession;

        [BsonElement("inventory")]
        public Dictionary<ObjectId, int> Inventory { get; set; } = new();

        [BsonElement("weapon")]
        public ObjectId? Weapon { get; set; } = null;

        [BsonElement("head")]
        public ObjectId? Head { get; set; } = null;

        [BsonElement("body")]
        public ObjectId? Body { get; set; } = null;

        [BsonElement("legs")]
        public ObjectId? Legs { get; set; } = null;

        [BsonElement("foots")]
        public ObjectId? Foots { get; set; } = null;

        [BsonElement("strength")]
        public int Strength { get; set; } = 1;

        [BsonElement("agility")]
        public int Agility { get; set; } = 1;

        [BsonElement("luck")]
        public int Luck { get; set; } = 1;

        [BsonElement("intellect")]
        public int Intellect { get; set; } = 1;

        [BsonElement("endurance")]
        public int Endurance { get; set; } = 1;

        [BsonElement("healthRegenTime")]
        public long HealthRegenTime { get; set; } = 1;

        [BsonElement("energyRegenTime")]
        public long EnergyRegenTime { get; set; } = 1;

        [BsonElement("createdAt")]
        public long CreatedAt { get; set; } = 1;

        [BsonElement("liveTime")]
        public long LiveTime { get; set; } = 1;
    }

    public sealed class ItemDocument
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; } = ObjectId.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = "Хуйня без имени";

        [BsonElement("description")]
        public string Description { get; set; } = "Долбаеб блять добавь описание предмету!!!";

        [BsonElement("type")]
        public ItemType Type { get; set; } = ItemType.Simple;

        [BsonElement("rank")]
        public Ranks Rank { get; set; } = Ranks.E;

        [BsonElement("canTrade")]
        public bool CanTrade { get; set; } = false;

        [BsonElement("max")]
        public int Max { get; set; } = -1;

        [BsonElement("bonuses")]
        public Dictionary<Characteristics, int> Bonuses { get; set; } = new();

        [BsonElement("needs")]
        public Dictionary<ItemNeeds, int> Needs { get; set; } = new();
    }
}