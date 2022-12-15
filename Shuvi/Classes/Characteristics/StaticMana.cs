using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Classes.Characteristics
{
    public class StaticMana : IStaticMana
    {
        public int Max { get; init; }
        public int Now { get; private set; }

        public StaticMana(int now, int max)
        {
            Now = now;
            Max = max;
        }

        public void ReduceMana(int amount)
        {
            Now -= amount;
            if (Now < 0)
                Now = 0;
        }
        public void RestoreMana(int amount)
        {
            Now += amount;
            if (Now > Max)
                Now = Max;
        }
    }
}
