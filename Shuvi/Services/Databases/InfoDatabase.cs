using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Shuvi.Classes.Map;

namespace Shuvi.Services.Databases
{
    public sealed class InfoDatabase
    {
        private readonly IMongoCollection<BsonDocument> _infoCollection;

        public WorldMap Map { get; init; }

        public InfoDatabase(IMongoCollection<BsonDocument> infoCollection)
        {
            _infoCollection = infoCollection;
            Map = LoadMap();
        }
        private WorldMap LoadMap()
        {
            WorldMap result = BsonSerializer.Deserialize<WorldMap>(_infoCollection.Find(new BsonDocument { { "_id", "Map" } }).Single());

            return result;
        }
    }
}