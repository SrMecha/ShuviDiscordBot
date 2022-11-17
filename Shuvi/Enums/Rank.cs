using Discord;
using System.Runtime.CompilerServices;

namespace ShuviBot.Enums.Ranks
{
    public enum Rank
    {
        E,
        D,
        C,
        B,
        A,
        S,
        SS,
        SSS
    }

    public static class RankEnumExtension
    {
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
        public static bool CanRankUp(this Rank target)
        {
            return target != Rank.SSS;
        }
    }
}