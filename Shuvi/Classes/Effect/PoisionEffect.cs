using Shuvi.Classes.Characteristics;
using Shuvi.Interfaces.Entity;

namespace Shuvi.Classes.Effect
{
    public class PoisionEffect : EffectBase
    {
        public int DamagePerHod { get; private set; }

        public PoisionEffect(string name, int timeLeft, int damage, CharacteristicBonuses? bonuses) : base(name, timeLeft, bonuses)
        {
            DamagePerHod = damage;
        }
        public override void Update(IEntity target)
        {
            TimeLeft--;
            target.ReduceHealth(DamagePerHod);
        }
    }
}
