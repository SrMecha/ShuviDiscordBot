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
    public class RatingCommandsModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;

        public RatingCommandsModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }
        [Command("AddRating", true)]
        public async Task AddRatingCommandAsync(
                [Summary("user")] IUser paramUser,
                [Summary("amount")] int amount
                )
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            var dbUser = await _database.Users.GetUser(Context.User.Id);
            dbUser.Rating.AddPoints(amount);
            await _database.Users.UpdateUser(
                paramUser.Id, 
                new UpdateDefinitionBuilder<UserData>().Set("Rating", dbUser.Rating.Points));
            var embed = new BotEmbedBuilder()
                .WithDescription($"Успешно добавили {amount} райтинговых очков пользователю {paramUser.Username}.")
                .Build();
            await ReplyAsync(embed: embed);
        }
        [Command("RemoveRating", true)]
        public async Task RemoveRatingCommandAsync(
                [Summary("user")] IUser paramUser,
                [Summary("amount")] int amount
                )
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            var dbUser = await _database.Users.GetUser(Context.User.Id);
            dbUser.Rating.RemovePoints(amount);
            await _database.Users.UpdateUser(
                paramUser.Id,
                new UpdateDefinitionBuilder<UserData>().Set("Rating", dbUser.Rating.Points));
            var embed = new BotEmbedBuilder()
                .WithDescription($"Рейтинг пользователя {paramUser.Username} успешно понижен на {amount}.")
                .Build();
            await ReplyAsync(embed: embed);
        }
        [Command("SetRating", true)]
        public async Task SetRatingCommandAsync(
                [Summary("user")] IUser paramUser,
                [Summary("amount")] int amount
                )
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            var dbUser = await _database.Users.GetUser(Context.User.Id);
            dbUser.Rating.SetPoints(amount);
            await _database.Users.UpdateUser(
                paramUser.Id,
                new UpdateDefinitionBuilder<UserData>().Set("Rating", dbUser.Rating.Points));
            var embed = new BotEmbedBuilder()
                .WithDescription($"Рейтинг пользователя {paramUser.Username} успешно установлен на {amount}.")
                .Build();
            await ReplyAsync(embed: embed);
        }

    }
}
