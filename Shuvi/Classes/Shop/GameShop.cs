using MongoDB.Driver;
using Shuvi.Classes.User;
using Shuvi.Enums;
using Shuvi.Interfaces.Shop;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.Shop
{
    public class GameShop : IShop
    {
        public IShopInfo Info { get; private set; }
        public IShopBasket ShopBasket { get; private set; }
        public ISellingPart Selling { get; private set; }
        public IPurchasingPart Purchasing { get; private set; }

        public GameShop(IShopInfo info, IShopBasket shopBasket, ISellingPart selling, IPurchasingPart purchasing)
        {
            Info = info;
            ShopBasket = shopBasket;
            Selling = selling;
            Purchasing = purchasing;
        }
        public GameShop(ShopData data)
        {
            Info = new ShopInfo(data.Name, data.Description);
            ShopBasket = new ShopBasket();
            Selling = new SellingPart(Info, ShopBasket, data.Sale);
            Purchasing = new PurchasingPart(Info, ShopBasket, data.Buy);
        }
        public void Reset()
        {
            ShopBasket.Clear();
        }
        public async Task Confirm(IDatabaseUser user)
        {
            user.Inventory.AddItems(ShopBasket);
            user.Wallet.Join(ShopBasket.Wallet);
            await user.UpdateUser(new UpdateDefinitionBuilder<UserData>()
                .Set("Inventory", user.Inventory.GetInvetoryCache())
                .Inc("Money", ShopBasket.Wallet.Money)
                .Inc("Dispoints", ShopBasket.Wallet.Dispoints));
        }
    }
}
