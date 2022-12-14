using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Equipment;
using Shuvi.Classes.Inventory;
using Shuvi.Enums;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Equipment;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.User
{
    public class DatabaseUser : IDatabaseUser
    {
        public ulong Id { get; init; }
        public IUserRating Rating { get; init; }
        public IUserWallet Wallet { get; init; }
        public UserRaces Race { get; init; }
        public UserProfessions Profession { get; init; }
        public IUserInventory Inventory { get; init; }
        public IEquipment Equipment { get; init; }
        public ICharacteristics Characteristic { get; init; }
        public IEnergy Energy { get; init; }
        public IMana Mana { get; init; }
        public IHealth Health { get; init; }
        public IUserStatistics Statistics { get; init; }
        public IUserLocation Location { get; init; }

        public DatabaseUser(UserData userData)
        {
            Id = userData.Id;
            Rating = new UserRating(userData.Rating);
            Wallet = new UserWallet(userData.Money, userData.Dispoints);
            Race = userData.Race;
            Profession = userData.Profession;
            Inventory = new UserInventory(userData.Inventory);
            Equipment = new UserEquipment(userData.Weapon, userData.Head, userData.Body, userData.Legs, userData.Foots);
            Characteristic = new UserCharacteristics(userData.Strength, userData.Agility, userData.Luck, userData.Intellect, userData.Endurance);
            Energy = new Energy(userData.EnergyRegenTime, Characteristic.Endurance);
            Mana = new Mana(userData.MaxMana, userData.ManaRegenTime);
            Health = new Health(UserSettings.HealthMax, userData.HealthRegenTime);
            Statistics = new UserStatistics(
                userData.CreatedAt, userData.LiveTime, userData.DeathCount, userData.DungeonComplite, userData.EnemyKilled, userData.MaxRating
                );
            Location = new UserLocation(userData.MapLocation, userData.MapRegion);
        }
    }
}
