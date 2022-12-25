using Shuvi.Classes.CustomEmoji;
using Shuvi.Classes.User;
using Shuvi.Extensions;
using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Classes.Characteristics
{
    public class Health : IHealth
    {
        public int Max { get; private set; }
        public long RegenTime { get; private set; }

        public Health(int max, long regenTime)
        {
            Max = max;
            RegenTime = regenTime;
        }

        public int GetCurrentHealth()
        {
            return Max - (GetRemainingRegenTime() / UserSettings.HealthPointRegenTime);
        }
        public int GetRemainingRegenTime()
        {
            int result = (int)(RegenTime - ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            return result > 0 ? result : 0;
        }
        public void ReduceHealth(int amount)
        {
            if (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() > RegenTime)
                RegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + (amount * UserSettings.HealthPointRegenTime);
            else
                RegenTime += amount * UserSettings.HealthPointRegenTime;
        }
        public string GetEmojiBar()
        {
            var healthFullEmojiCount = (byte)(GetCurrentHealth() / (Max / UserSettings.HealthDisplayMax));
            return $"{EmojiList.Get("healthFull").ToString()!.Multiple(healthFullEmojiCount)}" +
                $"{EmojiList.Get("healthEmpty").ToString()!.Multiple((byte)(UserSettings.HealthDisplayMax - healthFullEmojiCount))}";
        }
    }
}
