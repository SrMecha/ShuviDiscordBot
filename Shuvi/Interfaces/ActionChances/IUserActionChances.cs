using Shuvi.Enums;

namespace Shuvi.Interfaces.ActionChances
{
    public interface IUserActionChances : IActionChancesBase
    {
        public int Skill { get; set; }

        public FightAction GetRandomAction(bool withSpell, bool withSkill);
    }
}
