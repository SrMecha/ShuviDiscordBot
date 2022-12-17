using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shuvi.Services;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.Map;
using Shuvi.Interfaces.User;
using Shuvi.Extensions;

namespace Shuvi.Modules.SlashCommands
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
            IDatabaseUser dbUser = await _database.Users.GetUser(Context.User.Id);
            IUser discordUser = Context.User;
            await ViewAllMapAsync(dbUser, discordUser);
        }

        public async Task ViewAllMapAsync(IDatabaseUser dbUser, IUser discordUser)
        {
            Embed embed;
            MessageComponent components;
            IUserMessage? message = null;
            SocketMessageComponent? interaction = null;
            var currentRegion = _map.GetRegion(dbUser.Location.MapRegion);
            var currentLocation = currentRegion.GetLocation(dbUser.Location.MapLocation);
            do
            {
                embed = new UserEmbedBuilder(discordUser)
                    .WithAuthor("Карта Дисборда")
                    .WithDescription($"**Ваше местоположение:** {currentRegion.Name}\n**Локация:** {currentLocation.Name}")
                    .WithImageUrl(_map.Settings.PictureURL)
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
            embed = new UserEmbedBuilder(discordUser)
                   .WithAuthor(region.Name)
                   .WithDescription($"Открывается с ранга {region.NeededRank.ToRusString()}\n**Описание:**\n{region.Description}\n\n" +
                   $"**Локации:**\n```{string.Join("\n", region.Locations.Select(location => location.Name))}```")
                   .WithImageUrl(region.PictureURL)
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