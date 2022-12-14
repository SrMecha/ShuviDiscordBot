using Shuvi.Classes.CustomEmoji;
using Shuvi.Classes.User;
using Shuvi.Extensions;
using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Classes.Characteristics
{
    public class Energy : IEnergy
    {
        public int Max { get; init; }
        public long RegenTime { get; init; }

        public Energy(long regenTime, int endurance)
        {
            Max = GetMaxEnergy(endurance);
            RegenTime = regenTime;
        }

        public int GetCurrentEnergy()
        {
            return Max - (GetRemainingRegenTime() / UserSettings.EnergyPointRegenTime);
        }
        public int GetMaxEnergy(int endurance)
        {
            return UserSettings.StandartEnergy + (endurance / UserSettings.EndurancePerEnergy);
        }
        public int GetRemainingRegenTime()
        {
            int result = (int)(RegenTime - ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            return result > 0 ? result : 0;
        }
        public string GetEmojiBar()
        {
            var energyFullEmojiCount = (byte)(GetCurrentEnergy() / (Max / UserSettings.EnergyDisplayMax));
            return $"{EmojiList.Get("energyFull").ToString()!.Multiple(energyFullEmojiCount)}" +                    
                $"{EmojiList.Get("energyEmpty").ToString()!.Multiple((byte)(UserSettings.EnergyDisplayMax - energyFullEmojiCount))}";
        }
    }
}
