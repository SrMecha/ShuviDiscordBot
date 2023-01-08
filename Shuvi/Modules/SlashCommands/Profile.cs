using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
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
using MongoDB.Driver;
using Shuvi.Classes.User;

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
            await DeferAsync();
            var param = new InteractionParameters(await GetOriginalResponseAsync(), null);
            var discordUser = paramUser ?? Context.User;
            var dbUser = await _database.Users.GetUser(discordUser.Id);
            Embed embed;
            MessageComponent components;
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
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (param.Interaction != null)
                    await param.Interaction.DeferAsync();
                param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
                if (param.Interaction == null)
                {
                    await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
                    return;
                }
                switch (param.Interaction.Data.CustomId)
                {
                    case "equipment":
                        await EquipmnetPartAsync(param, dbUser, discordUser);
                        break;
                    case "location":
                         await MapPartAsync(param, dbUser, discordUser);
                        break;
                    case "statistics":
                        await StatisticsPartAsync(param, dbUser, discordUser);
                        break;
                    case "inventory":
                        await InventoryPartAsync(param, dbUser);
                        break;
                    case "upgrade":
                        await UpgradePartAsync(param, dbUser);
                        break;
                    default:
                        break;
                }
            } while (param.Interaction != null);
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
            return;
        }
        public async Task UpgradePartAsync(InteractionParameters param, IDatabaseUser dbUser)
        {
            var upgradePoints = dbUser.UpgradePoints.GetPoints();
            var charactersticToAdd = new List<int>() { 0, 0, 0, 0, 0, 0, 0 };
            var cursor = 0;
            while (true)
            {
                var embed = new UserEmbedBuilder(Context.User)
                   .WithAuthor("Улучшение")
                   .WithDescription($"Осталось очков прокачки: {upgradePoints}\n\n" +
                   $"{UpgradeCommandModule.SetCursor(cursor, 0)}**Сила:** {dbUser.Characteristic.Strength} " +
                   $"{(charactersticToAdd[0] != 0 ? $"| +{charactersticToAdd[0]}" : string.Empty)}\n" +
                   $"{UpgradeCommandModule.SetCursor(cursor, 1)}**Ловкость:** {dbUser.Characteristic.Agility} " +
                   $"{(charactersticToAdd[1] != 0 ? $"| +{charactersticToAdd[1]}" : string.Empty)}\n" +
                   $"{UpgradeCommandModule.SetCursor(cursor, 2)}**Удача:** {dbUser.Characteristic.Luck} " +
                   $"{(charactersticToAdd[2] != 0 ? $"| +{charactersticToAdd[2]}" : string.Empty)}\n" +
                   $"{UpgradeCommandModule.SetCursor(cursor, 3)}**Интеллект:** {dbUser.Characteristic.Intellect} " +
                   $"{(charactersticToAdd[3] != 0 ? $"| +{charactersticToAdd[3]}" : string.Empty)}\n" +
                   $"{UpgradeCommandModule.SetCursor(cursor, 4)}**Выносливость:** {dbUser.Characteristic.Endurance} " +
                   $"{(charactersticToAdd[4] != 0 ? $"| +{charactersticToAdd[4]}" : string.Empty)}\n" +
                   $"{UpgradeCommandModule.SetCursor(cursor, 5)}**Мана:** {dbUser.Mana.Max} " +
                   $"{(charactersticToAdd[5] != 0 ? $"| +{charactersticToAdd[5]}" : string.Empty)}\n" +
                   $"{UpgradeCommandModule.SetCursor(cursor, 6)}**Жизни:** {dbUser.Health.Max} " +
                   $"{(charactersticToAdd[6] != 0 ? $"| +{charactersticToAdd[6]}" : string.Empty)}\n")
                   .Build();
                var components = new ComponentBuilder()
                    .WithSelectMenu("choose", UpgradeCommandModule.GetUpgradeSelectMenu(), "Выберите параметр.")
                    .WithButton("+1", "+1", ButtonStyle.Success, disabled: upgradePoints < 1, row: 1)
                    .WithButton("+2", "+2", ButtonStyle.Success, disabled: upgradePoints < 2, row: 1)
                    .WithButton("+5", "+5", ButtonStyle.Success, disabled: upgradePoints < 5, row: 1)
                    .WithButton("Отмена", "back", ButtonStyle.Danger, row: 2)
                    .WithButton("Сбросить", "clear", ButtonStyle.Secondary, row: 2)
                    .WithButton("Принять", "apply", ButtonStyle.Success, row: 2)
                    .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (param.Interaction != null)
                    await param.Interaction.DeferAsync();
                param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
                if (param.Interaction == null)
                {
                    await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
                    return;
                }
                switch (param.Interaction.Data.CustomId)
                {
                    case "choose":
                        cursor = int.Parse(param.Interaction.Data.Values.First());
                        break;
                    case "-5" or "-2" or "-1" or "+1" or "+2" or "+5":
                        var amount = int.Parse(param.Interaction.Data.CustomId);
                        charactersticToAdd = UpgradeCommandModule.UpdateCharacteristic(charactersticToAdd, cursor, amount);
                        upgradePoints -= amount;
                        break;
                    case "back":
                        return;
                    case "clear":
                        upgradePoints = dbUser.UpgradePoints.GetPoints();
                        charactersticToAdd = new List<int>() { 0, 0, 0, 0, 0, 0, 0 };
                        break;
                    case "apply":
                        dbUser.Characteristic.Add(new UserCharacteristics(
                            charactersticToAdd[0],
                            charactersticToAdd[1],
                            charactersticToAdd[2],
                            charactersticToAdd[3],
                            charactersticToAdd[4]
                            ));
                        dbUser.Mana.SetMaxMana(dbUser.Mana.Max + charactersticToAdd[5]);
                        dbUser.Health.SetMaxHealth(dbUser.Health.Max + charactersticToAdd[6]);
                        await _database.Users.UpdateUser(
                            Context.User.Id,
                            new UpdateDefinitionBuilder<UserData>()
                            .Inc("Strength", charactersticToAdd[0])
                            .Inc("Agility", charactersticToAdd[1])
                            .Inc("Luck", charactersticToAdd[2])
                            .Inc("Intellect", charactersticToAdd[3])
                            .Inc("Endurance", charactersticToAdd[4])
                            .Inc("MaxMana", charactersticToAdd[5])
                            .Inc("MaxHealth", charactersticToAdd[6])
                            );
                        return;
                    default:
                        return;
                }
            }
        }
        public async Task InventoryPartAsync(InteractionParameters param, IDatabaseUser dbUser)
        {
            int maxPage = dbUser.Inventory.GetTotalEmbeds();
            int pageNow = 0;
            do
            {
                var embed = dbUser.Inventory.GetItemsEmbed(pageNow).ToEmbedBuilder()
                    .WithAuthor($"Инвентарь")
                    .WithFooter($"{Context.User.Username} | {Context.User.Id}", Context.User.GetAvatarUrl())
                    .WithColor(UserEmbedBuilder.StandartColor)
                    .Build();
                var components = new ComponentBuilder()
                    .WithButton("<", "<", ButtonStyle.Primary, disabled: pageNow <= 0)
                    .WithButton("Выйти", "exit", ButtonStyle.Danger)
                    .WithButton(">", ">", ButtonStyle.Primary, disabled: pageNow >= maxPage - 1)
                    .WithSelectMenu("choose", dbUser.Inventory.GetItemsSelectMenu(pageNow),
                    "Выберите предмет для просмотра", disabled: dbUser.Inventory.Count == 0)
                    .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (param.Interaction != null)
                    await param.Interaction.DeferAsync();
                param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
                if (param.Interaction == null)
                {
                    await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
                    return;
                }
                switch (param.Interaction.Data.CustomId)
                {
                    case "<":
                        pageNow--;
                        break;
                    case "exit":
                        return;
                    case "choose":
                        await ItemPartAsync(param, dbUser, new ObjectId(param.Interaction.Data.Values.First()));
                        break;
                    case ">":
                        pageNow++;
                        break;
                    default:
                        break;
                }
            } while (param.Interaction != null);
            return;
        }
        public async Task ItemPartAsync(InteractionParameters param, IDatabaseUser dbUser, ObjectId itemId)
        {
            var embed = dbUser.Inventory.GetItemEmbed(itemId).ToEmbedBuilder()
                .WithAuthor($"Просмотр предмета")
                .WithFooter($"{Context.User.Username} | {Context.User.Id}", Context.User.GetAvatarUrl())
                .WithColor(UserEmbedBuilder.StandartColor)
                .Build();
            var components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
            if (param.Interaction != null)
                await param.Interaction.DeferAsync();
            param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
        }
        public async Task EquipmnetPartAsync(InteractionParameters param, IDatabaseUser dbUser, IUser discordUser)
        {
            EquipmentItem? helmet = dbUser.Equipment.Head == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)dbUser.Equipment.Head, 0);
            EquipmentItem? armor = dbUser.Equipment.Body == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)dbUser.Equipment.Body, 0);
            EquipmentItem? leggings = dbUser.Equipment.Legs == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)dbUser.Equipment.Legs, 0);
            EquipmentItem? boots = dbUser.Equipment.Foots == null ? null : (EquipmentItem?)ItemFactory.CreateItem((ObjectId)dbUser.Equipment.Foots, 0);
            var embed = new UserEmbedBuilder(discordUser)
                .WithAuthor($"Экипировка")
                .AddField($"Шлем: {(helmet == null ? "Нету" : helmet.Name)}", $"{(helmet == null ? "** **" : helmet.GetBonusesInfo())}", true)
                .AddField($"Броня: {(armor == null ? "Нету" : armor.Name)}", $"{(armor == null ? "** **" : armor.GetBonusesInfo())}", true)
                .AddField("** **", "** **", false)
                .AddField($"Поножи: {(leggings == null ? "Нету" : leggings.Name)}", $"{(leggings == null ? "** **" : leggings.GetBonusesInfo())}", true)
                .AddField($"Ботинки: {(boots == null ? "Нету" : boots.Name)}", $"{(boots == null ? "** **" : boots.GetBonusesInfo())}", true)
                .Build();
            var components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
            if (param.Interaction != null)
                await param.Interaction.DeferAsync();
            param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
        }
        public async Task StatisticsPartAsync(InteractionParameters param, IDatabaseUser dbUser, IUser discordUser)
        {
            var created = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.Statistics.CreatedAt);
            var live = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dbUser.Statistics.LiveTime);
            var embed = new UserEmbedBuilder(discordUser)
                .WithAuthor("Статистика")
                .WithDescription($"**Максимальный рейтинг:** {dbUser.Statistics.MaxRating}\n" +
                $"**Всего врагов убито:** {dbUser.Statistics.EnemyKilled}\n" +
                $"**Всего подземелий пройдено:** {dbUser.Statistics.DungeonComplite}\n" +
                $"**Всего смертей:** {dbUser.Statistics.DeathCount}\n\n" +
                $"**Последняя смерть:** <t:{dbUser.Statistics.LiveTime}:R>\n\n" +
                $"**Аккаунт создан:** <t:{dbUser.Statistics.CreatedAt}:R>")
                .Build();
            var components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
            if (param.Interaction != null)
                await param.Interaction.DeferAsync();
            param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
        }
        public async Task MapPartAsync(InteractionParameters param, IDatabaseUser dbUser, IUser discordUser)
        {
            var region = _map.GetRegion(dbUser.Location.MapRegion);
            var embed = new UserEmbedBuilder(discordUser)
                .WithAuthor("Место нахождения")
                .WithDescription($"**Регион:** {region.Name}\n**Локация:** {region.GetLocation(dbUser.Location.MapLocation).Name}")
                .WithImageUrl(region.PictureURL)
                .Build();
            var components = new ComponentBuilder()
                .WithButton("Назад", "back", ButtonStyle.Danger)
                .Build();
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
            if (param.Interaction != null)
                await param.Interaction.DeferAsync();
            param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
        }
    }
}