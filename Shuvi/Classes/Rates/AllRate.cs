using MongoDB.Bson;
using Shuvi.Classes.Enemy;
using Shuvi.Classes.Inventory;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Rates;

namespace Shuvi.Classes.Rates
{
    public sealed class AllRate : IRate
    {
        private readonly Dictionary<ObjectId, int> _allDrop;
        private readonly int _min;
        private readonly int _max;
        
        public int All { get; init; }

        public AllRate(Dictionary<ObjectId, int> allDrop, int min = 1, int max = 1)
        {
            _allDrop = allDrop;
            _min = min;
            _max = max;
            All = _allDrop.Values.Sum();
        }
        public AllRate(EnemyDrop drop)
        {
            _allDrop = drop.Items;
            _min = drop.Min;
            _max = drop.Max;
            All = _allDrop.Values.Sum();
        }
        public AllRate()
        {
            _allDrop = new();
            _min = 1;
            _max = 1;
            All = 0;
        }
        public IDropInventory GetRandom()
        {
            var random = new Random();
            var result = new DropInventory();
            for (var i = 1; i <= random.Next(_min, _max + 1); i++)
            {
                var needCount = random.Next(0, _allDrop.Values.Sum()+1);
                var count = 0;
                foreach (var (id, amount) in _allDrop)
                {
                    count += amount;
                    if (needCount <= count)
                    {
                        result.AddItem(id, 1);
                        break;
                    }
                }
            }
            return result;
        }
        public IDropInventory GetRandom(IInventoryBase inventory)
        {
            var random = new Random();
            var result = new DropInventory();
            for (var i = 1; i <= random.Next(_min, _max + 1); i++)
            {
                var needCount = random.Next(0, _allDrop.Values.Sum() + 1);
                var count = 0;
                foreach (var (id, amount) in _allDrop)
                {
                    count += amount;
                    if (needCount <= count)
                    {
                        {
                            var item = inventory.GetItem(id);
                            var getAmount = item.Amount + result.GetItem(id).Amount;
                            if (item.Max == -1 || getAmount + 1 <= item.Max)
                                result.AddItem(item.Id, 1);
                        }
                        break;
                    }
                }
            }
            return result;
        }
    public IEnumerator<KeyValuePair<ObjectId, int>> GetEnumerator()
        {
            return _allDrop.GetEnumerator();
        }
    }
}