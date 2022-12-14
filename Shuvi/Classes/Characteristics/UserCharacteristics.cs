using Discord;
using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Classes.Characteristics
{
    public class UserCharacteristics : ICharacteristics
    {
        public int Strength { get; init; }
        public int Agility { get; init; }
        public int Luck { get; init; }
        public int Intellect { get; init; }
        public int Endurance { get; init; }

        public UserCharacteristics(int strength = 1, int agility = 1, int luck = 1, int intellect = 1, int endurance = 1)
        {
            Strength = strength;
            Agility = agility;
            Luck = luck;
            Intellect = intellect;
            Endurance = endurance;
        }

        public string ToRusString(ICharacteristics bonuses)
        {
            return $"**Сила:** {Strength} {(bonuses.Strength != 0 ? "| " + (bonuses.Strength > 0 ? $"+{bonuses.Strength}" : bonuses.Strength) : "")}\n" +
                $"**Ловкость:** {Agility} {(bonuses.Agility != 0 ? "| " + (bonuses.Agility > 0 ? $"+{bonuses.Agility}" : bonuses.Agility) : "")}\n" +
                $"**Удача:** {Luck} {(bonuses.Luck != 0 ? "| " + (bonuses.Luck > 0 ? $"+{bonuses.Luck}" : bonuses.Luck) : "")}\n" +
                $"**Интеллект:** {Intellect} {(bonuses.Intellect != 0 ? "| " + (bonuses.Intellect > 0 ? $"+{bonuses.Intellect}" : bonuses.Intellect) : "")}\n" +
                $"**Выносливость:** {Endurance} {(bonuses.Endurance != 0 ? "| " + (bonuses.Endurance > 0 ? $"+{bonuses.Endurance}" : bonuses.Endurance) : "")}";
        }
    }
}
