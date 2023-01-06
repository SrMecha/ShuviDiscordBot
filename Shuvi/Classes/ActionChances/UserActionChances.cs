using Shuvi.Enums;
using Shuvi.Interfaces.ActionChances;

namespace Shuvi.Classes.ActionChances
{
    public class UserActionChances : ActionChancesBase, IUserActionChances
    {
        public int Skill { get; set; } = 0;

        protected override List<int> SetChances()
        {
            var result = new List<int>();
            for (int i = 0; i < LightAttack; i++)
                result.Add((int)FightAction.LightAttack);
            for (int i = 0; i < HeavyAttack; i++)
                result.Add((int)FightAction.HeavyAttack);
            for (int i = 0; i < Dodge; i++)
                result.Add((int)FightAction.Dodge);
            for (int i = 0; i < Defense; i++)
                result.Add((int)FightAction.Defense);
            for (int i = 0; i < Spell; i++)
                result.Add((int)FightAction.Spell);
            for (int i = 0; i < Skill; i++)
                result.Add((int)FightAction.Skill);
            return result;
        }

        public virtual FightAction GetRandomAction(bool withSpell, bool withSkill)
        {
            _chances ??= SetChances();
            return ((FightAction)_chances.ElementAt(_random.Next(0, _chances.Count))) switch
            {
                FightAction.LightAttack => FightAction.LightAttack,
                FightAction.HeavyAttack => FightAction.HeavyAttack,
                FightAction.Defense => FightAction.Defense,
                FightAction.Dodge => FightAction.Dodge,
                FightAction.Spell => withSpell ? FightAction.Spell : (FightAction)_random.Next(0, 4),
                FightAction.Skill => withSpell ? FightAction.Skill : (FightAction)_random.Next(0, 4),
                _ => FightAction.LightAttack
            };

        }
    }
}
