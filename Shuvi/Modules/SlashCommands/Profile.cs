using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
using SummaryAttribute = Discord.Interactions.SummaryAttribute;
using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Map;
using Shuvi.Classes.CustomEmoji;
using Shuvi.Interfaces.User;
using Shuvi.Extensions;
using Shuvi.Enums;
using Shuvi.Classes.Characteristics;

namespace Shuvi.Modules.SlashCommands
{
    public class ProfileCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;
        private readonly WorldMap _map;

        public ProfileCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _map = _database.Map;
        }

        [SlashCommand("profile", "Информаиця о игроке")]
        public async Task ProfileCommandAsync([Summary("user", "Выберите пользователя.")] IUser? paramUser = null)
        {
            var discordUser = paramUser ?? Context.User;
            var dbUser = await _database.Users.GetUser(discordUser.Id);
            SocketMessageComponent? interaction = null;
            Embed embed;
            MessageComponent components;
            IUserMessage? botMessage = null;
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
                embed = new UserEmbedBuilder(discordUser)
                    .AddField($"**Ранг:** {dbUser.Rating.Rank.ToRusString()}",
                    $"**Рейтинг:** {dbUser.Rating.Points}{(dbUser.Rating.Rank.CanRankUp() ? "/" + (Rank)(dbUser.Rating.Rank + 1).GetNeedRating() : ' ')}\n" +
                    $"**Раса:** {dbUser.Race.ToRusString()}\n**Профессия:** {dbUser.Profession.ToRusString()}\n" +
                    $"**Диспоинты:** {dbUser.Wallet.Money:n0} {EmojiList.Get("money")}",
                    true)
                    .AddField("Характеристики:",
                    ((UserCharacteristics)dbUser.Characteristic).ToRusString(new CharacteristicBonuses(dbUser.Equipment)),
                    true)
                    .AddField("** **",
                    $"**Энергия:** [{dbUser.Energy.GetEmojiBar()}] " +
                    $"{dbUser.Energy.GetCurrentEnergy()}/{dbUser.Energy.Max}\n" +
                    $"{(dbUser.Energy.GetRemainingRegenTime() == 0 ? "" : $"[Восстановится <t:{dbUser.Energy.RegenTime}:R>]\n")}\n" +
                    $"**Здоровье:** [{dbUser.Health.GetEmojiBar()}] " +
                    $"{dbUser.Health.GetCurrentHealth()}/{dbUser.Health.Max}\n" +
                    $"{(dbUser.Health.GetRemainingRegenTime() == 0 ? "" : $"[Восстановится <t:{dbUser.Health.RegenTime}:R>]\n")}\n" +
                    $"**Мана:** [{dbUser.Mana.GetEmojiBar()}] " +
                    $"{dbUser.Mana.GetCurrentMana()}/{dbUser.Mana.Max}\n" +
                    $"{(dbUser.Mana.GetRemainingRegenTime() == 0 ? "" : $"[Восстановится <t:{dbUser.Mana.RegenTime}:R>]")}",
                    false)
                    .WithAuthor("Профиль")
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
                interaction = await WaitFor.UserButtonInteraction(_client, botMessage, Context.User.Id);
                switch (interaction.Data.CustomId)
                {
                    case "equipment":
                        interaction = await EquipmnetPartAsync(interaction, botMessage, dbUser, discordUser);
                        break;
                    case "location":
                        interaction = await MapPartAsync(interaction, botMessage, dbUser, discordUser);
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
        public async Task<SocketMessageComponent?> InventoryPartAsync(IDatabaseUser dbUser, SocketMessageComponent? interaction, IUserMessage message, IUser discordUser)
        {
            int maxPage = dbUser.Inventory.GetTotalEmbeds();
            int pageNow = 0;
            do
            {
                await message.ModifyAsync(msg =>
                {
                    msg.Content = "";
                    msg.Embed = dbUser.Inventory.GetItemsEmbed(pageNow).ToEmbedBuilder()
                    .WithAuthor($"Инвентарь")
                    .WithFooter($"{discordUser.Username} | {discordUser.Id}", discordUser.GetAvatarUrl())
                    .WithColor(UserEmbedBuilder.StandartColor)
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
        public async Task<SocketMessageComponent?> ItemPartAsync(SocketInteraction interaction, IUserMessage message, IDatabaseUser dbUser, IUser discordUser, ObjectId itemId)
        {
            await message.ModifyAsync(msg =>
            {
                msg.Embed = dbUser.Inventory.GetItemEmbed(itemId).ToEmbedBuilder()
                .WithAuthor($"Просмотр предмета")
                .WithFooter($"{discordUser.Username} | {discordUser.Id}", discordUser.GetAvatarUrl())
                    .WithColor(UserEmbedBuilder.StandartColor)
                .Build();
                msg.Components = new ComponentBuilder()
                    .WithButton("Назад", "back", ButtonStyle.Danger)
                    .Build();
            });
            await interaction.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, interaction.User.Id);
        }
        public async Task<SocketMessageComponent> EquipmnetPartAsync(SocketInteraction interaction, IUserMessage message, IDatabaseUser dbUser, IUser discordUser)
        {
            EquipmentItem? helmet = dbUser.Equipment.Head == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)dbUser.Equipment.Head, 0);
            EquipmentItem? armor = dbUser.Equipment.Body == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)dbUser.Equipment.Body, 0);
            EquipmentItem? leggings = dbUser.Equipment.Legs == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)dbUser.Equipment.Legs, 0);
            EquipmentItem? boots = dbUser.Equipment.Foots == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)dbUser.Equipment.Foots, 0);
            Embed embed = new UserEmbedBuilder(discordUser)
                .WithAuthor($"Экипировка")
                .AddField($"Шлем: {(helmet == null ? "Нету" : helmet.Name)}", $"{(helmet == null ? "** **" : helmet.GetBonusesInfo())}", true)
                .AddField($"Броня: {(armor == null ? "Нету" : armor.Name)}", $"{(armor == null ? "** **" : armor.GetBonusesInfo())}", true)
                .AddField("** **", "** **", false)
                .AddField($"Поножи: {(leggings == null ? "Нету" : leggings.Name)}", $"{(leggings == null ? "** **" : leggings.GetBonusesInfo())}", true)
                .AddField($"Ботинки: {(boots == null ? "Нету" : boots.Name)}", $"{(boots == null ? "** **" : boots.GetBonusesInfo())}", true)
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
        public async Task<SocketMessageComponent> StatisticsPartAsync(SocketInteraction interaction, IUserMessage message, IDatabaseUser dbUser, IUser discordUser)
        {
            TimeSpan created = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.Statistics.CreatedAt);
            TimeSpan live = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.Statistics.LiveTime);
            Embed embed = new UserEmbedBuilder(discordUser)
                .WithAuthor("Статистика")
                .WithDescription($"**Максимальный рейтинг:** {dbUser.Statistics.MaxRating}\n" +
                $"**Всего врагов убито:** {dbUser.Statistics.EnemyKilled}\n" +
                $"**Всего подземелий пройдено:** {dbUser.Statistics.DungeonComplite}\n" +
                $"**Всего смертей:** {dbUser.Statistics.DeathCount}\n\n" +
                $"**Время жизни:** {created:dd} дней | с {new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.Statistics.CreatedAt):f}\n\n" +
                $"**Аккаунт создан:** {new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.Statistics.LiveTime):f} | {live:dd} дней назад")
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
        public async Task<SocketMessageComponent> MapPartAsync(SocketInteraction interaction, IUserMessage message, IDatabaseUser dbUser, IUser discordUser)
        {
            MapRegion region = _map.GetRegion(dbUser.Location.MapRegion);
            Embed embed = new UserEmbedBuilder(discordUser)
                .WithAuthor("Место нахождения")
                .WithDescription($"**Регион:** {region.Name}\n**Локация:** {region.GetLocation(dbUser.Location.MapLocation).Name}")
                .WithImageUrl(region.PictureURL)
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