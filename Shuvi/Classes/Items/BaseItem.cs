using Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using Shuvi.Classes.Characteristics;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Enums;
using Shuvi.Extensions;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Items;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.Items
{
    public class BaseItem : IItem
    {
        public ObjectId Id { get; init; }
        public int Amount { get; protected set; }
        public string Name { get; init; }
        public string Description { get; init; }
        public ItemType Type { get; init; }
        public Rank Rank { get; init; }
        public bool CanTrade { get; init; }
        public bool CanLoose { get; init; }
        public int Max { get; init; }
        public virtual ICharacteristicBonuses Bonuses { get; protected set; } = new CharacteristicBonuses();
        public virtual Dictionary<ItemNeeds, int> Needs { get; protected set; } = new();

        public BaseItem(ItemData data, int amount)
        {
            Id = data.Id;
            Name = data.Name;
            Description = data.Description;
            Type = data.Type;
            Rank = data.Rank;
            CanTrade = data.CanTrade;
            CanLoose = data.CanLoose;
            Max = data.Max;
            Amount = amount;
        }
        protected static bool MeetNeeds(IDatabaseUser dbUser, ItemNeeds need, int amount)
        {
            return need switch
            {
                ItemNeeds.Strange => dbUser.Characteristic.Strength >= amount,
                ItemNeeds.Agility => dbUser.Characteristic.Agility >= amount,
                ItemNeeds.Luck => dbUser.Characteristic.Luck >= amount,
                ItemNeeds.Intellect => dbUser.Characteristic.Intellect >= amount,
                ItemNeeds.Endurance => dbUser.Characteristic.Endurance >= amount,
                ItemNeeds.Rank => (int)dbUser.Rating.Rank >= amount,
                _ => false
            };
        }
        public virtual Embed GetEmbed()
        {
            return new BotEmbedBuilder()
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {Name}\n**Тип:** {Type.ToRusString()}\n" +
                    $"**Ранг:** {Rank.ToRusString()}\n**Максимум в инвентаре:** {(Max < 0 ? "бесконечно" : Max)}\n\n" +
                    $"**Описание:**\n{Description}\n{(CanTrade ? "Можно обменять" : "Нельзя обменять")}")
                    .WithFooter($"ID: {Id}")
                    .WithColor(Rank.GetColor())
                    .Build();
        }
        public virtual Embed GetEmbed(IDatabaseUser dbUser, IUser user)
        {
            return new UserEmbedBuilder(user)
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {Name}\n**Тип:** {Type.ToRusString()}\n" +
                    $"**Ранг:** {Rank.ToRusString()}\n**Максимум в инвентаре:** {(Max < 0 ? "бесконечно" : Max)}\n**У вас есть:** {Amount}\n\n" +
                    $"**Описание:**\n{Description}\n{(CanTrade ? "Можно обменять" : "Нельзя обменять")}")
                    .WithFooter($"ID: {Id}")
                    .WithColor(Rank.GetColor())
                    .Build();
        }
        public virtual async Task ViewItemAsync(DiscordShardedClient client, InteractionParameters param, IUser discordUser)
        {
            var components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                .Build();
            await param.Message.ModifyAsync(msg => { msg.Embed = GetEmbed(); msg.Components = components; });
            if (param.Interaction != null)
                await param.Interaction.DeferAsync();
            param.Interaction = await WaitFor.UserButtonInteraction(client, param.Message, discordUser.Id);
        }
        public virtual async Task ViewItemAsync(DiscordShardedClient client, InteractionParameters param, IDatabaseUser dbUser, IUser discordUser)
        {
            var components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                .Build();
            await param.Message.ModifyAsync(msg => { msg.Embed = GetEmbed(dbUser, discordUser); msg.Components = components; });
            if (param.Interaction != null)
                await param.Interaction.DeferAsync();
            param.Interaction = await WaitFor.UserButtonInteraction(client, param.Message, dbUser.Id);
        }
    }
}