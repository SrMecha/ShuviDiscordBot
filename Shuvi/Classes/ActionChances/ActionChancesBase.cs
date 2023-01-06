using Shuvi.Enums;
using Shuvi.Interfaces.ActionChances;
using System;
using System.Threading.Channels;

namespace Shuvi.Classes.ActionChances
{
    public class ActionChancesBase : IActionChancesBase
    {
        protected readonly Random _random = new();
        protected List<int>? _chances;

        public int LightAttack { get; set; } = 2;
        public int HeavyAttack { get; set; } = 2;
        public int Dodge { get; set; } = 1;
        public int Defense { get; set; } = 1;
        public int Spell { get; set; } = 0;

        protected virtual List<int> SetChances()
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
            return result;
        }

        public virtual FightAction GetRandomAction(bool withSpell)
        {
            _chances ??= SetChances();
            return ((FightAction)_chances.ElementAt(_random.Next(0, _chances.Count))) switch
            {
                FightAction.LightAttack => FightAction.LightAttack,
                FightAction.HeavyAttack => FightAction.HeavyAttack,
                FightAction.Defense => FightAction.Defense,
                FightAction.Dodge => FightAction.Dodge,
                FightAction.Spell => withSpell ? FightAction.Spell : (FightAction)_random.Next(0, 4),
                _ =>  FightAction.LightAttack
            };

        }
    }
}
