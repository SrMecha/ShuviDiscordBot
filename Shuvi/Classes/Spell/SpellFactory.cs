using Shuvi.Classes.Spell.SpellList;
using Shuvi.Interfaces.Spell;

namespace Shuvi.Classes.Spell
{
    public static class SpellFactory
    {
        public static Dictionary<string, ISpell> Spells { get; } = new()
        {
            
        };

        public static ISpell GetSpell(string spellName)
        {
            if (Spells.TryGetValue(spellName, out var result))
                return result;
            return new VoidSpell();
        }
    }
}
