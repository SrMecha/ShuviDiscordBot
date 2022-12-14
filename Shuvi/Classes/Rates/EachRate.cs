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
        public IDropInventory GetRandom()
        {
            var random = new Random();
            var result = new DropInventory();
            foreach (var item in _drop)
                for (var i = item.Min; i <= random.Next(item.Min, ++item.Max); i++)
                    if (random.Next(1001) < item.Chance) // 1000 == 100%. Сделал так, что бы не использовать числа с запятой. 1 == 0.1%
                        result.AddItem(item.Id);
            return result;
        }
    }
}