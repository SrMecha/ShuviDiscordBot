using Discord;

namespace Shuvi.Classes.CustomEmbeds
{
    public sealed class UserDeadEmbedBuilder : EmbedBuilder
    {
        private static readonly Color _deadEmbedColor = new(248, 0, 0);

        public UserDeadEmbedBuilder()
        {
            WithAuthor("You dead");
            WithColor(_deadEmbedColor);
        }
        public static Embed Simple(string asassinName)
        {
            return new UserDeadEmbedBuilder()
                .WithDescription($"К сожалению вы умерли. Вас убил противник {asassinName}. " +
                $"Вы потеряли все свои вещи, деньги и славу. Может быть в следующий раз вам повезет больше.")
                .Build();
        }
    }
}
