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
using Shuvi.Classes.CustomEmoji;
using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Classes.ActionChances;
using Shuvi.Classes.Enemy;
using Shuvi.Classes.Rates;

namespace Shuvi.Modules.SlashCommands
{
    public class MapCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;
        private readonly WorldMap _map;

        public MapCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _map = _database.Map;
        }

        [SlashCommand("map", "Посмотреть карту мира.")]
        public async Task MapCommandAsync()
        {
            await DeferAsync();
            var param = new InteractionParameters(await GetOriginalResponseAsync(), null);
            IDatabaseUser dbUser = await _database.Users.GetUser(Context.User.Id);
            await ViewAllMapAsync(param, dbUser);
        }

        public async Task ViewAllMapAsync(InteractionParameters param, IDatabaseUser dbUser)
        {
            Embed embed;
            MessageComponent components;
            var currentRegion = _map.GetRegion(dbUser.Location.MapRegion);
            var currentLocation = currentRegion.GetLocation(dbUser.Location.MapLocation);
            do
            {
                embed = new UserEmbedBuilder(Context.User)
                    .WithAuthor("Карта Дисборда")
                    .WithDescription($"**Текущий регион:** {currentRegion.Name}\n**Локация:** {currentLocation.Name}")
                    .WithImageUrl(_map.Settings.PictureURL)
                    .Build();
                components = new ComponentBuilder()
                    .WithSelectMenu("choose", _map.GetRegionsSelectMenu(),
                            "Выберите регион для просмотра")
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
                await ViewRegionAsync(param, _map.GetRegion(int.Parse(param.Interaction.Data.Values.First())));
            } while (param.Interaction != null);
        }
        public async Task ViewRegionAsync(InteractionParameters param, MapRegion region)
        {
            Embed embed;
            MessageComponent components;
            while (true)
            {
                embed = new UserEmbedBuilder(Context.User)
                       .WithAuthor($"Дисборд | {region.Name}")
                       .WithDescription($"{region.Description}\n\n**Требуемый ранг:** {region.NeededRank.ToRusString()}\n" +
                       $"**Рекомендуемый ранг:** {region.RecomendedRank.ToRusString()}\n\n" +
                       $"**Локации**\n{GetLocationsCuteString(region.Locations)}")
                       .WithImageUrl(region.PictureURL)
                       .Build();
                components = new ComponentBuilder()
                    .WithSelectMenu("choose", region.GetLocationsSelectMenu(),
                                "Выберите локацию для просмотра")
                    .WithButton("Назад", "exit", ButtonStyle.Danger)
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
                    case "exit":
                        return;
                    case "choose":
                        await ViewLocationAsync(param, region, region.GetLocation(int.Parse(param.Interaction.Data.Values.First())));
                        break;
                    default:
                        return;
                }
            }
        }
        public async Task ViewLocationAsync(InteractionParameters param, MapRegion region, MapLocation location)
        {
            Embed embed;
            MessageComponent components;
            var category = "main";
            while (true)
            {
                embed = GetCategoryEmbed(category, region, location);
                components = new ComponentBuilder()
                    .WithButton("Основное", "main", ButtonStyle.Success, disabled: category == "main")
                    .WithButton("Противники", "enemies", ButtonStyle.Success, disabled: category == "enemies")
                    .WithButton("Ресурсы", "drop", ButtonStyle.Success, disabled: category == "drop")
                    .WithButton("Назад", "exit", ButtonStyle.Danger, row: 1)
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
                    case "exit":
                        return;
                    default:
                        category = param.Interaction.Data.CustomId;
                        break;
                }
            }
        }
        public Embed GetCategoryEmbed(string category, MapRegion region, MapLocation location)
        {
            return category switch
            {
                "main" => new UserEmbedBuilder(Context.User)
                    .WithAuthor($"{region.Name} | {location.Name}")
                    .WithDescription($"{location.Description}\n" +
                    $"**Рекомендуемый ранг:** {location.RecomendedRank.ToRusString()}\n\n")
                    .AddField("Магазины", "```В разработке```", true)
                    .AddField("Подземелья", "```В разработке```", true)
                    .WithImageUrl(location.PictureURL)
                    .Build(),
                "enemies" => new UserEmbedBuilder(Context.User)
                    .WithAuthor($"{region.Name} | {location.Name}")
                    .WithDescription($"{location.Description}\n" +
                    $"**Рекомендуемый ранг:** {location.RecomendedRank.ToRusString()}\n")
                    .AddField("Противники", $"```{GetEnemiesCuteString(location.EnemiesWithChance)}```", true)
                    .WithImageUrl(location.PictureURL)
                    .Build(),
                "drop" => new UserEmbedBuilder(Context.User)
                    .WithAuthor($"{region.Name} | {location.Name}")
                    .WithDescription($"{location.Description}\n" +
                    $"**Рекомендуемый ранг:** {location.RecomendedRank.ToRusString()}\n")
                    .AddField("Шахта",$"```{GetMineDropCuteString(location.MineDrop)}```", true)
                    .WithImageUrl(location.PictureURL)
                    .Build(),
                _ => new UserEmbedBuilder(Context.User).Build()
            };
        }
        public static string GetLocationsCuteString(List<MapLocation> locations)
        {
            var result = new List<string>();
            for (int i = 0; i < locations.Count - 1; i++)
                result.Add($"{EmojiList.Get("lineMiddle")}{locations[i].Name}");
            result.Add($"{EmojiList.Get("lineEnd")}{locations[locations.Count - 1].Name}");
            return result.Count == 0 ? "Нету" : string.Join("\n", result);
        }
        public static string GetEnemiesCuteString(EnemyRate enemies)
        {
            var result = new List<string>();
            foreach (var (id, chance) in enemies)
            {
                var enemy = AllEnemiesData.GetData(id);
                result.Add($"{enemy.Name} {(chance / (float)enemies.All):P2}");
            }
            return result.Count == 0 ? "Нету" : string.Join("\n", result);
        }
        public static string GetMineDropCuteString(List<DropData> drop)
        {
            var result = new List<string>();
            foreach (var data in drop)
            {
                var item = ItemFactory.CreateItem(data.Id, 1);
                result.Add($"{item.Name} {data.Chance / 1000.0f:P2}");
            }
            return result.Count == 0 ? "Нету" : string.Join("\n", result);
        }
    }
}