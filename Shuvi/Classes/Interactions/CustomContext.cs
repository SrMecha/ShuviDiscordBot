using Discord;
using Discord.Interactions;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.Interactions
{
    public class CustomContext
    {
        public IDatabaseUser DatabaseUser { get; private set; }
        public IUser DiscordUser { get; private set; }

        public CustomContext(IDatabaseUser databaseUser, IUser discordUser)
        {
            DatabaseUser = databaseUser;
            DiscordUser = discordUser;
        }
        public CustomContext(IDatabaseUser databaseUser, ShardedInteractionContext Context)
        {
            DatabaseUser = databaseUser;
            DiscordUser = Context.User;
        }
    }
}
