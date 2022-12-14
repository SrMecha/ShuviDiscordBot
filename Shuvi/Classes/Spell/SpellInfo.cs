using Shuvi.Enums;
using Shuvi.Interfaces.Spell;

namespace Shuvi.Classes.Spell
{
    

    public class SpellInfo : ISpellInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Cost { get; private set; }
        public List<UserRaces> AllowedRaces { get; private set; }

        public SpellInfo(string name, string description, int cost, List<UserRaces> allowedRaces)
        {
            Name = name;
            Description = description;
            Cost = cost;
            AllowedRaces = allowedRaces;
        }
    }
}
