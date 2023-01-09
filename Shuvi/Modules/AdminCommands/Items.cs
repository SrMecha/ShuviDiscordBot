using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Items;
using Shuvi.Classes.User;
using Shuvi.Services;
using Shuvi.StaticServices.AdminCheck;

namespace Shuvi.Modules.AdminCommands
{
    public class ItemsCommandsModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;

        public ItemsCommandsModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }
        [Command("GiveItem", true)]
        public async Task GiveItemCommandAsync(
                [Summary("user")] IUser paramUser,
                [Summary("id")] string id,
                [Summary("amount")] int amount
                )
        {
            if (!AdminCheckManager.IsAdmin(Context.User.Id))
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("У вас недостаточно прав."));
                return;
            }
            var dbUser = await _database.Users.GetUser(Context.User.Id);
            if (AllItemsData.GetItemData(new ObjectId(id)).Id == ObjectId.Empty)
            {
                await ReplyAsync(embed: ErrorEmbedBuilder.Simple("Предмета не найдено."));
                return;
            }
            dbUser.Inventory.AddItem(new ObjectId(id), amount);
            await _database.Users.UpdateUser(
                paramUser.Id,
                new UpdateDefinitionBuilder<UserData>().Set("Inventory", dbUser.Inventory.GetInvetoryCache()));
            var embed = new BotEmbedBuilder()
                .WithDescription($"Успешно выдали предмет пользователю {paramUser.Username}.")
                .Build();
            await ReplyAsync(embed: embed);
        }

    }
}
