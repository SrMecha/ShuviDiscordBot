using Shuvi.Classes.CustomEmoji;
using Shuvi.Classes.User;
using Shuvi.Extensions;
using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Classes.Characteristics
{
    public class Energy : IEnergy
    {
        public int Max { get; private set; }
        public long RegenTime { get; private set; }

        public Energy(long regenTime, int endurance)
        {
            Max = GetMaxEnergy(endurance);
            RegenTime = regenTime;
        }

        public int GetCurrentEnergy()
        {
            var result = (int)(Max - Math.Ceiling((float)GetRemainingRegenTime() / UserSettings.EnergyPointRegenTime));
            if (result < 0)
                return 0;
            return result > Max ? Max : result;
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
        public void ReduceEnergy(int amount)
        {
            if (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() > RegenTime)
                RegenTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + (amount * UserSettings.EnergyPointRegenTime);
            else
                RegenTime += amount * UserSettings.EnergyPointRegenTime;
        }
        public void IncreaseEnergy(int amount)
        {
            RegenTime -= amount * UserSettings.EnergyPointRegenTime;
        }
        public string GetEmojiBar()
        {
            var energyFullEmojiCount = (byte)(GetCurrentEnergy() / (Max / UserSettings.EnergyDisplayMax));
            return $"{EmojiList.Get("energyFull").ToString()!.Multiple(energyFullEmojiCount)}" +                    
                $"{EmojiList.Get("energyEmpty").ToString()!.Multiple((byte)(UserSettings.EnergyDisplayMax - energyFullEmojiCount))}";
        }
        public bool HaveEnergy(int amount)
        {
            return amount <= GetCurrentEnergy();
        }
        public void UpdateMaxEnergy(int endurance)
        {
            Max = GetMaxEnergy(endurance);
        }
    }
}
