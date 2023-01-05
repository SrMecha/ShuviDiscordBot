using MongoDB.Bson;
using Shuvi.Enums;
using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Interfaces.Items
{
    public interface IItem
    {
        public ObjectId Id { get; init; }
        public int Amount { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public ItemType Type { get; init; }
        public Rank Rank { get; init; }
        public bool CanTrade { get; init; }
        public bool CanLoose { get; init; }
        public int Max { get; init; }
        public ICharacteristicBonuses? Bonuses { get; }
        public Dictionary<ItemNeeds, int>? Needs { get; }
        public string GetBonusesInfo();
        public string GetNeedsInfo();
    }
}