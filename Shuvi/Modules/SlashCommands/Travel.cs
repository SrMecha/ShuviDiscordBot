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
using MongoDB.Driver;
using Shuvi.Classes.User;

namespace Shuvi.Modules.SlashCommands
{
    public class TravelCommandModule : InteractionModuleBase<ShardedInteractionContext>
    {
        private readonly DatabaseManagerService _database;
        private readonly DiscordShardedClient _client;
        private readonly WorldMap _map;

        public TravelCommandModule(IServiceProvider provider)
        {
            _database = provider.GetRequiredService<DatabaseManagerService>();
            _client = provider.GetRequiredService<DiscordShardedClient>();
            _map = _database.Map;
        }

        [SlashCommand("travel", "Перейти в другой регион или локацию.")]
        public async Task TravelCommandAsync()
        {
            await DeferAsync();
            var param = new InteractionParameters(await GetOriginalResponseAsync(), null);
            IDatabaseUser dbUser = await _database.Users.GetUser(Context.User.Id);
            await MainTravelChooseAsync(param, dbUser);
        }

        public async Task MainTravelChooseAsync(InteractionParameters param, IDatabaseUser dbUser)
        {
            Embed embed;
            MessageComponent components;
            var currentRegion = _map.GetRegion(dbUser.Location.MapRegion);
            var currentLocation = currentRegion.GetLocation(dbUser.Location.MapLocation);
            var travelRegionId = dbUser.Location.MapRegion;
            var travelLocationId = dbUser.Location.MapLocation;
            var travelRegion = currentRegion;
            var travelLocation = currentLocation;
            var isLocationChanged = false;
            var isRegionChanged = false;
            do
            {
                embed = new UserEmbedBuilder(Context.User)
                    .WithAuthor("Путешествие")
                    .WithDescription($"**Энергии осталось:** {dbUser.Energy.GetCurrentEnergy()}/{dbUser.Energy.Max}\n" +
                    $"Вы потратите {(isRegionChanged ? 1 : 0) + (isLocationChanged ? 1 : 0)} энергии на это путешествие.\n\n" +
                    $"**Регион:** {currentRegion.Name} {(!isRegionChanged ? "" : $" -> {travelRegion.Name}")}\n" +
                    $"**Локация:** {currentLocation.Name} {(!isLocationChanged ? "" : $" -> {travelLocation.Name}")}")
                    .WithImageUrl(_map.Settings.PictureURL)
                    .Build();
                components = new ComponentBuilder()
                    .WithSelectMenu("region", GetRegionsSelectMenu(dbUser), "Выберите регион.")
                    .WithSelectMenu("location", GetLocationsSelectMenu(travelRegion), "Выберите локацию.")
                    .WithButton("Назад", "exit", ButtonStyle.Danger)
                    .WithButton("Отправиться", "travel", ButtonStyle.Success, disabled: !(isLocationChanged || isRegionChanged))
                    .Build();
                await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = components; });
                if (param.Interaction != null)
                    await param.Interaction.DeferAsync();
                param.Interaction = await WaitFor.UserButtonInteraction(_client, param.Message, Context.User.Id);
                if (param.Interaction == null)
                {
                    await ModifyOriginalResponseAsync(msg => { msg.Components = new ComponentBuilder().Build(); });
                    return;
                }
                switch (param.Interaction.Data.CustomId)
                {
                    case "exit":
                        await ModifyOriginalResponseAsync(msg => { msg.Components = new ComponentBuilder().Build(); });
                        return;
                    case "region":
                        isRegionChanged = true;
                        isLocationChanged = true;
                        travelRegionId = int.Parse(param.Interaction.Data.Values.First());
                        travelRegion = _map.GetRegion(travelRegionId);
                        travelLocationId = 0;
                        travelLocation = travelRegion.GetLocation(0);
                        if (travelRegionId == dbUser.Location.MapRegion)
                        {
                            isRegionChanged = false;
                            isLocationChanged = false;
                        }
                        break;
                    case "location":
                        isLocationChanged = true;
                        travelLocationId = int.Parse(param.Interaction.Data.Values.First());
                        travelLocation = travelRegion.GetLocation(travelLocationId);
                        if (travelLocationId == dbUser.Location.MapLocation)
                        {
                            isLocationChanged = false;
                        }
                        break;
                    case "travel":
                        var amount = (isRegionChanged ? 1 : 0) + (isLocationChanged ? 1 : 0);
                        if (!dbUser.Energy.HaveEnergy(amount))
                        {
                            await param.Interaction.RespondAsync(embed: ErrorEmbedBuilder.Simple("У вас не хватает энергии."), ephemeral: true);
                            await ModifyOriginalResponseAsync(msg => { msg.Components = new ComponentBuilder().Build(); });
                            return;
                        }
                        dbUser.Location.SetRegion(travelRegionId);
                        dbUser.Location.SetLocation(travelLocationId);
                        dbUser.Energy.ReduceEnergy(amount);
                        await dbUser.UpdateUser(new UpdateDefinitionBuilder<UserData>()
                            .Set("EnergyRegenTime", dbUser.Energy.RegenTime)
                            .Set("MapLocation", dbUser.Location.MapLocation)
                            .Set("MapRegion", dbUser.Location.MapRegion)
                            );
                        embed = new UserEmbedBuilder(Context.User)
                            .WithAuthor("Путешествие")
                            .WithDescription($"Вы прибыли в место назначения!\n" +
                            $"**Регион:** {travelRegion.Name}\n**Локация:** {travelLocation.Name}")
                            .Build();
                        await ModifyOriginalResponseAsync(msg => { msg.Embed = embed; msg.Components = new ComponentBuilder().Build(); });
                        return;
                    default:
                        return;
                }
                
            } while (param.Interaction != null);
            
        }
        private List<SelectMenuOptionBuilder> GetRegionsSelectMenu(IDatabaseUser dbUser)
        {
            var result = new List<SelectMenuOptionBuilder>();
            for (int i = 0; i < _map.Regions.Count; i++)
            {
                var region = _map.GetRegion(i);
                if (region.NeededRank <= dbUser.Rating.Rank)
                {
                    var description = region.Description;
                    if (description.Length > 70)
                    {
                        description = $"{description[..70]}...";
                    }
                    result.Add(new SelectMenuOptionBuilder(region.Name, i.ToString(), description));
                }
            }
            if (result.Count < 1)
                result.Add(new SelectMenuOptionBuilder("None", "None"));
            return result;
        }
        private List<SelectMenuOptionBuilder> GetLocationsSelectMenu(MapRegion region)
        {
            var result = new List<SelectMenuOptionBuilder>();
            for (int i = 0; i < region.Locations.Count; i++)
            {
                var location = region.GetLocation(i);
                var description = location.Description;
                if (description.Length > 70)
                {
                    description = $"{description[..70]}...";
                }
                result.Add(new SelectMenuOptionBuilder(location.Name, i.ToString(), description));
            }
            if (result.Count < 1)
                result.Add(new SelectMenuOptionBuilder("None", "None"));
            return result;
        }
    }
}
