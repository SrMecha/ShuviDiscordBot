using Discord;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Enums;

namespace Shuvi.StaticServices.UserCheck
{
    public static class UserCommandsCheck
    {
        private static readonly Dictionary<ulong, ActiveCommands> _data = new();
        private static readonly Dictionary<ActiveCommands, string> _errors= new() 
        {
            {ActiveCommands.Hunt, "Вы не можете выполнить данное действие, пока у вас активна команда охоты." },
            {ActiveCommands.Upgrade, "Вы не можете выполнить данное действие, пока у вас активна команда улучшения." },
            {ActiveCommands.Shop, "Вы не можете выполнить данное действие, пока у вас активна команда магазина." }
        };

        public static void Add(ulong id, ActiveCommands command)
        {
            if (_data.ContainsKey(id))
                _data[id] |= command;
            else
                _data.Add(id, command);
        }
        public static void Remove(ulong id, ActiveCommands command)
        {
            if (!_data.ContainsKey(id))
                return;
            _data[id] ^= command;
            if (_data[id] == ActiveCommands.None)
                _data.Remove(id);
        }
        public static bool IsUseCommands(ulong id, ActiveCommands commands)
        {
            if (!_data.ContainsKey(id))
                return false;
            return _data[id].HasFlag(commands);
        }
        public static Embed? GetErrorEmbed(ulong id, ActiveCommands commands)
        {
            foreach (var (command, message) in _errors)
                if (commands.HasFlag(command))
                    return ErrorEmbedBuilder.Simple(message);
            return ErrorEmbedBuilder.Simple("UserCommandCheck.GetErrorEmbed() - неожиданная ошибка. Свяжитесь с разроботчиком.");
        }
    }
}
