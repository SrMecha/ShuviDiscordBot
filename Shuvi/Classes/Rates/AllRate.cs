using MongoDB.Bson;
using Shuvi.Classes.Enemy;
using Shuvi.Classes.Inventory;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Rates;

namespace Shuvi.Classes.Rates
{
    public sealed class AllRate : IRate
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
        public AllRate(EnemyDrop drop)
        {
            _allDrop = new();
            foreach (var (itemId, count) in drop.Items)
                for (int i = 0; i <= count; i++)
                    _allDrop.Add(itemId);
            _min = drop.Min;
            _max = drop.Max;
        }
        public AllRate()
        {
            _allDrop = new();
            _min = 1;
            _max = 1;
        }
        public IDropInventory GetRandom()
        {
            var random = new Random();
            var result = new DropInventory();
            for (var i = 0; i <= random.Next(_min, _max + 1); i++)
                result.AddItem(_allDrop.ElementAt(random.Next(0, _allDrop.Count)), 1);
            return result;
        }
    }
}