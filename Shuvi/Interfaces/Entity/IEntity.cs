using Shuvi.Enums;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Effect;
using Shuvi.Interfaces.Spell;
using Shuvi.Interfaces.Status;

namespace Shuvi.Interfaces.Entity
{
    public interface IEntity
    {
        public string Name { get; }
        public Rank Rank { get; }
        public IUserCharacteristics Characteristics { get; }
        public ISpell Spell { get; }
        public IStaticHealth Health { get; }
        public IStaticMana Mana { get; }
        public bool IsDead { get; }
        public ICharacteristicBonuses EffectBonuses { get; }
        public IEffects Effects { get; }

        public IActionResult CastSpell(IEntity target);
        public IActionResult DealLightDamage(IEntity target);
        public IActionResult DealHeavyDamage(IEntity target);
        public float CalculateLightDamage();
        public float CalculateHeavyDamage();
        public IActionResult PreparingForDefense(IEntity target);
        public IActionResult PreparingForDodge(IEntity target);
        public int BlockDamage(float damage);
        public bool IsDodged(IEntity assaulter, int hitBonusChance);
        public void RestoreHealth(int amount);
        public void ReduceHealth(int amount);
        public void RestoreMana(int amount);
        public void ReduceMana(int amount);
        public void Update();
    }
}