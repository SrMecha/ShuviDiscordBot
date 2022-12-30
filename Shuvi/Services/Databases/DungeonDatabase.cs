using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.Dungeon;

namespace Shuvi.Services.Databases
{
    public sealed class DungeonDatabase
    {
        private readonly IMongoCollection<DungeonData> _dungeonCollection;

        public Dictionary<ObjectId, DungeonData> Dungeons { get; init; }

        public DungeonDatabase(IMongoCollection<DungeonData> shopCollection)
        {
            _dungeonCollection = shopCollection;
            Dungeons = LoadDungeons();
        }
        private Dictionary<ObjectId, DungeonData> LoadDungeons()
        {
            Dictionary<ObjectId, DungeonData> result = new();
            foreach (DungeonData dungeonData in _dungeonCollection.FindSync(new BsonDocument { }).ToEnumerable<DungeonData>())
            {
                result.Add(dungeonData.Id, dungeonData);
            }
            return result;
        }
    }
}