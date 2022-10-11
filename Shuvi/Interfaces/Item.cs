using MongoDB.Bson;

namespace ShuviBot.Interfaces.Item
{
    public interface IBaseItem
    {
        public ObjectId GetId { get; }
        public Int32 GetAmount { get; }
    }

    public interface IItem
    {
        public ObjectId GetId { get; }
        public int GetAmount { get; }
        public string GetName { get; }
        public string GetDescription { get; }
        public int Max { get; }
    }
}