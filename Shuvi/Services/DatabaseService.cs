using MongoDB.Bson;
using MongoDB.Driver;
using ShuviBot.Extensions.User;
using ShuviBot.Extensions.Inventory;
using ShuviBot.Extensions.MongoDocuments;

namespace ShuviBot.Services
{

    public class DatabaseManager
    {
        private readonly MongoClient _mongo;
        private readonly UserDatabase _userDatabase;
        private readonly ItemDatabase _itemDatabase;
        private readonly AllItemsData _allItemsData;

        public DatabaseManager(string mongoKey)
        {
            _mongo = new MongoClient(mongoKey);
            _itemDatabase = new ItemDatabase(_mongo.GetDatabase("Shuvi").GetCollection<ItemDocument>("Items"));
            _allItemsData = _itemDatabase.GetAllItemsData();
            _userDatabase = new UserDatabase(_mongo.GetDatabase("Shuvi").GetCollection<UserDocument>("Users"), _allItemsData);
        }

        public UserDatabase Users 
        {
            get { return _userDatabase; }
        }

        public ItemDatabase Items
        {
            get { return _itemDatabase; }
        }

        public AllItemsData AllItemsData
        {
            get { return _allItemsData; }
        }
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
                Race = User.GenerateRandomUserRace(),
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
}