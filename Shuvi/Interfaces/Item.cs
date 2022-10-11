using MongoDB.Bson;
using ShuviBot.Enums.ItemType;
using ShuviBot.Enums.Ranks;

namespace ShuviBot.Interfaces.Item
{
    public interface IBaseItem
    {
        public ObjectId Id { get; }
        public int Amount { get; }
    }

    public interface IItem
    {
        public ObjectId Id { get; }
        public int Amount { get; }
        public string Name { get; }
        public string Description { get; }
        public ItemType Type { get; }
        public Ranks Rank { get; }
        public int Max { get; }
        public string GetBonusesInfo();
        public string GetNeedsInfo();
    }
}