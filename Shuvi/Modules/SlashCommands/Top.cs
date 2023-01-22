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
using Shuvi.StaticServices.UserTop;
using Shuvi.Extensions;

namespace Shuvi.Modules.SlashCommands
{
    public class TopCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;

        public TopCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
        }

        [SlashCommand("top", "Посмотреть топ по рейтингу.")]
        public async Task TopCommandAsync()
        {
            await DeferAsync();
            var message = await GetOriginalResponseAsync();
            var param = new InteractionParameters(message, message.Interaction as SocketMessageComponent);
            await ViewTopAsync(param);
        }
        public async Task ViewTopAsync(InteractionParameters param)
        {
            int maxPage = UserTopManager.GetTotalPages();
            int pageNow = 0;
            Embed embed;
            MessageComponent components;
            do
            {
                embed = UserTopManager.GetEmbed(pageNow);
                components = new ComponentBuilder()
                        .WithButton("<", "<", ButtonStyle.Primary, disabled: pageNow <= 0)
                        .WithButton("Выйти", "exit", ButtonStyle.Danger)
                        .WithButton(">", ">", ButtonStyle.Primary, disabled: pageNow >= maxPage - 1)
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
                    case ">":
                        pageNow++;
                        break;
                    default:
                        break;
                }
            } while (param.Interaction != null);
        }
    }
}
