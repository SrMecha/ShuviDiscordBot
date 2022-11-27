using MongoDB.Bson;
using MongoDB.Driver;
using ShuviBot.Extensions.User;
using ShuviBot.Extensions.Inventory;
using ShuviBot.Extensions.MongoDocuments;
using ShuviBot.Extensions.Map;
using MongoDB.Bson.Serialization;

namespace ShuviBot.Services
{

    public class DatabaseManager
    {
        private readonly MongoClient _mongo;
        private readonly UserDatabase _userDatabase;
        private readonly ItemDatabase _itemDatabase;
        private readonly InfoDatabase _infoDatabase;
        private readonly EnemiesDatabase _enemiesDatabase;
        private readonly ShopDatabase _shopDatabase;
        private readonly DungeonDatabase _dungeonDatabase;
        private readonly AllItemsData _allItemsData;
        private readonly WorldMap _map;

        public DatabaseManager(string mongoKey)
        {
            _mongo = new MongoClient(mongoKey);
            _itemDatabase = new ItemDatabase(_mongo.GetDatabase("Shuvi").GetCollection<ItemDocument>("Items"));
            _allItemsData = _itemDatabase.GetAllItemsData();
            _userDatabase = new UserDatabase(_mongo.GetDatabase("Shuvi").GetCollection<UserDocument>("Users"), _allItemsData);
            _infoDatabase = new InfoDatabase(_mongo.GetDatabase("Shuvi").GetCollection<BsonDocument>("Info"));
            _map = _infoDatabase.Map;
            _enemiesDatabase = new EnemiesDatabase(_mongo.GetDatabase("Shuvi").GetCollection<EnemyDocument>("Enemies"));
            _shopDatabase = new ShopDatabase(_mongo.GetDatabase("Shuvi").GetCollection<ShopDocument>("Shops"));
            _dungeonDatabase = new DungeonDatabase(_mongo.GetDatabase("Shuvi").GetCollection<DungeonDocument>("Dungeons"));
        }

        public UserDatabase Users => _userDatabase;
        public ItemDatabase Items => _itemDatabase;
        public InfoDatabase Info => _infoDatabase;
        public AllItemsData AllItemsData => _allItemsData;
        public WorldMap Map => _map;
        public EnemiesDatabase EnemiesDatabase => _enemiesDatabase;
        public ShopDatabase ShopDatabase => _shopDatabase;
        public DungeonDatabase DungeonDatabase => _dungeonDatabase;
    }

    public sealed class UserDatabase
    {
        private readonly IMongoCollection<UserDocument> _userCollection;
        private readonly AllItemsData _allItemsData;

        public UserDatabase(IMongoCollection<UserDocument> userCollection, AllItemsData allItemsData)
        {
            _userCollection = userCollection;
            _allItemsData = allItemsData;
        }
        public async Task<User> AddUser(ulong userId)
        {
            UserDocument userData = new UserDocument
            {
                Id = userId,
                Rating = 0,
                Money = 0,
                Race = UserFactory.GenerateRandomUserRace(),
                Profession = 0,
                Inventory = new Dictionary<ObjectId, int>(),
                Weapon = null,
                Head = null,
                Body = null,
                Legs = null,
                Foots = null,
                Strength = 1,
                Agility = 1,
                Luck = 1,
                Intellect = 1,
                Endurance = 1,
                HealthRegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                EnergyRegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                CreatedAt = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                LiveTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()
            };
            await _userCollection.InsertOneAsync(userData);
            return new User(userData, _allItemsData);
        }
        public async Task<User> GetUser(ulong userId)
        {
            User user;
            try {
                UserDocument userData = await _userCollection.Find(new BsonDocument { { "_id", (long)userId } }).SingleAsync();
                user = new User(userData, _allItemsData);
            } catch (InvalidOperationException) {
                user = await AddUser(userId);
            }
            return user;
        }
    }

    public sealed class ItemDatabase
    {
        private readonly IMongoCollection<ItemDocument> _itemCollection;
        private readonly AllItemsData _allItemsData;

        public ItemDatabase(IMongoCollection<ItemDocument> itemCollection)
        {
            _itemCollection = itemCollection;
            _allItemsData = LoadAllItems();
        }

        private AllItemsData LoadAllItems()
        {
            AllItemsData tempData = new();
            foreach (ItemDocument itemData in _itemCollection.FindSync(new BsonDocument { }).ToEnumerable<ItemDocument>())
            {
                tempData.AddItemData(itemData);
            }
            return tempData;
        }

        public ItemDocument GetItemData(ObjectId id)
        {
            return _allItemsData.GetItemData(id);
        }

        public AllItemsData GetAllItemsData()
        {
            return _allItemsData;
        }
    }

    public sealed class InfoDatabase
    {
        private readonly IMongoCollection<BsonDocument> _infoCollection;
        private readonly WorldMap _map;

        public InfoDatabase(IMongoCollection<BsonDocument> infoCollection)
        {
            _infoCollection = infoCollection;
            _map = LoadMap();
        }
        private WorldMap LoadMap()
        {
            return BsonSerializer.Deserialize<WorldMap>(_infoCollection.Find(new BsonDocument { { "_id", "Map" } }).Single());
        }

        public WorldMap Map => _map;
    }

    public sealed class EnemiesDatabase
    {
        private readonly IMongoCollection<EnemyDocument> _enemiesCollection;
        private readonly Dictionary<ObjectId, EnemyDocument> _enemies;

        public EnemiesDatabase(IMongoCollection<EnemyDocument> enemyCollection)
        {
            _enemiesCollection = enemyCollection;
            _enemies = LoadEnemies();
        }
        private Dictionary<ObjectId, EnemyDocument> LoadEnemies()
        {
            Dictionary<ObjectId, EnemyDocument> result = new();
            foreach (EnemyDocument enemyData in _enemiesCollection.FindSync(new BsonDocument { }).ToEnumerable<EnemyDocument>())
            {
                result.Add(enemyData.Id, enemyData);
            }
            return result;
        }

        public Dictionary<ObjectId, EnemyDocument> Enemies => _enemies;
    }

    public sealed class ShopDatabase
    {
        private readonly IMongoCollection<ShopDocument> _shopCollection;
        private readonly Dictionary<ObjectId, ShopDocument> _shops;

        public ShopDatabase(IMongoCollection<ShopDocument> shopCollection)
        {
            _shopCollection = shopCollection;
            _shops = LoadShops();
        }
        private Dictionary<ObjectId, ShopDocument> LoadShops()
        {
            Dictionary<ObjectId, ShopDocument> result = new();
            foreach (ShopDocument shopData in _shopCollection.FindSync(new BsonDocument { }).ToEnumerable<ShopDocument>())
            {
                result.Add(shopData.Id, shopData);
            }
            return result;
        }

        public Dictionary<ObjectId, ShopDocument> Shops => _shops;
    }

    public sealed class DungeonDatabase
    {
        private readonly IMongoCollection<DungeonDocument> _dungeonCollection;
        private readonly Dictionary<ObjectId, DungeonDocument> _dungeons;

        public DungeonDatabase(IMongoCollection<DungeonDocument> shopCollection)
        {
            _dungeonCollection = shopCollection;
            _dungeons = LoadDungeons();
        }
        private Dictionary<ObjectId, DungeonDocument> LoadDungeons()
        {
            Dictionary<ObjectId, DungeonDocument> result = new();
            foreach (DungeonDocument dungeonData in _dungeonCollection.FindSync(new BsonDocument { }).ToEnumerable<DungeonDocument>())
            {
                result.Add(dungeonData.Id, dungeonData);
            }
            return result;
        }

        public Dictionary<ObjectId, DungeonDocument> Dungeons => _dungeons;
    }
}