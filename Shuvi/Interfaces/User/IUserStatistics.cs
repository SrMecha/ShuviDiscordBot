namespace Shuvi.Interfaces.User
{
    public interface IUserStatistics
    {
        public long CreatedAt { get; init; }
        public long LiveTime { get; init; }
        public int DeathCount { get; init; }
        public int DungeonComplite { get; init; }
        public int EnemyKilled { get; init; }
        public int MaxRating { get; init; }
    }
}
