using System.Runtime.CompilerServices;

namespace ShuviBot.Enums.ItemType
{
    public enum EquipmentType
    {
        Helmet,
        Armor,
        Leggings,
        Boots
    }
    public enum ItemType
    {
        Test,
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
                ItemType.Test => "Предмет Шуви (Test)",
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
        public static bool WithBonuses(this ItemType target)
        {
            return target switch
            {
                ItemType.Test => false,
                ItemType.Weapon => true,
                ItemType.Helmet => true,
                ItemType.Armor => true,
                ItemType.Leggings => true,
                ItemType.Boots => true,
                ItemType.Simple => false,
                ItemType.Potion => false,
                ItemType.Chest => false,
                _ => false
            };
        }
    }
}
