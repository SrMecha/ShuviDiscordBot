using Discord;
using System.Runtime.CompilerServices;

namespace ShuviBot.Enums.Ranks
{
    public enum Ranks
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

    public static class RanksEnumExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this Ranks target)
        {
            return target switch
            {
                Ranks.E => nameof(Ranks.E),
                Ranks.D => nameof(Ranks.D),
                Ranks.C => nameof(Ranks.C),
                Ranks.B => nameof(Ranks.B),
                Ranks.A => nameof(Ranks.A),
                Ranks.S => nameof(Ranks.S),
                Ranks.SS => nameof(Ranks.SS),
                Ranks.SSS => nameof(Ranks.SSS),
                _ => "Error"
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Color GetColor(this Ranks target)
        {
            return target switch
            {
                Ranks.E => new Color((byte)255, (byte)255, (byte)255),
                Ranks.D => new Color((byte)101, (byte)150, (byte)227),
                Ranks.C => new Color((byte)78, (byte)105, (byte)205),
                Ranks.B => new Color((byte)137, (byte)71, (byte)255),
                Ranks.A => new Color((byte)213, (byte)43, (byte)230),
                Ranks.S => new Color((byte)235, (byte)76, (byte)75),
                Ranks.SS => new Color((byte)255, (byte)255, (byte)0),
                Ranks.SSS => new Color((byte)255, (byte)215, (byte)0),
                _ => new Color((byte)255, (byte)255, (byte)255)
            };
        }
    }
}