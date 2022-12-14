using Discord;

namespace Shuvi.Classes.CustomEmbeds
{
    public sealed class UserEmbedBuilder : EmbedBuilder
    {
        public static readonly Color StandartColor = new(240, 115, 47);

        public UserEmbedBuilder(IUser user)
        {
            WithFooter($"{user.Username} | {user.Id}", user.GetAvatarUrl());
            WithColor(StandartColor);
        }
    }
}