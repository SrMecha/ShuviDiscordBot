using Shuvi.Enums;
using System.Runtime.CompilerServices;

namespace Shuvi.Extensions
{
    public static class Int
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string WithSign(this int target)
        {
            return target > 0 ? $"+{target}" : target.ToString();
        }
    }
}
