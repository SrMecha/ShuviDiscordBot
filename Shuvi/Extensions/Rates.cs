using MongoDB.Bson;
using ShuviBot.Extensions.Inventory;

namespace ShuviBot.Extensions.Rates
{
    public sealed class EachRate
    {
        private readonly List<DropData> _drop;

        public EachRate(List<DropData> drop)
        {
            _drop = drop;
        }
        public DropInventory GetDrop()
        {
            Random random = new();
            DropInventory result = new();
            foreach (var item in _drop)
                for (var i = item.Min; i <= random.Next(item.Min, ++item.Max); i++)
                    if (random.Next(1001) < item.Chance) // 1001 == 100%. Сделал так, что бы не использовать числа с запятой. 1 == 0.1%
                        result.AddItem(item.Id);
            return result;
        }
    }

    public sealed class AllRate
    {
        private readonly List<ObjectId> _allDrop;
        private readonly int _min;
        private readonly int _max;

        public AllRate(Dictionary<ObjectId, int> allDrop, int min = 1, int max = 1)
        {
            _allDrop = new();
            foreach (var (itemId, count) in allDrop)
                for (int i = 0; i <= count; i++)
                    _allDrop.Add(itemId);
            _min = min;
            _max = max;
        }
        public AllRate()
        {
            _allDrop = new();
            _min = 1;
            _max = 1;
        }
    }

    public sealed class DropData
    {
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public int Chance { get; set; } = 0;
        public int Min { get; set; } = 1;
        public int Max { get; set; } = 1;
    }
}