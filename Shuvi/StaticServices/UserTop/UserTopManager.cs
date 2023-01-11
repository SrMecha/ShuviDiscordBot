using Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Interactions;
using Shuvi.Classes.Items;
using Shuvi.Classes.User;
using Shuvi.Interfaces.User;

namespace Shuvi.StaticServices.UserTop
{
    public static class UserTopManager
    {
        private static List<string>? _top;
        private static long? _lastUpdate;

        public static void LoadTop(List<UserData> usersData)
        {
            var result = new List<string>();
            for (int i = 0; i < usersData.Count; i++)
            {
                var data = usersData[i];
                result.Add($"**#{i + 1}** <@{data.Id}> | {data.Rating}");
            }
            _top = result;
            _lastUpdate = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }
        private static bool isDataSet()
        {
            return !(_top == null);
        }
        public static int GetTotalPages()
        {
            return ((_top!.Count + 9) / 10) < 1 ? 1 : (_top!.Count + 9) / 10;
        }
        public static Embed GetEmbed(int index)
        {
            if (!isDataSet())
                return new BotEmbedBuilder()
                    .WithAuthor("Топ по рейтингу")
                    .WithDescription("Топ еще не загружен...")
                    .WithFooter($"")
                    .Build();
            var result = new List<string>();
            for (int i = index * 10; i < _top!.Count && i < index * 10 + 10; i++)
            {
                result.Add(_top![i]);
            }
            return new BotEmbedBuilder()
                    .WithAuthor("Все предметы:")
                    .WithDescription($"{string.Join("\n", result)}\n\nОбновлено <t:{_lastUpdate}:R>")
                    .WithFooter($"Страница {index + 1}/{GetTotalPages()}")
                    .Build();
        }
    }
}
