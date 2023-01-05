namespace Shuvi.Interfaces.User
{
    public interface IUserStatistics
    {
        public long CreatedAt { get; }
        public long LiveTime { get; }
        public int DeathCount { get; }
        public int DungeonComplite { get; }
        public int EnemyKilled { get; }
        public int MaxRating { get; }

        public void AddEnemyKilled(int amount = 1);
        public void AddDeathCount(int amount = 1);
        public void AddDungeonComplite(int amount = 1);
        public void UpdateMaxRating(int amount);
        public void UpdateLiveTime();
    }
}
