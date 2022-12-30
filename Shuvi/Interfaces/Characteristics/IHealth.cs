namespace Shuvi.Interfaces.Characteristics
{
    public interface IHealth
    {
        public int Max { get; }
        public long RegenTime { get; }
        public int GetRemainingRegenTime();
        public int GetCurrentHealth();
        public void ReduceHealth(int amount);
        public string GetEmojiBar();
    }
}
