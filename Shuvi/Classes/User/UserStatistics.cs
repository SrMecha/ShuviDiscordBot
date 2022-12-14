using Shuvi.Classes.Characteristics;
using Shuvi.Interfaces.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shuvi.Classes.User
{
    public class UserStatistics : IUserStatistics
    {
        public long CreatedAt { get; init; }
        public long LiveTime { get; init; }
        public int DeathCount { get; init; }
        public int DungeonComplite { get; init; }
        public int EnemyKilled { get; init; }
        public int MaxRating { get; init; }

        public UserStatistics(long createdAt, long liveTime, int deathCount, int dungeonComplite, int enemyKiled, int maxRating)
        {
            CreatedAt = createdAt;
            LiveTime = liveTime;
            DeathCount = deathCount;
            DungeonComplite = dungeonComplite;
            EnemyKilled = enemyKiled;
            MaxRating = maxRating;
        }
    }
}
