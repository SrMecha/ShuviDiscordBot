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
            Embed embed = new EmbedBuilder()
                .WithAuthor(discordUser.Username, discordUser.GetAvatarUrl())
                .WithDescription($"**Ранг:** {dbUser.Rank.ToRusString()}\n**Рейтинг:** {dbUser.Rating}\n**Раса:** {dbUser.Race.ToRusString()}\n" +
                $"**Профессия:** {dbUser.Profession.ToRusString()}")
                .WithFooter($"ID: {discordUser.Id}")
                .WithColor(dbUser.Rank.GetColor())
            .Build();
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

            await RespondAsync(embed: embed, components: components);
            IUserMessage botMessage = await GetOriginalResponseAsync();
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
                    await InventoryCommandModule.ViewAllItemsAsync(_client, dbUser, botMessage, interaction);
                    break;
                case "upgrade":
                    break;
                default:
                    break;
            }
        }
    }
}