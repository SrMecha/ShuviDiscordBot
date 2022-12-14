using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Shuvi.Classes.Map;
using Shuvi.Classes.Shop;
using Shuvi.Classes.Enemy;
using Shuvi.Classes.Dungeon;
using Shuvi.Classes.Items;
using Shuvi.Classes.User;
using Shuvi.Interfaces.User;

namespace Shuvi.Services
{

    public class DatabaseManager
    {
        private readonly MongoClient _mongo;

        public UserDatabase Users { get; init; }
        public ItemDatabase Items { get; init; }
        public InfoDatabase Info { get; init; }
        public WorldMap Map { get; init; }
        public EnemiesDatabase EnemiesDatabase { get; init; }
        public ShopDatabase ShopDatabase { get; init; }
        public DungeonDatabase DungeonDatabase { get; init; }

        public DatabaseManager(string mongoKey)
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

    public sealed class UserDatabase
    {
        private readonly IMongoCollection<UserData> _userCollection;

        public UserDatabase(IMongoCollection<UserData> userCollection)
        {
            _userCollection = userCollection;
        }
        public async Task<IDatabaseUser> AddUser(ulong userId)
        {
            var userData = new UserData
            {
                Id = userId,
                Race = UserFactory.GenerateRandomUserRace(),
                ManaRegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                HealthRegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                EnergyRegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                CreatedAt = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                LiveTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()
            };
            await _userCollection.InsertOneAsync(userData);
            return new DatabaseUser(userData);
        }
        public async Task<IDatabaseUser> GetUser(ulong userId)
        {
            IDatabaseUser user;
            try {
                UserData userData = await _userCollection.Find(new BsonDocument { { "_id", (long)userId } }).SingleAsync();
                user = new DatabaseUser(userData);
            } catch (InvalidOperationException) {
                user = await AddUser(userId);
            }
            return user;
        }
    }

    public sealed class ItemDatabase
    {
        private readonly IMongoCollection<ItemData> _itemCollection;

        public ItemDatabase(IMongoCollection<ItemData> itemCollection)
        {
            _itemCollection = itemCollection;
            LoadAllItems();
        }

        private void LoadAllItems()
        {
            foreach (ItemData itemData in _itemCollection.FindSync(new BsonDocument { }).ToEnumerable<ItemData>())
            {
                AllItemsData.AddItemData(itemData);
            }
        }
    }

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

    public sealed class EnemiesDatabase
    {
        private readonly IMongoCollection<EnemyData> _enemiesCollection;

        public Dictionary<ObjectId, EnemyData> Enemies { get; init; }

        public EnemiesDatabase(IMongoCollection<EnemyData> enemyCollection)
        {
            _enemiesCollection = enemyCollection;
            Enemies = LoadEnemies();
        }
        private Dictionary<ObjectId, EnemyData> LoadEnemies()
        {
            Dictionary<ObjectId, EnemyData> result = new();
            foreach (EnemyData enemyData in _enemiesCollection.FindSync(new BsonDocument { }).ToEnumerable<EnemyData>())
            {
                result.Add(enemyData.Id, enemyData);
            }
            return result;
        }
    }

    public sealed class ShopDatabase
    {
        private readonly IMongoCollection<ShopData> _shopCollection;
        
        public Dictionary<ObjectId, ShopData> Shops { get; init; }

        public ShopDatabase(IMongoCollection<ShopData> shopCollection)
        {
            _shopCollection = shopCollection;
            Shops = LoadShops();
        }
        private Dictionary<ObjectId, ShopData> LoadShops()
        {
            Dictionary<ObjectId, ShopData> result = new();
            foreach (ShopData shopData in _shopCollection.FindSync(new BsonDocument { }).ToEnumerable<ShopData>())
            {
                result.Add(shopData.Id, shopData);
            }
            return result;
        }
    }

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