using Shuvi.Interfaces.Entity;
using Shuvi.Classes.Entity;
using Shuvi.Interfaces.Inventory;
using Shuvi.Classes.User;
using Shuvi.Classes.Inventory;
using Shuvi.Classes.Spell;
using Shuvi.Classes.Characteristics;
using Shuvi.Interfaces.Skills;
using Shuvi.Extensions;
using MongoDB.Driver;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Player
{
    public class PlayerBase : EntityBase, IPlayer
    {
        public ISkill Skill { get; init; }
        public IUserInventory Inventory { get; init; }

        public PlayerBase(string userName, DatabaseUser user)
        {
            Name = userName;
            Characteristics = user.Characteristic;
            Spell = SpellFactory.GetSpell(user.SpellName);
            Skill = user.Profession.GetSkill();
            Health = new StaticHealth(user.Health.GetCurrentHealth(), user.Health.Max);
            Mana = new StaticMana(user.Mana.GetCurrentMana(), user.Mana.Max);
            Inventory = new UserInventory();
        }
        public IActionResult UseSkill(IEntity target)
        {
            return Skill.UseSkill(this, target);
        }
        public override void Update()
        {
            base.Update();
            Skill.Update(this);
        }

    }
}
