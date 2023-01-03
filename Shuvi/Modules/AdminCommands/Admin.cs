using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Services;
using Shuvi.StaticServices.AdminCheck;

namespace Shuvi.Modules.AdminCommands
{
    public class AdminCommandsModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;

        public AdminCommandsModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }
        [Command("Admins", true)]
        public async Task AdminsCommandAsync()
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            var embed = new BotEmbedBuilder()
                .WithDescription(string.Join("\n", AdminCheckManager.Data.AdminIds))
                .Build();
            await ReplyAsync(embed: embed);
        }
        [Command("AddAdmin", true)]
        public async Task AddAdminCommandAsync([Summary("user")] IUser paramUser)
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            Embed embed;
            if (AdminCheckManager.AddAdmin(paramUser.Id))
            {
                embed = new BotEmbedBuilder()
                    .WithDescription($"Вы успешно добавили {paramUser.Username} в ряды админов.")
                    .WithColor(Color.Green)
                    .Build();
                await _database.Info.SetAdmins();
            }
            else
                embed = new BotEmbedBuilder()
                    .WithDescription($"Не получилось добавить {paramUser.Username} в ряды админов.")
                    .WithColor(Color.Red)
                    .Build();
            await ReplyAsync(embed: embed);
        }
        [Command("RemoveAdmin", true)]
        public async Task RemoveAdminCommandAsync([Summary("user")] IUser paramUser)
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            Embed embed;
            if (AdminCheckManager.RemoveAdmin(paramUser.Id))
            {
                embed = new BotEmbedBuilder()
                    .WithDescription($"Вы успешно убрали {paramUser.Username} из рядов админов.")
                    .WithColor(Color.Green)
                    .Build();
                await _database.Info.SetAdmins();
            }
            else
                embed = new BotEmbedBuilder()
                    .WithDescription($"Не получилось убрать {paramUser.Username} из рядов админов.")
                    .WithColor(Color.Red)
                    .Build();
            await ReplyAsync(embed: embed);
        }
    }
}
