using Discord;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Status;

namespace Shuvi.Interfaces.Spell
{
    public interface ISpell
    {
        public ISpellInfo Info { get; }

        public bool CanCast(IEntity player);
        public IActionResult Cast(IEntity player, IEntity target);
        public Embed GetInfoEmbed();
        public Embed GetInfoEmbed(IUser user);
    }
}
