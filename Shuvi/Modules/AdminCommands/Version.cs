using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.User;
using Shuvi.Services;
using Shuvi.StaticServices.AdminCheck;

namespace Shuvi.Modules.AdminCommands
{
    public class VersionCommandsModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;

        public VersionCommandsModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }
        [Command("SetVersion", true)]
        public async Task SetVersionCommandAsync(
                [Summary("version")] string version
                )
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            await _database.Info.SetVersion(version);
            var embed = new BotEmbedBuilder()
                .WithDescription($"Версия успешно установлена на {version}.")
                .Build();
            await ReplyAsync(embed: embed);
        }
    }
}
