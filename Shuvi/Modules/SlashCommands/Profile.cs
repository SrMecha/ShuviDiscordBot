using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ShuviBot.Services;
using ShuviBot.Extensions.User;
using ShuviBot.Enums.Ranks;
using ShuviBot.Enums.UserRaces;
using ShuviBot.Enums.UserProfessions;
using ShuviBot.Enums.ItemType;
using SummaryAttribute = Discord.Interactions.SummaryAttribute;
using ShuviBot.Extensions.Interactions;
using Shuvi.Extensions.EmojiList;
using ShuviBot.Extensions.String;
using MongoDB.Bson;
using ShuviBot.Extensions.Items;
using DnsClient;
using static System.Reflection.Metadata.BlobBuilder;
using ShuviBot.Extensions.Map;

namespace ShardedClient.Modules
{
    public class ProfileCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManager _database;
        private readonly DiscordShardedClient _client;
        private readonly WorldMap _map;

        public ProfileCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManager>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _map = provider.GetRequiredService<WorldMap>();
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
                        interaction = await EquipmnetPartAsync(interaction, botMessage, dbUser, discordUser);
                        break;
                    case "location":
                        break;
                    case "statistics":
                        interaction = await StatisticsPartAsync(interaction, botMessage, dbUser, discordUser);
                        break;
                    case "inventory":
                        interaction = await InventoryPartAsync(dbUser, interaction, botMessage, discordUser);
                        break;
                    case "upgrade":
                        break;
                    default:
                        break;
                }
            } while (interaction != null);
        }
        public async Task<SocketMessageComponent?> InventoryPartAsync(User dbUser, SocketMessageComponent? interaction, IUserMessage message, IUser discordUser)
        {
            int maxPage = dbUser.Inventory.GetTotalEmbeds();
            int pageNow = 0;
            do
            {
                await message.ModifyAsync(msg =>
                {
                    msg.Content = "";
                    msg.Embed = dbUser.Inventory.GetItemsEmbed(pageNow).ToEmbedBuilder()
                    .WithAuthor($"{discordUser.Username} | Инвентарь", discordUser.GetAvatarUrl())
                    .WithColor(dbUser.Rank.GetColor())
                    .Build();
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
                        interaction = await ItemPartAsync(interaction, message, dbUser, discordUser, new ObjectId(interaction.Data.Values.First()));
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
        public async Task<SocketMessageComponent?> ItemPartAsync(SocketInteraction interaction, IUserMessage message, User dbUser, IUser discordUser, ObjectId itemId)
        {
            await message.ModifyAsync(msg =>
            {
                msg.Embed = dbUser.Inventory.GetItemEmbed(itemId).ToEmbedBuilder()
                .WithAuthor($"{discordUser.Username} | Просмотр предмета", discordUser.GetAvatarUrl())
                .WithColor(dbUser.Rank.GetColor())
                .Build();
                msg.Components = new ComponentBuilder()
                    .WithButton("Назад", "back", ButtonStyle.Danger)
                    .Build();
            });
            await interaction.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, interaction.User.Id);
        }
        public async Task<SocketMessageComponent> EquipmnetPartAsync(SocketInteraction interaction, IUserMessage message, User dbUser, IUser discordUser)
        {
            EquipmentItem? helmet = dbUser.GetEquipment(EquipmentType.Helmet);
            EquipmentItem? armor = dbUser.GetEquipment(EquipmentType.Armor);
            EquipmentItem? leggings = dbUser.GetEquipment(EquipmentType.Leggings);
            EquipmentItem? boots = dbUser.GetEquipment(EquipmentType.Boots);
            Embed embed = new EmbedBuilder()
                .WithAuthor($"{discordUser.Username} | Экипировка", discordUser.GetAvatarUrl())
                .AddField($"Шлем: {(helmet == null ? "Нету": helmet.Name)}", $"{(helmet == null ? "** **": helmet.GetBonusesInfo())}", true)
                .AddField($"Шлем: {(armor == null ? "Нету" : armor.Name)}", $"{(armor == null ? "** **" : armor.GetBonusesInfo())}", true)
                .AddField("** **", "** **", false)
                .AddField($"Шлем: {(leggings == null ? "Нету" : leggings.Name)}", $"{(leggings == null ? "** **" : leggings.GetBonusesInfo())}", true)
                .AddField($"Шлем: {(boots == null ? "Нету" : boots.Name)}", $"{(boots == null ? "** **" : boots.GetBonusesInfo())}", true)
                .WithFooter($"ID: {discordUser.Id}")
                .WithColor(dbUser.Rank.GetColor())
                .Build();
            await message.ModifyAsync(msg =>
            {
                msg.Embed = embed;
                msg.Components = new ComponentBuilder()
                    .WithButton("Назад", "back", ButtonStyle.Danger)
                    .Build();
            });
            await interaction.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, interaction.User.Id);
        }
        public async Task<SocketMessageComponent> StatisticsPartAsync(SocketInteraction interaction, IUserMessage message, User dbUser, IUser discordUser)
        {
            TimeSpan created = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.CreatedAt);
            TimeSpan live = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.LiveTime);
            Embed embed = new EmbedBuilder()
                .WithAuthor($"{discordUser.Username} | Статистика", discordUser.GetAvatarUrl())
                .WithDescription($"**Максимальный рейтинг:** {dbUser.MaxRating}\n" +
                $"**Всего врагов убито:** {dbUser.EnemyKilled}\n" +
                $"**Всего подземелий пройдено:** {dbUser.DungeonComplite}\n" +
                $"**Всего смертей:** {dbUser.DeathCount}\n\n" +
                $"**Время жизни:** {created:dd} дней | с {new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.CreatedAt):f}\n\n" +
                $"**Аккаунт создан:** {new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.LiveTime):f} | {live:dd} дней назад")
                .WithFooter($"ID: {discordUser.Id}")
                .WithColor(dbUser.Rank.GetColor())
                .Build();
            await message.ModifyAsync(msg =>
            {
                msg.Embed = embed;
                msg.Components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                    .Build();
            });
            await interaction.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, interaction.User.Id);
        }
        public async Task<SocketMessageComponent> MapPartAsync(SocketInteraction interaction, IUserMessage message, User dbUser, IUser discordUser)
        {
            MapRegion region = _map.GetRegion(dbUser.MapRegion);
            Embed embed = new EmbedBuilder()
                .WithAuthor($"{discordUser.Username} | Место нахождения", discordUser.GetAvatarUrl())
                .WithDescription($"**Регион:** {region.Name}\n**Локация:** {region.GetLocation(dbUser.MapLocation).Name}")
                .WithImageUrl(region.PictureURL)
                .WithFooter($"ID: {discordUser.Id}")
                .WithColor(dbUser.Rank.GetColor())
                .Build();
            await message.ModifyAsync(msg =>
            {
                msg.Embed = embed;
                msg.Components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                    .Build();
            });
            await interaction.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, interaction.User.Id);
        }
    }
}