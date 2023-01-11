using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
using Shuvi.StaticServices.Logs;

namespace Shuvi.Modules.Events
{
    public class ReadyEventModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DiscordShardedClient _client;
        private readonly DatabaseManagerService _database;

        public ReadyEventModule(IServiceProvider provider)
        {
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client.ShardReady += ShardReadyAsync;
        }

        private Task ShardReadyAsync(DiscordSocketClient shard)
        {
            LoadLogs();
            Console.WriteLine($"Шард №{shard.ShardId} Подключён.");
            return Task.CompletedTask;
        }
        private void LoadLogs()
        {
            ServerLogs.Init(_database.Info.LoadLogs(), _client);
        }

    }
}