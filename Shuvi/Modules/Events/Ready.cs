using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;

namespace ShardedClient.Modules
{
    public class ReadyEventModule: InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DiscordShardedClient _client;

        public ReadyEventModule(IServiceProvider provider)
        {
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _client.ShardReady += ShardReadyAsync;
        }

        private Task ShardReadyAsync(DiscordSocketClient shard)
        {
            Console.WriteLine($"Шард №{shard.ShardId} Подключён.");
            return Task.CompletedTask;
        }


    }
}