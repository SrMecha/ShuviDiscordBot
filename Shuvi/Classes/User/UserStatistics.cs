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
        public long CreatedAt { get; private set; }
        public long LiveTime { get; private set; }
        public int DeathCount { get; private set; }
        public int DungeonComplite { get; private set; }
        public int EnemyKilled { get; private set; }
        public int MaxRating { get; private set; }

        public UserStatistics(long createdAt, long liveTime, int deathCount, int dungeonComplite, int enemyKiled, int maxRating)
        {
            CreatedAt = createdAt;
            LiveTime = liveTime;
            DeathCount = deathCount;
            DungeonComplite = dungeonComplite;
            EnemyKilled = enemyKiled;
            MaxRating = maxRating;
        }

        public void AddEnemyKilled(int amount = 1)
        {
            EnemyKilled += amount;
        }

        public void AddDeathCount(int amount = 1)
        {
            DeathCount += amount;
        }

        public void AddDungeonComplite(int amount = 1)
        {
            DungeonComplite += amount;
        }
    }
}
