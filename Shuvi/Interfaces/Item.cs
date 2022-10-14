using MongoDB.Bson;
using ShuviBot.Enums.Characteristics;
using ShuviBot.Enums.ItemNeeds;
using ShuviBot.Enums.ItemType;
using ShuviBot.Enums.Ranks;

namespace ShuviBot.Interfaces.Item
{
    public interface IItem
    {
        public ObjectId Id { get; }
        public int Amount { get; }
        public string Name { get; }
        public string Description { get; }
        public ItemType Type { get; }
        public Rank Rank { get; }
        public bool CanTrade { get; }
        public int Max { get; }
        public Dictionary<Characteristics, int> Bonuses { get; }
        public Dictionary<ItemNeeds, int> Needs { get; }
        public string GetBonusesInfo();
        public string GetNeedsInfo();
    }
}