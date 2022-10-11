using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ShuviBot.Services;
using ShuviBot.Extensions.Interactions;
using MongoDB.Bson;

namespace ShardedClient.Modules
{
    public class InfoCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManager _database;
        private readonly DiscordShardedClient _client;

        public InfoCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManager>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }

        [SlashCommand("info", "Информаиця о боте.")]
        public async Task InfoCommandAsync()
        {
            string msg = $"Шард №{Context.Client.GetShardFor(Context.Guild).ShardId}\n" +
                $"Серверов: {Context.Client.Guilds.Count}\n";
            ComponentBuilder components = new ComponentBuilder()
                .WithButton("Предметы", "AllItems", ButtonStyle.Primary);

            await RespondAsync(msg, components: components.Build());
            IUserMessage botMessage = await GetOriginalResponseAsync();
            SocketMessageComponent interaction = await WaitFor.UserButtonInteraction(_client, botMessage, Context.User.Id);
            if (interaction == null) return;
            switch (interaction.Data.CustomId)
            {
                case "AllItems": 
                    await ViewAllItemsAsync(interaction, botMessage);
                    break;
            }
        }

        public async Task ViewAllItemsAsync(SocketInteraction originalInteraction, IUserMessage message)
        {
            int maxPage = _database.AllItemsData.GetTotalEmbeds();
            int pageNow = 0;
            SocketMessageComponent interaction = (SocketMessageComponent)originalInteraction;
            while (interaction != null)
            {
                await message.ModifyAsync(msg =>
                {
                    msg.Embed = _database.AllItemsData.GetItemsEmbed(pageNow);
                    msg.Components = new ComponentBuilder()
                        .WithButton("<", "<", ButtonStyle.Primary, disabled: pageNow <= 0)
                        .WithButton(">", ">", ButtonStyle.Primary, disabled: pageNow >= maxPage-1)
                        .WithSelectMenu("choose", _database.AllItemsData.GetItemsSelectMenu(pageNow), "Выберите предмет для просмотра")
                        .Build();
                 });
                await interaction.DeferAsync();
                interaction = await WaitFor.UserButtonInteraction(_client, message, interaction.User.Id);
                switch (interaction.Data.CustomId)
                {
                    case "<":
                        pageNow--;
                        break;
                    case "choose":
                        interaction = await ViewItemAsync(interaction, message, new ObjectId(interaction.Data.Values.First()));
                        break;
                    case ">":
                        pageNow++;
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task<SocketMessageComponent> ViewItemAsync(SocketInteraction originalInteraction, IUserMessage message, ObjectId itemId)
        {
            await message.ModifyAsync(msg =>
            {
                msg.Embed = _database.AllItemsData.GetItemEmbed(itemId);
                msg.Components = new ComponentBuilder()
                    .WithButton("Назад", "back", ButtonStyle.Danger)
                    .Build();
            });
            await originalInteraction.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, originalInteraction.User.Id);
        }
    }
}