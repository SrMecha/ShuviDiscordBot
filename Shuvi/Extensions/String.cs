namespace Shuvi.Extensions
{
    public static class StringExtension
    {
        public static string Multiple(this string original, byte repeatCount)
        {
            if (repeatCount < 1) return string.Empty;
            return string.Create(original.Length * ++repeatCount, original, (span, value) =>
            {
                for (var i = 0; i < (span.Length - value.Length); i += value.Length)
                    for (var j = 0; j < value.Length; j++)
                        span[i + j] = value[j];
            });
        }
    }
}