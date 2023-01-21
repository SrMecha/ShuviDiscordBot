using Shuvi.Interfaces.User;

namespace Shuvi.Interfaces.Shop
{
    public interface IShop
    {
        public IShopInfo Info { get; }
        public IShopBasket ShopBasket { get; }
        public ISellingPart Selling { get; }
        public IPurchasingPart Purchasing { get; }
        public void Reset();
        public Task Confirm(IDatabaseUser user);
    }
}
