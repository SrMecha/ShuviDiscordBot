using Discord.WebSocket;
using Shuvi.Classes.CustomEmoji;

namespace Shuvi.StaticServices.Logs
{
    public static class ServerLogs
    {
        private static ulong _logChannelId;
        private static SocketTextChannel? _channel;

        public static void Init(LogsData data, DiscordShardedClient client)
        {
            _logChannelId = data.ServerLogChannelId;
            _channel = (client.GetChannel(_logChannelId) as SocketTextChannel);
        }
        private static bool IsChannelSet()
        {
            if (_channel == null)
            {
                Console.WriteLine("Логи для серверов не настроены.");
                return false;
            }
            return true;
        }
        public static async Task OnGuildAdd(SocketGuild guild)
        {
            if (!IsChannelSet())
                return;
            await _channel!.SendMessageAsync($"[{EmojiList.Get("goodMark")}] {guild.Name}(`{guild.Id}`) | {guild.MemberCount} уч.");
        }
        public static async Task OnGuildRemove(SocketGuild guild)
        {
            if (!IsChannelSet())
                return;
            await _channel!.SendMessageAsync($"[{EmojiList.Get("badMark")}] {guild.Name}(`{guild.Id}`) | {guild.MemberCount} уч.");
        }

    }
}
