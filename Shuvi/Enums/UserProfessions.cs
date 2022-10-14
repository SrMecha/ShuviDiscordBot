using System.Runtime.CompilerServices;

namespace ShuviBot.Enums.UserProfessions
{
    public enum UserProfessions
    {
        NoProfession,
        Proofer
    }

    public static class UserProfessionsEnumExtension
    {
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
    }
}