using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Entity;
using Shuvi.Classes.Spell;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Rates;
using Shuvi.Enums;
using Shuvi.Interfaces.Status;
using Shuvi.Classes.Rates;

namespace Shuvi.Classes.Enemy
{
    public class EnemyBase : EntityBase, IEnemy
    {
        private readonly List<int> _actionChances;

        public int RatingGet { get; init; }
        public string PictureUrl { get; init; }
        public IRate Drop { get; init; }

        public EnemyBase(EnemyData data, string picture)
        {
            Name = data.Name;
            Rank = data.Rank;
            PictureUrl = data.Pictures.GetValueOrDefault(picture, "https://www.engageconsultants.com/wp-content/uploads/2020/01/No-data.jpg");
            RatingGet = data.RatingGet;
            Characteristics = SetCharacteristics(data, data.UpgradePoints);
            Spell = SpellFactory.GetSpell(data.SpellName);
            Health = SetHealth(data.Health, data.UpgradePoints);
            Mana = SetMana(data.Mana, data.UpgradePoints);
            Drop = new AllRate(data.Drop);
            _actionChances = SetActionChanses(data.ActionChances);
        }
        private UserCharacteristics SetCharacteristics(EnemyData data, int amount)
        {
            return new UserCharacteristics(
                _random.Next(0, 2) == 0 ? data.Strength - _random.Next(0, amount) : data.Strength + _random.Next(0, amount),
                _random.Next(0, 2) == 0 ? data.Agility - _random.Next(0, amount) : data.Agility + _random.Next(0, amount),
                _random.Next(0, 2) == 0 ? data.Luck - _random.Next(0, amount) : data.Luck + _random.Next(0, amount),
                _random.Next(0, 2) == 0 ? data.Intellect - _random.Next(0, amount) : data.Intellect + _random.Next(0, amount),
                _random.Next(0, 2) == 0 ? data.Endurance - _random.Next(0, amount) : data.Endurance + _random.Next(0, amount)
                );
        }
        private StaticHealth SetHealth(int health, int amount)
        {
            var finalHealth = _random.Next(0, 2) == 0 
                ? 
                health - _random.Next(0, amount)*CharacteristicSettings.HealthPerUpPoint 
                : 
                health + _random.Next(0, amount) * CharacteristicSettings.HealthPerUpPoint;
            return new StaticHealth(finalHealth, finalHealth);
        }
        private StaticMana SetMana(int mana, int amount)
        {
            var finalMana = _random.Next(0, 2) == 0
                ?
                mana - _random.Next(0, amount) * CharacteristicSettings.ManaPerUpPoint
                :
                mana + _random.Next(0, amount) * CharacteristicSettings.ManaPerUpPoint;
            return new StaticMana(finalMana, finalMana);
        }
        private List<int> SetActionChanses(EnemyActionChances chances) 
        { 
            var result = new List<int>();
            for (int i = 0; i < chances.LightAttack; i++)
                result.Add((int)EnemyAction.LightAttack);
            for (int i = 0; i < chances.HeavyAttack; i++)
                result.Add((int)EnemyAction.HeavyAttack);
            for (int i = 0; i < chances.Dodge; i++)
                result.Add((int)EnemyAction.Dodge);
            for (int i = 0; i < chances.Defense; i++)
                result.Add((int)EnemyAction.Defense);
            return result;
        }
        public IActionResult RandomAction(IEntity target)
        {
            if (Spell.CanCast(this))
                return (EnemyAction)_actionChances.ElementAt(_random.Next(0, _actionChances.Count)) switch
                {
                    EnemyAction.LightAttack => DealLightDamage(target),
                    EnemyAction.HeavyAttack => DealHeavyDamage(target),
                    EnemyAction.Dodge => PreparingForDodge(target),
                    EnemyAction.Defense => PreparingForDefense(target),
                    EnemyAction.Spell => CastSpell(target),
                    _ => DealLightDamage(target)
                };
            else
                return (EnemyAction)_actionChances.ElementAt(_random.Next(0, _actionChances.Count)) switch
                {
                    EnemyAction.LightAttack => DealLightDamage(target),
                    EnemyAction.HeavyAttack => DealHeavyDamage(target),
                    EnemyAction.Dodge => PreparingForDodge(target),
                    EnemyAction.Defense => PreparingForDefense(target),
                    EnemyAction.Spell => DealLightDamage(target),
                    _ => DealLightDamage(target)
                };
        }
    }
}
