namespace Shuvi.Interfaces.Characteristics
{
    public interface IMana
    {
        public int Max { get; }
        public long RegenTime { get; }
        public int GetCurrentMana();
        public int GetRemainingRegenTime();
        public void ReduceMana(int amount);
        public string GetEmojiBar();
    }
}
