using Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using Shuvi.Classes.Interactions;
using Shuvi.Enums;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.User;

namespace Shuvi.Interfaces.Items
{
    public interface IItem
    {
        public ObjectId Id { get; init; }
        public int Amount { get; }
        public string Name { get; init; }
        public string Description { get; init; }
        public ItemType Type { get; init; }
        public Rank Rank { get; init; }
        public bool CanTrade { get; init; }
        public bool CanLoose { get; init; }
        public int Max { get; init; }
        public ICharacteristicBonuses? Bonuses { get; }
        public Dictionary<ItemNeeds, int>? Needs { get; }
        public Embed GetEmbed();
        public Embed GetEmbed(IDatabaseUser dbUser, IUser user);
        public Task ViewItemAsync(DiscordShardedClient client, InteractionParameters param, IUser discordUser);
        public Task ViewItemAsync(DiscordShardedClient client, InteractionParameters param, IDatabaseUser dbUser, IUser discordUser);
    }
}