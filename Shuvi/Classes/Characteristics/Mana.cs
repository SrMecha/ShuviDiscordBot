using Shuvi.Classes.CustomEmoji;
using Shuvi.Classes.User;
using Shuvi.Extensions;
using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Classes.Characteristics
{
    public class Mana : IMana
    {
        public int Max { get; private set; }
        public long RegenTime { get; private set; }

        public Mana(int max, long regenTime)
        {
            Max = max;
            RegenTime = regenTime;
        }
        public int GetCurrentMana()
        {
            var result = (int)(Max - Math.Ceiling((float)GetRemainingRegenTime() / UserSettings.ManaPointRegenTime));
            return result > Max ? Max : result;
        }
        public int GetRemainingRegenTime()
        {
            int result = (int)(RegenTime - ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            return result > 0 ? result : 0;
        }
        public void ReduceMana(int amount)
        {
            if (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() > RegenTime)
                RegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + (amount * UserSettings.ManaPointRegenTime);
            else
                RegenTime += amount * UserSettings.ManaPointRegenTime;
        }
        public string GetEmojiBar()
        {
            var manaFullEmojiCount = (byte)(GetCurrentMana() / (Max / UserSettings.ManaDisplayMax));
            return $"{EmojiList.Get("magicFull").ToString()!.Multiple(manaFullEmojiCount)}" +
                $"{EmojiList.Get("magicEmpty").ToString()!.Multiple((byte)(UserSettings.ManaDisplayMax - manaFullEmojiCount))}";
        }
    }
}
