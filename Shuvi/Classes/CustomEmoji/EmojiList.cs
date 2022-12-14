using Discord;

namespace Shuvi.Classes.CustomEmoji
{
    public static class EmojiList
    {
        public static readonly IDictionary<string, IEmote> _emotes = new Dictionary<string, IEmote>();
        private static readonly IEmote _notFound = Emote.Parse("<:404:1036314484790280232>");

        public static void Init()
        {
            IDictionary<string, string> emotesBase = new Dictionary<string, string>
            {
                { "_", "<:404:1036314484790280232>" },
                { "goodMark", "<:canceled:1031196187870629898>" },
                { "badMark", "<:checked:1031196185794449470>" },
                { "money", "<:dispoint:1015999481034068048>" },
                { "energyEmpty", "<:energy_empty:1030943969863024730>" },
                { "energyFull", "<:energy_full:1030943971532349491>" },
                { "healthEmpty", "<:health_empty:1030948347529412628>" },
                { "healthFull", "<:health_full:1030948349022588958>" },
                { "choosePoint", "<a:right_point:12313213123:>" },
                { "magicFull", "<:magic_full:1050772671002071160>" },
                { "magicEmpty", "<:magic_empty:1050772733828530218>" }
            };
            foreach (var (emoteKey, emoteCode) in emotesBase)
                if (Emote.TryParse(emoteCode, out var emote))
                    _emotes[emoteKey] = emote;
        }

        public static IEmote Get(string name)
        {
            if (_emotes.TryGetValue(name, out var emote))
                return emote;
            return _notFound;
        }
    }
}