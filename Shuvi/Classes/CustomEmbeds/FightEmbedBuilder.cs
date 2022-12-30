using Discord;

namespace Shuvi.Classes.CustomEmbeds
{
    public class FightEmbedBuilder : EmbedBuilder
    {
        public FightEmbedBuilder(string avatarUrl, string firstName, string secondNmae)
        {
            WithFooter($"{firstName} vs {secondNmae}", avatarUrl);
            WithColor(UserEmbedBuilder.StandartColor);
        }
    }
}
