using Discord.WebSocket;
using Discord;
using MongoDB.Driver;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.User;
using Shuvi.Extensions;
using Shuvi.Interfaces.Items;
using Shuvi.Interfaces.User;
using Shuvi.Interfaces.Status;
using Shuvi.Classes.Status;

namespace Shuvi.Classes.Items
{
    public class PotionItem : BaseItem, IUsefullItem
    {
        private static readonly Dictionary<string, string> _types = new()
        {
            { "Mana", "маны" },
            { "Health", "жизни" },
            { "Energy", "энергии" }
        };

        protected readonly string _characteristic;
        protected readonly int _amount;

        public PotionItem(ItemData data, int amount) : base(data, amount)
        {
            _characteristic = data.Charactteristic;
            _amount = data.Amount;
        }
        public override Embed GetEmbed()
        {
            return new BotEmbedBuilder()
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {Name}\n**Тип:** {Type.ToRusString()}\n" +
                    $"**Ранг:** {Rank.ToRusString()}\n**Максимум в инвентаре:** {(Max < 0 ? "бесконечно" : Max)}\n\n" +
                    $"**Описание:**\n{Description}\n{(CanTrade ? "Можно обменять" : "Нельзя обменять")}\n" +
                    $"Восполняет {_amount} ед. {_types[_characteristic]}.")
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
                    $"**Описание:**\n{Description}\n{(CanTrade ? "Можно обменять" : "Нельзя обменять")}\n" +
                    $"Восполняет {_amount} ед. {_types[_characteristic]}.")
                    .WithFooter($"ID: {Id}")
                    .WithColor(Rank.GetColor())
                    .Build();
        }

        public async Task<IActionResult> Use(IDatabaseUser dbUser)
        {
            Amount -= 1;
            switch (_characteristic)
            {
                case "Mana":
                    dbUser.Mana.IncreaseMana(_amount);
                    dbUser.Inventory.RemoveItem(Id, 1);
                    await dbUser.UpdateUser(new UpdateDefinitionBuilder<UserData>()
                        .Set("Mana", dbUser.Mana.RegenTime)
                        .Set("Inventory", dbUser.Inventory.GetInvetoryCache()));
                    return new ActionResult($"Вы восстановили {_amount} ед. маны.");
                case "Health":
                    dbUser.Health.IncreaseHealth(_amount);
                    dbUser.Inventory.RemoveItem(Id, 1);
                    await dbUser.UpdateUser(new UpdateDefinitionBuilder<UserData>()
                        .Set("Health", dbUser.Health.RegenTime)
                        .Set("Inventory", dbUser.Inventory.GetInvetoryCache()));
                    return new ActionResult($"Вы восстановили {_amount} ед. жизни.");
                case "Energy":
                    dbUser.Energy.IncreaseEnergy(_amount);
                    dbUser.Inventory.RemoveItem(Id, 1);
                    await dbUser.UpdateUser(new UpdateDefinitionBuilder<UserData>()
                        .Set("Energy", dbUser.Energy.RegenTime)
                        .Set("Inventory", dbUser.Inventory.GetInvetoryCache()));
                    return new ActionResult($"Вы восстановили {_amount} ед. энергии.");
                default:
                    return new ActionResult("Error");
            }
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
                    await param.Interaction.TryDeferAsync();
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
