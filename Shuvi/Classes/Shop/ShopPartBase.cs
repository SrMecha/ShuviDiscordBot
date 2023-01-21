using Discord;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.CustomEmoji;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.Items;
using Shuvi.Interfaces.Items;
using Shuvi.Interfaces.Shop;
using System.Security.Cryptography;

namespace Shuvi.Classes.Shop
{
    public class ShopPartBase : IShopPartBase
    {
        protected List<IProduct> _products;
        protected IShopInfo _shopInfo;
        protected IShopBasket _shopBasket;

        public ShopPartBase(IShopInfo shopInfo, IShopBasket shopBasket, List<ProductData> products)
        {
            _shopInfo = shopInfo;
            _products = new();
            _shopBasket = shopBasket;
            foreach (var product in products)
                _products.Add(new Product(product, ItemFactory.CreateItem(product.Id, product.Amount).Name));
        }
        public virtual Embed GetEmbed(CustomContext context, int page, int arrowIndex)
        {
            var productsInfo = new List<string>();
            for (int i = page * 10; i < _products.Count && i < page * 10 + 10; i++)
                productsInfo.Add($"{(arrowIndex == i - 10 * page ? EmojiList.Get("choosePoint") : "")}{_products[i].Name} | {_products[i].Amount}");
            return new UserEmbedBuilder(context.DiscordUser)
                .WithAuthor($"Магазин {_shopInfo.Name}")
                .AddField("Магазин:", string.Join("\n", productsInfo), true)
                .AddField("Корзина:", _shopBasket.GetItemsInfo(), true)
                .Build();
        }
        public virtual Embed GetEmbed(CustomContext context, int page)
        {
            var productsInfo = new List<string>();
            for (int i = page * 10; i < _products.Count && i < page * 10 + 10; i++)
                productsInfo.Add($"{_products[i].Name} | {_products[i].Amount}");
            return new UserEmbedBuilder(context.DiscordUser)
                .WithAuthor($"Магазин {_shopInfo.Name}")
                .AddField("Магазин:", string.Join("\n", productsInfo), true)
                .AddField("Корзина:", _shopBasket.GetItemsInfo(), true)
                .Build();
        }
        public IItem GetItem(int itemIndex)
        {
            return ItemFactory.CreateItem(_products[itemIndex].Id, 0);
        }
        public IItem GetItem(int page, int arrowIndex)
        {
            return ItemFactory.CreateItem(_products[page * 10 + arrowIndex].Id, 0);
        }
        public virtual List<SelectMenuOptionBuilder> GetSelectMenuOptions(int page)
        {
            var result = new List<SelectMenuOptionBuilder>();
            for (int i = page * 10; i < _products.Count && i < page * 10 + 10; i++)
            {
                result.Add(new SelectMenuOptionBuilder(
                    _products[i].Name,
                    i.ToString()
                    ));
            }
            if (result.Count < 1)
            {
                result.Add(new SelectMenuOptionBuilder("None", "None", "Nahuia ti bota slomal"));
            }
            return result;        
        }
        public int GetTotalEmbeds()
        {
            return ((_products.Count + 9) / 10) < 1 ? 1 : (_products.Count + 9) / 10;
        }
    }
}
