using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
using MongoDB.Bson;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.Items;
using Shuvi.StaticServices.AdminCheck;

namespace Shuvi.Modules.SlashCommands
{
    public class InfoCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;

        public InfoCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }

        [SlashCommand("info", "Информаиця о боте.")]
        public async Task InfoCommandAsync()
        {
            await DeferAsync();
            var param = new InteractionParameters(await GetOriginalResponseAsync(), null);
            var embed = new BotEmbedBuilder()
                .WithDescription($"Шард №{Context.Client.GetShardFor(Context.Guild).ShardId}\nСерверов: {Context.Client.Guilds.Count}\n")
                .Build();
            MessageComponent components;
            if (AdminCheckManager.IsAdmin(Context.User.Id))
                components = new ComponentBuilder()
                .WithButton("Предметы", "AllItems", ButtonStyle.Primary)
                .Build();
            else
                components = new ComponentBuilder()
                    .Build();

            await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
            if (param.Interaction != null)
                await param.Interaction.DeferAsync();
            param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
            if (param.Interaction == null)
            {
                if (AdminCheckManager.IsAdmin(Context.User.Id))
                    await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
                return;
            }
            switch (param.Interaction.Data.CustomId)
            {
                case "AllItems":
                    await ViewAllItemsAsync(param);
                    break;
            }
        }

        public async Task ViewAllItemsAsync(InteractionParameters param)
        {
            int maxPage = AllItemsData.GetTotalEmbeds();
            int pageNow = 0;
            while (param.Interaction != null)
            {
                var embed = AllItemsData.GetItemsEmbed(pageNow);
                var components = new ComponentBuilder()
                        .WithButton("<", "<", ButtonStyle.Primary, disabled: pageNow <= 0)
                        .WithButton(">", ">", ButtonStyle.Primary, disabled: pageNow >= maxPage - 1)
                        .WithSelectMenu("choose", AllItemsData.GetItemsSelectMenu(pageNow), "Выберите предмет для просмотра")
                        .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (param.Interaction != null)
                    await param.Interaction.DeferAsync();
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
                    case "choose":
                        await ItemFactory.CreateItem(new ObjectId(param.Interaction.Data.Values.First()), 1)
                            .ViewItemAsync(_client, param, Context.User);
                        break;
                    case ">":
                        pageNow++;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}