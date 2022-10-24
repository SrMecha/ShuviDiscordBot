using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ShuviBot.Services;
using ShuviBot.Extensions.User;
using ShuviBot.Extensions.Interactions;
using MongoDB.Bson;

namespace ShardedClient.Modules
{
    public class InventoryCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManager _database;
        private readonly DiscordShardedClient _client;

        public InventoryCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManager>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }

        [SlashCommand("inventory", "Посмотреть инвентарь")]
        public async Task InventoryCommandAsync()
        {
            await RespondAsync("** **");
            IUserMessage botMessage = await GetOriginalResponseAsync();
            User dbUser = await _database.Users.GetUser(Context.User.Id);
            await ViewAllItemsAsync(_client, dbUser, botMessage);
        }

        public static async Task ViewAllItemsAsync(DiscordShardedClient client, User dbUser, IUserMessage botMessage, SocketInteraction? originalInteraction = null)
        {
            int maxPage = dbUser.Inventory.GetTotalEmbeds();
            int pageNow = 0;
            SocketMessageComponent? interaction = null;
            if (originalInteraction != null) interaction = (SocketMessageComponent)originalInteraction;
            do
            {
                await botMessage.ModifyAsync(msg =>
                {
                    msg.Content = "";
                    msg.Embed = dbUser.Inventory.GetItemsEmbed(pageNow);
                    msg.Components = new ComponentBuilder()
                        .WithButton("<", "<", ButtonStyle.Primary, disabled: pageNow <= 0)
                        .WithButton("Выйти", "exit", ButtonStyle.Danger)
                        .WithButton(">", ">", ButtonStyle.Primary, disabled: pageNow >= maxPage - 1)
                        .WithSelectMenu("choose", dbUser.Inventory.GetItemsSelectMenu(pageNow), 
                        "Выберите предмет для просмотра", disabled: dbUser.Inventory.Count == 0)
                        .Build();
                });
                if (interaction != null) await interaction.DeferAsync();
                interaction = await WaitFor.UserButtonInteraction(client, botMessage, dbUser.Id);
                switch (interaction.Data.CustomId)
                {
                    case "<":
                        pageNow--;
                        break;
                    case "exit":
                        await botMessage.DeleteAsync();
                        interaction = null;
                        break;
                    case "choose":
                        interaction = await ViewItemAsync(client, interaction, botMessage, dbUser, new ObjectId(interaction.Data.Values.First()));
                        break;
                    case ">":
                        pageNow++;
                        break;
                    default:
                        break;
                }
            } while (interaction != null);
        }

        public static async Task<SocketMessageComponent> ViewItemAsync(DiscordShardedClient client, SocketInteraction originalInteraction, IUserMessage message, User dbUser, ObjectId itemId)
        {
            await message.ModifyAsync(msg =>
            {
                msg.Embed = dbUser.Inventory.GetItemEmbed(itemId);
                msg.Components = new ComponentBuilder()
                    .WithButton("Назад", "back", ButtonStyle.Danger)
                    .Build();
            });
            await originalInteraction.DeferAsync();
            return await WaitFor.UserButtonInteraction(client, message, originalInteraction.User.Id);
        }
    }
}