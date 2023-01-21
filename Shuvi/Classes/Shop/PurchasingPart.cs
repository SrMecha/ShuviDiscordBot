using Discord;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.CustomEmoji;
using Shuvi.Classes.Interactions;
using Shuvi.Extensions;
using Shuvi.Interfaces.Shop;

namespace Shuvi.Classes.Shop
{
    public class PurchasingPart : ShopPartBase, IPurchasingPart
    {
        public PurchasingPart(IShopInfo shopInfo, IShopBasket shopBasket, List<ProductData> products)
            : base(shopInfo, shopBasket, products)
        {

        }
        public void Buy(int page, int arrowIndex, CustomContext context, int amount = 1)
        {
            _shopBasket.BuyItem(_products[page * 10 + arrowIndex], amount);
        }
        public bool CanBuy(int page, int arrowIndex, CustomContext context, int amount = 1)
        {
            var product = _products[page * 10 + arrowIndex];
            var item = context.DatabaseUser.Inventory.GetItem(product.Id);
            return item.Max <= item.Amount + product.Amount * amount &&
                context.DatabaseUser.Wallet.Get(product.MoneyType) + _shopBasket.Wallet.Get(product.MoneyType) >= product.Price * amount;
        }
        public override Embed GetEmbed(CustomContext context, int page, int arrowIndex)
        {
            var productsInfo = new List<string>();
            for (int i = page * 10; i < _products.Count && i < page * 10 + 10; i++)
                productsInfo.Add($"{(arrowIndex == i - 10 * page ? EmojiList.Get("choosePoint") : "")}" +
                    $"{_products[i].Name} x{_products[i].Amount} {_products[i].MoneyType.GetEmoji()}");
            return new UserEmbedBuilder(context.DiscordUser)
                .WithAuthor($"Магазин {_shopInfo.Name} | Покупка")
                .WithDescription($"Золото: {context.DatabaseUser.Wallet.Money} {_shopBasket.Wallet.Money.WithSign()}" +
                $"\nДиспоинты: {context.DatabaseUser.Wallet.Dispoints} {_shopBasket.Wallet.Dispoints.WithSign()}")
                .AddField("Магазин:", string.Join("\n", productsInfo), true)
                .AddField("Корзина:", _shopBasket.GetItemsInfo(), true)
                .Build();
        }
        public override Embed GetEmbed(CustomContext context, int page)
        {
            var productsInfo = new List<string>();
            for (int i = page * 10; i < _products.Count && i < page * 10 + 10; i++)
                productsInfo.Add($"{_products[i].Name} x{_products[i].Amount} {_products[i].MoneyType.GetEmoji()}");
            return new UserEmbedBuilder(context.DiscordUser)
                .WithAuthor($"Магазин {_shopInfo.Name} | Покупка")
                .WithDescription($"Золото: {context.DatabaseUser.Wallet.Money} {_shopBasket.Wallet.Money.WithSign()}" +
                $"\nДиспоинты: {context.DatabaseUser.Wallet.Dispoints} {_shopBasket.Wallet.Dispoints.WithSign()}")
                .AddField("Магазин:", string.Join("\n", productsInfo), true)
                .AddField("Корзина:", _shopBasket.GetItemsInfo(), true)
                .Build();
        }
    }
}
