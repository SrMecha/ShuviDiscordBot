using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ShuviBot.Services;
using ShuviBot.Extensions.User;
using ShuviBot.Extensions.Interactions;
using MongoDB.Bson;
using ShuviBot.Extensions.Map;
using ShuviBot.Enums.Ranks;

namespace ShardedClient.Modules
{
    public class MapCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManager _database;
        private readonly DiscordShardedClient _client;
        private readonly WorldMap _map;

        public MapCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManager>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _map = _database.Map;
        }

        [SlashCommand("map", "Посмотреть карту мира.")]
        public async Task MapCommandAsync()
        {
            User dbUser = await _database.Users.GetUser(Context.User.Id);
            IUser discordUser = Context.User;
            await ViewAllMapAsync(dbUser, discordUser);
        }

        public async Task ViewAllMapAsync(User dbUser, IUser discordUser)
        {
            Embed embed;
            MessageComponent components;
            IUserMessage? message = null;
            SocketMessageComponent? interaction = null;
            MapRegion currentRegion = _map.GetRegion(dbUser.MapRegion);
            do
            {
                embed = new EmbedBuilder()
                    .WithAuthor("Карта Дисборда")
                    .WithDescription($"Ваше местоположение: **{currentRegion.Name}**\n**Локация:** {currentRegion.GetLocation(dbUser.MapLocation).Name}")
                    .WithImageUrl(_map.Settings.PictureURL)
                    .WithFooter(discordUser.Username, discordUser.GetAvatarUrl())
                    .Build();
                components = new ComponentBuilder()
                    .WithSelectMenu("choose", _map.GetRegionsSelectMenu(),
                            "Выберите регион для просмотра")
                    .Build();
                if (message == null)
                {
                    await RespondAsync(embed: embed, components: components);
                    message = await GetOriginalResponseAsync();
                }
                else
                    await message.ModifyAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (interaction != null)
                    await interaction.DeferAsync();
                interaction = await WaitFor.UserButtonInteraction(_client, message, dbUser.Id);
                interaction = await ViewRegionAsync(interaction, message, discordUser, _map.GetRegion(int.Parse(interaction.Data.Values.First())));
            } while (interaction != null);
        }

        public async Task<SocketMessageComponent?> ViewRegionAsync(SocketMessageComponent? interaction, IUserMessage message, IUser discordUser, MapRegion region)
        {
            Embed embed;
            MessageComponent components;

            embed = new EmbedBuilder()
                   .WithAuthor(region.Name)
                   .WithDescription($"Открывается с ранга {region.NeededRank.ToRusString()}\n**Описание:**\n{region.Description}")
                   .WithImageUrl(region.PictureURL)
                   .WithFooter(discordUser.Username, discordUser.GetAvatarUrl())
                   .Build();
            components = new ComponentBuilder()
                .WithButton("Назад", "exit", ButtonStyle.Danger)
                .Build();
            await message.ModifyAsync(msg => { msg.Embed = embed; msg.Components = components; });
            await interaction!.DeferAsync();
            return await WaitFor.UserButtonInteraction(_client, message, discordUser.Id);
        }
    }
}