using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Classes.Characteristics
{
    public class StaticHealth : IStaticHealth
    {
        public int Max { get; init; }
        public int Now { get; private set; }

        public StaticHealth(int now, int max)
        {
            Now = now;
            Max = max;
        }

        public void ReduceHealth(int amount)
        {
            Now -= amount;
            if (Now < 0)
                Now = 0;
        }
        public void RestoreHealth(int amount)
        {
            Now += amount;
            if (Now > Max)
                Now = Max;
        }
        public override string ToString()
        {
            return $"**Жизни:** {Now}/{Max}";
        }
        public string ToString(int intellect)
        {
            var isHide = Max / CharacteristicSettings.HealthPerUpPoint > intellect;
            return $"**Жизни:** {(isHide ? "?" : Now)}/{(isHide ? "?" : Max)}";
        }
    }
}
