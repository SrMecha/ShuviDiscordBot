using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
using MongoDB.Bson;
using Shuvi.Classes.Interactions;
using Shuvi.Interfaces.User;
using Shuvi.Extensions;

namespace Shuvi.Modules.SlashCommands
{
    public class InventoryCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;

        public InventoryCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }

        [SlashCommand("inventory", "Посмотреть инвентарь")]
        public async Task InventoryCommandAsync()
        {
            await DeferAsync();
            var param = new InteractionParameters(await GetOriginalResponseAsync(), null);
            IDatabaseUser dbUser = await _database.Users.GetUser(Context.User.Id);
            await ViewAllItemsAsync(param, dbUser);
        }

        public async Task ViewAllItemsAsync(InteractionParameters param, IDatabaseUser dbUser)
        {
            int maxPage = dbUser.Inventory.GetTotalEmbeds();
            int pageNow = 0;
            Embed embed;
            MessageComponent components;
            do
            {
                embed = dbUser.Inventory.GetItemsEmbed(pageNow);
                components = new ComponentBuilder()
                        .WithButton("<", "<", ButtonStyle.Primary, disabled: pageNow <= 0)
                        .WithButton("Выйти", "exit", ButtonStyle.Danger)
                        .WithButton(">", ">", ButtonStyle.Primary, disabled: pageNow >= maxPage - 1)
                        .WithSelectMenu("choose", dbUser.Inventory.GetItemsSelectMenu(pageNow),
                        "Выберите предмет для просмотра", disabled: dbUser.Inventory.Count == 0)
                        .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (param.Interaction != null)
                    await param.Interaction.TryDeferAsync();
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
                        await DeleteOriginalResponseAsync();
                        return;
                    case "choose":
                        await dbUser.Inventory.GetItem(new ObjectId(param.Interaction.Data.Values.First()))
                           .ViewItemAsync(_client, param, dbUser, Context.User);
                        break;
                    case ">":
                        pageNow++;
                        break;
                    default:
                        break;
                }
            } while (param.Interaction != null);
            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
            return;
        }
    }
}