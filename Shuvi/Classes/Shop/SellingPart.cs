using Discord;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.CustomEmoji;
using Shuvi.Classes.Interactions;
using Shuvi.Enums;
using Shuvi.Extensions;
using Shuvi.Interfaces.Shop;
using static MongoDB.Driver.WriteConcern;

namespace Shuvi.Classes.Shop
{
    public class SellingPart : ShopPartBase, ISellingPart
    {

        public SellingPart(IShopInfo shopInfo, IShopBasket shopBasket, List<ProductData> products) 
            : base(shopInfo, shopBasket, products)
        {

        }
        public bool CanSell(int page, int arrowIndex, CustomContext context, int amount = 1)
        {
            if (_products.Count < 1)
                return false;
            var product = _products[page * 10 + arrowIndex];
            return context.DatabaseUser.Inventory.GetItem(product.Id).Amount + _shopBasket.GetAmount(product) >= amount * product.Amount;
        }
        public void Sell(int page, int arrowIndex, CustomContext context, int amount = 1)
        {
            _shopBasket.SellItem(_products[page * 10 + arrowIndex], amount);
        }
        public int GetMaxSell(int page, int arrowIndex, CustomContext context)
        {
            if (_products.Count < 1)
                return 0;
            var product = _products[page * 10 + arrowIndex];
            return (context.DatabaseUser.Inventory.GetItem(product.Id).Amount + _shopBasket.GetAmount(product)) / product.Amount;
        }
        public override Embed GetEmbed(CustomContext context, int page, int arrowIndex)
        {
            var productsInfo = new List<string>();
            for (int i = page * 10; i < _products.Count && i < page * 10 + 10; i++)
                productsInfo.Add($"{(arrowIndex == i - 10 * page ? EmojiList.Get("choosePoint") : "")}" +
                    $"{_products[i].Name} {context.DatabaseUser.Inventory.GetItem(_products[i].Id).Amount + _shopBasket.GetAmount(_products[i])}" +
                    $"/{_products[i].Amount} = {_products[i].Price} {_products[i].MoneyType.GetEmoji()}");
            if (productsInfo.Count < 1)
                productsInfo.Add("Пусто.");
            return new UserEmbedBuilder(context.DiscordUser)
                .WithAuthor($"{_shopInfo.Name} | Продажа")
                .WithDescription($"Золото: {context.DatabaseUser.Wallet.Money} " +
                $"{_shopBasket.Wallet.Money.WithSign()} {MoneyType.Simple.GetEmoji()}" +
                $"\nДиспоинты: {context.DatabaseUser.Wallet.Dispoints} " +
                $"{_shopBasket.Wallet.Dispoints.WithSign()} {MoneyType.Dispoints.GetEmoji()}")
                .AddField("Магазин:", string.Join("\n", productsInfo), true)
                .AddField("Корзина:", _shopBasket.GetItemsInfo(), true)
                .WithFooter($"{context.DiscordUser.Username} | Страница {page + 1}/{GetTotalEmbeds()}")
                .Build();
        }
        public override Embed GetEmbed(CustomContext context, int page)
        {
            var productsInfo = new List<string>();
            for (int i = page * 10; i < _products.Count && i < page * 10 + 10; i++)
                productsInfo.Add($"{_products[i].Name} " +
                    $"{context.DatabaseUser.Inventory.GetItem(_products[i].Id).Amount + _shopBasket.GetAmount(_products[i])}" +
                    $"/{_products[i].Amount} = {_products[i].Price} {_products[i].MoneyType.GetEmoji()}");
            if (productsInfo.Count < 1)
                productsInfo.Add("Пусто.");
            return new UserEmbedBuilder(context.DiscordUser)
                .WithAuthor($"{_shopInfo.Name} | Продажа")
                .WithDescription($"Золото: {context.DatabaseUser.Wallet.Money} " +
                $"{_shopBasket.Wallet.Money.WithSign()} {MoneyType.Simple.GetEmoji()}" +
                $"\nДиспоинты: {context.DatabaseUser.Wallet.Dispoints} " +
                $"{_shopBasket.Wallet.Dispoints.WithSign()} {MoneyType.Dispoints.GetEmoji()}")
                .AddField("Магазин:", string.Join("\n", productsInfo), true)
                .AddField("Корзина:", _shopBasket.GetItemsInfo(), true)
                .WithFooter($"{context.DiscordUser.Username} | Страница {page + 1}/{GetTotalEmbeds()}")
                .Build();
        }
    }
}
