using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shuvi.Enums;

namespace Shuvi.Classes.Enemy
{
    public sealed class EnemyData
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Джибрил блять";
        public string Description { get; set; } = "Ща она тебе пизды даст. Шуви сразу Ислам примет.";
        public Rank Rank { get; set; } = Rank.E;
        public int RatingGet { get; set; } = 1;
        public int UpgradePoints { get; set; } = 5;
        public EnemyActionChances ActionChances { get; set; } = new();
        public Dictionary<string, string> Pictures { get; set; } = new();
        public string SpellName { get; set; } = string.Empty;
        public int Strength { get; set; } = 1;
        public int Agility { get; set; } = 1;
        public int Luck { get; set; } = 1;
        public int Intellect { get; set; } = 1;
        public int Endurance { get; set; } = 1;
        public int Mana { get; set; } = 10;
        public int Health { get; set; } = 100;
        public EnemyDrop Drop { get; set; } = new();
    }
}