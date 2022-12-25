using Discord.Commands;

namespace Shuvi.Interfaces.Characteristics
{
    public interface IEnergy
    {
        public int Max { get; init; }
        public long RegenTime { get; init; }
        public int GetRemainingRegenTime();
        public int GetMaxEnergy(int endurance);
        public int GetCurrentEnergy();
        public string GetEmojiBar();
    }
}
