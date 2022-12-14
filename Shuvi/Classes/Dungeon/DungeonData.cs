using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shuvi.Classes.Dungeon
{
    public sealed class DungeonData
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Икея";
    }
}