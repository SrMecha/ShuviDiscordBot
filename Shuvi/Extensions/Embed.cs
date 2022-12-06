using Discord;

namespace ShuviBot.Extensions.CustomEmbed
{
    sealed class UserEmbedBuilder : EmbedBuilder
    {
        public static readonly Color StandartColor = new(240, 115, 47);

        public UserEmbedBuilder(IUser user)
        {
            WithFooter($"{user.Username} | {user.Id}", user.GetAvatarUrl());
            WithColor(StandartColor);
        }
    }

    sealed class BotEmbedBuilder : EmbedBuilder
    {
        public BotEmbedBuilder()
        {
            WithColor(UserEmbedBuilder.StandartColor);
        }
    }
}