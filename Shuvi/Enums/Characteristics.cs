using Discord;
using System.Runtime.CompilerServices;

namespace ShuviBot.Enums.Characteristics
{
    public enum Characteristics
    {
        Strange,
        Agility,
        Luck,
        Intellect,
        Endurance
    }

    public static class CharacteristicEnumExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this Characteristics target)
        {
            return target switch
            {
                Characteristics.Strange => "Сила",
                Characteristics.Agility => "Ловкость",
                Characteristics.Luck => "Удача",
                Characteristics.Intellect => "Интеллект",
                Characteristics.Endurance => "Выносливость",
                _ => "Error"
            };
        }
    }
}