using Shuvi.Classes.CustomEmoji;
using Shuvi.Classes.User;
using Shuvi.Extensions;
using Shuvi.Interfaces.Characteristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shuvi.Classes.Characteristics
{
    public class Mana : IMana
    {
        public int Max { get; init; }
        public long RegenTime { get; init; }

        public Mana(int max, long regenTime)
        {
            Max = max;
            RegenTime = regenTime;
        }
        public int GetCurrentMana()
        {
            return Max - (GetRemainingRegenTime() / UserSettings.ManaPointRegenTime);
        }
        public int GetRemainingRegenTime()
        {
            int result = (int)(RegenTime - ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            return result > 0 ? result : 0;
        }
        public string GetEmojiBar()
        {
            var manaFullEmojiCount = (byte)(GetCurrentMana() / (Max / UserSettings.ManaDisplayMax));
            return $"{EmojiList.Get("magicFull").ToString()!.Multiple(manaFullEmojiCount)}" +
                $"{EmojiList.Get("magicEmpty").ToString()!.Multiple((byte)(UserSettings.ManaDisplayMax - manaFullEmojiCount))}";
        }
    }
}
