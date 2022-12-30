using Discord;

namespace Shuvi.Classes.CustomEmbeds
{
    public sealed class ErrorEmbedBuilder : EmbedBuilder
    {
        private static readonly Color _errorEmbedColor = new(255, 0, 51);

        public ErrorEmbedBuilder()
        {
            WithAuthor("Ошибка!");
            WithColor(_errorEmbedColor);
        }
        public static Embed Simple(string description)
        {
            return new EmbedBuilder().WithAuthor("Ошибка!").WithDescription(description).WithColor(_errorEmbedColor).Build();
        }
    }
}
