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
    }
}