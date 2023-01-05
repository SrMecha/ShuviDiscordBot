using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Services;
using Shuvi.StaticServices.AdminCheck;

namespace Shuvi.Modules.AdminCommands
{
    public class HelpCommandsModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;
        private readonly CommandService _commands;

        public HelpCommandsModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _commands = provider.GetRequiredService<CommandService>();
        }
        [Command("Help", true)]
        public async Task HelpCommandAsync()
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            var result = new List<string>();
            
            foreach (var command in _commands.Commands.ToList())
            {
                var commandParams = new List<string>();
                foreach (var param in command.Parameters)
                {
                    if (param.IsOptional)
                        commandParams.Add($"[{param.Name}]");
                    else
                        commandParams.Add($"<{param.Name}>");
                }
                result.Add($"**{command.Name}** {(commandParams.Count != 0 ? string.Join(" ", commandParams) : "")}");
            }
            var embed = new BotEmbedBuilder()
                .WithDescription(string.Join("\n", result))
                .Build();
            await ReplyAsync(embed: embed);
        }
    }
}
