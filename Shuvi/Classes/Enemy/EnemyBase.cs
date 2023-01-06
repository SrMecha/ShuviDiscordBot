using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Entity;
using Shuvi.Classes.Spell;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Rates;
using Shuvi.Enums;
using Shuvi.Interfaces.Status;
using Shuvi.Classes.Rates;
using Shuvi.Classes.ActionChances;
using Shuvi.Interfaces.ActionChances;

namespace Shuvi.Classes.Enemy
{
    public class EnemyBase : EntityBase, IEnemy
    {
        public int RatingGet { get; init; }
        public string PictureUrl { get; init; }
        public IRate Drop { get; init; }
        public IEnemyActionChances ActionChances { get; init; }

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
            ActionChances = data.ActionChances;
        }
        private UserCharacteristics SetCharacteristics(EnemyData data, int amount)
        {
            ++amount;
            return new UserCharacteristics(
                _random.Next(0, 2) == 0 ? data.Strength : data.Strength + _random.Next(0, amount),
                _random.Next(0, 2) == 0 ? data.Agility : data.Agility + _random.Next(0, amount),
                _random.Next(0, 2) == 0 ? data.Luck : data.Luck + _random.Next(0, amount),
                _random.Next(0, 2) == 0 ? data.Intellect : data.Intellect + _random.Next(0, amount),
                _random.Next(0, 2) == 0 ? data.Endurance : data.Endurance + _random.Next(0, amount)
                );
        }
        private StaticHealth SetHealth(int health, int amount)
        {
            ++amount;
            var finalHealth = _random.Next(0, 2) == 0 
                ? 
                health
                : 
                health + _random.Next(0, amount) * CharacteristicSettings.HealthPerUpPoint;
            return new StaticHealth(finalHealth, finalHealth);
        }
        private StaticMana SetMana(int mana, int amount)
        {
            ++amount;
            var finalMana = _random.Next(0, 2) == 0
                ?
                mana
                :
                mana + _random.Next(0, amount) * CharacteristicSettings.ManaPerUpPoint;
            return new StaticMana(finalMana, finalMana);
        }
        public IActionResult RandomAction(IEntity target)
        {
            return ActionChances.GetRandomAction(Spell.CanCast(this)) switch
            {
                FightAction.LightAttack => DealLightDamage(target),
                FightAction.HeavyAttack => DealHeavyDamage(target),
                FightAction.Dodge => PreparingForDodge(target),
                FightAction.Defense => PreparingForDefense(target),
                FightAction.Spell => CastSpell(target),
                _ => DealLightDamage(target)
            };
        }
    }
}
