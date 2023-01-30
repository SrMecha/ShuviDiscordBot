using Discord;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Status;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Spell;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Spell
{
    public class SpellBase : ISpell
    {
        public virtual ISpellInfo Info { get; private set; } = new SpellInfo("Без заклинания", "Без описания", 0, new());

        public virtual IActionResult Cast(IEntity player, IEntity target)
        {
            return new ActionResult("У вас нету заклинания");
        }
        public virtual bool CanCast(IEntity player)
        {
            return player.Mana.Now >= Info.Cost;
        }
        public Embed GetInfoEmbed()
        {
            return new EmbedBuilder()
                .WithAuthor("Просмотр заклинания")
                .WithDescription($"**Название:** {Info.Name}**Стоимость:** {Info.Cost}\n**Описание:** {Info.Description}")
                .Build();
        }
        public Embed GetInfoEmbed(IUser user)
        {
            return new UserEmbedBuilder(user)
                .WithAuthor("Просмотр заклинания")
                .WithDescription($"**Название:** {Info.Name}**Стоимость:** {Info.Cost}\n**Описание:** {Info.Description}")
                .Build();
        }
    }
}
