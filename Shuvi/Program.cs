using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ShuviBot.Services;

namespace ShuviBot
{
    class Program
    {
        static void Main()
            => new Program()
                .MainAsync()
                .GetAwaiter()
                .GetResult();

        public async Task MainAsync()
        {
            string? botToken = Environment.GetEnvironmentVariable("BotToken", EnvironmentVariableTarget.User);
            string? mongoKey = Environment.GetEnvironmentVariable("MongoKey", EnvironmentVariableTarget.User);

            if (botToken == null || mongoKey == null)
            {
                Console.WriteLine("Переменные среды не установлены. (MongoKey or BotToken)");
                return;
            }
            var config = new DiscordSocketConfig
            {
                TotalShards = 1
            };

            ServiceProvider services = BuildServices(config, mongoKey);
            DiscordShardedClient client = services.GetRequiredService<DiscordShardedClient>();

            await services.GetRequiredService<InteractionHandlingService>()
                .InitializeAsync();

            await client.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private ServiceProvider BuildServices(DiscordSocketConfig config, string mongoKey) 
        { 
            return new ServiceCollection()
                .AddSingleton(new DiscordShardedClient(config))
                .AddSingleton<CommandService>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordShardedClient>()))
                .AddSingleton<InteractionHandlingService>()
                .AddSingleton(new DatabaseManager(mongoKey))
                .BuildServiceProvider();
        } 
    }
}