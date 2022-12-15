namespace Shuvi.Interfaces.Characteristics
{
    public interface IStaticMana
    {
        public int Max { get; init; }
        public int Now { get; }
        public void ReduceMana(int amount);
        public void RestoreMana(int amount);
    }
}
