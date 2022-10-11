using System.Runtime.CompilerServices;

namespace ShuviBot.Enums.ItemType
{
    public enum ItemType
    {
        Unknown,
        Weapon,
        Helmet,
        Armor,
        Leggings,
        Boots,
        Simple,
        Potion,
        Chest
    }

    public static class ItemTypeEnumExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this ItemType target)
        {
            return target switch
            {
                ItemType.Unknown => "Предмет Шуви (Test)",
                ItemType.Weapon => "Оружие",
                ItemType.Helmet => "Шлем",
                ItemType.Armor => "Броня",
                ItemType.Leggings => "Штаны",
                ItemType.Boots => "Ботинки",
                ItemType.Simple => "Предмет",
                ItemType.Potion => "Зелье",
                ItemType.Chest => "Сундук",
                _ => "Предмет Шуви (Unknown)"
            };
        }
    }
}
