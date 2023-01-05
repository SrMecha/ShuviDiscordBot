namespace Shuvi.Interfaces.Characteristics
{
    public interface IEnergy
    {
        public int Max { get; }
        public long RegenTime { get; }
        public int GetRemainingRegenTime();
        public int GetMaxEnergy(int endurance);
        public int GetCurrentEnergy();
        public void ReduceEnergy(int amount);
        public void IncreaseEnergy(int amount);
        public string GetEmojiBar();
        public bool HaveEnergy(int amount);
        public void UpdateMaxEnergy(int endurance);
    }
}
