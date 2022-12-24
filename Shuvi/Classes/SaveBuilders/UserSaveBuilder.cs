using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.User;
using Shuvi.Enums;

namespace Shuvi.Classes.SaveBuilders
{
    public class UserSaveBuilder
    {
        public readonly UpdateDefinitionBuilder<BsonDocument> _data;

        public UserSaveBuilder()
        {
            _data = new();
        }
        public BsonDocument Build()
        {
            return _data.ToBsonDocument();
        }
        public UserSaveBuilder ReduceEnergy(long time, int amount)
        {
            if (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() > time)
                _data.Set("EnergyRegenTime", ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + (amount * UserSettings.EnergyPointRegenTime));
            else
                _data.Set("EnergyRegenTime", time + (amount * UserSettings.EnergyPointRegenTime));
            return this;
        }
        public UserSaveBuilder ReduceHealth(long time, int amount)
        {
            if (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() > time)
                _data.Set("HealthRegenTime", ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + (amount * UserSettings.HealthPointRegenTime));
            else
                _data.Set("HealthRegenTime", time + (amount * UserSettings.HealthPointRegenTime));
            return this;
        }
        public UserSaveBuilder ReduceMana(long time, int amount)
        {
            if (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() > time)
                _data.Set("ManaRegenTime", ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + (amount * UserSettings.ManaPointRegenTime));
            else
                _data.Set("ManaRegenTime", time + (amount * UserSettings.ManaPointRegenTime));
            return this;
        }
        public UserSaveBuilder SetWeapon(ObjectId? id)
        {
            _data.Set("Weapon", id);
            return this;
        }
        public UserSaveBuilder SetHelmet(ObjectId? id)
        {
            _data.Set("Head", id);
            return this;
        }
        public UserSaveBuilder SetArmor(ObjectId? id)
        {
            _data.Set("Body", id);
            return this;
        }
        public UserSaveBuilder SetLeggings(ObjectId? id)
        {
            _data.Set("Legs", id);
            return this;
        }
        public UserSaveBuilder SetBoots(ObjectId? id)
        {
            _data.Set("Foots", id);
            return this;
        }
        public UserSaveBuilder SetInventory(Dictionary<ObjectId, int> inventory)
        {
            _data.Set("Inventory", new BsonDocument(inventory));
            return this;
        }
        public UserSaveBuilder SetItem(ObjectId id, int amount)
        {
            _data.Set($"Inventory.{id}", amount);
            return this;
        }
        public UserSaveBuilder SetSpell(string name)
        {
            _data.Set("Spell", name);
            return this;
        }
        public UserSaveBuilder SetMoney(int amount)
        {
            _data.Set("Money", amount);
            return this;
        }
        public UserSaveBuilder IncMoney(int amount)
        {
            _data.Inc("Money", amount);
            return this;
        }
        public UserSaveBuilder SetDispoints(int amount)
        {
            _data.Set("Dispoints", amount);
            return this;
        }
        public UserSaveBuilder IncDispoints(int amount)
        {
            _data.Inc("Dispoints", amount);
            return this;
        }
        public UserSaveBuilder SetRace(UserRaces race)
        {
            _data.Set("Race", race);
            return this;
        }
        public UserSaveBuilder SetProfession(UserProfessions profession)
        {
            _data.Set("Profession", profession);
            return this;
        }
        public UserSaveBuilder SetStrength(int amount)
        {
            _data.Set("Strength", amount);
            return this;
        }
        public UserSaveBuilder IncStrength(int amount)
        {
            _data.Inc("Strength", amount);
            return this;
        }
        public UserSaveBuilder SetAgility(int amount)
        {
            _data.Set("Agility", amount);
            return this;
        }
        public UserSaveBuilder IncAgility(int amount)
        {
            _data.Inc("Agility", amount);
            return this;
        }
        public UserSaveBuilder SetLuck(int amount)
        {
            _data.Set("Luck", amount);
            return this;
        }
        public UserSaveBuilder IncLuck(int amount)
        {
            _data.Inc("Luck", amount);
            return this;
        }
        public UserSaveBuilder SetIntellect(int amount)
        {
            _data.Set("Intellect", amount);
            return this;
        }
        public UserSaveBuilder IncIntellect(int amount)
        {
            _data.Inc("Intellect", amount);
            return this;
        }
        public UserSaveBuilder SetEndurance(int amount)
        {
            _data.Set("Endurance", amount);
            return this;
        }
        public UserSaveBuilder IncEndurance(int amount)
        {
            _data.Inc("Endurance", amount);
            return this;
        }
        public UserSaveBuilder SetMaxMana(int amount)
        {
            _data.Set("MaxMana", amount);
            return this;
        }
        public UserSaveBuilder IncMaxMana(int amount)
        {
            _data.Inc("MaxMana", amount);
            return this;
        }
        public UserSaveBuilder SetMaxHealth(int amount)
        {
            _data.Set("MaxHealth", amount);
            return this;
        }
        public UserSaveBuilder IncMaxHealth(int amount)
        {
            _data.Inc("MaxHealth", amount);
            return this;
        }
        public UserSaveBuilder SetMapLocation(int location)
        {
            _data.Set("MapLocation", location);
            return this;
        }
        public UserSaveBuilder SetMapRegion(int region)
        {
            _data.Set("MapRegion", region);
            return this;
        }
        public UserSaveBuilder SetLiveTime(long time)
        {
            _data.Set("LiveTime", time);
            return this;
        }
        public UserSaveBuilder IncDeathCount(int amount)
        {
            _data.Inc("DeathCount", amount);
            return this;
        }
        public UserSaveBuilder IncDungeonComplite(int amount)
        {
            _data.Inc("DungeonComplite", amount);
            return this;
        }
        public UserSaveBuilder IncEnemyKilled(int amount)
        {
            _data.Inc("EnemyKilled", amount);
            return this;
        }
        public UserSaveBuilder SetMaxRating(int amount)
        {
            _data.Set("MaxRating", amount);
            return this;
        }
        public UserSaveBuilder IncMaxRating(int amount)
        {
            _data.Inc("MaxRating", amount);
            return this;
        }
    }
}
