using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.User;
using Shuvi.Interfaces.User;

namespace Shuvi.Services.Databases
{
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
            try
            {
                UserData userData = await _userCollection.Find(new BsonDocument { { "_id", (long)userId } }).SingleAsync();
                user = new DatabaseUser(userData);
            }
            catch (InvalidOperationException)
            {
                user = await AddUser(userId);
            }
            return user;
        }
        public async Task UpdateUser(ulong userId, BsonDocument updateConfig)
        {
            await _userCollection.UpdateOneAsync(new BsonDocument { { "_id", (long)userId } }, updateConfig);
        }
    }
}