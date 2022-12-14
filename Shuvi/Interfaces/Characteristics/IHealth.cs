namespace Shuvi.Interfaces.Characteristics
{
    public interface IHealth
    {
        public int Max { get; init; }
        public long RegenTime { get; init; }
        public int GetRemainingRegenTime();
        public int GetCurrentHealth();
        public string GetEmojiBar();
    }
}
