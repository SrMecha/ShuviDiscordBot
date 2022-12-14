using Shuvi.Classes.Status;
using Shuvi.Enums;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Spell;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Spell.SpellList
{
    public class VoidSpell : ISpell
    {
        public ISpellInfo Info { get; private set; } = new SpellInfo("Без заклинания", "Без описания", 0, new() { UserRaces.ExMachina });

        public IActionResult Cast(IEntity player, IEntity target)
        {
            return new ActionResult("У вас нету заклинания");
        }
        public bool CanCast(IEntity player)
        {
            return false;
        }
    }
}
