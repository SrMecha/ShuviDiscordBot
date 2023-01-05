using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Shuvi.Classes.Map;
using Shuvi.StaticServices.AdminCheck;

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
            AdminCheckManager.SetAdmins(LoadAdmins());
        }
        private WorldMap LoadMap()
        {
            WorldMap result = BsonSerializer.Deserialize<WorldMap>(_infoCollection.Find(new BsonDocument { { "_id", "Map" } }).Single());
            foreach (var region in result.Regions)
                foreach (var location in region.Locations)
                    location.ConfigureRates();
            return result;
        }
        private AdminsData LoadAdmins()
        {
            return BsonSerializer.Deserialize<AdminsData>(_infoCollection.Find(new BsonDocument { { "_id", "Admins" } }).Single());
        }
        public async Task SetAdmins()
        {
            await _infoCollection.UpdateOneAsync(
                new BsonDocument { { "_id", "Admins" } }, 
                new UpdateDefinitionBuilder<BsonDocument>().Set("AdminIds", AdminCheckManager.Data.AdminIds)
                );
        }
    }
}