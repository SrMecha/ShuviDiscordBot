using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.User;
using Shuvi.Interfaces.User;
using Shuvi.Services.Caches;

namespace Shuvi.Services.Databases
{
    public sealed class UserDatabase
    {
        private readonly IMongoCollection<UserData> _userCollection;
        private readonly UserCacheManager _cacheUsers;

        public UserDatabase(IMongoCollection<UserData> userCollection)
        {
            _userCollection = userCollection;
            _cacheUsers = new();
            StartCacheCleaner();
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
            _cacheUsers.TryAddUser(userData);
            return new DatabaseUser(userData);
        }
        public async Task<IDatabaseUser> GetUser(ulong userId)
        {
            if (_cacheUsers.TryGetUser(userId, out IDatabaseUser? user))
                return user!;
            try
            {
                UserData userData = await _userCollection.Find(new BsonDocument { { "_id", (long)userId } }).SingleAsync();
                _cacheUsers.TryAddUser(userData);
            }
            catch (InvalidOperationException)
            {
                await AddUser(userId);
            }
            return _cacheUsers.GetUser(userId);
        }
        public async Task UpdateUser(ulong userId, UpdateDefinition<UserData> updateConfig)
        {
            await _userCollection.UpdateOneAsync(new BsonDocument { { "_id", (long)userId } }, updateConfig);
        }
        public void StartCacheCleaner()
        {
            _ = Task.Run(async () => {
                while (true)
                {
                    await Task.Delay(300);
                    _cacheUsers.DeleteNotUsedCache();
                }
            });
        }
    }
}