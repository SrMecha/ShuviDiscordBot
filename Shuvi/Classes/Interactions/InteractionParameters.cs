using Discord;
using Discord.WebSocket;

namespace Shuvi.Classes.Interactions
{
    public class InteractionParameters
    {
        public IUserMessage Message { get; set; }
        public SocketMessageComponent? Interaction { get; set; }

        public InteractionParameters(IUserMessage message, SocketMessageComponent? interaction)
        {
            Message = message;
            Interaction = interaction;
        }
    }
}
