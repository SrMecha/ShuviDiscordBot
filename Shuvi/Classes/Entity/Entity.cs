using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Effect;
using Shuvi.Classes.Spell;
using Shuvi.Classes.Status;
using Shuvi.Enums;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Effect;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Spell;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Entity
{
    public class EntityBase : IEntity
    {
        protected bool _isPreparingForDefense = false;
        protected bool _isPreparingForDodge = false;
        protected readonly Random _random = new();

        public string Name { get; protected init; } = string.Empty;
        public Rank Rank { get; protected init; } = Rank.E;
        public ICharacteristics Characteristics { get; protected init; } = new UserCharacteristics();
        public ISpell Spell { get; protected init; } = SpellFactory.GetSpell("");
        public IStaticHealth Health { get; protected init; } = new StaticHealth(100, 100);
        public IStaticMana Mana { get; protected init; } = new StaticMana(10, 10);
        public bool IsDead => Health.Now <= 0;
        public ICharacteristicBonuses EffectBonuses { get; private set; } = new CharacteristicBonuses();
        public IEffects Effects { get; private set; } = new Effects();

        public IActionResult DealLightDamage(IEntity target)
        {
            if (target.IsDodged(this, 30))
                return new ActionResult($"{Name} промахнулся лёгким ударом.");
            int damage;
            if (IsCritical(target))
            {
                damage = target.BlockDamage(CalculateLightDamage()) * 2;
                target.ReduceHealth(damage);
                return new ActionResult($"{Name} нанёс {damage} урона критическим ударом.");
            }
            damage = target.BlockDamage(CalculateLightDamage());
            target.ReduceHealth(damage);
            return new ActionResult($"{Name} нанёс {damage} урона лёгким ударом.");
        }
        public IActionResult DealHeavyDamage(IEntity target)
        {
            if (target.IsDodged(this, 0))
                return new ActionResult($"{Name} промахнулся тяжёлым ударом.");
            int damage;
            if (IsCritical(target))
            {
                damage = target.BlockDamage(CalculateHeavyDamage()) * 2;
                target.ReduceHealth(damage);
                return new ActionResult($"{Name} нанёс {damage} урона критическим ударом.");
            }
            damage = target.BlockDamage(CalculateHeavyDamage());
            target.ReduceHealth(damage);
            return new ActionResult($"{Name} нанёс {damage} урона тяжёлым ударом.");
        }
        public float CalculateLightDamage()
        {
            return (Characteristics.Strength + EffectBonuses.Strength) * _random.Next(70, 81) / 100.0f;
        }
        public float CalculateHeavyDamage()
        {
            return (Characteristics.Strength + EffectBonuses.Strength) * _random.Next(120, 131) / 100.0f;
        }
        public int BlockDamage(float damage)
        {
            var blockedDamageBonus = 0.0f;
            if (_isPreparingForDefense)
                blockedDamageBonus = (Characteristics.Endurance + EffectBonuses.Endurance) / 2.0f;
            var outDamage =  (int)(damage - ((Characteristics.Endurance + EffectBonuses.Endurance + blockedDamageBonus) / 3.0f) + 0.5f);
            // 0.5f для округления float в большую сторону.
            if (outDamage < 0)
                outDamage = 0;
            return outDamage;
        }
        public IActionResult CastSpell(IEntity target)
        {
            return Spell.Cast(this, target);
        }
        public IActionResult PreparingForDefense(IEntity target)
        {
            _isPreparingForDefense = true;
            if (IsInvisiblePreparing(target))
                return new ActionResult($"{Name} к чему-то готовится...");
            return new ActionResult($"{Name} готовится защищаться.");
        }
        public IActionResult PreparingForDodge(IEntity target)
        {
            _isPreparingForDodge = true;
            if (IsInvisiblePreparing(target))
                return new ActionResult($"{Name} к чему-то готовится...");
            return new ActionResult($"{Name} готовится уклоняться.");
        }
        private bool IsInvisiblePreparing(IEntity target)
        {
            var invisiblePreparingChance = (30 +
                Characteristics.Intellect + EffectBonuses.Intellect - (target.Characteristics.Intellect + target.EffectBonuses.Intellect)) / 2;
            return _random.Next(0, 101) <= invisiblePreparingChance;
        }
        public bool IsCritical(IEntity target)
        {
            var standartCriticalBonus = 10;
            var criticalChance = (standartCriticalBonus +
                Characteristics.Luck + EffectBonuses.Luck - (target.Characteristics.Luck + target.EffectBonuses.Luck)) / 2;
            return _random.Next(0, 101) <= criticalChance;
        }
        public bool IsDodged(IEntity assaulter, int hitBonusChance)
        {
            var dodgeBonus = 60;
            if (_isPreparingForDodge)
                dodgeBonus = 120;
            dodgeBonus -= hitBonusChance;
            var dodgeChance = (dodgeBonus + 
                (Characteristics.Agility + EffectBonuses.Agility - (assaulter.Characteristics.Agility + assaulter.EffectBonuses.Agility))) / 2;
            return _random.Next(0, 101) <= dodgeChance;
        }
        public void ReduceHealth(int amount)
        {
            Health.ReduceHealth(amount);
        }
        public void ReduceMana(int amount)
        {
            Mana.ReduceMana(amount);
        }
        public void RestoreHealth(int amount)
        {
            Health.RestoreHealth(amount);
        }
        public void RestoreMana(int amount)
        {
            Mana.RestoreMana(amount);
        }
        public virtual void Update()
        {
            _isPreparingForDefense = false;
            _isPreparingForDodge = false;
            Effects.UpdateAll(this);
        }
    }
}
