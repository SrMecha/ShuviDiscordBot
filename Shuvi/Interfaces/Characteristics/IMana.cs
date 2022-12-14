namespace Shuvi.Interfaces.Characteristics
{
    public interface IMana
    {
        public int Max { get; init; }
        public long RegenTime { get; init; }
        public int GetCurrentMana();
        public int GetRemainingRegenTime();
        public string GetEmojiBar();

    }
}
