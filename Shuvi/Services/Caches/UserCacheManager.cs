using Shuvi.Classes.User;
using Shuvi.Interfaces.User;

namespace Shuvi.Services.Caches
{
    public class UserCacheManager
    {
        private const int _deleteCacheAfter = 600;
        private readonly Dictionary<ulong, IDatabaseUser> _userCache = new();
        private readonly Dictionary<ulong, long> _timeCache = new();

        public bool TryAddUser(IDatabaseUser user)
        {
            _timeCache.TryAdd(user.Id, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            return _userCache.TryAdd(user.Id, user);
        }
        public bool TryAddUser(UserData userData)
        {
            _timeCache.TryAdd(userData.Id, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            return _userCache.TryAdd(userData.Id, new DatabaseUser(userData));
        }
        public bool TryRemoveUser(ulong id)
        {
            _timeCache.Remove(id);
            return _userCache.Remove(id);
        }
        public bool TryGetUser(ulong id, out IDatabaseUser? user)
        {
            return _userCache.TryGetValue(id, out user);
        }
        public IDatabaseUser GetUser(ulong id)
        {
            return _userCache.GetValueOrDefault(id)!;
        }
        public void DeleteNotUsedCache()
        {
            long currentTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            for (int i = _timeCache.Count; i > 0; i--)
            {
                var element = _timeCache.ElementAt(i);
                if (element.Value + _deleteCacheAfter > currentTime)
                {
                    _timeCache.Remove(element.Key);
                    _userCache.Remove(element.Key);
                }
            }
        }
    }
}
