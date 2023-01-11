using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Classes.CustomEmoji;
using Shuvi.Services;
using Shuvi.StaticServices.Logs;

namespace Shuvi
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
            string? botToken = LoadEnviroment("BotToken");
            string? mongoKey = LoadEnviroment("MongoKey");

            if (botToken == null || mongoKey == null)
            {
                Console.WriteLine("Переменные среды не установлены. (MongoKey or BotToken)");
                return;
            }
            var config = new DiscordSocketConfig
            {
                TotalShards = 1,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            ServiceProvider services = BuildServices(config, mongoKey);
            DiscordShardedClient client = services.GetRequiredService<DiscordShardedClient>();

            await services.GetRequiredService<InteractionHandlingService>()
                .InitializeAsync();
            await services.GetRequiredService<CommandHandlingService>()
                .InitializeAsync();
            InitStaticClasses();

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
                .AddSingleton<CommandHandlingService>()
                .AddSingleton(new DatabaseManagerService(mongoKey))
                .BuildServiceProvider();
        } 

        private void InitStaticClasses()
        {
            EmojiList.Init();
        }
        private string? LoadEnviroment(string name) 
        {
            var result = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            if (result == null)
                result = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            else
                return result;
            if (result == null)
                result = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
            else
                return result;
            return result;
        }
    }
}