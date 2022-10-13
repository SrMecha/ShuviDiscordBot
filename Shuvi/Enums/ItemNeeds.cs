using System.Runtime.CompilerServices;
using ShuviBot.Enums.Ranks;

namespace ShuviBot.Enums.ItemNeeds
{
    public enum ItemNeeds
    {
        Strange,
        Agility,
        Luck,
        Intellect,
        Endurance,
        Rank
    }

    public static class ItemNeedsEnumExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this ItemNeeds target)
        {
            return target switch
            {
                ItemNeeds.Strange => "����",
                ItemNeeds.Agility => "��������",
                ItemNeeds.Luck => "�����",
                ItemNeeds.Intellect => "���������",
                ItemNeeds.Endurance => "������������",
                ItemNeeds.Rank => "����",
                _ => "Error"
            };
        }

        public static string GetFormatString(this ItemNeeds target, int amount)
        {
            return target switch
            {
                ItemNeeds.Strange => amount.ToString(),
                ItemNeeds.Agility => amount.ToString(),
                ItemNeeds.Luck => amount.ToString(),
                ItemNeeds.Intellect => amount.ToString(),
                ItemNeeds.Endurance => amount.ToString(),
                ItemNeeds.Rank => ((Ranks.Rank)amount).ToRusString(),
                _ => amount.ToString()
            };
        }
    }
}