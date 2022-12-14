using Shuvi.Interfaces.Inventory;

namespace Shuvi.Interfaces.Rates
{
    public interface IRate
    {
        public IDropInventory GetRandom();
    }
}
