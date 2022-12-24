using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.Map;
using Shuvi.Classes.Shop;
using Shuvi.Classes.Enemy;
using Shuvi.Classes.Dungeon;
using Shuvi.Classes.Items;
using Shuvi.Classes.User;
using Shuvi.Services.Databases;

namespace Shuvi.Services
{

    public class DatabaseManagerService
    {
        private readonly MongoClient _mongo;

        public UserDatabase Users { get; init; }
        public ItemDatabase Items { get; init; }
        public InfoDatabase Info { get; init; }
        public WorldMap Map { get; init; }
        public EnemiesDatabase EnemiesDatabase { get; init; }
        public ShopDatabase ShopDatabase { get; init; }
        public DungeonDatabase DungeonDatabase { get; init; }

        public DatabaseManagerService(string mongoKey)
        {
            _mongo = new MongoClient(mongoKey);
            Items = new ItemDatabase(_mongo.GetDatabase("Shuvi").GetCollection<ItemData>("Items"));
            Users = new UserDatabase(_mongo.GetDatabase("Shuvi").GetCollection<UserData>("Users"));
            Info = new InfoDatabase(_mongo.GetDatabase("Shuvi").GetCollection<BsonDocument>("Info"));
            Map = Info.Map;
            EnemiesDatabase = new EnemiesDatabase(_mongo.GetDatabase("Shuvi").GetCollection<EnemyData>("Enemies"));
            ShopDatabase = new ShopDatabase(_mongo.GetDatabase("Shuvi").GetCollection<ShopData>("Shops"));
            DungeonDatabase = new DungeonDatabase(_mongo.GetDatabase("Shuvi").GetCollection<DungeonData>("Dungeons"));
        }
    }
}