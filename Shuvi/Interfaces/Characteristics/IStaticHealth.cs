namespace Shuvi.Interfaces.Characteristics
{
    public interface IStaticHealth
    {
        public int Max { get; init; }
        public int Now { get; }
        public void ReduceHealth(int amount);
        public void RestoreHealth(int amount);
    }
}
