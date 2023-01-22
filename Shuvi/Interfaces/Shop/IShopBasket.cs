using Discord;
using Shuvi.Classes.Interactions;
using Shuvi.Interfaces.User;

namespace Shuvi.Interfaces.Shop
{
    public interface IShopBasket
    {
        public IUserWallet Wallet { get; }
        public void BuyItem(IProduct product, int amount);
        public void SellItem(IProduct product, int amount);
        public int GetAmount(IProduct product);
        public string GetItemsInfo();
        public Embed GetFinishEmbed(CustomContext context);
        public void Clear();
        public IEnumerator<KeyValuePair<IProduct, int>> GetEnumerator();
    }
}
