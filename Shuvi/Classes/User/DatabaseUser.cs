using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.ActionChances;
using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Equipment;
using Shuvi.Classes.Inventory;
using Shuvi.Enums;
using Shuvi.Interfaces.ActionChances;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Equipment;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.User;
using Shuvi.Services.Databases;

namespace Shuvi.Classes.User
{
    public class DatabaseUser : IDatabaseUser
    {
        protected readonly UserDatabase _database; 

        public ulong Id { get; init; }
        public IUserRating Rating { get; init; }
        public IUserUpgradePoints UpgradePoints { get; init; }
        public IUserWallet Wallet { get; init; }
        public string SpellName { get; private set; }
        public UserRaces Race { get; private set; }
        public UserProfessions Profession { get; private set; }
        public IUserInventory Inventory { get; init; }
        public IUserActionChances ActionChances { get; private set; }
        public IEquipment Equipment { get; init; }
        public IUserCharacteristics Characteristic { get; init; }
        public IEnergy Energy { get; init; }
        public IMana Mana { get; init; }
        public IHealth Health { get; init; }
        public IUserStatistics Statistics { get; init; }
        public IUserLocation Location { get; init; }

        public DatabaseUser(UserData userData, UserDatabase database)
        {
            _database = database;
            Id = userData.Id;
            Rating = new UserRating(userData.Rating);
            Wallet = new UserWallet(userData.Money, userData.Dispoints);
            SpellName = userData.Spell;
            Race = userData.Race;
            Profession = userData.Profession;
            Inventory = new UserInventory(userData.Inventory);
            ActionChances = userData.ActionChances;
            Equipment = new UserEquipment(userData.Weapon, userData.Helmet, userData.Armor, userData.Leggings, userData.Boots);
            Characteristic = new UserCharacteristics(userData.Strength, userData.Agility, userData.Luck, userData.Intellect, userData.Endurance);
            Energy = new Energy(userData.EnergyRegenTime, Characteristic.Endurance);
            Mana = new Mana(userData.MaxMana, userData.ManaRegenTime);
            Health = new Health(userData.MaxHealth, userData.HealthRegenTime);
            Statistics = new UserStatistics(
                userData.CreatedAt, userData.LiveTime, userData.DeathCount, userData.DungeonComplite, userData.EnemyKilled, userData.MaxRating
                );
            Location = new UserLocation(userData.MapLocation, userData.MapRegion);
            UpgradePoints = new UserUpgradePoints(Rating, Characteristic, Mana, Health);
        }
        public async Task UpdateUser(UpdateDefinition<UserData> updateConfig)
        {
            await _database.UpdateUser(Id, updateConfig);
        }
        public void SetSpell(string name)
        {
            SpellName = name;
        }
        public void SetRace(UserRaces race)
        {
            Race = race;
        }
        public void SetProfession(UserProfessions profession)
        {
            Profession = profession;
        }
    }
}
