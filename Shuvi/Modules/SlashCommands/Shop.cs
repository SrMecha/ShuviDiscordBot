using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
using MongoDB.Bson;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Interfaces.Shop;
using Shuvi.Extensions;
using Shuvi.Classes.CustomEmoji;
using Shuvi.Enums;
using Shuvi.StaticServices.UserCheck;

namespace Shuvi.Modules.SlashCommands
{
    public class ShopCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;
        private string _part = "purchasing";

        public ShopCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }

        [SlashCommand("shop", "Сходить в магазин.")]
        public async Task ShopCommandAsync()
        {
            await DeferAsync();
            var param = new InteractionParameters(await GetOriginalResponseAsync(), null);
            var users = new CustomContext(await _database.Users.GetUser(Context.User.Id), Context.User);
            if (UserCommandsCheck.IsUseCommands(Context.User.Id, ActiveCommands.Hunt))
            {
                await ModifyOriginalResponseAsync(
                    msg => { msg.Embed = UserCommandsCheck.GetErrorEmbed(Context.User.Id, ActiveCommands.Hunt); }
                    );
                return;
            }
            UserCommandsCheck.Add(Context.User.Id, ActiveCommands.Shop);
            await ViewAllShopsAsync(param, users);
            UserCommandsCheck.Remove(Context.User.Id, ActiveCommands.Shop);
        }

        public async Task ViewAllShopsAsync(InteractionParameters param, CustomContext users)
        {
            var location = _database.Map.GetRegion(users.DatabaseUser.Location.MapRegion)
                .GetLocation(users.DatabaseUser.Location.MapLocation);
            Embed embed;
            if (location.Shops.Count < 1)
            {
                embed = new UserEmbedBuilder(Context.User)
                    .WithDescription("В этой локации нету магазинов. Попробуйте поискать их на карте.\nКарта мира: `/map`\nПерейти в локацию: `/travel`")
                    .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; });
                return;
            }
            embed = new UserEmbedBuilder(Context.User)
                    .WithDescription("Выберите магазин, в который желаете зайти.")
                    .Build();
            var components = new ComponentBuilder()
                .WithSelectMenu("choose", location.GetShopsSelectMenu(), "Выберите магазин.")
                .WithButton("Выйти", "exit", ButtonStyle.Danger)
                .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
            param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
            if (param.Interaction == null)
            {
                await ModifyOriginalResponseAsync(msg => { msg.Components = new ComponentBuilder().Build(); });
                return;
            }
            switch (param.Interaction.Data.CustomId)
            {
                case "choose":
                    await MainShopAsync(param, users, location.GetShop(new ObjectId(param.Interaction.Data.Values.First())));
                    return;
                case "exit":
                    await DeleteOriginalResponseAsync();
                    return;
            }
        }

        public async Task MainShopAsync(InteractionParameters param, CustomContext users, IShop shop)
        {
            while (true)
            {
                switch (_part)
                {
                    case "purchasing":
                        await PurchasingPartAsync(param, users, shop);
                        break;
                    case "selling":
                        await SellingPartAsync(param, users, shop);
                        break;
                    default:
                        return;
                }
            }
        }
        public async Task PurchasingPartAsync(InteractionParameters param, CustomContext users, IShop shop)
        {
            var page = 0;
            var maxPage = shop.Purchasing.GetTotalEmbeds() - 1;
            var arrow = 0;
            while (true)
            {
                var embed = shop.Purchasing.GetEmbed(users, page, arrow);
                var components = new ComponentBuilder()
                    .WithButton("Покупка", "purchasing", ButtonStyle.Primary, row: 0, disabled: true, emote: EmojiList.Get("choosePoint"))
                    .WithButton("Продажа", "selling", ButtonStyle.Primary, row: 0, disabled: false)
                    .WithSelectMenu("choose", shop.Purchasing.GetSelectMenuOptions(page), "Выберите предмет.", disabled: !shop.Purchasing.HaveProducts())
                    .WithButton("+1", "1", ButtonStyle.Success, row: 2, disabled: !shop.Purchasing.CanBuy(page, arrow, users, 1))
                    .WithButton("+2", "2", ButtonStyle.Success, row: 2, disabled: !shop.Purchasing.CanBuy(page, arrow, users, 2))
                    .WithButton("+5", "5", ButtonStyle.Success, row: 2, disabled: !shop.Purchasing.CanBuy(page, arrow, users, 5))
                    .WithButton("<", "<", ButtonStyle.Primary, row: 3, disabled: page < 1)
                    .WithButton("Инфо", "info", ButtonStyle.Primary, row: 3, disabled: !shop.Purchasing.HaveProducts())
                    .WithButton(">", ">", ButtonStyle.Primary, row: 3, disabled: page >= maxPage)
                    .WithButton("Выйти", "exit", ButtonStyle.Danger, row: 4)
                    .WithButton("Сбросить", "clear", ButtonStyle.Secondary, row: 4)
                    .WithButton("Подтвердить", "confirm", ButtonStyle.Success, row: 4)
                    .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                await param.Interaction.TryDeferAsync();
                param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
                if (param.Interaction == null)
                {
                    await ModifyOriginalResponseAsync(msg => { msg.Components = new ComponentBuilder().Build(); });
                    _part = "break";
                    return;
                }
                switch (param.Interaction.Data.CustomId)
                {
                    case "selling":
                        _part = "selling";
                        return;
                    case "purchasing":
                        _part = "purchasing";
                        return;
                    case "choose":
                        arrow = int.Parse(param.Interaction.Data.Values.First());
                        break;
                    case "1" or "2" or "5":
                        shop.Purchasing.Buy(page, arrow, users, int.Parse(param.Interaction.Data.CustomId));
                        break;
                    case "<":
                        page -= 1;
                        arrow = 0;
                        break;
                    case ">":
                        page += 1;
                        arrow = 0;
                        break;
                    case "info":
                        await shop.Purchasing.GetItem(page, arrow).ViewItemAsync(_client, param, users.DiscordUser);
                        break;
                    case "clear":
                        shop.Reset();
                        break;
                    case "exit":
                        await DeleteOriginalResponseAsync();
                        _part = "break";
                        return;
                    case "confirm":
                        await shop.Confirm(users.DatabaseUser);
                        await ModifyOriginalResponseAsync(msg => { msg.Embed = shop.ShopBasket.GetFinishEmbed(users);
                            msg.Components = new ComponentBuilder().Build(); });
                        _part = "break";
                        return;
                }
            }
        }
        public async Task SellingPartAsync(InteractionParameters param, CustomContext users, IShop shop)
        {
            var page = 0;
            var maxPage = shop.Selling.GetTotalEmbeds() - 1;
            var arrow = 0;
            while (true)
            {
                var embed = shop.Selling.GetEmbed(users, page, arrow);
                var components = new ComponentBuilder()
                    .WithButton("Покупка", "purchasing", ButtonStyle.Primary, row: 0, disabled: false)
                    .WithButton("Продажа", "selling", ButtonStyle.Primary, row: 0, disabled: true, emote: EmojiList.Get("choosePoint"))
                    .WithSelectMenu("choose", shop.Selling.GetSelectMenuOptions(page), "Выберите предмет.", disabled: !shop.Selling.HaveProducts())
                    .WithButton("-1", "1", ButtonStyle.Success, row: 2, disabled: !shop.Selling.CanSell(page, arrow, users, 1))
                    .WithButton("-2", "2", ButtonStyle.Success, row: 2, disabled: !shop.Selling.CanSell(page, arrow, users, 2))
                    .WithButton("-5", "5", ButtonStyle.Success, row: 2, disabled: !shop.Selling.CanSell(page, arrow, users, 5))
                    .WithButton("<", "<", ButtonStyle.Primary, row: 3, disabled: page < 1)
                    .WithButton("Инфо", "info", ButtonStyle.Primary, row: 3, disabled: !shop.Selling.HaveProducts())
                    .WithButton(">", ">", ButtonStyle.Primary, row: 3, disabled: page >= maxPage)
                    .WithButton("Выйти", "exit", ButtonStyle.Danger, row: 4)
                    .WithButton("Сбросить", "clear", ButtonStyle.Secondary, row: 4)
                    .WithButton("Подтвердить", "confirm", ButtonStyle.Success, row: 4)
                    .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                await param.Interaction.TryDeferAsync();
                param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
                if (param.Interaction == null)
                {
                    await ModifyOriginalResponseAsync(msg => { msg.Components = new ComponentBuilder().Build(); });
                    _part = "break";
                    return;
                }
                switch (param.Interaction.Data.CustomId)
                {
                    case "selling":
                        _part = "selling";
                        return;
                    case "purchasing":
                        _part = "purchasing";
                        return;
                    case "choose":
                        arrow = int.Parse(param.Interaction.Data.Values.First());
                        break;
                    case "1" or "2" or "5":
                        shop.Selling.Sell(page, arrow, users, int.Parse(param.Interaction.Data.CustomId));
                        break;
                    case "<":
                        page -= 1;
                        arrow = 0;
                        break;
                    case ">":
                        page += 1;
                        arrow = 0;
                        break;
                    case "info":
                        await shop.Selling.GetItem(page, arrow).ViewItemAsync(_client, param, users.DiscordUser);
                        break;
                    case "clear":
                        shop.Reset();
                        break;
                    case "exit":
                        await DeleteOriginalResponseAsync();
                        _part = "break";
                        return;
                    case "confirm":
                        await shop.Confirm(users.DatabaseUser);
                        await ModifyOriginalResponseAsync(msg =>
                        {
                            msg.Embed = shop.ShopBasket.GetFinishEmbed(users);
                            msg.Components = new ComponentBuilder().Build();
                        });
                        _part = "break";
                        return;
                }
            }
        }
    }
}
