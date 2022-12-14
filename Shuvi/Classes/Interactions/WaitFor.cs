using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Shuvi.Classes.Interactions
{
    public sealed class WaitFor
    {
        public static async Task<SocketMessageComponent> UserButtonInteraction(DiscordShardedClient client, IUserMessage message, ulong userId)
        {
            bool check(SocketInteraction inter)
            {
                if (inter.Type != InteractionType.MessageComponent) return false;
                return (inter as SocketMessageComponent)!.Message.Id == message.Id && inter.User.Id == userId;
            }
            SocketInteraction output = await InteractionUtility.WaitForInteractionAsync(client, new TimeSpan(0, 5, 0), check);
            return (output as SocketMessageComponent)!;
        }
    }
}