using Discord.Interactions;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

namespace Shuvi.Extensions
{
    public static class Interactions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static async Task TryDeferAsync(this SocketMessageComponent? target)
        {
            if (target == null)
                return;
            try
            {
                await target.DeferAsync();
            }
            catch
            {

            }
        }
    }
}
