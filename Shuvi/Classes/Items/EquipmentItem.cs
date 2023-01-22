using Discord.WebSocket;
using Discord;
using Shuvi.Classes.Characteristics;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Enums;
using Shuvi.Extensions;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.User;
using Shuvi.Classes.CustomEmoji;
using MongoDB.Driver;
using Shuvi.Classes.User;
using System.Reflection.Metadata;

namespace Shuvi.Classes.Items
{
    public sealed class EquipmentItem : BaseItem
    {
        public override ICharacteristicBonuses Bonuses { get; protected set; }
        public override Dictionary<ItemNeeds, int> Needs { get; protected set; }

        public EquipmentItem(ItemData data, int amount) : base(data, amount)
        {
            Bonuses = data.Bonuses;
            Needs = data.Needs;
        }
        public string GetBonusesInfo()
        {
            var result = "";
            foreach (var (bonus, amount) in Bonuses)
            {
                result += $"{bonus.ToRusString()} {amount.WithSign()}\n";
            }
            if (result == "")
            {
                result = "Нету бонусов.";
            }
            return result;
        }
        public string GetNeedsInfo()
        {
            var result = "";
            foreach (var need in Needs)
            {
                result += $"{need.Key.ToRusString()} {need.Key.GetFormatString(need.Value)}+\n";
            }
            if (result == "")
            {
                result = "Нету требований.";
            }
            return result;
        }
        public string GetNeedsInfo(IDatabaseUser dbUser)
        {
            var result = "";
            foreach (var (need, amount) in Needs)
            {
                result += MeetNeeds(dbUser, need, amount) ? EmojiList.Get("goodMark").ToString() : EmojiList.Get("badMark").ToString();
                result += $" {need.ToRusString()} {need.GetFormatString(amount)}+\n";
            }
            if (result == "")
            {
                result = "Нету требований.";
            }
            return result;
        }
        private bool CanEquip(IDatabaseUser dbUser)
        {
            foreach (var (need, amount) in Needs)
                if (!MeetNeeds(dbUser, need, amount))
                    return false;
            return true;
        }
        public override Embed GetEmbed()
        {
            return new BotEmbedBuilder()
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {Name}\n**Тип:** {Type.ToRusString()}\n" +
                    $"**Ранг:** {Rank.ToRusString()}\n**Максимум в инвентаре:** {(Max < 0 ? "бесконечно" : Max)}\n\n" +
                    $"**Описание:**\n{Description}\n{(CanTrade ? "Можно обменять" : "Нельзя обменять")}\n\n" +
                    $"**Бонусы:** \n{GetBonusesInfo()}\n" +
                    $"**Требования:**\n {GetNeedsInfo()}")
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
                    $"**Бонусы:** \n{GetBonusesInfo()}\n" +
                    $"**Требования:**\n {GetNeedsInfo(dbUser)}")
                    .WithFooter($"ID: {Id}")
                    .WithColor(Rank.GetColor())
                    .Build();
        }
        public override async Task ViewItemAsync(DiscordShardedClient client, InteractionParameters param, IDatabaseUser dbUser, IUser discordUser)
        {
            while (true)
            {
                var components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                .WithButton("Надеть", "equip", ButtonStyle.Success, disabled: dbUser.Equipment.GetEquipment(Type) == Id || !CanEquip(dbUser))
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
                    case "equip":
                        if (!dbUser.Inventory.HaveItem(Id, Amount))
                        {
                            await param.Interaction.RespondAsync(embed: ErrorEmbedBuilder.Simple("У вас нету данного предмета."), ephemeral: true);
                            param.Interaction = null;
                            return;
                        }
                        dbUser.Equipment.SetEquipment(Type, Id);
                        await dbUser.UpdateUser(new UpdateDefinitionBuilder<UserData>().Set(Type.ToEngString(), Id));
                        var embed = new EmbedBuilder().WithDescription("Предмет экипирован.").WithColor(Color.Green).Build();
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