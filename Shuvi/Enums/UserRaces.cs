using System.Runtime.CompilerServices;

namespace ShuviBot.Enums.UserRaces
{
    public enum UserRaces
    {
        ExMachina
    }

    public static class UserRaceEnumExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string ToRusString(this UserRaces target)
        {
            return target switch
            {
                UserRaces.ExMachina => "Экс-Машина",
                _ => "Бог"
            };
        }
    }
}