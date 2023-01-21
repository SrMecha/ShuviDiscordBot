using Shuvi.Classes.User;
using Shuvi.Interfaces.Shop;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.Shop
{
    public class ShopBasket : IShopBasket
    {
        protected Dictionary<IProduct, int> _storage;

        public IUserWallet Wallet { get; private set; } = new UserWallet();

        public ShopBasket()
        {
            _storage = new();
        }
        public void BuyItem(IProduct product, int amount)
        {
            if (_storage.ContainsKey(product))
                _storage[product] += amount * product.Amount;
            else
                _storage[product] = amount * product.Amount;
            Wallet.Add(product.MoneyType, product.Price * amount);
        }
        public void SellItem(IProduct product, int amount)
        {
            if (_storage.ContainsKey(product))
                _storage[product] -= amount * product.Amount;
            else
                _storage[product] = -amount * product.Amount;
            Wallet.Remove(product.MoneyType, product.Price * amount);
        }
        public string GetItemsInfo()
        {
            var productsInfo = new List<string>();
            foreach (var (product, amount) in _storage)
                productsInfo.Add($"{(amount > 0 ? $"+{amount}" : amount)} {product.Name}");
            return string.Join("\n", productsInfo);
        }
        public IEnumerator<KeyValuePair<IProduct, int>> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }
        public int GetAmount(IProduct product)
        {
            return _storage.GetValueOrDefault(product, 0);
        }
    }
}
