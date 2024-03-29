﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Shuvi.Enums;
using Shuvi.Classes.ActionChances;

namespace Shuvi.Classes.User
{
    [BsonIgnoreExtraElements]
    public sealed class UserData
    {
        [BsonId]
        public ulong Id { get; set; } = 0;
        public int Rating { get; set; } = 0;
        public int Money { get; set; } = 0;
        public int Dispoints { get; set; } = 0;
        public string Spell { get; set; } = string.Empty;
        public UserRaces Race { get; set; } = UserRaces.ExMachina;
        public UserProfessions Profession { get; set; } = UserProfessions.NoProfession;
        public Dictionary<ObjectId, int> Inventory { get; set; } = new();
        public UserActionChances ActionChances { get; set; } = new();
        public ObjectId? Weapon { get; set; } = null;
        public ObjectId? Helmet { get; set; } = null;
        public ObjectId? Armor { get; set; } = null;
        public ObjectId? Leggings { get; set; } = null;
        public ObjectId? Boots { get; set; } = null;
        public int Strength { get; set; } = 1;
        public int Agility { get; set; } = 1;
        public int Luck { get; set; } = 1;
        public int Intellect { get; set; } = 1;
        public int Endurance { get; set; } = 1;
        public int MaxMana { get; set; } = UserSettings.StandartMana;
        public int MaxHealth { get; set; } = UserSettings.StandartHealth;
        public long ManaRegenTime { get; set; } = 1;
        public long HealthRegenTime { get; set; } = 1;
        public long EnergyRegenTime { get; set; } = 1;
        public long CreatedAt { get; set; } = 1;
        public long LiveTime { get; set; } = 1;
        public int DeathCount { get; set; } = 0;
        public int DungeonComplite { get; set; } = 0;
        public int EnemyKilled { get; set; } = 0;
        public int MaxRating { get; set; } = 0;
        public int MapLocation { get; set; } = 0;
        public int MapRegion { get; set; } = 0;
    }
}
