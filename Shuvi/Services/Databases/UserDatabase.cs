using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.User;
using Shuvi.Enums;
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
        public async Task<IDatabaseUser> AddUser(ulong id)
        {
            var userData = new UserData
            {
                Id = id,
                Race = UserFactory.GenerateRandomUserRace(),
                ManaRegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                HealthRegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                EnergyRegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                CreatedAt = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                LiveTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()
            };
            await _userCollection.InsertOneAsync(userData);
            _cacheUsers.TryAddUser(userData, this);
            return new DatabaseUser(userData, this);
        }
        public async Task<IDatabaseUser> GetUser(ulong id)
        {
            if (_cacheUsers.TryGetUser(id, out IDatabaseUser? user))
                return user!;
            try
            {
                UserData userData = await _userCollection.Find(new BsonDocument { { "_id", (long)id } }).SingleAsync();
                _cacheUsers.TryAddUser(userData, this);
            }
            catch (InvalidOperationException)
            {
                await AddUser(id);
            }
            return _cacheUsers.GetUser(id);
        }
        public async Task<IDatabaseUser> KillUser(ulong id)
        {
            var user = await GetUser(id);
            user.Rating.SetPoints(user.Rating.Points/2);
            user.Mana.IncreaseMana(9999);
            user.Energy.IncreaseEnergy(9999);
            user.Health.IncreaseHealth(9999);
            user.Mana.SetMaxMana(10);
            user.Health.SetMaxHealth(100);
            user.Energy.UpdateMaxEnergy(1);
            user.Characteristic.Reset();
            user.Wallet.SetMoney(0);
            user.Inventory.Clear();
            user.Statistics.AddDeathCount(1);
            user.SetProfession(UserProfessions.NoProfession);
            user.SetRace(UserFactory.GenerateRandomUserRace());
            user.SetSpell(string.Empty);
            user.Statistics.UpdateLiveTime();
            user.Equipment.RemoveAllEquipment();
            user.Location.SetRegion(0);
            user.Location.SetLocation(0);
            await UpdateUser(
                id,
                new UpdateDefinitionBuilder<UserData>()
                .Set("Rating", user.Rating.Points)
                .Set("Money", user.Wallet.Money)
                .Set("Spell", user.SpellName)
                .Set("Race", user.Race)
                .Set("Profession", user.Profession)
                .Set("Inventory", user.Inventory.GetInvetoryCache())
                .Set("Weapon", user.Equipment.Weapon)
                .Set("Helmet", user.Equipment.Helmet)
                .Set("Armor", user.Equipment.Armor)
                .Set("Leggings", user.Equipment.Leggings)
                .Set("Boots", user.Equipment.Boots)
                .Set("Strength", user.Characteristic.Strength)
                .Set("Agility", user.Characteristic.Agility)
                .Set("Luck", user.Characteristic.Luck)
                .Set("Intellect", user.Characteristic.Intellect)
                .Set("Endurance", user.Characteristic.Endurance)
                .Set("MaxMana", user.Mana.Max)
                .Set("MaxHealth", user.Health.Max)
                .Set("HealthRegenTime", user.Health.RegenTime)
                .Set("EnergyRegenTime", user.Energy.RegenTime)
                .Set("LiveTime", user.Statistics.LiveTime)
                .Set("DeathCount", user.Statistics.DeathCount)
                .Set("MapLocation", user.Location.MapLocation)
                .Set("MapRegion", user.Location.MapRegion)
                );
            return user;
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
                    await Task.Delay(600);
                    _cacheUsers.DeleteNotUsedCache();
                }
            });
        }
    }
}