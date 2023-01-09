using Discord;
using Shuvi.Classes.Skills;
using Shuvi.Enums;
using Shuvi.Interfaces.Skills;
using System.Runtime.CompilerServices;

namespace Shuvi.Extensions
{
    public static class EnumExtension
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
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToEngString(this ItemType target)
        {
            return target switch
            {
                ItemType.Test => nameof(ItemType.Test),
                ItemType.Weapon => nameof(ItemType.Weapon),
                ItemType.Helmet => nameof(ItemType.Helmet),
                ItemType.Armor => nameof(ItemType.Armor),
                ItemType.Leggings => nameof(ItemType.Leggings),
                ItemType.Boots => nameof(ItemType.Boots),
                ItemType.Simple => nameof(ItemType.Simple),
                ItemType.Potion => nameof(ItemType.Potion),
                ItemType.Chest => nameof(ItemType.Chest),
                _ => string.Empty
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this Characteristic target)
        {
            return target switch
            {
                Characteristic.Strength => "Сила",
                Characteristic.Agility => "Ловкость",
                Characteristic.Luck => "Удача",
                Characteristic.Intellect => "Интеллект",
                Characteristic.Endurance => "Выносливость",
                _ => "Error"
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this ItemNeeds target)
        {
            return target switch
            {
                ItemNeeds.Strange => "Сила",
                ItemNeeds.Agility => "Ловкость",
                ItemNeeds.Luck => "Удача",
                ItemNeeds.Intellect => "Интеллект",
                ItemNeeds.Endurance => "Выносливость",
                ItemNeeds.Rank => "Ранг",
                _ => "Error"
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string GetFormatString(this ItemNeeds target, int amount)
        {
            return target switch
            {
                ItemNeeds.Strange => amount.ToString(),
                ItemNeeds.Agility => amount.ToString(),
                ItemNeeds.Luck => amount.ToString(),
                ItemNeeds.Intellect => amount.ToString(),
                ItemNeeds.Endurance => amount.ToString(),
                ItemNeeds.Rank => ((Rank)amount).ToRusString(),
                _ => amount.ToString()
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this Rank target)
        {
            return target switch
            {
                Rank.E => nameof(Rank.E),
                Rank.D => nameof(Rank.D),
                Rank.C => nameof(Rank.C),
                Rank.B => nameof(Rank.B),
                Rank.A => nameof(Rank.A),
                Rank.S => nameof(Rank.S),
                Rank.SS => nameof(Rank.SS),
                Rank.SSS => nameof(Rank.SSS),
                _ => "Error"
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Color GetColor(this Rank target)
        {
            return target switch
            {
                Rank.E => new Color((byte)255, (byte)255, (byte)255),
                Rank.D => new Color((byte)101, (byte)150, (byte)227),
                Rank.C => new Color((byte)78, (byte)105, (byte)205),
                Rank.B => new Color((byte)137, (byte)71, (byte)255),
                Rank.A => new Color((byte)213, (byte)43, (byte)230),
                Rank.S => new Color((byte)235, (byte)76, (byte)75),
                Rank.SS => new Color((byte)255, (byte)255, (byte)0),
                Rank.SSS => new Color((byte)255, (byte)215, (byte)0),
                _ => new Color((byte)255, (byte)255, (byte)255)
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int GetNeedRating(this Rank target)
        {
            return target switch
            {
                Rank.E => 0,
                Rank.D => 100,
                Rank.C => 300,
                Rank.B => 600,
                Rank.A => 1000,
                Rank.S => 2000,
                Rank.SS => 3500,
                Rank.SSS => 5000,
                _ => -1
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool CanRankUp(this Rank target)
        {
            return target != Rank.SSS;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this UserProfessions target)
        {
            return target switch
            {
                UserProfessions.NoProfession => "Нету",
                UserProfessions.Proofer => "Разведчик",
                _ => "Нету"
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this UserRaces target)
        {
            return target switch
            {
                UserRaces.ExMachina => "Экс-Машина",
                _ => "Неизвестный"
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ISkill GetSkill(this UserProfessions target)
        {
            return target switch
            {
                UserProfessions.NoProfession => new VoidSkill(),
                UserProfessions.Proofer => new ProoferSkill(),
                _ => new VoidSkill()
            };
        }
    }
}
