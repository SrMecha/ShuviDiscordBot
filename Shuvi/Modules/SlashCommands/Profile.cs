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
using Discord.Commands;

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
            Embed embed = new EmbedBuilder()
                .WithAuthor(discordUser.Username, discordUser.GetAvatarUrl())
                .WithDescription($"**Ранг:** {dbUser.Rank.ToRusString()}\n**Рейтинг:** {dbUser.Rating}\n**Раса:** {dbUser.Race.ToRusString()}\n" +
                $"**Профессия:** {dbUser.Profession.ToRusString()}")
                .WithFooter($"ID: {discordUser.Id}")
                .WithColor(dbUser.Rank.GetColor())
                .Build();

            await RespondAsync(embed: embed);
        }
    }
}