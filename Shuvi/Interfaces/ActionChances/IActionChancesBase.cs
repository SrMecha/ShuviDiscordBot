using Shuvi.Enums;

namespace Shuvi.Interfaces.ActionChances
{
    public interface IActionChancesBase
    {
        public int LightAttack { get; set; }
        public int HeavyAttack { get; set; }
        public int Dodge { get; set; }
        public int Defense { get; set; }
        public int Spell { get; set; }

        public FightAction GetRandomAction(bool withSpell);
    }
}
