using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;

namespace ShuviBot.Extensions.CustomList
{
    public class ListWithChance
    {
        private readonly Dictionary<ObjectId, ItemWithChance> _items;

        public ListWithChance(List<BsonDocument> items)
        {
            _items = Parse(items);
        }
        private Dictionary<ObjectId, ItemWithChance> Parse(List<BsonDocument> items)
        {
            Dictionary<ObjectId, ItemWithChance> result = new();
            foreach (BsonDocument item in items)
            {
                result.Add((ObjectId)item["Id"], BsonSerializer.Deserialize<ItemWithChance>(item));
            }
            return result;
        }

        public Dictionary<ObjectId, ItemWithChance> Items => _items;
    }

    public class ItemWithChance
    {
        public ObjectId Id { get; set; } = new();
        public int Chance { get; set; } = 0;
        public int Max { get; set; } = 1;
    }
}