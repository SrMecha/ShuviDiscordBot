using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Effect;
using Shuvi.Interfaces.Spell;

namespace Shuvi.Interfaces.Entity
{
    public interface IEntity
    {
        public string Name { get; }
        public ICharacteristics Characteristics { get; }
        public ISpell Spell { get; }
        public IStaticHealth Health { get; }
        public IStaticMana Mana { get; }
        public bool IsDead { get; }
        public ICharacteristicBonuses EffectBonuses { get; }
        public IEffects Effects { get; }

        public void DealDamage(IEntity target);
        public int CalculateDamage();
        public int BlockDamage(int damage);
        public bool IsDodged(IEntity assaulter);
        public bool IsCritical(IEntity target);
        public void RestoreHealth(int amount);
        public void ReduceHealth(int amount);
        public void RestoreMana(int amount);
        public void ReduceMana(int amount);
        public void Update();
    }
}