using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
using MongoDB.Bson;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.Items;
using Shuvi.Interfaces.User;
using Shuvi.Enums;
using Shuvi.StaticServices.UserCheck;
using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Player;
using Shuvi.Classes.CustomEmoji;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using Shuvi.Classes.User;

namespace Shuvi.Modules.SlashCommands
{
    public class UpgradeCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;

        public UpgradeCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }

        [SlashCommand("upgrade", "Повысить свои характеристики.")]
        public async Task UpgradeCommandAsync()
        {
            await DeferAsync();
            var param = new InteractionParameters(await GetOriginalResponseAsync(), null);
            if (UserCommandsCheck.IsUseCommands(Context.User.Id, ActiveCommands.Upgrade))
            {
                await ModifyOriginalResponseAsync(
                    msg => { msg.Embed = UserCommandsCheck.GetErrorEmbed(Context.User.Id, ActiveCommands.Upgrade); }
                    );
                return;
            }
            UserCommandsCheck.Add(Context.User.Id, ActiveCommands.Upgrade);
            var dbUser = await _database.Users.GetUser(Context.User.Id);
            await MainUpgradeCommandAsync(param, dbUser);
            UserCommandsCheck.Remove(Context.User.Id, ActiveCommands.Upgrade);
        }

        public async Task MainUpgradeCommandAsync(InteractionParameters param, IDatabaseUser dbUser)
        {
            var upgradePoints = dbUser.UpgradePoints.GetPoints();
            var charactersticToAdd = new List<int>() { 0, 0, 0, 0, 0, 0, 0 };
            var cursor = 0;
            while (true)
            {
                var embed = new UserEmbedBuilder(Context.User)
                   .WithAuthor("Улучшение")
                   .WithDescription($"Осталось очков прокачки: {upgradePoints}\n\n" +
                   $"{SetCursor(cursor, 0)}**Сила:** {dbUser.Characteristic.Strength} " +
                   $"{(charactersticToAdd[0] != 0 ? $"| +{charactersticToAdd[0]}" : string.Empty)}\n" +
                   $"{SetCursor(cursor, 1)}**Ловкость:** {dbUser.Characteristic.Agility} " +
                   $"{(charactersticToAdd[1] != 0 ? $"| +{charactersticToAdd[1]}" : string.Empty)}\n" +
                   $"{SetCursor(cursor, 2)}**Удача:** {dbUser.Characteristic.Luck} " +
                   $"{(charactersticToAdd[2] != 0 ? $"| +{charactersticToAdd[2]}" : string.Empty)}\n" +
                   $"{SetCursor(cursor, 3)}**Интеллект:** {dbUser.Characteristic.Intellect} " +
                   $"{(charactersticToAdd[3] != 0 ? $"| +{charactersticToAdd[3]}" : string.Empty)}\n" +
                   $"{SetCursor(cursor, 4)}**Выносливость:** {dbUser.Characteristic.Endurance} " +
                   $"{(charactersticToAdd[4] != 0 ? $"| +{charactersticToAdd[4]}" : string.Empty)}\n" +
                   $"{SetCursor(cursor, 5)}**Мана:** {dbUser.Mana.Max} " +
                   $"{(charactersticToAdd[5] != 0 ? $"| +{charactersticToAdd[5]}" : string.Empty)}\n" +
                   $"{SetCursor(cursor, 6)}**Жизни:** {dbUser.Health.Max} " +
                   $"{(charactersticToAdd[6] != 0 ? $"| +{charactersticToAdd[6]}" : string.Empty)}\n")
                   .Build();
                var components = new ComponentBuilder()
                    .WithSelectMenu("choose", GetUpgradeSelectMenu(), "Выберите параметр.")
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
                        charactersticToAdd = UpdateCharacteristic(charactersticToAdd, cursor, amount);
                        upgradePoints -= amount;
                        break;
                    case "back":
                        await DeleteOriginalResponseAsync();
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
                        dbUser.Energy.UpdateMaxEnergy(dbUser.Characteristic.Endurance);
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
                        await ModifyOriginalResponseAsync(
                            msg => { msg.Components = new ComponentBuilder().WithButton("Готово", "null", disabled: true).Build(); }
                        );
                        return;
                    default:
                        return;
                }
            }
        }

        public static string SetCursor(int now, int need)
        {
            return now == need ? EmojiList.Get("choosePoint").ToString()! : string.Empty;
        }
        public static List<SelectMenuOptionBuilder> GetUpgradeSelectMenu()
        {
            return new List<SelectMenuOptionBuilder>()
            {
                new SelectMenuOptionBuilder("Сила", "0"),
                new SelectMenuOptionBuilder("Ловкость", "1"),
                new SelectMenuOptionBuilder("Удача", "2"),
                new SelectMenuOptionBuilder("Интеллект", "3"),
                new SelectMenuOptionBuilder("Выносливость", "4"),
                new SelectMenuOptionBuilder("Мана", "5"),
                new SelectMenuOptionBuilder("Жизни", "6")
            };
        }
        public static List<int> UpdateCharacteristic(List<int> characteristic, int cursor, int amount)
        {
            var toUp = 1;
            if (cursor == 6)
                toUp = CharacteristicSettings.HealthPerUpPoint;
            if (cursor == 5)
                toUp = CharacteristicSettings.ManaPerUpPoint;
            characteristic[cursor] += amount * toUp;
            return characteristic;
        }
    }
}
