using MongoDB.Bson;
using Shuvi.Classes.Characteristics;
using Shuvi.Enums;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Items;

namespace Shuvi.Classes.Items
{
    public class BaseItem : IItem
    {
        public ObjectId Id { get; init; }
        public int Amount { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public ItemType Type { get; init; }
        public Rank Rank { get; init; }
        public bool CanTrade { get; init; }
        public int Max { get; init; }
        public virtual ICharacteristicBonuses Bonuses { get; protected set; } = new CharacteristicBonuses();
        public virtual Dictionary<ItemNeeds, int> Needs { get; protected set; } = new();

        public BaseItem(ItemData data, int amount)
        {
            Id = data.Id;
            Name = data.Name;
            Description = data.Description;
            Type = data.Type;
            Rank = data.Rank;
            CanTrade = data.CanTrade;
            Max = data.Max;
            Amount = amount;
        }

        public virtual string GetBonusesInfo()
        {
            return "Нету бонусов.";
        }
        public virtual string GetNeedsInfo()
        {
            return "Нету требований.";
        }
    }
}