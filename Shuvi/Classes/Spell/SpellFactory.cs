using Shuvi.Classes.Spell.SpellList;
using Shuvi.Enums;
using Shuvi.Interfaces.Spell;

namespace Shuvi.Classes.Spell
{
    public static class SpellFactory
    {
        public static Dictionary<string, ISpell> Spells { get; } = new()
        {
            {"FrozenWave", new FrozenWave() }
        };
        public static Dictionary<UserRaces, Dictionary<string, ISpell>>? SpellsByRace { get; private set; } = null;

        public static ISpell GetSpell(string spellName)
        {
            if (Spells.TryGetValue(spellName, out var result))
                return result;
            return new VoidSpell();
        }
        public static void Init()
        {
            var result = new Dictionary<UserRaces, Dictionary<string, ISpell>>();
            foreach (var (id, spell)  in Spells)
            {
                foreach(var race in spell.Info.AllowedRaces)
                {
                    if (result.ContainsKey(race))
                        result[race].Add(id, spell);
                    else
                        result.Add(race, new() { { id, spell } });
                }
            }
            SpellsByRace = result;
        }
        public static IEnumerable<ISpell> GetSpells(UserRaces race)
        {
            if (SpellsByRace == null)
                Init();
            return SpellsByRace!.GetValueOrDefault(race, new()).Values;
        }
        public static IEnumerable<KeyValuePair<string, ISpell>> GetSpellsWithId(UserRaces race)
        {
            if (SpellsByRace == null)
                Init();
            return SpellsByRace!.GetValueOrDefault(race, new());
        }
    }
}
