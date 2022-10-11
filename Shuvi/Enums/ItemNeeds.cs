using System.Runtime.CompilerServices;

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
    }
}