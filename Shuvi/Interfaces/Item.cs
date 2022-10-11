using MongoDB.Bson;

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
        public int Max { get; }
        public string GetBonusesInfo();
        public string GetNeedsInfo();
    }
}