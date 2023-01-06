using Shuvi.Interfaces.Entity;
using Shuvi.Classes.Entity;
using Shuvi.Interfaces.Inventory;
using Shuvi.Classes.Inventory;
using Shuvi.Classes.Spell;
using Shuvi.Classes.Characteristics;
using Shuvi.Interfaces.Skills;
using Shuvi.Extensions;
using Shuvi.Interfaces.Status;
using Shuvi.Interfaces.User;
using Shuvi.Enums;
using Shuvi.Interfaces.ActionChances;

namespace Shuvi.Classes.Player
{
    public class PlayerBase : EntityBase, IPlayer
    {
        public ISkill Skill { get; init; }
        public IUserInventory Inventory { get; init; }
        public IUserActionChances ActionChances { get; private set; }

        public PlayerBase(string userName, IDatabaseUser user)
        {
            Name = userName;
            Rank = user.Rating.Rank;
            Characteristics = user.Characteristic;
            Spell = SpellFactory.GetSpell(user.SpellName);
            Skill = user.Profession.GetSkill();
            Health = new StaticHealth(user.Health.GetCurrentHealth(), user.Health.Max);
            Mana = new StaticMana(user.Mana.GetCurrentMana(), user.Mana.Max);
            Inventory = new UserInventory();
            ActionChances = user.ActionChances;
        }
        public IActionResult UseSkill(IEntity target)
        {
            return Skill.UseSkill(this, target);
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
                FightAction.Skill => UseSkill(target),
                _ => DealLightDamage(target)
            };
        }
        public override void Update()
        {
            base.Update();
            Skill.Update(this);
        }

    }
}
