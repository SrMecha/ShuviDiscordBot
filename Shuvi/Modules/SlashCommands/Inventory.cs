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

        [SlashCommand("inventory", "���������� ���������")]
        public async Task InventoryCommandAsync()
        {
            User dbUser = await _database.Users.GetUser(Context.User.Id);
            await ViewAllItemsAsync(dbUser);
        }

        public async Task ViewAllItemsAsync(User dbUser)
        {
            int maxPage = dbUser.Inventory.GetTotalEmbeds();
            int pageNow = 0;
            Embed embed;
            MessageComponent components;
            IUserMessage? message = null;
            SocketMessageComponent? interaction = null;
            do
            {
                embed = dbUser.Inventory.GetItemsEmbed(pageNow);
                components = new ComponentBuilder()
                        .WithButton("<", "<", ButtonStyle.Primary, disabled: pageNow <= 0)
                        .WithButton("�����", "exit", ButtonStyle.Danger)
                        .WithButton(">", ">", ButtonStyle.Primary, disabled: pageNow >= maxPage - 1)
                        .WithSelectMenu("choose", dbUser.Inventory.GetItemsSelectMenu(pageNow),
                        "�������� ������� ��� ���������", disabled: dbUser.Inventory.Count == 0)
                        .Build();
                if (message == null)
                {
                    await RespondAsync(embed: embed, components: components);
                    message = await GetOriginalResponseAsync();
                }
                else
                {
                    await message.ModifyAsync(msg => { msg.Embed = embed; msg.Components = components; });
                }
                if (interaction != null) 
                    await interaction.DeferAsync();
                interaction = await WaitFor.UserButtonInteraction(_client, message, dbUser.Id);
                switch (interaction.Data.CustomId)
                {
                    case "<":
                        pageNow--;
                        break;
                    case "exit":
                        await message.DeleteAsync();
                        return;
                    case "choose":
                        interaction = await ViewItemAsync(interaction, message, dbUser, new ObjectId(interaction.Data.Values.First()));
                        break;
                    case ">":
                        pageNow++;
                        break;
                    default:
                        break;
                }
            } while (interaction != null);
        }

        public async Task<SocketMessageComponent> ViewItemAsync(SocketInteraction interaction, IUserMessage message, User dbUser, ObjectId itemId)
        {
            await message.ModifyAsync(msg =>
            {
                msg.Embed = dbUser.Inventory.GetItemEmbed(itemId);
                msg.Components = new ComponentBuilder()
                    .WithButton("�����", "back", ButtonStyle.Danger)
                    .Build();
            });
            await interaction.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, interaction.User.Id);
        }
    }
}