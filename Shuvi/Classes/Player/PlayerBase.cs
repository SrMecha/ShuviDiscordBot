using Shuvi.Interfaces.Entity;
using Shuvi.Classes.Entity;
using Shuvi.Interfaces.Inventory;
using Shuvi.Classes.User;
using Shuvi.Classes.Inventory;
using Shuvi.Classes.Spell;
using Shuvi.Classes.Characteristics;

namespace Shuvi.Classes.Player
{
    public class PlayerBase : EntityBase, IPlayer
    {
        public IUserInventory Inventory { get; init; }

        public PlayerBase(string userName, DatabaseUser user)
        {
            Name = userName;
            Characteristics = user.Characteristic;
            Spell = SpellFactory.GetSpell(user.SpellName);
            Health = new StaticHealth(user.Health.GetCurrentHealth(), user.Health.Max);
            Mana = new StaticMana(user.Mana.GetCurrentMana(), user.Mana.Max);
            Inventory = new UserInventory();
        }
    }
}
