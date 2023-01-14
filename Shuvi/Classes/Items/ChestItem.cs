using Discord.WebSocket;
using Discord;
using MongoDB.Driver;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.Status;
using Shuvi.Classes.User;
using Shuvi.Extensions;
using Shuvi.Interfaces.Items;
using Shuvi.Interfaces.Status;
using Shuvi.Interfaces.User;
using Shuvi.Classes.Rates;
using MongoDB.Bson;
using Shuvi.Classes.CustomEmoji;

namespace Shuvi.Classes.Items
{
    public class ChestItem : BaseItem, IUsefullItem
    {
        protected readonly int _dispointsMin;
        protected readonly int _dispointsMax;
        protected readonly AllRate _drop;
        private readonly string _dropInfo;

        public ChestItem(ItemData data, int amount) : base(data, amount)
        {
            _dispointsMin = data.Drop.DispointsMin;
            _dispointsMax = data.Drop.DispointsMax;
            _drop = new(data.Drop.Items);
            _dropInfo = GetPossibleDropInfo();
        }
        protected string GetPossibleDropInfo()
        {
            var result = new List<string>();
            foreach (var (id, chance) in _drop)
            {
                var item = ItemFactory.CreateItem(id, 1);
                result.Add($"{item.Name} {chance / (float)_drop.All:P1}");
            }
            return string.Join("\n", result);
        }
        public override Embed GetEmbed()
        {
            return new BotEmbedBuilder()
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {Name}\n**Тип:** {Type.ToRusString()}\n" +
                    $"**Ранг:** {Rank.ToRusString()}\n**Максимум в инвентаре:** {(Max < 0 ? "бесконечно" : Max)}\n\n" +
                    $"**Описание:**\n{Description}\n{(CanTrade ? "Можно обменять" : "Нельзя обменять")}\n\n" +
                    $"**Возможный дроп:**\n {_dropInfo}")
                    .WithFooter($"ID: {Id}")
                    .WithColor(Rank.GetColor())
                    .Build();
        }
        public override Embed GetEmbed(IDatabaseUser dbUser, IUser user)
        {
            return new UserEmbedBuilder(user)
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {Name}\n**Тип:** {Type.ToRusString()}\n" +
                    $"**Ранг:** {Rank.ToRusString()}\n**Максимум в инвентаре:** {(Max < 0 ? "бесконечно" : Max)}\n**У вас есть:** {Amount}\n\n" +
                    $"**Описание:**\n{Description}\n{(CanTrade ? "Можно обменять" : "Нельзя обменять")}\n\n" +
                    $"**Возможный дроп:**\n {_dropInfo}")
                    .WithFooter($"ID: {Id}")
                    .WithColor(Rank.GetColor())
                    .Build();
        }

        public async Task<IActionResult> Use(IDatabaseUser dbUser)
        {
            Amount -= 1;
            var dispoints = new Random().Next(_dispointsMin, _dispointsMax + 1);
            var drop = _drop.GetRandom(dbUser.Inventory);
            dbUser.Wallet.AddDispoints(dispoints);
            dbUser.Inventory.AddItems(drop);
            dbUser.Inventory.RemoveItem(Id, 1);
            await dbUser.UpdateUser(new UpdateDefinitionBuilder<UserData>()
                        .Inc("Dispoints", dispoints)
                        .Set("Inventory", dbUser.Inventory.GetInvetoryCache()));
            return new ActionResult($"{(dispoints < 1 ? string.Empty : $"+{dispoints} {EmojiList.Get("money")}")}\n{drop.GetDropInfo()}");
        }

        public override async Task ViewItemAsync(DiscordShardedClient client, InteractionParameters param, IDatabaseUser dbUser, IUser discordUser)
        {
            while (true)
            {
                var components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                .WithButton("Использовать", "use", ButtonStyle.Success, disabled: !dbUser.Inventory.HaveItem(Id, 1))
                .Build();
                await param.Message.ModifyAsync(msg => { msg.Embed = GetEmbed(dbUser, discordUser); msg.Components = components; });
                if (param.Interaction != null)
                    await param.Interaction.DeferAsync();
                param.Interaction = await WaitFor.UserButtonInteraction(client, param.Message, dbUser.Id);
                if (param.Interaction == null)
                {
                    await param.Message.ModifyAsync(msg => { msg.Components = new ComponentBuilder().Build(); });
                    return;
                }
                switch (param.Interaction.Data.CustomId)
                {
                    case "use":
                        if (!dbUser.Inventory.HaveItem(Id, 1))
                        {
                            await param.Interaction.RespondAsync(embed: ErrorEmbedBuilder.Simple("У вас нету данного предмета."), ephemeral: true);
                            return;
                        }
                        var result = await Use(dbUser);
                        var embed = new EmbedBuilder().WithDescription(result.Description).WithColor(Color.Green).Build();
                        await param.Interaction.RespondAsync(embed: embed, ephemeral: true);
                        param.Interaction = null;
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
