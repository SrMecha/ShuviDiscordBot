using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shuvi.Classes.Rates;

namespace Shuvi.Classes.Enemy
{
    public sealed class EnemyData
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Джибрил блять";
        public string Description { get; set; } = "Ща она тебе пизды даст. Шуви сразу Ислам примет.";
        public List<AllRate> Drop { get; set; } = new();
    }
}