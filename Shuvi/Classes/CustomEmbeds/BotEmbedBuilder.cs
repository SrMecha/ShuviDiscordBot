using Discord;

namespace Shuvi.Classes.CustomEmbeds
{
    public sealed class BotEmbedBuilder : EmbedBuilder
    {
        public BotEmbedBuilder()
        {
            WithColor(UserEmbedBuilder.StandartColor);
        }
    }
}