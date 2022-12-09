using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShuviBot.Enums.ItemType;
using ShuviBot.Enums.Ranks;
using ShuviBot.Enums.UserProfessions;
using ShuviBot.Enums.UserRaces;
using ShuviBot.Enums.Characteristics;
using ShuviBot.Enums.ItemNeeds;
using ShuviBot.Extensions.Rates;

namespace ShuviBot.Extensions.MongoDocuments
{
    public sealed class UserDocument
    {
        [BsonElement("_id")]
        public ulong Id { get; set; } = 0;
        public int Rating { get; set; } = 0;
        public int Money { get; set; } = 0;
        public UserRaces Race { get; set; } = UserRaces.ExMachina;
        public UserProfessions Profession { get; set; } = UserProfessions.NoProfession;
        public Dictionary<ObjectId, int> Inventory { get; set; } = new();
        public ObjectId? Weapon { get; set; } = null;
        public ObjectId? Head { get; set; } = null;
        public ObjectId? Body { get; set; } = null;
        public ObjectId? Legs { get; set; } = null;
        public ObjectId? Foots { get; set; } = null;
        public int Strength { get; set; } = 1;
        public int Agility { get; set; } = 1;
        public int Luck { get; set; } = 1;
        public int Intellect { get; set; } = 1;
        public int Endurance { get; set; } = 1;
        public int MaxMana { get; set; } = 10;
        public long ManaRegenTime { get; set; } = 1;
        public long HealthRegenTime { get; set; } = 1;
        public long EnergyRegenTime { get; set; } = 1;
        public long CreatedAt { get; set; } = 1;
        public long LiveTime { get; set; } = 1;
        public int DeathCount { get; set; } = 0;
        public int DungeonComplite { get; set; } = 0;
        public int EnemyKilled { get; set; } = 0;
        public int MaxRating { get; set; } = 0;
        public int MapLocation { get; set; } = 0;
        public int MapRegion { get; set; } = 0;
    }

    public sealed class ItemDocument
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Хуйня без имени";
        public string Description { get; set; } = "Долбаеб блять добавь описание предмету!!!";
        public ItemType Type { get; set; } = ItemType.Simple;
        public Rank Rank { get; set; } = Rank.E;
        public bool CanTrade { get; set; } = false;
        public int Max { get; set; } = -1;
        public Dictionary<Characteristics, int> Bonuses { get; set; } = new();
        public Dictionary<ItemNeeds, int> Needs { get; set; } = new();
    }

    public sealed class EnemyDocument
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Джибрил блять";
        public string Description { get; set; } = "Ща она тебе пизды даст. Шуви сразу Ислам примет.";
        public List<AllRate> Drop { get; set; } = new();
    }

    public sealed class ShopDocument
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Икея";
    }

    public sealed class DungeonDocument
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Икея";
    }
}