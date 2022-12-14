using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Effects;

namespace Shuvi.Interfaces.Entity
{
    public interface IEntity
    {
        public string Name { get; init; }
        public ICharacteristics Characteristics { get; init; }
        public IHealth Health { get; init; }
        public IMana Mana { get; init; }
        public bool IsDead { get; }
        public ICharacteristicBonuses EffectBonuses { get; }
        public IEffects Effects { get; }

        public void DealDamage(IEntity target);
        public int CalculateDamage();
        public int BlockDamage(int damage);
        public bool TryDodge(IEntity assaulter);
        public void RestoreHealth(int amount);
        public void ReduceHealth(int amount);
        public void RestoreMana(int amount);
        public void ReduceMana(int amount);
        public void Update();
    }
}