using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Effect;
using Shuvi.Classes.Status;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Spell;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Spell.SpellList
{
    public class FrozenWave : ISpell
    {
        public ISpellInfo Info { get; private set; } = new SpellInfo("Ледяная волна",
            "Возводит впереди себя огромную ледяную дорогу, способная за считаные секунды заморозить врага.", 10, new());

        public IActionResult Cast(IEntity player, IEntity target)
        {
            player.Mana.ReduceMana(Info.Cost);
            if (target.IsDodged(player, 30))
                return new ActionResult($"{player.Name} промахнулся Ледяной волной.");
            var damage = target.BlockDamage(player.Characteristics.Strength * 1.3f);
            target.ReduceHealth(damage);
            var bonuses = new CharacteristicBonuses();
            bonuses.Agility -= target.Characteristics.Agility / 2;
            target.Effects.Add(new EffectBase("Замарозка", 2, bonuses));
            return new ActionResult($"{player.Name} нанёс {damage} урона Ледяной волной.");
        }
        public bool CanCast(IEntity player)
        {
            return player.Mana.Now >= Info.Cost;
        }
    }
}
