using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Effect;
using Shuvi.Classes.Spell;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Effect;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Spell;

namespace Shuvi.Classes.Entity
{
    public class Entity : IEntity
    {
        public string Name { get; protected init; } = string.Empty;
        public ICharacteristics Characteristics { get; protected init; } = new UserCharacteristics();
        public ISpell Spell { get; protected init; } = SpellFactory.GetSpell("");
        public IStaticHealth Health { get; protected init; } = new StaticHealth(100, 100);
        public IStaticMana Mana { get; protected init; } = new StaticMana(10, 10);
        public bool IsDead => Health.Now <= 0;
        public ICharacteristicBonuses EffectBonuses { get; private set; } = new CharacteristicBonuses();
        public IEffects Effects { get; private set; } = new Effects();

        public int BlockDamage(int damage)
        {
            throw new NotImplementedException();
        }
        public int CalculateDamage()
        {
            throw new NotImplementedException();
        }
        public void DealDamage(IEntity target)
        {
            throw new NotImplementedException();
        }
        public bool IsCritical(IEntity target)
        {
            throw new NotImplementedException();
        }
        public bool IsDodged(IEntity assaulter)
        {
            throw new NotImplementedException();
        }
        public void ReduceHealth(int amount)
        {
            throw new NotImplementedException();
        }
        public void ReduceMana(int amount)
        {
            throw new NotImplementedException();
        }
        public void RestoreHealth(int amount)
        {
            throw new NotImplementedException();
        }
        public void RestoreMana(int amount)
        {
            throw new NotImplementedException();
        }
        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
