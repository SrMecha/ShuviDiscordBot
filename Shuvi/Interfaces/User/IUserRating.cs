using Shuvi.Enums;
using Shuvi.Interfaces.Status;

namespace Shuvi.Interfaces.User
{
    public interface IUserRating
    {
        public int Points { get; }
        public Rank Rank { get; }
        public Rank GetRank(int rating);
        public IRatingResult AddPoints(int amount, Rank rank);
        public IRatingResult AddPoints(int amount);
        public IRatingResult RemovePoints(int amount);
        public IRatingResult SetPoints(int amount);
    }
}
