using MongoDB.Bson;
using Shuvi.Classes.Inventory;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Rates;

namespace Shuvi.Classes.Rates
{
    public sealed class EachRate : IRate
    {
        private readonly List<DropData> _drop;

        public EachRate(List<DropData> drop)
        {
            _drop = drop;
        }
        public EachRate()
        {
            _drop = new();
        }
        public IDropInventory GetRandom()
        {
            var random = new Random();
            var result = new DropInventory();
            foreach (var item in _drop)
                for (var i = 0; i <= random.Next(0, ++item.Max); i++)
                    if (random.Next(1001) < item.Chance) // 1000 == 100%. Сделал так, что бы не использовать числа с запятой. 1 == 0.1%
                        result.AddItem(item.Id, 1);
            return result;
        }
        public IEnumerator<DropData> GetEnumerator()
        {
            return _drop.GetEnumerator();
        }
    }
}