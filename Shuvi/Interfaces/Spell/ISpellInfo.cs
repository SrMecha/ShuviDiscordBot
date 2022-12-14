using Shuvi.Enums;

namespace Shuvi.Interfaces.Spell
{
    public interface ISpellInfo
    {
        public string Name { get; }
        public string Description { get; }
        public int Cost { get; }
        public List<UserRaces> AllowedRaces { get; }
    }
}
