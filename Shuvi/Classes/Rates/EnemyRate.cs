using MongoDB.Bson;
using Shuvi.Interfaces.Rates;

namespace Shuvi.Classes.Rates
{
    public class EnemyRate : IIdRate
    {
        private readonly List<ObjectId> _allDrop;

        public EnemyRate(Dictionary<ObjectId, int> allDrop)
        {
            _allDrop = new();
            foreach (var (itemId, count) in allDrop)
                for (int i = 0; i <= count; i++)
                    _allDrop.Add(itemId);
        }
        public EnemyRate()
        {
            _allDrop = new();
        }
        public ObjectId GetRandom()
        {
            var random = new Random();
            return _allDrop.ElementAt(random.Next(0, _allDrop.Count));
        }
    }
    
}
