using Shuvi.Interfaces.Inventory;

namespace Shuvi.Interfaces.Rates
{
    public interface IRate
    {
        public IDropInventory GetRandom();
        public IDropInventory GetRandom(IInventoryBase inventory);
    }
}
