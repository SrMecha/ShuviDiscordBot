using MongoDB.Bson;
using ShuviBot.Enums.Ranks;
using ShuviBot.Enums.ItemType;
using ShuviBot.Enums.UserProfessions;
using ShuviBot.Enums.UserRaces;
using ShuviBot.Extensions.Inventory;
using ShuviBot.Extensions.Items;
using ShuviBot.Extensions.MongoDocuments;
using ShuviBot.Extensions.Bonuses;

namespace ShuviBot.Extensions.User
{
    public sealed class UserSettings
    {
        private const int _healthPointRegenTime = 60;
        private const int _energyPointRegenTime = 180;
        private const int _endurancePerEnergy = 10;
        private const int _standartEnergy = 10;
        private const int _energyDisplayMax = 5;
        private const int _healthDisplayMax = 5;
        private const int _healthMax = 100;

        public static int HealthPointRegenTime => _healthPointRegenTime;
        public static int EnergyPointRegenTime => _energyPointRegenTime;
        public static int EndurancePerEnergy => _endurancePerEnergy;
        public static int StandartEnergy => _standartEnergy;
        public static int HealthDisplayMax => _healthDisplayMax;
        public static int EnergyDisplayMax => _energyDisplayMax;
        public static int HealthMax => _healthMax;
    }

    public sealed class UserFactory
    {
        public static UserRaces GenerateRandomUserRace()
        {
            Array values = Enum.GetValues(typeof(UserRaces));
            UserRaces? randomRaceOrNull = (UserRaces?)values.GetValue(new Random().Next(values.Length));
            UserRaces randomRace = randomRaceOrNull ?? UserRaces.ExMachina;
            return randomRace;

        }
    }

    public sealed class User
    {
        private readonly ulong _id;
        private readonly int _rating;
        private readonly Rank _rank;
        private readonly int _money;
        private readonly UserRaces _race;
        private readonly UserProfessions _profession;
        private readonly UserInventory _inventory;
        private readonly ObjectId? _weapon;
        private readonly ObjectId? _head;
        private readonly ObjectId? _body;
        private readonly ObjectId? _legs;
        private readonly ObjectId? _foots;
        private readonly int _strength;
        private readonly int _agility;
        private readonly int _luck;
        private readonly int _intellect;
        private readonly int _endurance;
        private readonly long _healthRegenTime;
        private readonly long _energyRegenTime;
        private readonly long _createdAt;
        private readonly long _liveTime;
        private readonly UserBonuses _bonuses;

        public User(UserDocument userData, AllItemsData itemsConfig)
        {
            _id = userData.Id;
            _rating = userData.Rating;
            _rank = GetRank();
            _money = userData.Money;
            _race = userData.Race;
            _profession = userData.Profession;
            _inventory = new UserInventory(userData.Inventory, itemsConfig);
            _weapon = userData.Weapon;
            _head = userData.Head;
            _body = userData.Body;
            _legs = userData.Legs;
            _foots = userData.Foots;
            _strength = userData.Strength;
            _agility = userData.Agility;
            _luck = userData.Luck;
            _intellect = userData.Intellect;
            _endurance = userData.Endurance;
            _healthRegenTime = userData.HealthRegenTime;
            _energyRegenTime = userData.EnergyRegenTime;
            _createdAt = userData.CreatedAt;
            _liveTime = userData.LiveTime;
            _bonuses = GetBonuses();
        }

        public ulong Id => _id;
        public int Rating => _rating;
        public Rank Rank => _rank;
        public int Money => _money;
        public UserRaces Race => _race;
        public UserProfessions Profession => _profession;
        public UserInventory Inventory => _inventory;
        public ObjectId? Weapon => _weapon;
        public ObjectId? Head => _head;
        public ObjectId? Body => _body;
        public ObjectId? Legs => _legs;
        public ObjectId? Foots => _foots;
        public UserBonuses Bonuses => _bonuses;
        public int Strength => _strength;
        public int Agility => _agility;
        public int Luck => _luck;
        public int Intellect => _intellect;
        public int Endurance => _endurance;
        public long HealthRegenTime => _healthRegenTime;
        public long EnergyRegenTime => _energyRegenTime;
        public long CreatedAt => _createdAt;
        public long LiveTime => _liveTime;

        public int GetRemainingHealthRegenTime()
        {
            int result = (int)(HealthRegenTime - ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            return result > 0 ? result : 0;
        }
        public int GetCurrentHealth()
        {
            return 100 - (GetRemainingHealthRegenTime() / UserSettings.HealthPointRegenTime);
        }
        public int GetRemainingEnergyRegenTime()
        {
            int result = (int)(EnergyRegenTime - ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            return result > 0 ? result : 0;
        }
        public int GetMaxEnergy()
        {
            return UserSettings.StandartEnergy + (Endurance / UserSettings.EndurancePerEnergy);
        }
        public int GetCurrentEnergy()
        {
            return GetMaxEnergy() - (GetRemainingEnergyRegenTime() / UserSettings.EnergyPointRegenTime);
        }
        public int GetLiveTime()
        {
            return (int)(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() - LiveTime);
        }
        public Rank GetRank()
        {
            return Rating switch
            {
                < 100 => 0,
                >= 100 and < 200 => (Rank)1,
                >= 200 and < 500 => (Rank)2,
                >= 500 and < 800 => (Rank)3,
                >= 800 and < 1100 => (Rank)4,
                >= 1100 and < 1500 => (Rank)5,
                >= 1500 and < 2000 => (Rank)6,
                >= 2000 => (Rank)7
            };
        }
        public UserBonuses GetBonuses()
        {
            List<EquipmentItem> items = new();
            List<ObjectId?> list = new() { Weapon, Head, Body, Legs, Foots};
            foreach(ObjectId? itemId in list)
            {
                if (itemId == null) continue;
                items.Append((EquipmentItem)Inventory.GetItem((ObjectId)itemId));
            }
            return new UserBonuses(items);
        }
        public EquipmentItem? GetEquipment(EquipmentType type)
        {
            return type switch
            {
                EquipmentType.Helmet => Head == null ? null : (EquipmentItem)Inventory.GetItem((ObjectId)Head),
                EquipmentType.Armor => Body == null ? null : (EquipmentItem)Inventory.GetItem((ObjectId)Body),
                EquipmentType.Leggings => Legs == null ? null : (EquipmentItem)Inventory.GetItem((ObjectId)Legs),
                EquipmentType.Boots => Foots == null ? null : (EquipmentItem)Inventory.GetItem((ObjectId)Foots),
                _ => null
            };
        }

    }
}