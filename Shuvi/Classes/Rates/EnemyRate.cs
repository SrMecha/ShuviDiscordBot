using MongoDB.Bson;
using Shuvi.Interfaces.Rates;

namespace Shuvi.Classes.Rates
{
    public class EnemyRate : IIdRate
    {
        private readonly Dictionary<ObjectId, int> _allDrop;

        public int All { get; init; }

        public EnemyRate(Dictionary<ObjectId, int> allDrop)
        {
            _allDrop = allDrop;
            All = _allDrop.Values.Sum();
        }
        public EnemyRate()
        {
            _allDrop = new();
            All = 0;
        }
        public ObjectId GetRandom()
        {
            var random = new Random();
            var needCount = random.Next(0, _allDrop.Values.Sum() + 1);
            var count = 0;
            foreach (var (id, amount) in _allDrop)
            {
                count += amount;
                if (needCount <= count)
                {
                    return id;
                }
            }
            return _allDrop.Keys.First();
        }
        public IEnumerator<KeyValuePair<ObjectId, int>> GetEnumerator()
        {
            return _allDrop.GetEnumerator();
        }
    }
    
}
