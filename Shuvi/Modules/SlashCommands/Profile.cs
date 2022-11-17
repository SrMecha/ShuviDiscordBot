using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ShuviBot.Services;
using ShuviBot.Extensions.User;
using ShuviBot.Enums.Ranks;
using ShuviBot.Enums.UserRaces;
using ShuviBot.Enums.UserProfessions;
using SummaryAttribute = Discord.Interactions.SummaryAttribute;
using ShuviBot.Extensions.Interactions;
using Shuvi.Extensions.EmojiList;
using ShuviBot.Extensions.String;
using MongoDB.Bson;

namespace ShardedClient.Modules
{
    public class ProfileCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManager _database;
        private readonly DiscordShardedClient _client;

        public ProfileCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManager>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }

        [SlashCommand("profile", "Информаиця о игроке")]
        public async Task ProfileCommandAsync([Summary("user", "Выберите пользователя.")] IUser? paramUser = null)
        {
            IUser discordUser = paramUser ?? Context.User;
            User dbUser = await _database.Users.GetUser(discordUser.Id);
            SocketMessageComponent? interaction = null;
            Embed embed;
            MessageComponent components;
            IUserMessage? botMessage = null;
            byte healthFullEmojiCount;
            byte energyFullEmojiCount;
            if (discordUser.Id == Context.User.Id)
            {
                components = new ComponentBuilder()
                    .WithButton("Экипировка", "equipment", ButtonStyle.Primary, row: 0)
                    .WithButton("Локация", "location", ButtonStyle.Primary, row: 0)
                    .WithButton("Статистика", "statistics", ButtonStyle.Primary, row: 0)
                    .WithButton("Инвентарь", "inventory", ButtonStyle.Primary, row: 1)
                    .WithButton("Улучшения", "upgrade", ButtonStyle.Primary, row: 1)
                    .Build();
            }
            else
            {
                components = new ComponentBuilder()
                    .WithButton("Экипировка", "equipment", ButtonStyle.Primary, row: 0)
                    .WithButton("Локация", "location", ButtonStyle.Primary, row: 0)
                    .WithButton("Статистика", "statistics", ButtonStyle.Primary, row: 0)
                    .Build();
            }
            do 
            {
                healthFullEmojiCount = (byte)(dbUser.GetCurrentHealth() / (UserSettings.HealthMax / UserSettings.HealthDisplayMax));
                energyFullEmojiCount = (byte)(dbUser.GetCurrentEnergy() / (dbUser.GetMaxEnergy() / UserSettings.EnergyDisplayMax));
                embed = new EmbedBuilder()
                    .AddField($"**Ранг:** {dbUser.Rank.ToRusString()}",
                    $"**Рейтинг:** {dbUser.Rating}{(dbUser.Rank.CanRankUp() ? "/" + (global::ShuviBot.Enums.Ranks.Rank)(dbUser.Rank + 1).GetNeedRating() : ' ')}\n" +
                    $"**Раса:** {dbUser.Race.ToRusString()}\n**Профессия:** {dbUser.Profession.ToRusString()}\n" +
                    $"**Диспоинты:** {dbUser.Money:n} {EmojiList.Get("money")}",
                    true)
                    .AddField("Характеристики:",
                    $"**Сила:** {dbUser.Strength} {(dbUser.Bonuses.Strength >= 0 ? "| +" + dbUser.Bonuses.Strength : dbUser.Bonuses.Strength)}\n" +
                    $"**Ловкость:** {dbUser.Agility} {(dbUser.Bonuses.Agility >= 0 ? "| +" + dbUser.Bonuses.Agility : dbUser.Bonuses.Agility)}\n" +
                    $"**Удача:** {dbUser.Luck} {(dbUser.Bonuses.Luck >= 0 ? "| +" + dbUser.Bonuses.Luck : dbUser.Bonuses.Luck)}\n" +
                    $"**Интеллект:** {dbUser.Intellect} {(dbUser.Bonuses.Intellect >= 0 ? "| +" + dbUser.Bonuses.Intellect : dbUser.Bonuses.Intellect)}\n" +
                    $"**Выносливость:** {dbUser.Endurance} {(dbUser.Bonuses.Endurance >= 0 ? "| +" + dbUser.Bonuses.Endurance : "| " + dbUser.Bonuses.Endurance)}",
                    true)
                    .AddField("** **",
                    $"[{EmojiList.Get("energyFull").ToString()!.Multiple(energyFullEmojiCount)}" +
                    $"{EmojiList.Get("energyEmpty").ToString()!.Multiple((byte)(UserSettings.EnergyDisplayMax - energyFullEmojiCount))}] " +
                    $"{dbUser.GetCurrentEnergy()}/{dbUser.GetMaxEnergy()}\n" +
                    $"{(dbUser.GetRemainingEnergyRegenTime() == 0 ? "" : $"[Восстановится <t:{dbUser.EnergyRegenTime}:R>]\n")}\n" +
                    $"[{EmojiList.Get("healthFull").ToString()!.Multiple(healthFullEmojiCount)}" +
                    $"{EmojiList.Get("healthEmpty").ToString()!.Multiple((byte)(UserSettings.HealthDisplayMax - healthFullEmojiCount))}] " +
                    $"{dbUser.GetCurrentHealth()}/100\n" +
                    $"{(dbUser.GetRemainingHealthRegenTime() == 0 ? "" : $"[Восстановится <t:{dbUser.HealthRegenTime}:R>]")}",
                    false)
                    .WithAuthor(discordUser.Username, discordUser.GetAvatarUrl())
                    .WithFooter($"ID: {discordUser.Id}")
                    .WithColor(dbUser.Rank.GetColor())
                .Build();
                if (botMessage == null)
                {
                    await RespondAsync(embed: embed, components: components);
                    botMessage = await GetOriginalResponseAsync();
                }
                else
                    await botMessage.ModifyAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (interaction != null)
                    await interaction.DeferAsync();
                interaction = await WaitFor.UserButtonInteraction(_client, botMessage, dbUser.Id);
                switch (interaction.Data.CustomId)
                {
                    case "equipment":
                        break;
                    case "location":
                        break;
                    case "statistics":
                        break;
                    case "inventory":
                        interaction = await InventoryPartAsync(dbUser, interaction, botMessage);
                        break;
                    case "upgrade":
                        break;
                    default:
                        break;
                }
            } while (interaction != null);
        }
        public async Task<SocketMessageComponent?> InventoryPartAsync(User dbUser, SocketMessageComponent? interaction, IUserMessage message)
        {
            int maxPage = dbUser.Inventory.GetTotalEmbeds();
            int pageNow = 0;
            do
            {
                await message.ModifyAsync(msg =>
                {
                    msg.Content = "";
                    msg.Embed = dbUser.Inventory.GetItemsEmbed(pageNow);
                    msg.Components = new ComponentBuilder()
                        .WithButton("<", "<", ButtonStyle.Primary, disabled: pageNow <= 0)
                        .WithButton("Выйти", "exit", ButtonStyle.Danger)
                        .WithButton(">", ">", ButtonStyle.Primary, disabled: pageNow >= maxPage - 1)
                        .WithSelectMenu("choose", dbUser.Inventory.GetItemsSelectMenu(pageNow),
                        "Выберите предмет для просмотра", disabled: dbUser.Inventory.Count == 0)
                        .Build();
                });
                if (interaction != null) await interaction.DeferAsync();
                interaction = await WaitFor.UserButtonInteraction(_client, message, dbUser.Id);
                switch (interaction.Data.CustomId)
                {
                    case "<":
                        pageNow--;
                        break;
                    case "exit":
                        return interaction;
                    case "choose":
                        interaction = await ItemPartAsync(interaction, message, dbUser, new ObjectId(interaction.Data.Values.First()));
                        break;
                    case ">":
                        pageNow++;
                        break;
                    default:
                        break;
                }
            } while (interaction != null);
            return interaction;
        }
        public async Task<SocketMessageComponent?> ItemPartAsync(SocketInteraction interaction, IUserMessage message, User dbUser, ObjectId itemId)
        {
            await message.ModifyAsync(msg =>
            {
                msg.Embed = dbUser.Inventory.GetItemEmbed(itemId);
                msg.Components = new ComponentBuilder()
                    .WithButton("Назад", "back", ButtonStyle.Danger)
                    .Build();
            });
            await interaction.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, interaction.User.Id);
        }
    }
}